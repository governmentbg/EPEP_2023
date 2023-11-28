using eCase.Common.Db;
using eCase.Common.NLog;
using eCase.Data.Linq;
using eCase.Domain.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace eCase.Data.Core
{
    public enum DbKey
    {
        Main,
        BlobStorage
    }
    
    public class UnitOfWork : IUnitOfWork
    {
        private static readonly Func<EntityReference, EntityKey> RelatedEndCachedValueAccessor = ExpressionHelper.GetFieldAccessor<EntityReference, EntityKey>("_cachedForeignKey");

        private static object syncRoot = new object();

        private static Dictionary<string, DbCompiledModel> compiledModelDictionary = new Dictionary<string, DbCompiledModel>();

        private bool disposed = false;

        private DbContext context = null;

        private ILogger logger;
        private IEnumerable<IDbContextInitializer> contextInitializers;

        private Lazy<IEnumerable<IEventHandler>> eventHandlers;

        private string ContextName;

        public UnitOfWork(DbKey dbKey, ILogger logger, IEnumerable<IDbConfiguration> configurations, IEnumerable<IDbContextInitializer> contextInitializers, Lazy<IEnumerable<IEventHandler>> eventHandlers)
        {
            ContextName = SelectDb(dbKey);

            this.contextInitializers = contextInitializers;
            this.eventHandlers = eventHandlers;
            this.logger = logger;

            Initialize(configurations);
        }

        private string SelectDb(DbKey dbKey)
        {
            switch (dbKey)
            {
                case DbKey.Main:
                    return "DbContextMain";
                case DbKey.BlobStorage:
                    return ConfigurationManager.AppSettings["eCase.Service:CurrentBlobStorageConnectionString"];
                default: 
                    throw new NotSupportedException();
            }
        }

        public DbContext DbContext
        {
            get
            {
                if (this.context == null)
                {
                    this.context = new LoggingDbContext(this.logger, "Name=" + ContextName, compiledModelDictionary[ContextName]);
                    this.context.Configuration.LazyLoadingEnabled = false;
                    this.context.Configuration.ProxyCreationEnabled = false;
                    this.context.Configuration.UseDatabaseNullSemantics = true;

                    foreach (IDbContextInitializer contextInitializer in this.contextInitializers)
                    {
                        contextInitializer.InitializeContext(context);
                    }

#if DEBUG
                    this.context.Database.Log = (s) => Debug.WriteLine(s);
#endif
                }

                return this.context;
            }
        }

        public void Save()
        {
            try
            {
                this.context.ChangeTracker.DetectChanges();

                this.DispatchEvents();

                // we evaluate the entries in advance because some of them may become detached
                // as others before them are deleted (in case they have dependent key)
                var addedOrModifiedEntityEntries =
                    ((IObjectContextAdapter)this.context).ObjectContext.ObjectStateManager
                    .GetObjectStateEntries(EntityState.Added | EntityState.Modified)
                    .Where(e => !e.IsRelationship)
                    .ToList();

                foreach (ObjectStateEntry entry in addedOrModifiedEntityEntries)
                {
                    // skip if a parent entity was deleted and a dependent entity becomes detached
                    if (entry.State == EntityState.Detached)
                    {
                        continue;
                    }

                    if (entry.RelationshipManager.GetAllRelatedEnds()
                        .Any(re =>
                            re is EntityReference &&
                            re.RelationshipSet.ElementType.RelationshipEndMembers
                                .Any(rem => rem.Name == re.TargetRoleName &&
                                    rem.DeleteBehavior == OperationAction.Cascade) &&
                            RelatedEndCachedValueAccessor((EntityReference)re).EntityContainerName.Contains("EntityHasNullForeignKey")))
                    {
                        ((IObjectContextAdapter)this.context).ObjectContext.DeleteObject(entry.Entity);
                    }
                }

                this.context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DataUpdateConcurrencyException();
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder validationErrorsText = new StringBuilder();
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        validationErrorsText.AppendFormat(
                            "Class: {0}, Property: {1}, Error: {2}\n\n",
                            validationErrors.Entry.Entity.GetType().FullName,
                            validationError.PropertyName,
                            validationError.ErrorMessage);
                    }
                }

                this.logger.Error("DbEntityValidationException\n" + validationErrorsText.ToString());

                throw;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public DbContextTransaction BeginTransaction()
        {
            return this.DbContext.Database.BeginTransaction();
        }

        public DbContextTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return this.DbContext.Database.BeginTransaction(isolationLevel);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing && this.context != null)
                {
                    this.context.Dispose();
                }

                this.context = null;
                this.disposed = true;
            }
        }

        private void Initialize(IEnumerable<IDbConfiguration> configurations)
        {
            if (!compiledModelDictionary.ContainsKey(ContextName))
            {
                lock (syncRoot)
                {
                    if (!compiledModelDictionary.ContainsKey(ContextName))
                    {
                        Database.SetInitializer<DbContext>(null);

                        DbModelBuilder modelBuilder = new DbModelBuilder();

                        modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
                        modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

                        foreach (IDbConfiguration configuration in configurations)
                        {
                            configuration.AddConfiguration(modelBuilder);
                        }

                        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[ContextName].ConnectionString))
                        {
                            compiledModelDictionary[ContextName] = modelBuilder.Build(connection).Compile();
                        }
                    }
                }
            }
        }

        private void DispatchEvents()
        {
            var domainEventEntities = this.context.ChangeTracker.Entries<IEventEmitter>()
                .Select(po => po.Entity)
                .Where(po => po.Events.Any())
                .ToArray();

            foreach (var entity in domainEventEntities)
            {
                var events = entity.Events.ToArray();
                entity.Events.Clear();
                foreach (var domainEvent in events)
                {
                    foreach (var eventHandler in this.eventHandlers.Value)
                    {
                        eventHandler.Handle(domainEvent);
                    }
                }
            }
        }
    }
}

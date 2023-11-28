using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using eCase.Data.Core;
using eCase.Domain.Entities;

namespace eCase.Data.Repositories
{
    public interface ILawyerRepository : IAggregateRepository<Lawyer>
    {
        Lawyer GetLawyerByNumber(string number);

        List<Lawyer> GetAllLawyers();

        List<Lawyer> GetAllLawyersFromCertainDate(DateTime dateFrom);

        List<Lawyer> FindLawyers(string term, int? limit);
    }

    internal class LawyerRepository : AggregateRepository<Lawyer>, ILawyerRepository
    {
        public LawyerRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public Lawyer GetLawyerByNumber(string number)
        {
            return this.Set()
                    .Where(t => t.Number == number)
                    .FirstOrDefault();
        }

        public List<Lawyer> GetAllLawyers()
        {
            return this.Set()
                    .Select(t => t).ToList();
        }


        public List<Lawyer> GetAllLawyersFromCertainDate(DateTime dateFrom)
        {
            return this.Set()
                    .Where(t => t.CreateDate >= dateFrom)
                    .ToList();
        }

        public List<Lawyer> FindLawyers(string term, int? limit = null)
        {
            IQueryable<Lawyer> lawyers = this.Set();

            string[] searchWords = term.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string word in searchWords)
            {
                lawyers = lawyers.Where(e => e.Number.Contains(word) || e.Name.Contains(word));
            }

            if (limit.HasValue)
                lawyers = lawyers.Take(limit.Value);

            return lawyers.ToList();
        }
    }
}
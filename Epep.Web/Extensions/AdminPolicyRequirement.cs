using Epep.Core.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Epep.Web.Extensions
{
    /// <summary>
    /// Проверка за роли Администратор или Администратор на инфраструктурата
    /// </summary>
    public class AdminPolicyRequirement : AuthorizationHandler<AdminPolicyRequirement>, IAuthorizationRequirement
    {
        public const string Name = "AdminPolicyRequirement";

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminPolicyRequirement requirement)
        {
            if (context.User == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }
            int userType = context.User.GetUserType();

            if (userType != NomenclatureConstants.UserTypes.GlobalAdmin && userType != NomenclatureConstants.UserTypes.Administrator)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}

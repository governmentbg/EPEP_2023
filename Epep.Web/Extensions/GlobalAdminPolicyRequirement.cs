using Epep.Core.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Epep.Web.Extensions
{
    /// <summary>
    /// Проверка за потребител от тип Администратор на инфраструктурата
    /// </summary>
    public class GlobalAdminPolicyRequirement : AuthorizationHandler<GlobalAdminPolicyRequirement>, IAuthorizationRequirement
    {
        public const string Name = "GlobalAdminPolicy";

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, GlobalAdminPolicyRequirement requirement)
        {
            if (context.User == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }
            int userType = context.User.GetUserType();

            if (userType != NomenclatureConstants.UserTypes.GlobalAdmin)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}

using Epep.Core.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Epep.Web.Extensions
{
    /// <summary>
    /// Проверка за потребител от тип Адвокат
    /// </summary>
    public class LawyerPolicyRequirement : AuthorizationHandler<LawyerPolicyRequirement>, IAuthorizationRequirement
    {
        public const string Name = "LawyerPolicy";

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, LawyerPolicyRequirement requirement)
        {
            if (context.User == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }
            int userType = context.User.GetUserType();

            if (userType != NomenclatureConstants.UserTypes.Lawyer)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}

using Epep.Core.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Epep.Web.Extensions
{
    /// <summary>
    /// Проверка за потребител от тип Представляващ на организация
    /// </summary>
    public class OrganizationRepresentativePolicyRequirement : AuthorizationHandler<OrganizationRepresentativePolicyRequirement>, IAuthorizationRequirement
    {
        public const string Name = "OrganizationRepresentativePolicy";

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OrganizationRepresentativePolicyRequirement requirement)
        {
            if (context.User == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }
            int userType = context.User.GetUserType();

            if (userType != NomenclatureConstants.UserTypes.OrganizationRepresentative)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}

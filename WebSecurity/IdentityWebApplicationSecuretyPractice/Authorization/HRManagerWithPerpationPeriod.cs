using Microsoft.AspNetCore.Authorization;

namespace IdentityWebApplicationSecuretyPractice.Authorization
{
    public class HRManagerWithPerpationPeriod : IAuthorizationRequirement
    {
        public HRManagerWithPerpationPeriod(int periodInMonth)
        {
            PeriodInMonth = periodInMonth;
        }

        public int PeriodInMonth { get; }
    }

    public class HRManagerWithPerpationPeriodHandler : AuthorizationHandler<HRManagerWithPerpationPeriod>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HRManagerWithPerpationPeriod requirement)
        {
            if (!context.User.HasClaim(c => c.Type == "EmployeeHiringDate"))
                return Task.CompletedTask;

            if (DateTime.TryParse(context.User.FindFirst(c => c.Type == "EmployeeHiringDate")?.Value, out DateTime employeeHiringDate))
            {
                var period = DateTime.Now - employeeHiringDate;
                if (period.Days > 30 * requirement.PeriodInMonth)
                    context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}

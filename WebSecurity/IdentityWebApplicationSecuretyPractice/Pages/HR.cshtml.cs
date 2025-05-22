using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityWebApplicationSecuretyPractice.Pages
{
    [Authorize(Policy = "MustBelongToHrDepartmentAndBeManger")]
    public class HRModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}

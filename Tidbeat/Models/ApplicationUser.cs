using Microsoft.AspNetCore.Identity;

namespace Tidbeat.Models {
    public class ApplicationUser : IdentityUser {
        [PersonalData]
        public string FullName { get; set; }
    }
}

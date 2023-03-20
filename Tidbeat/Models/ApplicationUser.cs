using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

namespace Tidbeat.Models {
    public class ApplicationUser : IdentityUser {
        [PersonalData]
        public string FullName { get; set; }

        [PersonalData]
        public DateTime BirthdayDate { get; set; }

        [PersonalData]
        public string Gender { get; set; }

        [PersonalData]
        public string? FavoriteSongId { get; set; }

        [PersonalData]
        public string? AboutMe { get; set; }

        [PersonalData]
        public string? ImagePath { get; set; }
    }
}

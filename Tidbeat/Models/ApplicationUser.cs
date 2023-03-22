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
        public string? FavoriteSongIds { get; set; } = "";

        [PersonalData]
        public string? AboutMe { get; set; }

        [PersonalData]
        public string? ImagePath { get; set; }

        public List<string> DeserializeFavoriteSongIds()
        {
            if (FavoriteSongIds == null)
            {
                return new List<string>();
            }

            return FavoriteSongIds.Split(',').Where(s => s.Length > 1).ToList();
        }

        public void SerializeFavoriteSongIds(List<string> songIds)
        {
            FavoriteSongIds = string.Join(',', songIds.Where(s => s.Length > 1));

            // Workaround to not allow invalid IDs (considering them IDs with one character or less)

        }
    }
}

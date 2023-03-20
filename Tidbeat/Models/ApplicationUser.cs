using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

namespace Tidbeat.Models {
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string FullName { get; set; }

        [PersonalData]
        public DateTime BirthdayDate { get; set; }

        [PersonalData]
        public string Gender { get; set; }

        // Comma separated list of song ids
        [PersonalData]
        public string? FavoriteSongIds { get; set; }

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

            return FavoriteSongIds.Split(',').ToList();
        }

        public void SerializeFavoriteSongIds(List<string> songIds)
        {
            FavoriteSongIds = string.Join(',', songIds);
        }

        
    }
}

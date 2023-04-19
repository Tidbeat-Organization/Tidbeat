using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;
using Tidbeat.Enums;

namespace Tidbeat.Models {
    /// <summary>
    /// The user model.
    /// </summary>
    public class ApplicationUser : IdentityUser {
        /// <summary>
        /// The user's full name.
        /// </summary>
        [PersonalData]
        public string FullName { get; set; }

        /// <summary>
        /// The user's birthday date.
        /// </summary>
        [PersonalData]
        public DateTime BirthdayDate { get; set; }

        /// <summary>
        /// The user's gender.
        /// </summary>
        [PersonalData]
        public string Gender { get; set; }

        /// <summary>
        /// The user's favorited song IDs.
        /// </summary>
        [PersonalData]
        public string? FavoriteSongIds { get; set; }

        /// <summary>
        /// The user's about me text.
        /// </summary>
        [PersonalData]
        public string? AboutMe { get; set; }

        /// <summary>
        /// The user's image path. The path is always assuming the root is wwwroot/.
        /// </summary>
        [PersonalData]
        public string? ImagePath { get; set; }

        /// <summary>
        /// The user's favorite genre.
        /// </summary>
        [PersonalData]
        public string? FavoriteGenre { get; set; }

        /// <summary>
        /// The user's country.
        /// </summary>
        [PersonalData]
        public string? Country { get; set; }

        public bool? IsBanned { get; set; }

        public string? reason { get; set; }

        public List<BanUser>? Bans { get; set; }
        public RoleType Role { get; set; }


        public string FirstName() {
            return FullName.Split(' ')[0];
        }

        public string LastName() {
            var allNames = FullName.Split(' ');
            return allNames[allNames.Length - 1];
        }

        public string FirstAndLastName() { 
            return FirstName() + " " + LastName();
        }

        /// <summary>
        /// Function which deserializes the FavoriteSongIds string into a list of strings.
        /// </summary>
        /// <returns></returns>
        public List<string> DeserializeFavoriteSongIds()
        {
            if (string.IsNullOrEmpty(FavoriteSongIds))
            {
                return new List<string>();
            }

            return FavoriteSongIds.Split(',').Where(s => s.Length > 1).ToList();
        }

        /// <summary>
        /// Function which serializes a list of strings into a string.
        /// </summary>
        /// <param name="songIds"></param>
        public void SerializeFavoriteSongIds(List<string> songIds)
        {
            FavoriteSongIds = string.Join(',', songIds.Where(s => s.Length > 1));

            // Workaround to not allow invalid IDs (considering them IDs with one character or less)

        }
    }
}

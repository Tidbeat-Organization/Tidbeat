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

        /// <summary>
        /// A boolean about the ban status of the user.
        /// </summary>
        public bool? IsBanned { get; set; }

        /// <summary>
        /// A reason for the ban status.
        /// </summary>
        public string? reason { get; set; }

        /// <summary>
        /// A list of bans a user may have.
        /// </summary>
        public List<BanUser>? Bans { get; set; }

        /// <summary>
        /// The current role of the user.
        /// </summary>
        public RoleType Role { get; set; }

        public DateTime? CreationDate { get; set; }

        /// <summary>
        /// A method for fetching a formatted FirstName.
        /// </summary>
        /// <returns></returns>
        public string FirstName() {
            return FullName.Split(' ')[0];
        }

        /// <summary>
        /// A method for fetching a formatted LastName.
        /// </summary>
        /// <returns></returns>
        public string LastName() {
            var allNames = FullName.Trim().Split(' ');
            if (allNames.Length == 1) {
                return "";
            }
            return allNames[allNames.Length - 1];
        }

        /// <summary>
        /// A method for fetching a concatenation of the first and last names.
        /// </summary>
        /// <returns></returns>
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

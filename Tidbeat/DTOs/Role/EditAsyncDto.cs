using Tidbeat.Enums;

namespace Tidbeat.DTOs.Role {
    /// <summary>
    /// The data transfer object for the EditAsync method in the RoleController.
    /// </summary>
    public class EditAsyncDto {
        /// <summary>
        /// The id of the user about to be edited.
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// The new name of the user.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The new about me of the user.
        /// </summary>
        public string? About { get; set; }

        /// <summary>
        /// The new role of the user.
        /// </summary>
        public string? RoleType { get; set; }

        /// <summary>
        /// A boolean about whether the user's photo should be deleted.
        /// </summary>
        public bool? ShouldDeletePhoto { get; set; }

        /// <summary>
        /// The value of time a user should be banned for.
        /// </summary>
        public double? BanNumber { get; set; }

        /// <summary>
        /// The time units a user should be banned for.
        /// </summary>
        public string? BanTime { get; set; }

        /// <summary>
        /// The reason for banning the user.
        /// </summary>
        public string? BanReason { get; set; }
    }
}

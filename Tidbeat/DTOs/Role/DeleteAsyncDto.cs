namespace Tidbeat.DTOs.Role {
    /// <summary>
    /// The data transfer object for the DeleteAsync method in the RoleController.
    /// </summary>
    public class DeleteAsyncDto {
        /// <summary>
        /// The id of the user about to be deleted.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The reason for deleting the user.
        /// </summary>
        public string? Reason { get; set; }
    }
}

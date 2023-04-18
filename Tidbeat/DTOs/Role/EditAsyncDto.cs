using Tidbeat.Enums;

namespace Tidbeat.DTOs.Role {
    public class EditAsyncDto {
        public string? UserId { get; set; }
        public string? Name { get; set; }
        public string? About { get; set; }
        public string? RoleType { get; set; }
        public bool? ShouldDeletePhoto { get; set; }
        public double? BanNumber { get; set; }
        public string? BanTime { get; set; }
        public string? BanReason { get; set; }
    }
}

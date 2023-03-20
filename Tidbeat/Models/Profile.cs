using MessagePack;
using Microsoft.AspNetCore.Identity;

namespace Tidbeat.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public ApplicationUser User { get; set; }

    }
}

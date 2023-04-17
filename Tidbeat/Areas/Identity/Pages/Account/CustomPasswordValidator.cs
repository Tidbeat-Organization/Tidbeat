using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using Tidbeat.Models;

namespace Tidbeat.Areas.Identity.Pages.Account {
    public class CustomPasswordValidator<TUser> : IPasswordValidator<TUser> where TUser : class {
        public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password) {
            var errors = new List<string>();

            // Does not start with an underscore
            if (password.StartsWith("_")) {
                errors.Add("Password cannot start with an underscore.");
            }

            // Contains at least one lowercase letter
            if (!Regex.IsMatch(password, @"[a-z]")) {
                errors.Add("Password must contain at least one lowercase letter.");
            }

            // Contains at least one uppercase letter
            if (!Regex.IsMatch(password, @"[A-Z]")) {
                errors.Add("Password must contain at least one uppercase letter.");
            }

            // Contains at least one digit
            if (!Regex.IsMatch(password, @"\d")) {
                errors.Add("Password must contain at least one digit.");
            }

            // Has a minimum length of 6 characters
            if (password.Length < 6) {
                errors.Add("Password must be at least 6 characters long.");
            }

            if (errors.Count > 0) {
                List<IdentityError> identityErrors = new List<IdentityError>();
                foreach (var errorMessage in errors) {
                    identityErrors.Add(new IdentityError {
                        Description = errorMessage
                    });
                }
                return IdentityResult.Failed(identityErrors.ToArray());
            }

            return IdentityResult.Success;
        }
    }

}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Tidbeat.Areas.Identity.Pages.Account.Manage {
    public class EditPhotoModel : PageModel {
        public void OnGet() {

        }

        public async Task<IActionResult> OnPostEditAsync() {
            var photoFile = Request.Form.Files.GetFile("photoFile");
            if (photoFile != null && photoFile.Length > 0) {
                // Save the file to wwwroot/images
                var fileName = $"{Guid.NewGuid()}_{photoFile.FileName}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "user_images", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create)) {
                    await photoFile.CopyToAsync(stream);
                }

                // Update the photo with the new file name
                // logic to update the photo with the new file name goes here
            }

            // Redirect back to the index page
            return RedirectToPage("/Photos/Index");
        }


        public IActionResult OnPostDelete() {
            // logic to delete the photo goes here
            return RedirectToPage("/Photos/Index");
        }

        public async Task<IActionResult> OnPost() {
            if (Request.Form.ContainsKey("editButton")) {
                return await OnPostEditAsync();
            }
            else if (Request.Form.ContainsKey("deleteButton")) {
                return OnPostDelete();
            }
            else {
                return Page();
            }
        }
    }
}

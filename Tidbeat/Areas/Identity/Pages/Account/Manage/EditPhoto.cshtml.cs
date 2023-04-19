using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tidbeat.Controllers;
using Tidbeat.Models;

namespace Tidbeat.Areas.Identity.Pages.Account.Manage {
    /// <summary>
    /// The model class for the EditPhoto page.
    /// </summary>
    [IgnoreAntiforgeryToken]
    public class EditPhotoModel : PageModel {
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// The application user.
        /// </summary>
        public ApplicationUser ApplicationUser { get; private set; }

        /// <summary>
        /// The constructor for the EditPhoto model.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        public EditPhotoModel(UserManager<ApplicationUser> userManager) {
            _userManager = userManager;
        }

        /// <summary>
        /// The input model for the EditPhoto page.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetAsync() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            ApplicationUser = user;

            if (User != null && User.Identity.IsAuthenticated)
            {
                var userr = await _userManager.GetUserAsync(User);
                var request = HttpContext.Request;
                var currentUrl = string.Format("{0}://{1}", request.Scheme, request.Host);
                TempData["Friends"] = await UtilityClass.SideBarAsync(userr.Id, currentUrl);
            }

            return Page();
        }

        /// <summary>
        /// The post method for the EditPhoto page. Encodes the image and saves it.
        /// </summary>
        /// <param name="ImgStr">The coded image.</param>
        /// <param name="ImgName">The name of the image.</param>
        /// <returns>The page itself.</returns>
        public async Task<IActionResult> OnPostSaveImage(string ImgStr, string ImgName) {
            string imageName = Guid.NewGuid() + "_" + ImgName + ".jpg";

            //set the image path
            string imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "user_images", imageName);

            var base64Data = ImgStr.Split(',')[1];

            byte[] imageBytes = Convert.FromBase64String(base64Data);
            using (var stream = new FileStream(imgPath, FileMode.Create)) {
                stream.Write(imageBytes, 0, imageBytes.Length);
            }

            await OnPostDelete();

            var currentUser = await _userManager.GetUserAsync(User);
            currentUser.ImagePath = "\\" + Path.Combine("images", "user_images", imageName);
            await _userManager.UpdateAsync(currentUser);

            return Page();
        }

        /// <summary>
        /// The post method for the EditPhoto page. Saves the image to the server.
        /// </summary>
        /// <returns>A redirect to the page itself..</returns>
        public async Task<IActionResult> OnPostEditAsync() {
            var photoFile = Request.Form.Files.GetFile("photoFile");
            if (photoFile != null && photoFile.Length > 0) {
                // Save the file to wwwroot/images
                var fileName = $"{Guid.NewGuid()}_{photoFile.FileName}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "user_images", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create)) {
                    await photoFile.CopyToAsync(stream);
                }

                var currentUser = await _userManager.GetUserAsync(User);
                currentUser.ImagePath = "\\" + Path.Combine("images", "user_images", fileName);
                await _userManager.UpdateAsync(currentUser);
            }

            // Redirect back to the index page
            return Redirect("~/Identity/Account/Manage/EditPhoto");
        }

        /// <summary>
        /// The post method for the EditPhoto page. Deletes the image in the server.
        /// </summary>
        /// <returns>A redirect to the page itself.</returns>
        public async Task<IActionResult> OnPostDelete() {
            var currentUser = await _userManager.GetUserAsync(User);
            if (!string.IsNullOrEmpty(currentUser.ImagePath)) {
                var currentDirectory = Directory.GetCurrentDirectory();
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", currentUser.ImagePath.TrimStart('\\'));
                if (System.IO.File.Exists(filePath)) {
                    System.IO.File.Delete(filePath);
                    currentUser.ImagePath = null;
                    await _userManager.UpdateAsync(currentUser);
                }
            }
            return Redirect("~/Identity/Account/Manage/EditPhoto");
        }

        /// <summary>
        /// The post method for the EditPhoto page. Checks which button was pressed.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPost() {
            if (Request.Form.ContainsKey("saveButtonSubmit")) {
                return await OnPostSaveImage(Request.Form["photoString"], Request.Form["photoName"]);
            }
            else if (Request.Form.ContainsKey("deleteButtonModal")) {
                return await OnPostDelete();
            }
            else {
                return Page();
            }
        }

        /// <summary>
        /// Gets the user's photo url.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetUserPhotoUrlAsync() {
            var currentUser = await _userManager.GetUserAsync(User);
            return currentUser.ImagePath;
        }
    }
}

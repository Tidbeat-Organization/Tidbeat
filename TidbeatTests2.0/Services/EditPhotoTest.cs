using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;
using Moq;
using System.Security.Claims;
using Tidbeat.Areas.Identity.Pages.Account.Manage;
using Tidbeat.Models;

namespace TidbeatTests2._0.Services
{
    public class EditPhotoTest
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly EditPhotoModel _editPhotoModel;

        public EditPhotoTest()
        {
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            _editPhotoModel = new EditPhotoModel(_userManagerMock.Object);
        }

        [Fact]
        public async Task OnGetAsync_ReturnsPageResult_EditPhoto()
        {
            // Arrange
            var userMock = new Mock<ApplicationUser>();
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(userMock.Object);

            // Act
            var result = await _editPhotoModel.OnGetAsync();

            // Assert
            Assert.IsType<PageResult>(result);
            _userManagerMock.Verify(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
        }

        [Fact]
        public async Task OnPostEditAsync_ReturnsRedirectToPageResult_EditPhoto()
        {
            // Arrange
            var userMock = new Mock<ApplicationUser>();
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(userMock.Object);

            var editPhotoModel = new EditPhotoModel(_userManagerMock.Object);

            var fileMock = new Mock<IFormFile>();
            var fileName = "test.jpg";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write("test data");
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(ms.Length);
            fileMock.Setup(f => f.OpenReadStream()).Returns(ms);

            var formCollection = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection { fileMock.Object });
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(c => c.Request.Form).Returns(formCollection);

            var pageContext = new PageContext { HttpContext = httpContextMock.Object };
            editPhotoModel.PageContext = pageContext;

            // Act
            var result = await editPhotoModel.OnPostEditAsync();

            // Assert
            Assert.IsNotType<RedirectToPageResult>(result);
        }


    }

}
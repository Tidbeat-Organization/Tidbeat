using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using Tidbeat.Areas.Identity.Pages.Account.Manage;
using Tidbeat.Models;
using Xunit;

namespace TidbeatTests2._0.Services {
    public class IndexManageTest {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly IndexModel _indexModel;

        public IndexManageTest() {
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(_userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(), null, null, null, null);

            var mockStringLocalizer = new Mock<IStringLocalizer<IndexModel>>();

            // Define the localized string
            var localizedString = "Hello, world!";

            // Set up the behavior of the Moq object
            mockStringLocalizer.Setup(sl => sl[It.IsAny<string>()])
                              .Returns(new LocalizedString("your_profile_has_been_updated", localizedString));
            _indexModel = new IndexModel(_userManagerMock.Object, _signInManagerMock.Object, mockStringLocalizer.Object);
        }

        [Fact]
        public async Task OnGetAsync_ReturnsPageResult_Index() {
            // Arrange
            var userId = "1";
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(() => new ApplicationUser());

            // Act
            var result = await _indexModel.OnGetAsync();

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_ReturnsRedirect_Index() {
            // Arrange
            var userId = "1";
            var user = new ApplicationUser();
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(() => user);
            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);
            _signInManagerMock.Setup(x => x.RefreshSignInAsync(It.IsAny<ApplicationUser>()))
                .Returns(Task.CompletedTask);

            _indexModel.Input = new IndexModel.InputModel {
                FullName = "John Doe",
                BirthdayDate = new DateTime(1990, 1, 1),
                Gender = "Male",
                AboutMe = "Hello world",
                FavoriteGenre = "Action",
                Country = "USA"
            };

            // Act
            var result = await _indexModel.OnPostAsync();

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            var redirectResult = (RedirectToPageResult)result;
            _signInManagerMock.Verify(x => x.RefreshSignInAsync(user), Times.Once);
            _userManagerMock.Verify(x => x.UpdateAsync(user), Times.Once);
        }

    }
}

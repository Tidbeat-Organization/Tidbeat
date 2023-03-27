using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tidbeat.Areas.Identity.Pages.Account.Manage;
using Tidbeat.Models;

namespace TidbeatTests2._0.Services {
    public class ChangePasswordModelTests {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly Mock<ILogger<ChangePasswordModel>> _loggerMock;
        private readonly Mock<IStringLocalizer<ChangePasswordModel>> _localizerMock;

        public ChangePasswordModelTests() {
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null, null, null, null, null, null, null, null);

            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                _userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null, null, null, null);

            _loggerMock = new Mock<ILogger<ChangePasswordModel>>();
            _localizerMock = new Mock<IStringLocalizer<ChangePasswordModel>>();
        }

        [Fact]
        public async Task OnGetAsync_ReturnsPageResult_ChangePassword() {
            // Arrange
            var model = new ChangePasswordModel(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _loggerMock.Object,
                _localizerMock.Object);

            var user = new ApplicationUser { Id = "1" };
            _userManagerMock.Setup(u => u.GetUserAsync(null)).ReturnsAsync(user);
            _userManagerMock.Setup(u => u.HasPasswordAsync(user)).ReturnsAsync(true);

            // Act
            var result = await model.OnGetAsync();

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_ReturnsPageResult_WhenModelStateIsValid() {
            // Arrange
            var model = new ChangePasswordModel(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _loggerMock.Object,
                _localizerMock.Object);

            model.Input = new ChangePasswordModel.InputModel {
                OldPassword = "oldpassword",
                NewPassword = "Newp@ssw0rd",
                ConfirmPassword = "Newp@ssw0rd"
            };

            var user = new ApplicationUser { Id = "user1" };

            // set up the _userManagerMock to return the user when GetUserAsync is called
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.ChangePasswordAsync(user, "oldpassword", "Newp@ssw0rd"))
    .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await model.OnPostAsync();

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
        }


    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Primitives;
using Moq;
using System.Security.Claims;
using Tidbeat.Areas.Identity.Pages.Account.Manage;
using Tidbeat.Models;

namespace TidbeatTests2._0.Services
{
    public class EmailTest
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManager;
        private readonly Mock<IEmailSender> _emailSender;
        private readonly Mock<IStringLocalizer<EmailModel>> _localizer;
        public EmailTest()
        {
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            _signInManager = new Mock<SignInManager<ApplicationUser>>();
            _emailSender = new Mock<IEmailSender>();
            _localizer = new Mock<IStringLocalizer<EmailModel>>();
        }

        [Fact]
        public async Task OnGetAsync_ReturnsRedirectToPageResult_Email()
        {
            // Arrange
            var userMock = new Mock<ApplicationUser>();
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(userMock.Object);

            var emailModel = new EmailModel(_userManagerMock.Object, null, null,_localizer.Object);


            // Act
            var result = await emailModel.OnGetAsync();

            // Assert
            Assert.IsNotType<RedirectToPageResult>(result);
        }


    }

}
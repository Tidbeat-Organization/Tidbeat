using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tidbeat.Controllers;
using Tidbeat.Data;
using Microsoft.AspNetCore.Identity;
using Tidbeat.Models;

namespace TidbeatTests2._0.Services
{
    public class ReportControllerTest
    {

        private ApplicationDbContext _context;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;

        public ReportControllerTest()
        {
            var fixture = new ApplicationDbContextFixture();
            _context = fixture.ApplicationDbContext;
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null, null, null, null, null, null, null, null
            );
        }

        [Fact]
        public async Task GetMultipleReportsTest_ReportsController()
        {
            // Arrange
            var controller = new ReportsController(_context, _mockUserManager.Object);
          
            // Act
            var result = await controller.Index("", "", "","","");

            // Assert
            var viewResult = Assert.IsAssignableFrom<IActionResult>(result);

        }

        [Fact]
        public async Task GetReportTest_ReportsController()
        {
            // Arrange
            var controller = new ReportsController(_context, _mockUserManager.Object);

            // Act
            var result = await controller.Details(new Guid());

            // Assert
            var viewResult = Assert.IsAssignableFrom<IActionResult>(result);

        }

        [Fact]
        public async Task CreateReportTest_ReportsController()
        {
            // Arrange
            var controller = new ReportsController(_context, _mockUserManager.Object);

            var report = new Report() {DetailedReason="yes",Status=Tidbeat.Enums.ReportStatus.Open,Reason=Tidbeat.Enums.ReportReason.Other,ReportItemType=Tidbeat.Enums.ReportedItemType.User };
            // Act
            var result = await controller.Details(new Guid());

            // Assert
            var viewResult = Assert.IsAssignableFrom<IActionResult>(result);

        }

    }
}

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
using Microsoft.EntityFrameworkCore;
using System.Reflection;

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
            var normalUser = new ApplicationUser
            {
                FullName = "Utilizador Normal",
                UserName = "user@gmail.com",
                Email = "user@gmail.com",
                BirthdayDate = DateTime.Now,
                Gender = "Masculino",
                FavoriteSongIds = null,
                PasswordHash = null,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                Role = Tidbeat.Enums.RoleType.NormalUser
            };
            _context.Users.Add(normalUser);
            var modUser = new ApplicationUser
            {
                FullName = "Utilizador Special",
                UserName = "userspecial@gmail.com",
                Email = "userspecial@gmail.com",
                BirthdayDate = DateTime.Now,
                Gender = "Masculino",
                FavoriteSongIds = null,
                PasswordHash = null,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                Role = Tidbeat.Enums.RoleType.Moderator
            };
            _context.Users.Add(normalUser);
            _context.Users.Add(modUser);
            var report = new Report()
            {
                Id = new Guid(),
                DetailedReason = "yes",
                Date = DateTime.Now,
                ModAssigned = modUser,
                Reason = Tidbeat.Enums.ReportReason.Other,
                ReportItemType = Tidbeat.Enums.ReportedItemType.User,
                ReportItemId = normalUser.Id,
                Status = Tidbeat.Enums.ReportStatus.Created,
                UserReported = normalUser,
                UserReporter = modUser
            };
            _context.Report.Add(report);
            _context.SaveChanges();
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
            var result = await controller.Details(_context.Report.FirstOrDefaultAsync().Result.Id);

            // Assert
            var viewResult = Assert.IsAssignableFrom<IActionResult>(result);

        }

        [Fact]
        public async Task CreateReportTest_ReportsController()
        {
            // Arrange
            var controller = new ReportsController(_context, _mockUserManager.Object);

            var user = await _context.Users.FirstOrDefaultAsync();
            var report = new Report()
            {
                Id = Guid.NewGuid(),
                DetailedReason = "yes",
                Date = DateTime.Now,
                ModAssigned = user,
                Reason = Tidbeat.Enums.ReportReason.Other,
                ReportItemType = Tidbeat.Enums.ReportedItemType.User,
                ReportItemId = user.Id,
                Status = Tidbeat.Enums.ReportStatus.Created,
                UserReported = user,
                UserReporter = user
            };
            
            // Act
            var result = await controller.Create(report);

            // Assert
            var viewResult = Assert.IsAssignableFrom<IActionResult>(result);
            Assert.Equal(1, _context.Report.ToListAsync().Result.Count);

        }

        [Fact]
        public async Task IndexAction_ReportsController() {
            var user1 = new ApplicationUser() {
                Id = Guid.NewGuid().ToString(),
                FullName = "John Smith",
                BirthdayDate = DateTime.Now,
                Gender = "male"
            };
            var user2 = new ApplicationUser() {
                Id = Guid.NewGuid().ToString(),
                FullName = "Jane Doe",
                BirthdayDate = DateTime.Now,
                Gender = "female"
            };
            var reports = new List<Report>
            {
                new Report { UserReported = user1, UserReporter = user2, Status = Tidbeat.Enums.ReportStatus.Created, Date = DateTime.Now, Reason = Tidbeat.Enums.ReportReason.Other },
                new Report { UserReported = user2, UserReporter = user1, Status = Tidbeat.Enums.ReportStatus.Created, Date = DateTime.Now, Reason = Tidbeat.Enums.ReportReason.HateSpeech },
            };
            foreach (var report in reports) {
                _context.Report.Add(report);
            }
            _context.SaveChanges();
            
            var _reportsController = new ReportsController(_context, _mockUserManager.Object);

            // Act
            var result = await _reportsController.Index("", "", "", "", "");
            var result1 = await _reportsController.Index("John", "", "", "", "");

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsType<ViewResult>(result1);
        }
    }
}

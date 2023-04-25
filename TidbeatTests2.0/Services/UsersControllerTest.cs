using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tidbeat.Controllers;
using Tidbeat.Data;
using Tidbeat.Models;

namespace TidbeatTests2._0.Services {
    public class UsersControllerTest {
        private ApplicationDbContext _context;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;

        public UsersControllerTest() {
            var fixture = new ApplicationDbContextFixture();
            _context = fixture.ApplicationDbContext;
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                               Mock.Of<IUserStore<ApplicationUser>>(),
                                              null, null, null, null, null, null, null, null
                                                         );
            var normalUser = new ApplicationUser {
                FullName = "Utilizador Normal",
                UserName = ""
            };
        }

        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfUsers() {
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
            _context.Users.Add(user1);
            _context.Users.Add(user2);
            _context.SaveChanges();
            var users = new List<ApplicationUser> { user1, user2 };
            _mockUserManager.Setup(u => u.Users)
                .Returns(users.AsQueryable());

            

            // Arrange
            var controller = new UsersController(_mockUserManager.Object);

            // Act
            var result = await controller.Index("", "", "");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
        }
    }
}

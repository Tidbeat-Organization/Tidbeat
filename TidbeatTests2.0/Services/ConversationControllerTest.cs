using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tidbeat.Controllers;
using Tidbeat.Data;
using Tidbeat.Models;
using Tidbeat.Services;

namespace TidbeatTests2._0.Services
{
    public class ConversationControllerTest
    {
        private readonly ApplicationDbContext _fixture;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;

        public ConversationControllerTest()
        {
            var fixture = new ApplicationDbContextFixture();
            _fixture = fixture.ApplicationDbContext;

            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null, null, null, null, null, null, null, null
            );
        }

        [Fact]
        public async Task UserDeletesConversations_ConversationsController()
        {
            var user = new ApplicationUser()
            {
                Id = "f1c1c9b9-ea8f-483d-b0a3-cf63085d3b3d",
                FullName = "User one",
                BirthdayDate = DateTime.Now,
                Gender = "male"
            };
            var user2 = new ApplicationUser()
            {
                Id = "f1c1c9b9-ea8f-483d-b0a3-cf63085d3b3e",
                FullName = "User two",
                BirthdayDate = DateTime.Now,
                Gender = "male"
            };
            var conversation = new Conversation()
            {
                Id = "d5e5c5fb-7a57-4a6d-a61d-b9e6b74cd037",
                StartDate = DateTime.Now,
                IsGroupConversation = true,
            };
            var participant = new Participant()
            {
                Conversation = conversation,
                User = user
            };
            var participant2 = new Participant()
            {
                Conversation = conversation,
                User = user2
            };
            var message = new Message
            {
                Id = 1,
                Text = "Test",
                Created = DateTime.Now,
                Status = 0,
                Conversation = conversation,
                User = user
            };

            _fixture.Users.Add(user);
            _fixture.Users.Add(user2);
            _fixture.Conversations.Add(conversation);
            _fixture.Participants.Add(participant);
            _fixture.Participants.Add(participant2);
            _fixture.Messages.Add(message);
            _fixture.SaveChanges();

            var chatBeatService = new ChatBeatService(_fixture, _mockUserManager.Object);
            _ = chatBeatService.RemoveMessageFromDatabase(message.Id, user.Id);

            var messageFromDb = _fixture.Messages.FirstOrDefault(m => m.Id == message.Id);

            var controller = new ConversationsController(_fixture, _mockUserManager.Object, chatBeatService, null);
            var taskResult = controller.ExitConversation(conversation.Id);
            _fixture.SaveChanges();
            Assert.Null(messageFromDb);
            var amount = _fixture.Conversations.Count();
            var firstElement = _fixture.Conversations.FirstOrDefault();
            Assert.Equal(0, amount);
            Assert.NotEqual(conversation, firstElement);

        }
        [Fact]
        public async Task GetAllConversations_ConversationsController()
        {
            var user = new ApplicationUser()
            {
                Id = "f1c1c9b9-ea8f-483d-b0a3-cf63085d3b3d",
                FullName = "User one",
                BirthdayDate = DateTime.Now,
                Gender = "male"
            };
            var secondUser = new ApplicationUser()
            {
                Id = Guid.NewGuid().ToString(),
                FullName = "User one",
                BirthdayDate = DateTime.Now,
                Gender = "male"
            };
            var conversation = new Conversation()
            {
                Id = "d5e5c5fb-7a57-4a6d-a61d-b9e6b74cd037",
                StartDate = DateTime.Now,
                IsGroupConversation = true,
            };
            var participant = new Participant()
            {
                Conversation = conversation,
                User = user
            };
            _fixture.Users.Add(user);
            _fixture.Users.Add(secondUser);
            var resultdb = _fixture.Conversations.Add(conversation);
            _fixture.Participants.Add(participant);
            for (int i = 0; i < 30; i++)
            {
                var message = new Message
                {
                    Text = "Test" + i,
                    Created = DateTime.Now,
                    Status = 0,
                    Conversation = conversation,
                    User = user
                };
                if (i % 3 == 0)
                {
                    message.User = secondUser;
                }
                _fixture.Messages.Add(message);
            }
            _fixture.SaveChanges();
            var chatBeatService = new ChatBeatService(_fixture, _mockUserManager.Object);
            Assert.NotNull(resultdb);
            Assert.Equal(typeof(EntityEntry<Conversation>), resultdb.GetType());
            Assert.Equal(typeof(Conversation), conversation.GetType());
        }
    }
}

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

namespace TidbeatTests2._0.Services {
    public class ChatBeatServiceTest
    {
        private readonly ApplicationDbContext _fixture;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;

        public ChatBeatServiceTest()
        {
            var fixture = new ApplicationDbContextFixture();
            _fixture = fixture.ApplicationDbContext;

            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null, null, null, null, null, null, null, null
            );
        }

        [Fact]
        public async Task AddMessageToDatabaseTest_ChatBeatService()
        {
            // Arrange
            var chatBeatService = new ChatBeatService(_fixture, _mockUserManager.Object);

            var user = new ApplicationUser()
            {
                Id = "f1c1c9b9-ea8f-483d-b0a3-cf63085d3b3d",
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
            var message = new Message
            {
                Text = "Test",
                Created = DateTime.Now,
                User = user,
                Status = 0,
                Conversation = conversation,
            };

            _fixture.Users.Add(user);
            //_fixture.Messages.Add(message);
            _fixture.Conversations.Add(conversation);
            _fixture.Participants.Add(participant);
            _fixture.SaveChanges();

            _mockUserManager.Setup(u => u.FindByIdAsync(user.Id)).ReturnsAsync(user);

            // Act
            await chatBeatService.AddMessageToDatabase(conversation.Id, user.Id, "Test");

            // Assert
            var messageFromDb = _fixture.Messages.FirstOrDefault(m => m.Text == message.Text);
            Assert.NotNull(messageFromDb);
            Assert.Equal(message.Text, messageFromDb.Text);
            //Assert.Equal(message.Created, messageFromDb.Time);
            Assert.Equal(message.User.Id, messageFromDb.User.Id);
        }

        [Fact]
        public void UserEditsMessageTest_ChatBeatService()
        {
            var user = new ApplicationUser()
            {
                Id = "f1c1c9b9-ea8f-483d-b0a3-cf63085d3b3d",
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
            _fixture.Conversations.Add(conversation);
            _fixture.Participants.Add(participant);
            _fixture.Messages.Add(message);
            _fixture.SaveChanges();

            var chatBeatService = new ChatBeatService(_fixture, _mockUserManager.Object);
            _ = chatBeatService.EditMessageInDatabase(user.Id, message.Id, "Edited message");

            var messageFromDb = _fixture.Messages.FirstOrDefault(m => m.Id == message.Id);
            Assert.NotNull(messageFromDb);
            Assert.Equal("Edited message", messageFromDb.Text);
        }

        [Fact]
        public void UserRemovesMessageTest_ChatBeatService()
        {
            var user = new ApplicationUser()
            {
                Id = "f1c1c9b9-ea8f-483d-b0a3-cf63085d3b3d",
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
            _fixture.Conversations.Add(conversation);
            _fixture.Participants.Add(participant);
            _fixture.Messages.Add(message);
            _fixture.SaveChanges();

            var chatBeatService = new ChatBeatService(_fixture, _mockUserManager.Object);
            _ = chatBeatService.RemoveMessageFromDatabase(message.Id, user.Id);

            var messageFromDb = _fixture.Messages.FirstOrDefault(m => m.Id == message.Id);
            Assert.Null(messageFromDb);
        }

        [Fact]
        public async Task Get20MostRecentMessagesTest_ChatBeatService()
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
            var participant2 = new Participant()
            {
                Conversation = conversation,
                User = secondUser
            };
            _fixture.Users.Add(user);
            _fixture.Users.Add(secondUser);
            _fixture.Conversations.Add(conversation);
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
            if (_fixture.Messages.Count() != 30)
            {
                throw new Exception("Messages not added");
            }



            var chatBeatService = new ChatBeatService(_fixture, _mockUserManager.Object);
            var messages = await chatBeatService.GetRecentMessages(conversation.Id, 20, 0);

            if (messages.Count() != 20)
            {
                throw new Exception("Messages not added");
            }

            Assert.Equal(20, messages.Count());
            Assert.Equal("Test29", messages[0].Text);
            Assert.Equal("Test28", messages[1].Text);
            Assert.Equal("Test27", messages[2].Text);
            Assert.Equal("Test10", messages[19].Text);
        }

        [Fact]
        public async Task UserCreatesConversation_ConversationsService()
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
            var result = _fixture.Conversations.Add(conversation);
            _fixture.Participants.Add(participant);
            _fixture.SaveChanges();
            var chatBeatService = new ChatBeatService(_fixture, _mockUserManager.Object);
            Assert.NotNull(result);
            Assert.Equal(typeof(EntityEntry<Conversation>), result.GetType());
            Assert.Equal(typeof(Conversation), conversation.GetType());
        }


    }
}

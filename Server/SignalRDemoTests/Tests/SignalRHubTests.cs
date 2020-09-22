using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using SignalRDemo.Hub.Interfaces;
using SignalRDemo.Hubs;
using SignalRDemo.Models;

using Xunit;

namespace SignalRDemoTests.Tests
{
    public class SignalRHubTests
    {
        #region Constants

        public const string ConnectionId = "zJr9DJOXwHTgZvE-Ah8jIA292e06ac1";

        /// <summary>
        /// The general chat group.
        /// </summary>
        private const string generalChatGroup = "generalChatGroup";

        #endregion

        #region Fields

        /// <summary>
        /// The user identifier for notifications.
        /// </summary>
        public static Guid userId = Guid.Parse("e49d5dd6-af4e-4fef-83b8-436014a03698");

        #endregion

        #region Mocks

        /// <summary>
        /// Hub Caller Context.
        /// </summary>
        public static readonly Mock<HubCallerContext> hubCallerContextMock = new Mock<HubCallerContext>();

        /// <summary>
        /// The hub context mock.
        /// </summary>
        public static readonly Mock<IHubContext<SignalRHub, ISignalRHub>> hubContextMock = new Mock<IHubContext<SignalRHub, ISignalRHub>>();

        /// <summary>
        /// The mock clients.
        /// </summary>
        public static readonly Mock<IHubCallerClients<ISignalRHub>> mockClients = new Mock<IHubCallerClients<ISignalRHub>>();

        /// <summary>
        /// The mock group manager.
        /// </summary>
        public static readonly Mock<IGroupManager> mockGroupManager = new Mock<IGroupManager>();

        /// <summary>
        /// The signalR hub mock.
        /// </summary>
        public static readonly Mock<ISignalRHub> signalRHubMock = new Mock<ISignalRHub>();

        #endregion

        #region Tests

        /// <summary>
        /// Should broadcast message on connected asynchronous.
        /// </summary>
        [Fact]
        public async Task ShouldBroadCastMessageOnConnectedAsync()
        {
            using (SignalRHub signalRHub = BuildHub())
            {
                // Setup received message

                string clientMessage = string.Format("Connected to the SignalR Hub! ConnectionId: " + hubCallerContextMock.Object.ConnectionId);

                // Call OnConnectedAsync

                await signalRHub.OnConnectedAsync();

                // Verify

                mockClients.Verify(client => client.Caller.BroadcastMessage(It.Is<string>(message => message == clientMessage)), Times.Once());
            };
        }

        /// <summary>
        /// Should broadcast new message to the chat room.
        /// </summary>
        [Fact]
        public async Task ShouldBroadcastMessageToChatRoom()
        {
            using (SignalRHub signalRHub = BuildHub())
            {
                // Setup received message

                ChatMessage newMessage = new ChatMessage()
                {
                    Id = hubCallerContextMock.Object.ConnectionId,
                    Payload = "Test message!",
                    Date = DateTime.UtcNow
                };

                // Call New Message

                await signalRHub.NewMessage("Test message!");

                // Verify if message was broadcasted to group

                mockClients.Verify(clients => clients.Group(generalChatGroup).BroadcastMessage(It.Is<ChatMessage>(message => message.Payload == newMessage.Payload)), Times.Once());
            };
        }

        /// <summary>
        /// Should join chat room.
        /// </summary>
        [Fact]
        public async Task ShouldJoinChatRoom()
        {
            using (SignalRHub signalRHub = BuildHub())
            {
                // Setup received message

                ChatMessage chatMessage = new ChatMessage()
                {
                    Id = "ChatBot",
                    Payload = "Client with ID: " + hubCallerContextMock.Object.ConnectionId + " joined the chat!",
                    Date = DateTime.UtcNow
                };

                // Call Join Chat Room

                await signalRHub.JoinChatRoom();

                // Verify if the user was added to the group

                mockGroupManager.Verify(group => group.AddToGroupAsync(It.Is<string>(connectionId => connectionId == hubCallerContextMock.Object.ConnectionId), It.Is<string>(groupName => groupName == generalChatGroup), It.IsAny<CancellationToken>()), Times.Once());

                // Verify if message was broadcasted to group

                mockClients.Verify(clients => clients.Group(generalChatGroup).BroadcastMessage(It.Is<ChatMessage>(message => message.Payload == chatMessage.Payload)), Times.Once());
            };
        }

        /// <summary>
        /// Should leave chat room.
        /// </summary>
        [Fact]
        public async Task ShouldLeaveChatRoom()
        {
            using (SignalRHub signalRHub = BuildHub())
            {
                // Setup received message

                ChatMessage chatMessage = new ChatMessage()
                {
                    Id = "ChatBot",
                    Payload = "Client with ID: " + hubCallerContextMock.Object.ConnectionId + " has left the chat!",
                    Date = DateTime.UtcNow
                };

                // Call Leave Chat Room

                await signalRHub.LeaveChatRoom();

                // Verify if the user was added to the group

                mockGroupManager.Verify(group => group.RemoveFromGroupAsync(It.Is<string>(connectionId => connectionId == hubCallerContextMock.Object.ConnectionId), It.Is<string>(groupName => groupName == generalChatGroup), It.IsAny<CancellationToken>()), Times.Once());

                // Verify if message was broadcasted to group

                mockClients.Verify(clients => clients.Group(generalChatGroup).BroadcastMessage(It.Is<ChatMessage>(message => message.Payload == chatMessage.Payload)), Times.Once());
            };
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Builds the signalR hub.
        /// </summary>
        /// <returns>The SinglarHub manager.</returns>
        public SignalRHub BuildHub()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddLogging();

            // Add dependency

            ////services.AddSingleton(someService);

            IServiceProvider provider = services.BuildServiceProvider();

            // Setup the hub

            hubCallerContextMock.Setup(context => context.ConnectionId).Returns(ConnectionId);
            mockClients.Setup(clients => clients.Caller).Returns(signalRHubMock.Object);
            mockClients.Setup(clients => clients.Group(It.IsAny<string>()).BroadcastMessage(It.IsAny<string>())).Returns(Task.CompletedTask);

            // Create Hub

            SignalRHub signalRHub = new SignalRHub(provider)
            {
                Clients = mockClients.Object,
                Context = hubCallerContextMock.Object,
                Groups = mockGroupManager.Object
            };

            return signalRHub;
        }

        #endregion
    }
}
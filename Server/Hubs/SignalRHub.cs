using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using SignalRDemo.Models;

namespace SignalRDemo.Hubs
{
    /// <summary>
    /// SignalR Hub.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.SignalR.Hub"/>
    public class SignalRHub : Hub
    {
        #region Constants

        /// <summary>
        /// The broadcast method name.
        /// </summary>
        private const string broadcastMethodName = "broadcastChannel";

        /// <summary>
        /// The chat group
        /// </summary>
        private const string chatGroup = "chatGroup";

        /// <summary>
        /// The messages method name.
        /// </summary>
        private const string messagesMethodName = "messageReceived";

        #endregion

        #region Fields

        /// <summary>
        /// The service provider.
        /// </summary>
        private readonly IServiceProvider serviceProvider;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        private ILogger<SignalRHub> Logger
        {
            get
            {
                return this.serviceProvider.GetRequiredService<ILogger<SignalRHub>>();
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalRHub"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public SignalRHub(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        #endregion

        #region Hub Methods

        /// <summary>
        /// Called when a new connection is established with the hub.
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            // Send a message to user that it has connected to the service

            this.Logger.LogInformation("Connected client with Id = {0}", this.Context.ConnectionId);

            await this.Clients.Caller.SendAsync(broadcastMethodName, "Connected to the SignalR Hub! ConnectionId: " + this.Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Joins the chat room.
        /// </summary>
        public async Task JoinChatRoom()
        {
            // Add user to the chat group

            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, chatGroup);

            this.Logger.LogInformation("Added user with connectionId = {0} to group name = {1}", this.Context.ConnectionId, chatGroup);

            // Notify other users that new client is connected

            Message message = new Message()
            {
                Id = "ChatBot",
                Payload = "Client with ID: " + this.Context.ConnectionId + " joined the chat!",
                Date = DateTime.UtcNow
            };

            await this.Clients.Group(chatGroup).SendAsync(messagesMethodName, message);
        }

        /// <summary>
        /// Leaves the chat room.
        /// </summary>
        public async Task LeaveChatRoom()
        {
            // Notify other users that new client is connected

            Message message = new Message()
            {
                Id = "ChatBot",
                Payload = "Client with ID: " + this.Context.ConnectionId + " has left the chat!",
                Date = DateTime.UtcNow
            };

            await this.Clients.Group(chatGroup).SendAsync(messagesMethodName, message);

            // Remove user from the chat group

            await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, chatGroup);

            this.Logger.LogInformation("Removed user with connectionId = {0} from group name = {1}", this.Context.ConnectionId, chatGroup);
        }

        /// <summary>
        /// Broadcasts the new message.
        /// </summary>
        /// <param name="message">The message.</param>
        public async Task NewMessage(string message)
        {
            Message newMessage = new Message()
            {
                Id = this.Context.ConnectionId,
                Payload = message,
                Date = DateTime.UtcNow
            };

            await this.Clients.Group(chatGroup).SendAsync(messagesMethodName, newMessage);

            this.Logger.LogInformation("User with Id = {0} send to group name = {1} following message = {2} at {3}", newMessage.Id, chatGroup, newMessage.Payload, newMessage.Date.ToShortTimeString());
        }

        #endregion
    }
}
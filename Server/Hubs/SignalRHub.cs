using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using SignalRDemo.Hub.Interfaces;
using SignalRDemo.Models;

namespace SignalRDemo.Hubs
{
    /// <summary>
    /// SignalR Hub.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.SignalR.Hub"/>
    public class SignalRHub : Hub<ISignalRHub>
    {
        #region Constants

        /// <summary>
        /// The general chat group.
        /// </summary>
        private const string generalChatGroup = "generalChatGroup";

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
        /// <value>The logger.</value>
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

            await this.Clients.Caller.BroadcastMessage("Connected to the SignalR Hub! ConnectionId: " + this.Context.ConnectionId);

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

            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, generalChatGroup);

            this.Logger.LogInformation("Added user with connectionId = {0} to group name = {1}", this.Context.ConnectionId, generalChatGroup);

            // Notify other users that new client is connected

            ChatMessage message = new ChatMessage()
            {
                Id = "ChatBot",
                Payload = "Client with ID: " + this.Context.ConnectionId + " joined the chat!",
                Date = DateTime.UtcNow
            };

            await this.Clients.Group(generalChatGroup).BroadcastMessage(message);
        }

        /// <summary>
        /// Leaves the chat room.
        /// </summary>
        public async Task LeaveChatRoom()
        {
            // Notify other users that new client is connected

            ChatMessage message = new ChatMessage()
            {
                Id = "ChatBot",
                Payload = "Client with ID: " + this.Context.ConnectionId + " has left the chat!",
                Date = DateTime.UtcNow
            };

            await this.Clients.Group(generalChatGroup).BroadcastMessage(message);

            // Remove user from the chat group

            await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, generalChatGroup);

            this.Logger.LogInformation("Removed user with connectionId = {0} from group name = {1}", this.Context.ConnectionId, generalChatGroup);
        }

        /// <summary>
        /// Broadcasts the new message.
        /// </summary>
        /// <param name="message">The message.</param>
        public async Task NewMessage(string message)
        {
            ChatMessage newMessage = new ChatMessage()
            {
                Id = this.Context.ConnectionId,
                Payload = message,
                Date = DateTime.UtcNow
            };

            await this.Clients.Group(generalChatGroup).BroadcastMessage(newMessage);

            this.Logger.LogInformation("User with Id = {0} send to group name = {1} following message = {2} at {3}", newMessage.Id, generalChatGroup, newMessage.Payload, newMessage.Date.ToShortTimeString());
        }

        #endregion
    }
}
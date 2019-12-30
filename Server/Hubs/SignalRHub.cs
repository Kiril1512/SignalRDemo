using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SignalRDemo.Models;

namespace SignalRDemo.Hubs
{
    /// <summary>
    /// SignalR Hub.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.SignalR.Hub" />
    public class SignalRHub : Hub
    {
        #region Constants

        /// <summary>
        /// The broadcast method name.
        /// </summary>
        private const string broadcastMethodName = "broadcastChannel";

        /// <summary>
        /// The messages method name.
        /// </summary>
        private const string messagesMethodName = "messageReceived";

        /// <summary>
        /// The chat group
        /// </summary>
        private const string chatGroup = "chatGroup";

        #endregion

        #region Hub Methods

        /// <summary>
        /// Called when a new connection is established with the hub.
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            // Send a message to user that it has connected to the service
            
            await this.Clients.Caller.SendAsync(broadcastMethodName, "Connected to the SignalR Hub! ConnectionId: " + this.Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        #endregion

        #region Public Methods

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
        }

        /// <summary>
        /// Joins the chat room.
        /// </summary>
        public async Task JoinChatRoom()
        {
            // Add user to the chat group

            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, chatGroup);

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
        }

        #endregion
    }
}

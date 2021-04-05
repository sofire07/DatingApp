using AutoMapper;
using Logic.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Model;
using Model.DataTransfer;
using Model.Extensions;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IMessageLogic _messageLogic;
        private readonly IHubContext<PresenceHub> _presenceHub;
        private readonly PresenceTracker _tracker;
        private readonly IUserRepo _userRepo;

        public MessageHub(IMessageLogic messageLogic, IHubContext<PresenceHub> presenceHub, PresenceTracker tracker, IUserRepo userRepo)
        {
            _messageLogic = messageLogic;
            _presenceHub = presenceHub;
            _tracker = tracker;
            _userRepo = userRepo;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"].ToString();
            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var messages = await _messageLogic.GetMessageThread(Context.User.GetUsername(), otherUser);

            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
            await AddToGroup(groupName);
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await RemoveFromMessageGroup();
            await base.OnDisconnectedAsync(ex);
        }

        public async Task SendMessage(CreateMessageDto dto)
        {
            var username = Context.User.GetUsername();
            var user = await _userRepo.GetUserByUsername(username);
            var result = await _messageLogic.AddMessage(dto, username);
            var groupName = GetGroupName(username, dto.RecipientUsername);
            var group = await _messageLogic.GetMessageGroup(groupName);

            if(group.Connections.Any(x=>x.Username == dto.RecipientUsername))
            {
                result.MessageRead = DateTime.Now;
            }

            else
            {
                var connections = await _tracker.GetConnectionsForUser(dto.RecipientUsername);
                if(connections != null)
                {
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                        new { userName = result.SenderUsername, knownAs = user.KnownAs});
                }
            }

            if (result != null)
            {
                await Clients.Group(groupName).SendAsync("NewMessage", result);
            }
            throw new HubException("The message was not sent. Please try again.");
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        private async Task<bool> AddToGroup(string groupName)
        {
            var group = await _messageLogic.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());
            bool result = false;
            if (group == null)
            {
                group = new Group(groupName);
                result = await _messageLogic.AddGroup(group);
            }
            group.Connections.Add(connection);
            return result;
        }

        private async Task RemoveFromMessageGroup()
        {
            var connection = await _messageLogic.GetConnection(Context.ConnectionId);
            await _messageLogic.RemoveConnection(connection);
        }
    }
}

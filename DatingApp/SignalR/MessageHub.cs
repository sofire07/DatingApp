using AutoMapper;
using Logic.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Model.DataTransfer;
using Model.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IMessageLogic _messageLogic;

        public MessageHub(IMessageLogic messageLogic, IMapper mapper)
        {
            _messageLogic = messageLogic;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"].ToString();
            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var messages = await _messageLogic.GetMessageThread(Context.User.GetUsername(), otherUser);

            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await base.OnDisconnectedAsync(ex);
        }

        public async Task SendMessage(CreateMessageDto dto)
        {
            var username = Context.User.GetUsername();
            var result = await _messageLogic.AddMessage(dto, username);
            if (result != null)
            {
                var group = GetGroupName(username, dto.RecipientUsername);
                await Clients.Group(group).SendAsync("NewMessage", result);
            }
            throw new HubException("The message was not sent. Please try again.");
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}

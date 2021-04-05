using AutoMapper;
using Logic.Interfaces;
using Model;
using Model.DataTransfer;
using Model.Helpers;
using Repository;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class MessageLogic : IMessageLogic
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IUserRepo _userRepo;
        private readonly IMapper _mapper;

        public MessageLogic(IMessageRepository messageRepo, IUserRepo userRepo, IMapper mapper)
        {
            _messageRepo = messageRepo;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        public async Task<bool> AddGroup(Group group)
        {
            return await _messageRepo.AddGroup(group);
        }

        public async Task<MessageDto> AddMessage(CreateMessageDto dto, string sender)
        {
            var loggedIn = await _userRepo.GetUserByUsername(sender);
            var recipient = await _userRepo.GetUserByUsername(dto.RecipientUsername);
            Message message = new Message
            {
                Sender = loggedIn,
                SenderId = loggedIn.Id,
                SenderUsername = loggedIn.UserName,
                Recipient = recipient,
                RecipientId = recipient.Id,
                RecipientUsername = recipient.UserName,
                MessageContent = dto.MessageContent,
            };
            if (await _messageRepo.AddMessage(message)) return _mapper.Map<MessageDto>(message);
            return null;
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _messageRepo.GetConnection(connectionId);
        }

        public async Task<MessageDto> GetMessage(int id)
        {
            return _mapper.Map<MessageDto>(await _messageRepo.GetMessage(id));
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _messageRepo.GetMessageGroup(groupName);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            return await _messageRepo.GetMessagesForUser(messageParams);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUser, string recipient)
        {
            var messages = await _messageRepo.GetMessageThread(currentUser, recipient);
            var unreadMessages = messages.Where(r => r.MessageRead == null && r.RecipientUsername == currentUser).ToList();

            if (unreadMessages.Any())
            {
                foreach(Message message in unreadMessages)
                {
                    message.MessageRead = DateTime.Now;
                }
                await _messageRepo.SaveAllAsync();
            }


            return _mapper.Map<IEnumerable<MessageDto>>(messages);

        }

        public async Task<bool> RemoveConnection(Connection connection)
        {
            return await _messageRepo.RemoveConnection(connection);
        }
    }
}

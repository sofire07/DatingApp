using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.DataTransfer;
using Model.Helpers;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public MessageRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> AddGroup(Group group)
        {
            _context.Groups.Add(group);
            return await SaveAllAsync();
        }

        public async Task<bool> AddMessage(Message message)
        {
            await _context.Messages.AddAsync(message);
            return await SaveAllAsync();
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Message> GetMessage(int id)
        {
            var result = await _context.Messages.Include(x=>x.Recipient.Photos).Include(x=>x.Sender.Photos).FirstOrDefaultAsync();
            return result;
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups
                    .Include(x => x.Connections)
                    .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages.OrderByDescending(m => m.MessageSent).Include(x => x.Sender.Photos).Include(x => x.Recipient.Photos).AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.Username),
                "Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username),
                _ => query.Where(u => u.Recipient.UserName == messageParams.Username && u.MessageRead == null)
            };

            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(string currentUser, string recipient)
        {
            var messages = await _context.Messages
                .Include(x=>x.Recipient.Photos)
                .Include(x=>x.Sender.Photos)
                .Where(x => (x.RecipientUsername == currentUser && x.SenderUsername == recipient)
                || (x.SenderUsername == currentUser && x.RecipientUsername == recipient)).ToListAsync();

            return messages;
        }

        public async Task<bool> RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
            return await SaveAllAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

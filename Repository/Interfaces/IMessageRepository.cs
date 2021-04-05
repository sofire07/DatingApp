using Model;
using Model.DataTransfer;
using Model.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IMessageRepository
    {
        Task<bool> AddMessage(Message message);
        Task<Message> GetMessage(int id);
        Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<Message>> GetMessageThread(string currentUser, string recipient);
        Task<bool> SaveAllAsync();
    }
}

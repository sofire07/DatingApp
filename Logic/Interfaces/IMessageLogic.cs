using Model;
using Model.DataTransfer;
using Model.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Interfaces
{
    public interface IMessageLogic
    {

        Task<MessageDto> AddMessage(CreateMessageDto dto, string sender);
        Task<MessageDto> GetMessage(int id);
        Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUser, string recipient);
    }
}

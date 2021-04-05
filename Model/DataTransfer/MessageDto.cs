using System;
using System.Collections.Generic;
using System.Text;

namespace Model.DataTransfer
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string SenderUsername { get; set; }
        public string SenderPhotoUrl{ get; set; }
        public string RecipientId { get; set; }
        public string RecipientUsername { get; set; }
        public string RecipientPhotoUrl { get; set; }
        public string MessageContent { get; set; }
        public DateTime MessageSent { get; set; } = DateTime.Now;
        public DateTime? MessageRead { get; set; }
    }
}

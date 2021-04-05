using System;
using System.Collections.Generic;
using System.Text;

namespace Model.DataTransfer
{
    public class CreateMessageDto
    {
        public string RecipientUsername { get; set; }
        public string MessageContent { get; set; }
    }
}

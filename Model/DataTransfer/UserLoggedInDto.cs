using System;
using System.Collections.Generic;
using System.Text;

namespace Model.DataTransfer
{
    public class UserLoggedInDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Token { get; set; }
        public DateTime TokenExpires { get; set; }
    }
}

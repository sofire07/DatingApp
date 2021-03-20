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
        public string PhotoUrl { get; set; }
        public string PhoneNumber { get; set; }
        public string Token { get; set; }
        public DateTime TokenExpires { get; set; }
        public int Age { get; set; }
        public DateTime DoB { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public ICollection<PhotoDto> Photos { get; set; }
    }
}

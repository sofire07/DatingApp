using System;
using System.Collections.Generic;
using System.Text;

namespace Model.DataTransfer
{
    public class EditPassword
    {
        public UserLoggedInDto UserLoggedIn { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}

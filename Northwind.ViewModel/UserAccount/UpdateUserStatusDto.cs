using System;
using System.Collections.Generic;
using System.Text;

namespace Northwind.ViewModel.UserAccount
{
    public class UpdateUserStatusDto
    {
        public string Username { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}

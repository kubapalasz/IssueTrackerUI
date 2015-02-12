using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace IssueTracker
{
    public class User
    {
        public string Name { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }

        public long Id { get; internal set; }
    }
}

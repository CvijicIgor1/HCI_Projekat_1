using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sistem_za_upravljanje_sadrzajima.Enumeracije;

namespace Sistem_za_upravljanje_sadrzajima.Modeli
{
    [Serializable]
    public class User
    {
        public string Username { get; set; }    
        public string Password { get; set; }
        public UserRole Role { get; set; }

        public User() { }

        public User(string username, string password, UserRole role)
        {
            Username = username;
            Password = password;
            Role = role;
        }
    }
}

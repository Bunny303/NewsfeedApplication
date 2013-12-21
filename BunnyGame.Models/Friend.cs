using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyGame.Models
{
    public class Friend
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}

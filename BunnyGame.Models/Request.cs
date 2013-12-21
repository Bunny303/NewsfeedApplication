using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyGame.Models
{
    public class Request
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int SenderId { get; set; }
        public virtual User Sender { get; set; }
        public int RecipientId { get; set; }
        public virtual User Recipient { get; set; }
        public bool Answer { get; set; }
    }
}

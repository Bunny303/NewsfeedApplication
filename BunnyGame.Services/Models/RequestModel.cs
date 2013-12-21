using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BunnyGame.Services.Models
{
    public class ReceiveRequestModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public string SenderAvatar { get; set; }
        public bool Answer { get; set; }
    }
}
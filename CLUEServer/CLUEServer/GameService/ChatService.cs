using GameService.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Services
{
    public partial class ChatService : IChatManager
    {
        private static readonly Dictionary<string, IChatManagerCallback> chat
        public void ConnectToChat(string username, int code)
        {
            throw new NotImplementedException();
        }

        public void DisconnectFromChat(string username)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(int code, Message message)
        {
            throw new NotImplementedException();
        }
    }
}

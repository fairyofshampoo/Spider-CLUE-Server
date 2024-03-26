using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using TestsServer.SpiderClueService;
using Xunit;

namespace TestsServer
{
    public class ChatTest 
    {
        private static ChatManagerClient chatProxy;
        private static ChatManagerCallbackImplementation chatCallbackImplementation;
        
        public ChatTest() 
        {
            chatCallbackImplementation = new ChatManagerCallbackImplementation();
            chatProxy = new ChatManagerClient(new InstanceContext(chatCallbackImplementation));
        }

        [Fact]
        public async void ConnectToChatTestSuccess()
        {
            string gamertag = "Star3oy";
            string codeMatch = "123456";
            chatProxy.ConnectToChat(gamertag, codeMatch);
            await Task.Delay(4000);
            Assert.True(chatCallbackImplementation.isMessageBack);
            chatProxy.DisconnectFromChat(gamertag);
        }

        [Fact]
        public async void BroadcastMessageTestSuccess()
        {
            string codeMatch = "123456";
            string gamertag = "Star3oy";
            chatProxy.ConnectToChat(gamertag, codeMatch);

            var message = new SpiderClueService.Message
            {
                Text = "hola",
                GamerTag = "Star3oy",
            };
            await Task.Delay(4000);
            chatProxy.BroadcastMessage(codeMatch, message);
            await Task.Delay(4000);
            Assert.True(chatCallbackImplementation.isMessageBack);
        }
    }

    public class ChatManagerCallbackImplementation : IChatManagerCallback
    {
        public bool isMessageBack { get; set; }
        public void ReceiveMessages(SpiderClueService.Message[] messages)
        {
            isMessageBack = true;
            Console.WriteLine("¡Callback ejecutado!");
        }
    }

}

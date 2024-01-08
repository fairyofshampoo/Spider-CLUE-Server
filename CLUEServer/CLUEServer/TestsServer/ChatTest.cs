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
    public class ChatTest : IDisposable
    {
        public static ChatManagerClient chatProxy;
        public static ChatManagerCallbackImplementation chatCallbackImplementation;
        
        public ChatTest() 
        {
            chatCallbackImplementation = new ChatManagerCallbackImplementation();
            chatProxy = new ChatManagerClient(new InstanceContext(chatCallbackImplementation));
        }

        public void Dispose()
        {
            chatProxy.DisconnectFromChat("Star3oy");
        }


        [Fact]
        public async void ConnectToChatTestSuccess()
        {
            chatProxy.ConnectToChat("Star3oy", "123456");

            await Task.Delay(10000);

            Assert.True(chatCallbackImplementation.isMessageBack);
        }

        [Fact]
        public async void BroadcastMessageTestSuccess()
        {
            string codeMatch = "123456";

            var message = new SpiderClueService.Message
            {
                Text = "hola",
                GamerTag = "Star3oy",
            };

            chatProxy.ConnectToChat("Star3oy", "123456");
            chatProxy.BroadcastMessage(codeMatch, message);

            await Task.Delay(10000);

            Assert.True(chatCallbackImplementation.isMessageBack);
        }
    }

    public class ChatManagerCallbackImplementation : IChatManagerCallback
    {
        public bool isMessageBack { get; set; }

        public ChatManagerCallbackImplementation()
        {
            isMessageBack = false;
        }
        public void ReceiveMessages(SpiderClueService.Message[] messages)
        {
            isMessageBack = true;
            Console.WriteLine("¡Callback ejecutado!");
        }
    }

}

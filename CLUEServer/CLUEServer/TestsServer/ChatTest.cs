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
            
        }


        [Fact]
        public async void ConnectToChatTestSuccess()
        {
            chatProxy.ConnectToChat("Star3oy", "123456");

            await Task.Delay(4000);

            Assert.True(chatCallbackImplementation.isMessageBack);
        }

        [Fact]
        public async void BroadcastMessageTestSuccess()
        {
            string codeMatch = "123456";
            chatProxy.ConnectToChat("Star3oy", codeMatch);

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

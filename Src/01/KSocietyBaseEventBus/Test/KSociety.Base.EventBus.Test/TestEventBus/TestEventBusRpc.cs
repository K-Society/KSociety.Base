using System.Threading;
using KSociety.Base.EventBus.Abstractions.EventBus;
using KSociety.Base.EventBus.Test.IntegrationEvent.Event;
using KSociety.Base.EventBus.Test.IntegrationEvent.EventHandling;
using KSociety.Base.EventBusRabbitMQ;
using Xunit;

namespace KSociety.Base.EventBus.Test.TestEventBus
{
    public class TestEventBusRpc : Test
    {
        private IEventBusRpcClient _eventBusRpcClient;
        private IEventBusRpcServer _eventBusRpcServer;

        public TestEventBusRpc()
        {

            ;
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                StartServer();
            }).Start();

            //Thread client = new Thread(StartClient);
            //StartServer();

            //new Thread(StartClient).Start();
            //StartServer();
            StartClient();
            
        }

        private void StartServer()
        {
            _eventBusRpcServer = new EventBusRabbitMqRpcServer(PersistentConnection, LoggerFactory,
                new TestRpcServerHandler(LoggerFactory, ComponentContext), null, EventBusParameters, "ServerTest", CancellationToken.None);
            _eventBusRpcServer.SubscribeRpcServer<TestIntegrationEventRpc, TestIntegrationEventReply, TestRpcServerHandler>("pippo.server");

        }

        private void StartClient()
        {
            _eventBusRpcClient = new EventBusRabbitMqRpcClient(PersistentConnection, LoggerFactory,
                new TestRpcClientHandler(LoggerFactory, ComponentContext), null, EventBusParameters, "ClientTest", CancellationToken.None);
            _eventBusRpcClient.SubscribeRpcClient<TestIntegrationEventReply, TestRpcClientHandler>("pippo.client");
        }

        [Fact]
        public async void SendData()
        {
            const string expectedName1 = "SuperPippo1";
            const string expectedName2 = "SuperPippo2";
            const string expectedName3 = "SuperPippo3";

            TestIntegrationEventReply result1 = null;
            TestIntegrationEventReply result2 = null;
            TestIntegrationEventReply result3 = null;

            for (int i = 0; i < 1; i++)
            {
                ;
                result1 = await _eventBusRpcClient
                    .CallAsync<TestIntegrationEventReply>(new TestIntegrationEventRpc("pippo.server", "pippo.client",
                        expectedName1, null));
                    //.ConfigureAwait(false);
                ; 
                //result2 = await _eventBusRpcClient
                //    .CallAsync<TestIntegrationEventReply>(new TestIntegrationEventRpc("pippo.server", "pippo.client",
                //        expectedName2, null))
                //    .ConfigureAwait(false);
                //;
                //result3 = await _eventBusRpcClient
                //    .CallAsync<TestIntegrationEventReply>(new TestIntegrationEventRpc("pippo.server", "pippo.client",
                //        expectedName3, null))
                //    .ConfigureAwait(false);
                //;
            }

            ;
            Assert.NotNull(result1);
            Assert.NotEmpty(result1.TestName);
            Assert.Equal(expectedName1, result1.TestName);
            //;
            //Assert.NotNull(result2);
            //Assert.NotEmpty(result2.TestName);
            //Assert.Equal(expectedName2, result2.TestName);
            //;
            //Assert.NotNull(result3);
            //Assert.NotEmpty(result3.TestName);
            //Assert.Equal(expectedName3, result3.TestName);
            //;
        }
    }
}

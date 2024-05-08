// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Test.TestEventBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using KSociety.Base.EventBus.Abstractions.EventBus;
    using IntegrationEvent.Event;
    using IntegrationEvent.EventHandling;
    using EventBusRabbitMQ;
    using Xunit;
    using KSociety.Base.EventBusRabbitMQ.Helper;

    public class TestEventBusRpc : Test
    {
        private IEventBusRpcClient<TestIntegrationEventReply> _eventBusRpcClient;
        private IEventBusRpcServer _eventBusRpcServer;
        private IEventBusRpc _eventBusRpc;
        public Subscriber Subscriber { get; }

        public TestEventBusRpc()
        {

            this.Subscriber = new Subscriber(this.LoggerFactory, this.PersistentConnection, this.EventBusParameters, 10, true);

            //this.Subscriber.
            new Thread(async () =>
            {
                Thread.CurrentThread.IsBackground = true;
                //await this.StartServer().ConfigureAwait(false);
                await this.Subscriber
                    .SubscribeServer<TestRpcServerHandler, TestIntegrationEventRpc, TestIntegrationEventReply>("TestBus", "ServerTest", "pippo.server", new TestRpcServerHandler(this.LoggerFactory, this.ComponentContext))
                    .ConfigureAwait(false);
            }).Start();

            //Thread client = new Thread(StartClient);
            //StartServer();

            //new Thread(StartClient).Start();
            //_ = this.StartServer();
            //this.StartClient();

            //this.StartRpc();

            this.Subscriber
                .SubscribeClient<TestRpcClientHandler, TestIntegrationEventReply>("TestBus", "ClientTest", "pippo.client", new TestRpcClientHandler(this.LoggerFactory, this.ComponentContext))
                .ConfigureAwait(false);

        }

        private async ValueTask StartRpc()
        {
            this._eventBusRpc = new EventBusRabbitMqRpc(
                this.PersistentConnection, this.LoggerFactory,
                new TestRpcHandler(this.LoggerFactory, this.ComponentContext), null, this.EventBusParameters,
                "RpcTest");
            this._eventBusRpc.Initialize<TestIntegrationEventRpc>();
            await this._eventBusRpc.SubscribeRpc<TestIntegrationEventRpc, TestIntegrationEventReply, TestRpcHandler>("pippo.rpc");

        }

        private async ValueTask StartServer()
        {
            this._eventBusRpcServer = new EventBusRabbitMqRpcServer(this.PersistentConnection, this.LoggerFactory,
                new TestRpcServerHandler(this.LoggerFactory, this.ComponentContext), null, this.EventBusParameters,
                "ServerTest");
            this._eventBusRpcServer.InitializeServer<TestIntegrationEventRpc, TestIntegrationEventReply>();
            await this._eventBusRpcServer.SubscribeRpcServer<TestIntegrationEventRpc, TestIntegrationEventReply, TestRpcServerHandler>("pippo.server");

        }

        private async void StartClient()
        {
            this._eventBusRpcClient = new EventBusRabbitMqRpcClient<TestIntegrationEventReply>(this.PersistentConnection, this.LoggerFactory,
                new TestRpcClientHandler(this.LoggerFactory, this.ComponentContext), null, this.EventBusParameters,
                "ClientTest");
            this._eventBusRpcClient.Initialize();
            await this._eventBusRpcClient.SubscribeRpcClient<TestRpcClientHandler>("pippo.client");
        }

        [Fact]
        public async void SendData()
        {

            //await Task.Delay(500000);
            const string expectedName1 = "SuperPippo1";
            //const string expectedName2 = "SuperPippo2";
            //const string expectedName3 = "SuperPippo3";

            TestIntegrationEventReply result1 = null;
            //TestIntegrationEventReply result2 = null;
            //TestIntegrationEventReply result3 = null;
            var source = new CancellationTokenSource();
            //CancellationToken token = source.Token;
            for (var i = 0; i < 1; i++)
            {
                //result1 = await this._eventBusRpcClient
                //    .CallAsync<TestIntegrationEventReply>(new TestIntegrationEventRpc("pippo.server", "pippo.client",
                //        expectedName1, null));

                try
                {
                    source.CancelAfter(100000);
                    result1 =
                        await ((IEventBusRpcClient<TestIntegrationEventReply>)this.Subscriber
                            .EventBus["TestBus_Client"]).CallAsync(new TestIntegrationEventRpc("pippo.server", "pippo.client", expectedName1, null), source.Token);
                    //.CallAsync<TestIntegrationEvent>(
                    //    new TestIntegrationEventRpc("pippo.server", "pippo.client", expectedName1, null),
                    //    source.Token);
                }
                catch (TaskCanceledException)
                {
                    ;
                }
                catch (Exception)
                {
                    ;
                }

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

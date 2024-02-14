// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Test.ProtoModel
{
    using System;
    using IntegrationEvent.Event;

    public static class Configuration
    {
        public static void ProtoBufConfiguration()
        {
            try
            {
                ProtoBuf.Meta.RuntimeTypeModel.Default.Add(typeof(KSociety.Base.EventBus.Events.IntegrationEvent), true)
                    .AddSubType(600, typeof(BaseTestIntegrationEvent));

                ProtoBuf.Meta.RuntimeTypeModel.Default.Add(typeof(BaseTestIntegrationEvent), true)
                    .AddSubType(600, typeof(TestIntegrationEvent));

                ProtoBuf.Meta.RuntimeTypeModel.Default.Add(typeof(Events.IntegrationEventRpc), true)
                    .AddSubType(601, typeof(BaseTestIntegrationEventRpc));

                ProtoBuf.Meta.RuntimeTypeModel.Default.Add(typeof(BaseTestIntegrationEventRpc), true)
                    .AddSubType(601, typeof(TestIntegrationEventRpc));

                ProtoBuf.Meta.RuntimeTypeModel.Default.Add(typeof(Events.IntegrationEventReply), true)
                    .AddSubType(602, typeof(BaseTestIntegrationEventReply));

                ProtoBuf.Meta.RuntimeTypeModel.Default.Add(typeof(BaseTestIntegrationEventReply), true)
                    .AddSubType(602, typeof(TestIntegrationEventReply));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Configuration " + ex.Message + " " + ex.StackTrace);
            }
        }
    }
}

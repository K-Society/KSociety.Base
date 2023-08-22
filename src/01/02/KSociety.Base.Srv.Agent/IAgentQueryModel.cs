// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Agent
{
    using InfraSub.Shared.Interface;

    public interface IAgentQueryModel<in TObject, T> : 
        IAgentQueryModelBase<TObject, T>, 
        IAgentQueryModelAsync<TObject, T>
        where TObject : IIdObject
        where T : IObject
    {

    }
}

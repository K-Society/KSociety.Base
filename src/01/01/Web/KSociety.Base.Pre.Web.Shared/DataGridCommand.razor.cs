// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Pre.Web.Shared
{
    using KSociety.Base.InfraSub.Shared.Interface;
    using Srv.Agent.List.GridView;

    public partial class DataGridCommand<T, TList, TGridView> where T : IObject
        where TList : IList<T>
        where TGridView : IAgentQueryModel<T, TList>
    {
    }
}

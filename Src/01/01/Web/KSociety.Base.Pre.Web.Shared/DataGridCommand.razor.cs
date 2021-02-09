using KSociety.Base.InfraSub.Shared.Interface;
using KSociety.Base.Srv.Agent.List.GridView;

namespace KSociety.Base.Pre.Web.Shared
{
    public partial class DataGridCommand<T, TList, TGridView> where T : IObject
        where TList : InfraSub.Shared.Interface.IList<T>
        where TGridView : IAgentQueryModel<T, TList>
    {
    }
}

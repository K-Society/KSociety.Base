using KSociety.Base.InfraSub.Shared.Interface;
using KSociety.Base.Pre.Form.Presenter.Abstractions;
using KSociety.Base.Pre.Form.View.Abstractions;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.Pre.Form.Presenter.Forms
{
    public abstract class PresenterReadOnly<TView, T, TList, TQuery> : PresenterBase<TView, T, TList, TQuery>, IPresenterReadOnly<TView, T, TList>
        where T : IObject
        where TList : IList<T>
        where TQuery : Srv.Agent.List.GridView.IAgentQueryModel<T, TList>
        where TView : IViewReadOnly<T, TList>
    {
        protected PresenterReadOnly(TView view, TQuery queryModel, ILoggerFactory loggerFactory)
            : base(view, queryModel, loggerFactory)
        {

        }
    }
}

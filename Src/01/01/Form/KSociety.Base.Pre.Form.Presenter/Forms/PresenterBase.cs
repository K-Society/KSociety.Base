using KSociety.Base.InfraSub.Shared.Interface;
using KSociety.Base.Pre.Form.Presenter.Abstractions;
using KSociety.Base.Pre.Form.View.Abstractions;
using Microsoft.Extensions.Logging;
using System;

namespace KSociety.Base.Pre.Form.Presenter.Forms
{
    public class PresenterBase<TView, T, TList, TQuery> : IPresenterBase<TView, T, TList>
        where T : IObject
        where TList : IList<T>
        where TQuery : Srv.Agent.List.GridView.IAgentQueryModel<T, TList>
        where TView : IViewBase<T, TList>
    {
        protected readonly ILogger<PresenterBase<TView, T, TList, TQuery>> Logger;
        private TQuery _queryModel;
        protected TView View;


        protected PresenterBase(TView view, TQuery queryModel, ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<PresenterBase<TView, T, TList, TQuery>>();
            _queryModel = queryModel;
            View = view;

            WireViewEvents();

        }

        public TView GetView()
        {
            return View;
        }

        private void WireViewEvents()
        {
            View.LoadData += ViewLoadDataAsync;
        }

        private void ViewLoadData(object sender, EventArgs e)
        {
            try
            {
                //Console.WriteLine("ViewLoadData");
                View.ListView = _queryModel.LoadAllRecords();
                //View.ShowErrorMessage("ViewLoadData"); //No
            }
            catch (Exception ex)
            {
                string[] lines = { ex.Message, ex.StackTrace };
                System.IO.File.WriteAllLines(@"C:\JOB\ViewLoadData.txt", lines);
            }
        }

        private async void ViewLoadDataAsync(object sender, EventArgs e)
        {
            try
            {
                //Console.WriteLine("ViewLoadDataAsync");
                if (_queryModel is null)
                {
                    //Console.WriteLine("_queryModel is null");
                }
                else if (View is null)
                {
                    //Console.WriteLine("View is null");
                }
                else
                {
                    View.ListView = await _queryModel.LoadAllRecordsAsync().ConfigureAwait(false);
                }

            }
            catch (Exception ex)
            {
                string[] lines = { ex.Message, ex.StackTrace };
                System.IO.File.WriteAllLines(@"C:\JOB\ViewLoadDataAsync.txt", lines);
            }
        }
    }
}

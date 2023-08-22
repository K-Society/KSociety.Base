// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Pre.Form.Presenter.Forms
{
    using InfraSub.Shared.Interface;
    using Abstractions;
    using KSociety.Base.Pre.Form.View.Abstractions;
    using Microsoft.Extensions.Logging;
    using System;

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
            this.Logger = loggerFactory.CreateLogger<PresenterBase<TView, T, TList, TQuery>>();
            this._queryModel = queryModel;
            this.View = view;

            this.WireViewEvents();

        }

        public TView GetView()
        {
            return this.View;
        }

        private void WireViewEvents()
        {
            this.View.LoadData += this.ViewLoadDataAsync;
        }

        private void ViewLoadData(object sender, EventArgs e)
        {
            try
            {
                //Console.WriteLine("ViewLoadData");
                this.View.ListView = this._queryModel.LoadAllRecords();
                //View.ShowErrorMessage("ViewLoadData"); //No
            }
            catch (Exception ex)
            {
                string[] lines = {ex.Message, ex.StackTrace};
                System.IO.File.WriteAllLines(@"C:\JOB\ViewLoadData.txt", lines);
            }
        }

        private async void ViewLoadDataAsync(object sender, EventArgs e)
        {
            try
            {
                //Console.WriteLine("ViewLoadDataAsync");
                if (this._queryModel is null)
                {
                    Console.WriteLine("_queryModel is null");
                }
                else if (this.View is null)
                {
                    Console.WriteLine("View is null");
                }
                else
                {
                    this.View.ListView = await this._queryModel.LoadAllRecordsAsync().ConfigureAwait(false);
                }

            }
            catch (Exception ex)
            {
                string[] lines = {ex.Message, ex.StackTrace};
                System.IO.File.WriteAllLines(@"C:\JOB\ViewLoadDataAsync.txt", lines);
            }
        }
    }
}

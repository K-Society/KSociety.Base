// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Agent
{
    using Contract;
    using Microsoft.Extensions.Logging;
    using System.Threading;
    using System.Threading.Tasks;

    public class AgentCommandImportExport<
        TCommandImportExport, TCommandImportExportAsync,
        TAddReq, TAddRes,
        TUpdateReq, TUpdateRes,
        TCopyReq, TCopyRes,
        TModifyFieldReq, TModifyFieldRes,
        TRemoveReq, TRemoveRes,
        TImportReq, TImportRes,
        TExportReq, TExportRes> : AgentCommand<TCommandImportExport, TCommandImportExportAsync, TAddReq, TAddRes,
            TUpdateReq, TUpdateRes, TCopyReq, TCopyRes, TModifyFieldReq, TModifyFieldRes, TRemoveReq, TRemoveRes>,
        IAgentCommandImportExport<
            TAddReq, TAddRes,
            TUpdateReq, TUpdateRes,
            TCopyReq, TCopyRes,
            TModifyFieldReq, TModifyFieldRes,
            TRemoveReq, TRemoveRes,
            TImportReq, TImportRes,
            TExportReq, TExportRes>
        where TCommandImportExport : class, ICommandImportExport<TAddReq, TAddRes, TUpdateReq, TUpdateRes, TCopyReq,
            TCopyRes, TModifyFieldReq, TModifyFieldRes, TRemoveReq, TRemoveRes, TImportReq, TImportRes, TExportReq,
            TExportRes>
        where TCommandImportExportAsync : class, ICommandImportExportAsync<TAddReq, TAddRes, TUpdateReq, TUpdateRes,
            TCopyReq, TCopyRes, TModifyFieldReq, TModifyFieldRes, TRemoveReq, TRemoveRes, TImportReq, TImportRes,
            TExportReq, TExportRes>
        where TAddReq : class
        where TAddRes : class
        where TUpdateReq : class
        where TUpdateRes : class
        where TCopyReq : class
        where TCopyRes : class
        where TModifyFieldReq : class
        where TModifyFieldRes : class
        where TRemoveReq : class
        where TRemoveRes : class
        where TImportReq : class
        where TImportRes : class
        where TExportReq : class
        where TExportRes : class
    {

        private readonly IAgentImport<TImportReq, TImportRes> _agentImport;
        private readonly IAgentExport<TExportReq, TExportRes> _agentExport;

        public AgentCommandImportExport(IAgentConfiguration agentConfiguration, ILoggerFactory loggerFactory)
            : base(agentConfiguration, loggerFactory)
        {
            this._agentImport =
                new AgentImport<TCommandImportExport, TCommandImportExportAsync, TImportReq, TImportRes>(
                    agentConfiguration, loggerFactory);
            this._agentExport =
                new AgentExport<TCommandImportExport, TCommandImportExportAsync, TExportReq, TExportRes>(
                    agentConfiguration, loggerFactory);
        }

        public virtual TImportRes? ImportData(TImportReq request, CancellationToken cancellationToken = default)
        {
            return this._agentImport.ImportData(request, cancellationToken);
        }

        public virtual async ValueTask<TImportRes?> ImportDataAsync(TImportReq request,
            CancellationToken cancellationToken = default)
        {
            return await this._agentImport.ImportDataAsync(request, cancellationToken).ConfigureAwait(false);
        }

        public virtual TExportRes? ExportData(TExportReq request, CancellationToken cancellationToken = default)
        {
            return this._agentExport.ExportData(request, cancellationToken);
        }

        public virtual async ValueTask<TExportRes?> ExportDataAsync(TExportReq request,
            CancellationToken cancellationToken = default)
        {
            return await this._agentExport.ExportDataAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}

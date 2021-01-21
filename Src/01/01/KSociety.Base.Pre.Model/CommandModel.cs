//using KSociety.Base.Srv.Agent;
//using System.Threading;
//using System.Threading.Tasks;

//namespace KSociety.Base.Pre.Model
//{
//    public class CommandModel<
//        TRemove,
//        TAddReq,
//        TAddRes,
//        TUpdateReq,
//        TUpdateRes,
//        TCopyReq,
//        TCopyRes,
//        TModifyField> : ICommandModel<TRemove,
//        TAddReq, TAddRes, TUpdateReq, TUpdateRes, TCopyReq, TCopyRes, TModifyField>
//        where TRemove : class
//        where TAddReq : class
//        where TAddRes : class
//        where TUpdateReq : class
//        where TUpdateRes : class
//        where TCopyReq : class
//        where TCopyRes : class
//        where TModifyField : class
//    {

//        private readonly IAgentCommand<TRemove, TAddReq, TAddRes,
//            TUpdateReq, TUpdateRes, TCopyReq,
//            TCopyRes, TModifyField> _agentCommand;

//        public CommandModel(IAgentCommand<TRemove, TAddReq, TAddRes,
//            TUpdateReq, TUpdateRes, TCopyReq,
//            TCopyRes, TModifyField> agentCommand)
//        {
//            _agentCommand = agentCommand;
//        }

//        public bool Remove(TRemove removeItem, CancellationToken cancellationToken = default)
//        {
//            return _agentCommand.Remove(removeItem);
//        }

//        public async ValueTask<bool> RemoveAsync(TRemove removeItem, CancellationToken cancellationToken = default)
//        {
//            return await _agentCommand.RemoveAsync(removeItem, cancellationToken);
//        }

//        public TAddRes Add(TAddReq addItem, CancellationToken cancellationToken = default)
//        {
//            return _agentCommand.Add(addItem);
//        }

//        public TUpdateRes Update(TUpdateReq updateItem, CancellationToken cancellationToken = default)
//        {
//            return _agentCommand.Update(updateItem);
//        }

//        public async ValueTask<TUpdateRes> UpdateAsync(TUpdateReq updateItem, CancellationToken cancellationToken = default)
//        {
//            return await _agentCommand.UpdateAsync(updateItem, cancellationToken);
//        }

//        public TCopyRes Copy(TCopyReq copyItem, CancellationToken cancellationToken = default)
//        {
//            return _agentCommand.Copy(copyItem);
//        }

//        public async ValueTask<TAddRes> AddAsync(TAddReq addItem, CancellationToken cancellationToken = default)
//        {
//            return await _agentCommand.AddAsync(addItem, cancellationToken);
//        }

//        public async ValueTask<TCopyRes> CopyAsync(TCopyReq copyItem, CancellationToken cancellationToken = default)
//        {
//            return await _agentCommand.CopyAsync(copyItem, cancellationToken);
//        }

//        public bool ModifyField(TModifyField modifyItemField, CancellationToken cancellationToken = default)
//        {
//            return _agentCommand.ModifyField(modifyItemField);
//        }

//        public async ValueTask<bool> ModifyFieldAsync(TModifyField modifyItemField, CancellationToken cancellationToken = default)
//        {
//            return await _agentCommand.ModifyFieldAsync(modifyItemField, cancellationToken);
//        }
//    }
//}

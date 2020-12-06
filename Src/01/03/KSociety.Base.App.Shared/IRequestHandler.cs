namespace KSociety.Base.App.Shared
{
    public interface IRequestListHandlerWithResponse<in TRequest, in TRequestList, out TResponse>
        where TRequest : IRequest
        where TRequestList : IKbAppList<TRequest>
        where TResponse : IResponse
    {
        TResponse Execute(TRequestList request);
    }

    public interface IRequestHandlerWithResponse<in TRequest, out TResponse>
        where TRequest : IRequest
        where TResponse : IResponse
    {
        TResponse Execute(TRequest request);
    }
    
    public interface IRequestHandler<in TRequest>
        where TRequest : IRequest
    {
        void Execute(TRequest request);
    }

    public interface IRequestHandlerWithResponse<out TResponse>
        where TResponse : IResponse
    {
        TResponse Execute();
    }

    public interface IRequestHandler
    {
        void Execute();
    }
}
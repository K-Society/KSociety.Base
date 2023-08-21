namespace KSociety.Base.EventBus.Abstractions.Handler
{
    using System.Threading.Tasks;

    public interface IDynamicIntegrationEventHandler
    {
        ValueTask Handle(dynamic eventData);
    }
}
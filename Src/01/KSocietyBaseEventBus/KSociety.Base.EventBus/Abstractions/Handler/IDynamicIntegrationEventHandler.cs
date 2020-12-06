using System.Threading.Tasks;

namespace KSociety.Base.EventBus.Abstractions.Handler
{
    public interface IDynamicIntegrationEventHandler
    {
        ValueTask Handle(dynamic eventData);
    }
}

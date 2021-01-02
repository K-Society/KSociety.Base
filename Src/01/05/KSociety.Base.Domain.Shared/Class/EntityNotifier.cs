using KSociety.Base.InfraSub.Shared.Interface;

namespace KSociety.Base.Domain.Shared.Class
{
    public class EntityNotifier
        : Entity 
    {
        protected INotifierMediatorService NotifierMediatorService;

        #region [Constructor]
        public EntityNotifier()
        {

        }

        public EntityNotifier(INotifierMediatorService notifierMediatorService)
        {
            NotifierMediatorService = notifierMediatorService;

        }

        #endregion

        public void AddNotifierMediatorService(INotifierMediatorService notifierMediatorService)
        {
            NotifierMediatorService = notifierMediatorService;
        }
    }
}

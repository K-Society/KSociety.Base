using KSociety.Base.InfraSub.Shared.Interface;

namespace KSociety.Base.Domain.Shared.Class
{
    public class BaseEntityNotifier
        : BaseEntity
    {
        protected INotifierMediatorService NotifierMediatorService;

        #region [Constructor]
        public BaseEntityNotifier()
        {


        }

        public BaseEntityNotifier(INotifierMediatorService notifierMediatorService)
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

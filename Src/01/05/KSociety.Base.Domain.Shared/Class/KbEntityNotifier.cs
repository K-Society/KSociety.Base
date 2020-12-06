using KSociety.Base.InfraSub.Shared.Interface;

namespace KSociety.Base.Domain.Shared.Class
{
    public class KbEntityNotifier
        : KbEntity 
    {
        protected INotifierMediatorService NotifierMediatorService;

        #region [Constructor]
        public KbEntityNotifier()
        {

        }

        public KbEntityNotifier(INotifierMediatorService notifierMediatorService)
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

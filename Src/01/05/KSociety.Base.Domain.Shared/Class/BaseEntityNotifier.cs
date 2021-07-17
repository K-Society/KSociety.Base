using KSociety.Base.InfraSub.Shared.Interface;

namespace KSociety.Base.Domain.Shared.Class
{
    /// <inheritdoc/>
    public class BaseEntityNotifier
        : BaseEntity
    {
        protected INotifierMediatorService NotifierMediatorService;

        #region [Constructor]

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public BaseEntityNotifier()
        {


        }

        /// <summary>
        /// Constructor with <see cref="INotifierMediatorService"/>.
        /// </summary>
        /// <param name="notifierMediatorService"><see cref="INotifierMediatorService"/></param>
        public BaseEntityNotifier(INotifierMediatorService notifierMediatorService)
        {
            NotifierMediatorService = notifierMediatorService;

        }

        #endregion

        /// <summary>
        /// Add the <see cref="INotifierMediatorService"/> to the domain entity.
        /// </summary>
        /// <param name="notifierMediatorService"><see cref="INotifierMediatorService"/></param>
        public void AddNotifierMediatorService(INotifierMediatorService notifierMediatorService)
        {
            NotifierMediatorService = notifierMediatorService;
        }
    }
}

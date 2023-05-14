using KSociety.Base.InfraSub.Shared.Interface;

namespace KSociety.Base.Domain.Shared.Class
{
    /// <inheritdoc/>
    public class EntityNotifier
        : Entity
    {
        protected INotifierMediatorService? NotifierMediatorService;

        #region [Constructor]

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public EntityNotifier()
        {

        }

        /// <summary>
        /// Constructor with <see cref="INotifierMediatorService"/> parameter.
        /// </summary>
        /// <param name="notifierMediatorService"><see cref="INotifierMediatorService"/></param>
        public EntityNotifier(INotifierMediatorService notifierMediatorService)
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
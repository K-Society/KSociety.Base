// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Domain.Shared.Class
{
    using KSociety.Base.InfraSub.Shared.Interface;

    /// <inheritdoc/>
    public class BaseEntityNotifier
        : BaseEntity
    {
        protected INotifierMediatorService? NotifierMediatorService;

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
            this.NotifierMediatorService = notifierMediatorService;

        }

        #endregion

        /// <summary>
        /// Add the <see cref="INotifierMediatorService"/> to the domain entity.
        /// </summary>
        /// <param name="notifierMediatorService"><see cref="INotifierMediatorService"/></param>
        public void AddNotifierMediatorService(INotifierMediatorService notifierMediatorService)
        {
            this.NotifierMediatorService = notifierMediatorService;
        }
    }
}

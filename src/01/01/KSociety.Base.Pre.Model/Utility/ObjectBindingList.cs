// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Pre.Model.Utility
{
    using InfraSub.Shared.Interface;
    using System.ComponentModel;
    using System.Threading;

    public class ObjectBindingList<T> : BindingList<T> where T : IObject
    {
        public SynchronizationContext SynchronizationContext { private get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:KSociety.Base.Pre.Model.Utility.ObjectBindingList`1" /> class.
        /// </summary>
        protected ObjectBindingList()
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:KSociety.Base.Pre.Model.Utility.ObjectBindingList`1" /> class.
        /// </summary>
        /// <param name="list">An <see cref="T:System.Collections.Generic.IList`1" /> of items to be contained in the <see cref="T:System.ComponentModel.BindingList`1" />.</param>
        protected ObjectBindingList(System.Collections.Generic.IList<T> list)
            : base(list)
        {
        }

        protected override void OnAddingNew(AddingNewEventArgs e)
        {
            if (this.SynchronizationContext == null)
            {
                this.BaseAddingNew(e);
            }
            else
            {
                SynchronizationContext.Current?.Send(delegate
                {
                    this.BaseAddingNew(e);
                }, null);
            }
        }

        private void BaseAddingNew(AddingNewEventArgs e)
        {
            base.OnAddingNew(e);
        }

        protected override void OnListChanged(ListChangedEventArgs e)
        {
            if (this.SynchronizationContext == null)
            {
                this.BaseListChanged(e);
            }
            else
            {
                this.SynchronizationContext.Send(delegate { this.BaseListChanged(e); }, null);
            }
        }

        private void BaseListChanged(ListChangedEventArgs e)
        {
            base.OnListChanged(e);
        }

        //protected override void SuspendBinding(SuspendBindingEventArgs e)
        //{

        //}
    }

}

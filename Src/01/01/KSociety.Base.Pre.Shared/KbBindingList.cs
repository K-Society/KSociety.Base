using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using KSociety.Base.InfraSub.Shared.Interface;

namespace KSociety.Base.Pre.Shared
{
    public class KbBindingList<T> : BindingList<T> where T : IObject
    {
        public SynchronizationContext SynchronizationContext { private get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:KSociety.Base.Pre.Shared.KbBindingList`1" /> class.
        /// </summary>
        protected KbBindingList()
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:KSociety.Base.Pre.Shared.KbBindingList`1" /> class.
        /// </summary>
        /// <param name="list">An <see cref="T:System.Collections.Generic.IList`1" /> of items to be contained in the <see cref="T:System.ComponentModel.BindingList`1" />.</param>
        protected KbBindingList(IList<T> list)
            : base(list)
        {
        }

        protected override void OnAddingNew(AddingNewEventArgs e)
        {
            if (SynchronizationContext == null)
            {
                BaseAddingNew(e);
            }
            else
            {
                SynchronizationContext.Current?.Send(delegate
                {
                    BaseAddingNew(e);
                }, null);
            }
        }

        private void BaseAddingNew(AddingNewEventArgs e)
        {
            base.OnAddingNew(e);
        }

        protected override void OnListChanged(ListChangedEventArgs e)
        {
            if (SynchronizationContext == null)
            {
                BaseListChanged(e);
            }
            else
            {
                SynchronizationContext.Send(delegate { BaseListChanged(e); }, null);
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

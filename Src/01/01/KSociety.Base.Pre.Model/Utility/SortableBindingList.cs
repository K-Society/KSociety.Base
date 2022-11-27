using KSociety.Base.InfraSub.Shared.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KSociety.Base.Pre.Model.Utility
{
    /// <inheritdoc />
    /// <summary>
    /// Provides a generic collection that supports data binding and additionally supports sorting.
    /// See http://msdn.microsoft.com/en-us/library/ms993236.aspx
    /// See https://martinwilley.com/net/code/forms/sortablebindinglist.html
    /// If the elements are IComparable it uses that; otherwise compares the ToString()
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public class SortableBindingList<T> : ObjectBindingList<T> where T : IObject
    {
        private bool _isSorted;
        private ListSortDirection _sortDirection = ListSortDirection.Ascending;
        private PropertyDescriptor _sortProperty;

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:KSociety.Base.Pre.Model.Utility.SortableBindingList`1" /> class.
        /// </summary>
        public SortableBindingList()
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:KSociety.Base.Pre.Model.Utility.SortableBindingList`1" /> class.
        /// </summary>
        /// <param name="list">An <see cref="T:System.Collections.Generic.IList`1" /> of items to be contained in the <see cref="T:System.ComponentModel.BindingList`1" />.</param>
        public SortableBindingList(System.Collections.Generic.IList<T> list)
            : base(list)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets a value indicating whether the list supports sorting.
        /// </summary>
        protected override bool SupportsSortingCore => true;

        /// <inheritdoc />
        /// <summary>
        /// Gets a value indicating whether the list is sorted.
        /// </summary>
        protected override bool IsSortedCore => _isSorted;

        /// <inheritdoc />
        /// <summary>
        /// Gets the direction the list is sorted.
        /// </summary>
        protected override ListSortDirection SortDirectionCore => _sortDirection;

        /// <inheritdoc />
        /// <summary>
        /// Gets the property descriptor that is used for sorting the list if sorting is implemented in a derived class; otherwise, returns null
        /// </summary>
        protected override PropertyDescriptor SortPropertyCore => _sortProperty;

        /// <inheritdoc />
        /// <summary>
        /// Removes any sort applied with ApplySortCore if sorting is implemented
        /// </summary>
        protected override void RemoveSortCore()
        {
            _sortDirection = ListSortDirection.Ascending;
            _sortProperty = null;
            _isSorted = false;
        }

        /// <inheritdoc />
        /// <summary>
        /// Sorts the items if overridden in a derived class
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="direction"></param>
        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            _sortProperty = prop;
            _sortDirection = direction;

            if (Items is not List<T> list) return;

            list.Sort(Compare);

            _isSorted = true;
            //fire an event that the list has been changed.
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }


        private int Compare(T lhs, T rhs)
        {
            var result = OnComparison(lhs, rhs);
            //invert if descending
            if (_sortDirection == ListSortDirection.Descending)
                result = -result;
            return result;
        }

        private int OnComparison(T lhs, T rhs)
        {
            object lhsValue = lhs == null ? null : _sortProperty.GetValue(lhs);
            object rhsValue = rhs == null ? null : _sortProperty.GetValue(rhs);
            if (lhsValue == null)
            {
                return (rhsValue == null) ? 0 : -1; //nulls are equal
            }

            if (rhsValue == null)
            {
                return 1; //first has value, second doesn't
            }

            if (lhsValue is IComparable comparable)
            {
                return comparable.CompareTo(rhsValue);
            }

            return lhsValue.Equals(rhsValue)
                ? 0
                : string.Compare(lhsValue.ToString(), rhsValue.ToString(), StringComparison.Ordinal);
            //not comparable, compare ToString
        }
    }
}
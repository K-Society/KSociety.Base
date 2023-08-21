namespace KSociety.Base.Domain.Shared.Event
{
    using System;
    using MediatR;

    /// <summary>
    /// The EntityModifyField class.
    /// </summary>
    public class EntityModifyField : INotification
    {
        public string FieldName { get; }
        public string? FieldValue { get; }
        public DateTime Entered { get; }

        /// <summary>
        /// Create a new EntityModifyField 
        /// </summary>
        /// <param name="fieldName">Field Name</param>
        /// <param name="fieldValue">Field Value</param>
        /// <param name="entered">Entered</param>
        public EntityModifyField(string fieldName, string? fieldValue, DateTime entered)
        {
            this.FieldName = fieldName;
            this.FieldValue = fieldValue;
            this.Entered = entered;
        }

        /// <summary>
        /// Create a new EntityModifyField 
        /// </summary>
        /// <param name="fieldName">Field Name</param>
        /// <param name="fieldValue">Field Value</param>
        public EntityModifyField(string fieldName, string? fieldValue)
        :this(fieldName, fieldValue, DateTime.Now) { }
    }
}

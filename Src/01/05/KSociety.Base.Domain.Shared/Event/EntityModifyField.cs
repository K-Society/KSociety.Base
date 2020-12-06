using System;
using MediatR;

namespace KSociety.Base.Domain.Shared.Event
{
    public class EntityModifyField : INotification
    {
        public string FieldName { get; }
        public string FieldValue { get; }
        public DateTime Entered { get; }

        public EntityModifyField(string fieldName, string fieldValue, DateTime entered)
        {
            FieldName = fieldName;
            FieldValue = fieldValue;
            Entered = entered;
        }
    }
}

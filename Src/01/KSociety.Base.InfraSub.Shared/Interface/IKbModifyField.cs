using System;

namespace KSociety.Base.InfraSub.Shared.Interface
{
    public interface IKbModifyField
    {
        Guid Id { get; set; }
        string FieldName { get; set; }
        string Value { get; set; }
    }
}

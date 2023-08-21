namespace KSociety.Base.InfraSub.Shared.Interface
{
    using System;

    public interface IModifyField
    {
        Guid Id { get; set; }
        string FieldName { get; set; }
        string Value { get; set; }
    }
}
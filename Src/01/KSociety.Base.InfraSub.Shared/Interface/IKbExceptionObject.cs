namespace KSociety.Base.InfraSub.Shared.Interface
{
    public interface IKbExceptionObject
    {
        string ErrorMessage { get; set; }
        string ErrorStackTrace { get; set; }
    }
}

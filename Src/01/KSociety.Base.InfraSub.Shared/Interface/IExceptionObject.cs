namespace KSociety.Base.InfraSub.Shared.Interface;

public interface IExceptionObject
{
    string ErrorMessage { get; set; }
    string ErrorStackTrace { get; set; }
}
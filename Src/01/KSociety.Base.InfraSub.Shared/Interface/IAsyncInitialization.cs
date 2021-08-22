using System.Threading.Tasks;

namespace KSociety.Base.InfraSub.Shared.Interface
{
    /// <summary>
    /// Marks a type as requiring asynchronous initialization and provides the result of that initialization.
    /// </summary>
    public interface IAsyncInitialization
    {
        /// <summary>
        /// The result of the asynchronous initialization of this instance.
        /// </summary>
        ValueTask Initialization { get; }
    }
}

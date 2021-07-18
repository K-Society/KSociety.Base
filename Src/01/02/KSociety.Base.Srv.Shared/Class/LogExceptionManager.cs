using System;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.Srv.Shared.Class
{
    public static class LogExceptionManager
    {
        public static Exception LogException(ILogger logger, Exception ex)
        {
            logger.LogError("Execute Exception: " + ex.Message + " - " + ex.StackTrace);
            if (ex.InnerException == null) { return ex; }
            logger.LogError("Execute InnerException: " + ex.InnerException.Message + " - " + ex.InnerException.StackTrace);
            if (ex.InnerException.InnerException == null) { return ex.InnerException; }
            logger.LogError("Execute Inner Inner Exception: " + ex.InnerException.InnerException.Message + " - " + ex.InnerException.InnerException.StackTrace);
            return ex.InnerException.InnerException;
        }
    }
}

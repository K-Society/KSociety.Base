using System;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.Srv.Shared.Class
{
    public static class LogExceptionManager
    {
        public static Exception LogException(ILogger logger, Exception ex)
        {
            logger.LogError(ex, "Execute Exception: ");
            if (ex.InnerException == null) return ex;
            logger.LogError(ex.InnerException, "Execute InnerException: ");
            if (ex.InnerException.InnerException == null) return ex.InnerException;
            logger.LogError(ex.InnerException.InnerException, "Execute Inner Inner Exception: ");
            return ex.InnerException.InnerException;
        }
    }
}
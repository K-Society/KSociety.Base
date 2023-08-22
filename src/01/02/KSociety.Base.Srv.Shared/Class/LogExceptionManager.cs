// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Shared.Class
{
    using System;
    using Microsoft.Extensions.Logging;

    public static class LogExceptionManager
    {
        public static Exception LogException(ILogger logger, Exception ex)
        {
            logger.LogError(ex, "Execute Exception: ");
            if (ex.InnerException == null)
            {
                return ex;
            }

            logger.LogError(ex.InnerException, "Execute InnerException: ");
            if (ex.InnerException.InnerException == null)
            {
                return ex.InnerException;
            }

            logger.LogError(ex.InnerException.InnerException, "Execute Inner Inner Exception: ");
            return ex.InnerException.InnerException;
        }
    }
}

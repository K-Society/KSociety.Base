// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Infra.Shared.Class
{
    using KSociety.Base.Infra.Abstraction.Interface;
    using Interface;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TLoggerFactory"></typeparam>
    /// <typeparam name="TConfiguration"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    public class DatabaseFactory<TLoggerFactory, TConfiguration, TContext>
        : DatabaseFactoryBase<TLoggerFactory, TConfiguration, TContext>, IDatabaseFactory<TContext>
        where TLoggerFactory : ILoggerFactory
        where TConfiguration : IDatabaseConfiguration
        where TContext : DbContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="mediator"></param>
        public DatabaseFactory(TLoggerFactory loggerFactory, TConfiguration configuration, IMediator mediator)
            : base(loggerFactory, configuration, mediator)
        {

        }
    }
}

using KSociety.Base.Infra.Shared.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.Infra.Shared.Class
{
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
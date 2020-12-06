using KSociety.Base.Infra.Shared.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.Infra.Shared.Class
{
    public class DatabaseFactory<TLoggerFactory, TConfiguration, TContext>
        : DatabaseFactoryBase<TLoggerFactory, TConfiguration, TContext>, IDatabaseFactory<TContext>
        where TLoggerFactory : ILoggerFactory
        where TConfiguration : IDatabaseConfiguration
        where TContext : DbContext
    {

        public DatabaseFactory(TLoggerFactory loggerFactory, TConfiguration configuration, IMediator mediator)
            : base(loggerFactory, configuration, mediator)
        {

        }
    }
}

using KSociety.Base.Infra.Shared.Class;
using Microsoft.EntityFrameworkCore.Design;

namespace KSociety.Base.Infra.Shared.Interface;

/// <include file='..\Doc\ContextFactory.xml' path='docs/members[@name="ContextFactory"]/ContextFactory/*'/>
public interface IContextFactory<out TContext> 
    : IDesignTimeDbContextFactory<TContext> where TContext : DatabaseContext
{

}
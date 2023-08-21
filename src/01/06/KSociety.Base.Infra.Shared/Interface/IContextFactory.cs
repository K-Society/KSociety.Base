namespace KSociety.Base.Infra.Shared.Interface
{
    using Class;
    using Microsoft.EntityFrameworkCore.Design;

    /// <include file='..\Doc\ContextFactory.xml' path='docs/members[@name="ContextFactory"]/ContextFactory/*'/>
    public interface IContextFactory<out TContext>
        : IDesignTimeDbContextFactory<TContext> where TContext : DatabaseContext
    {

    }
}

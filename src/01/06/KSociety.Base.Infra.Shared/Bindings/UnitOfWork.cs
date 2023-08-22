// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Infra.Shared.Bindings
{
    using Autofac;
    using KSociety.Base.Infra.Abstraction.Interface;
    using Class;

    /// <summary>
    /// The UnitOfWork module for Autofac.
    /// </summary>
    /// <typeparam name="TContext">TContext is DatabaseContext</typeparam>
    public class UnitOfWork<TContext> : Module where TContext : DatabaseContext
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Infra.Shared.Class.UnitOfWork<TContext>>().As<IDatabaseUnitOfWork>();
        }
    }
}

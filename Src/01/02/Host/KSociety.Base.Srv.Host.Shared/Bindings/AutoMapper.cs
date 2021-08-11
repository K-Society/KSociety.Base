using System.Collections.Generic;
using System.Linq;
using Autofac;
using AutoMapper;

namespace KSociety.Base.Srv.Host.Shared.Bindings
{
    /// <summary>
    /// 
    /// </summary>
    public class AutoMapper : Module
    {
        private readonly string[] _assemblies;

        public AutoMapper(string[] assemblies)
        {
            _assemblies = assemblies;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = _assemblies.Select(System.Reflection.Assembly.LoadFrom).ToArray();

            //RegisterAutoMapper Profile
            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => typeof(Profile).IsAssignableFrom(t) && !t.IsAbstract && t.IsPublic)
                .As<Profile>();

            builder.Register(c => new MapperConfiguration(cfg =>
            {
                foreach (var profile in c.Resolve<IEnumerable<Profile>>())
                {
                    cfg.AddProfile(profile);
                }
            })).AsSelf().SingleInstance();

            builder.Register(c => c.Resolve<MapperConfiguration>()
                    .CreateMapper(c.Resolve))
                .As<IMapper>()
                .InstancePerLifetimeScope();
        }
    }
}

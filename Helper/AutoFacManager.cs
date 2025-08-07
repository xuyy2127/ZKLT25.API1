using Autofac;
using System.Reflection;

namespace ZKLT25.API.Helper
{
    public class AutoFacManager : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var Services = Assembly.Load("ZKLT25.API");
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).Where(t => t.Name.EndsWith("Repo") || t.Name.EndsWith("Service") || t.Name.Contains("Service")).AsImplementedInterfaces();
            base.Load(builder);
        }
    }
}
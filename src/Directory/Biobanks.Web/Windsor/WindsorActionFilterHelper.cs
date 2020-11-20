using Castle.MicroKernel;
using System.Reflection;

namespace Biobanks.Web.Windsor
{
    public static class WindsorActionFilterHelper
    {
        public static void InjectProperties(this IKernel kernel, object target)
        {
            var type = target.GetType();

            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.CanWrite && kernel.HasComponent(property.PropertyType))
                {
                    var value = kernel.Resolve(property.PropertyType);

                    property.SetValue(target, value, null);
                }
            }
        }
    }
}
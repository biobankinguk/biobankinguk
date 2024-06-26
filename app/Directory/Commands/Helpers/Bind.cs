using System.CommandLine.Binding;
using Microsoft.Extensions.DependencyInjection;

namespace Biobanks.Directory.Commands.Helpers;

internal static class Bind
{
  public static ServiceProviderBinder<T> FromServiceProvider<T>() => ServiceProviderBinder<T>.Instance;

  internal class ServiceProviderBinder<T> : BinderBase<T>
  {
    public static ServiceProviderBinder<T> Instance { get; } = new();

    protected override T GetBoundValue(BindingContext bindingContext)
      => (T)bindingContext.GetRequiredService(typeof(T));
  }
}

using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using Microsoft.Extensions.DependencyInjection;

namespace Biobanks.Submissions.Api.Commands.Helpers;

public static class ConfigureCommandServices
{
  /// <summary>
  /// Specify DI Services configuration then build a service provider from that configuration.
  /// 
  /// Approximately similar interface to the standard .NET Hosting Model's ConfigureServices()
  /// </summary>
  /// <param name="_"></param>
  /// <param name="c">The invocation context from withina Command Handler</param>
  /// <param name="configure">The configuration action</param>
  /// <returns></returns>
  public static IServiceProvider ConfigureServices(this Command _, InvocationContext c, Action<InvocationContext, IServiceCollection> configure)
  {
    var s = new ServiceCollection();
    configure.Invoke(c, s);
    return s.BuildServiceProvider();
  }

  /// <summary>
  /// Specify DI Services configuration then build a service provider from that configuration.
  /// 
  /// Approximately similar interface to the standard .NET Hosting Model's ConfigureServices()
  /// </summary>
  /// <param name="_"></param>
  /// <param name="configure">The configuration action</param>
  /// <returns></returns>
  public static IServiceProvider ConfigureServices(this Command _, Action<IServiceCollection> configure)
  {
    var s = new ServiceCollection();
    configure.Invoke(s);
    return s.BuildServiceProvider();
  }
}

using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory.Contracts;

public interface IRecaptchaService
{
  /// <summary>
  /// Recaptcha specifically for the register service.
  /// </summary>
  /// <returns></returns>
  Task<bool> ValidateRegisterEntity();
}

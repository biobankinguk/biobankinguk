using System.Threading.Tasks;
using Biobanks.Directory.Models.Register;

namespace Biobanks.Directory.Services.Directory.Contracts;

public interface IRecaptchaService
{
  /// <summary>
  /// Verify a recaptcha token against the Google API and return the response
  /// </summary>
  /// <param name="recaptchaToken">A users Recaptcha response</param>
  /// <returns>A Recaptcha Response object.</returns>
  Task<RecaptchaResponse> VerifyToken(string recaptchaToken);
}

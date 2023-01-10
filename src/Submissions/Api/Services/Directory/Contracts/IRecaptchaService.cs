using System.Threading.Tasks;
using Biobanks.Submissions.Api.Models.Register;

namespace Biobanks.Submissions.Api.Services.Directory.Contracts;

public interface IRecaptchaService
{
  /// <summary>
  /// Verify a recaptcha token against the Google API and return the response
  /// </summary>
  /// <param name="recaptchaToken">A users Recaptcha response</param>
  /// <returns>A Recaptcha Response object.</returns>
  Task<RecaptchaResponse> VerifyToken(string recaptchaToken);
}

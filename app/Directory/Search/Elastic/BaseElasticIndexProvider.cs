using Biobanks.Directory.Search.Exceptions;
using Nest;

namespace Biobanks.Directory.Search.Elastic
{
    public abstract class BaseElasticIndexProvider
    {
        protected static void HandleIndexResponse(IResponse indexResponse)
        {
            if (!indexResponse.IsValid)
            {
                throw new InvalidIndexResponseException(indexResponse.OriginalException?.Message);
            }

            if (indexResponse.IsValid && indexResponse.ServerError != null)
            {
                throw new InvalidIndexResponseException(indexResponse.ServerError.Error.Reason);
            }
        }
    }
}

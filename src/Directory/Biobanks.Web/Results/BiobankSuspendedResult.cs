using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Entities.Data;
using Directory.Services;

namespace Biobanks.Web.Results
{
    public class BiobankSuspendedResult : ActionResult
    {
        public string BiobankName { get; private set; }

        public BiobankSuspendedResult(string biobankName)
        {
            BiobankName = biobankName;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            //We actually don't perform any useful action in this result;
            //We just use the result in OnAuthenticationChallenge to identify what happened
        }
    }
}

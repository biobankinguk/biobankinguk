using System;
using System.Web.Mvc;

namespace Biobanks.Web.Results
{
    public class UnauthorizedResult : ActionResult
    {

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            //We actually don't perform any useful action in this result;
            //We just use the result in OnAuthenticationChallenge to identify what happened
        }
    }
}
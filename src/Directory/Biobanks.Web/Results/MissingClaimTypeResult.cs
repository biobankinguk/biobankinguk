using System;
using System.Web.Mvc;

namespace Biobanks.Web.Results
{
    public class MissingClaimTypeResult : ActionResult
    {
        public string ExpectedClaimType { get; private set; }

        public MissingClaimTypeResult(string expectedClaimType)
        {
            ExpectedClaimType = expectedClaimType;
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
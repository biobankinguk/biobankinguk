using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biobanks.Web.Models.Shared
{
    public class ErrorStatusModel
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
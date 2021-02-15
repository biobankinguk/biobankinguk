using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biobanks.Services.Dto
{
    public class ErrorStatusModelDTO
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}

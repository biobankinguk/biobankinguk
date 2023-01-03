using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biobanks.Data.Transforms.Url;
public class InvalidUrlSchemeException : Exception
{
  public InvalidUrlSchemeException(string message) : base(message) { }
}

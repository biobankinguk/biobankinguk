using Nest;
using System;


namespace Biobanks.Search.Dto.Documents
{
    public class OtherTermsDocument
    {
        [Keyword(Name = "name")]
        public string Name { get; set; }
    }
}

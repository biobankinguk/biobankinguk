using System;
using System.Collections.Generic;
using System.Text;

namespace Publications.Models
{
    class Publication
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Authors { get; set; }

        public string Journal { get; set; }

        public int Year { get; set; }

        public string DOI { get; set; }
    }
}

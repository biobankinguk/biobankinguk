using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Directory.Entity.Data
{
    public class Annotation
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Uri { get; set; }

        public List<Publication> Publications { get; set; }
    }
}

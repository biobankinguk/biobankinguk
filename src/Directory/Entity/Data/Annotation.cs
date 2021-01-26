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

        public string AnnotationId { get; set; }
        public string Name { get; set; }

        public string Uri { get; set; }

        public virtual ICollection<Publication> Publications { get; set; }
    }
}

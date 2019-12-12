using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftwareRequirements.Models.Db
{
    public class Requirement
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Column(TypeName = "jsonb")]
        public string Profile { get; set; }

        public int? ParentId { get; set; }

        public virtual Requirement Parent { get; set; }

        public virtual List<Requirement> Requirements { get; set; }   
    }
}
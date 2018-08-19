using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShiftScheduler.Models
{
    public class Engineer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        
        public IList<Shift> Shifts { get; set; }
        
        public Engineer()
        {
            Shifts = new List<Shift>();
        }
    }
}
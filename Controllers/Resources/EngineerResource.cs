using System.Collections.Generic;

namespace ShiftScheduler.Controllers.Resources
{
    public class EngineerResource
    {
         public int Id { get; set; }
        public string Name { get; set; }
        
        public IList<ShiftResource> Shifts { get; set; }
        
        public EngineerResource()
        {
            Shifts = new List<ShiftResource>();
        }
    }
}
using System;

namespace ShiftScheduler.Models
{
    public class Shift
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public byte Slot { get; set; }
        
        public Engineer Engineer { get; set; }
        public int EngineerId { get; set; }
    }
}
using System;
using ShiftScheduler.Models;

namespace ShiftScheduler.Controllers.Resources
{
    public class ShiftResource
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public byte Slot { get; set; }        
        public string EngineerName { get; set; }
    }
}
using AutoMapper;
using ShiftScheduler.Models;
using ShiftScheduler.Controllers.Resources;

namespace ShiftScheduler.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Domain to API Resource
            CreateMap<Shift, ShiftResource>().ForMember(sr => sr.EngineerName, opt => opt.MapFrom(s => s.Engineer.Name));
            CreateMap<Engineer, EngineerResource>();
        }
    }
}
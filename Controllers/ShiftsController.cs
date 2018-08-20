using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShiftScheduler.Controllers.Resources;
using ShiftScheduler.Models;
using ShiftScheduler.Persistence;

namespace ShiftScheduler.Controllers
{
    [Route("/api/shifts")]
    public class ShiftsController : Controller
    {
        private readonly ShiftSchedulerDbContext _context;
        private readonly IMapper _mapper;

        //Assumptions
        //1- Assume that Company X has 10 engineers.
        //Already populate the database with 10 engineers.
        readonly List<Engineer> _engineers;
        //2- Assume that the schedule will span two weeks and start on the first working day of the upcoming week.
        private const int SpanWeeksCount = 2;

        private const int WorkingDaysPerWeekCount = 5;
        private const byte ShiftsPerdayCount = 2;
        private const DayOfWeek FirstWorkingDay = DayOfWeek.Monday;

        public ShiftsController(ShiftSchedulerDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _engineers = context.Engineers.ToList();
        }

        private bool IsValidShift(Shift shift, List<Shift> shifts, int indexOfSlot)
        {
            var numberOfShifts = ShiftsPerdayCount * SpanWeeksCount * WorkingDaysPerWeekCount;
            //An engineer can do at most one-half day shift in a day.
            var todayShifts = shifts.Where(ns => ns.Date == shift.Date).ToList();
            if (todayShifts.Any(ts => ts.EngineerId == shift.EngineerId))
                return false;
            if (indexOfSlot == numberOfShifts - (ShiftsPerdayCount - 1) && _engineers.Any(eng => shifts.Count(s => s.EngineerId == eng.Id) == 0))
                return false;

            //An engineer cannot have two afternoon shifts on consecutive days.
            //Friday and Monday are considered consecutive days.
            var oneDayBeforeShiftDate = shift.Date.AddDays(-1);
            while (oneDayBeforeShiftDate.DayOfWeek == DayOfWeek.Saturday || oneDayBeforeShiftDate.DayOfWeek == DayOfWeek.Sunday)
            {
                oneDayBeforeShiftDate = oneDayBeforeShiftDate.AddDays(-1);
            }

            var yesterdayLastShift = shifts.FirstOrDefault(s => s.Date == oneDayBeforeShiftDate && s.Slot == ShiftsPerdayCount);
            if (indexOfSlot == numberOfShifts - ShiftsPerdayCount && yesterdayLastShift != null && shifts.Count(s => s.EngineerId == yesterdayLastShift.EngineerId) == 1)
            {
                return false;
            }
            if (shift.Slot == ShiftsPerdayCount)
            {
                yesterdayLastShift = shifts.FirstOrDefault(s => s.Date == oneDayBeforeShiftDate && s.Slot == ShiftsPerdayCount);
                if (yesterdayLastShift != null && yesterdayLastShift.EngineerId == shift.EngineerId)
                    return false;
            }

            //If an engineer work on two consecutive days are eligible to get two days exemption.
            //Friday and Monday are considered consecutive days.
            var twoDayBeforeShiftDate = oneDayBeforeShiftDate.AddDays(-1);
            while (twoDayBeforeShiftDate.DayOfWeek == DayOfWeek.Saturday || twoDayBeforeShiftDate.DayOfWeek == DayOfWeek.Sunday)
            {
                twoDayBeforeShiftDate = twoDayBeforeShiftDate.AddDays(-1);
            }

            var twoDayBeforeShifts = shifts.Where(s => s.Date == oneDayBeforeShiftDate && s.Date == twoDayBeforeShiftDate).ToList();
            if (twoDayBeforeShifts.Any() && twoDayBeforeShifts.Count(ts => ts.EngineerId == shift.EngineerId) >= 2)
                return false;

            var threeDayBeforeShiftDate = twoDayBeforeShiftDate.AddDays(-1);
            while (threeDayBeforeShiftDate.DayOfWeek == DayOfWeek.Saturday || threeDayBeforeShiftDate.DayOfWeek == DayOfWeek.Sunday)
            {
                threeDayBeforeShiftDate = threeDayBeforeShiftDate.AddDays(-1);
            }
            var twoDayBeforeYShifts = shifts.Where(s => s.Date == twoDayBeforeShiftDate && s.Date == threeDayBeforeShiftDate).ToList();
            if (twoDayBeforeYShifts.Any() && twoDayBeforeYShifts.Count(ts => ts.EngineerId == shift.EngineerId) >= 2)
                return false;

            //Each engineer should have completed one whole day of support in any 2 week period.
            if (shifts.Count(s => s.EngineerId == shift.EngineerId) == 2)
            {
                return false;
            }

            return true;
        }

        [HttpGet]
        public async Task<IActionResult> GetShifts()
        {
            var shifts = await _context.Shifts.Include(e => e.Engineer).OrderBy(e => e.Date).ThenBy(e => e.Slot).ToListAsync();
            var upcomingWeekFirstDay = DateTime.Now.Date;
            while ((upcomingWeekFirstDay.DayOfWeek != FirstWorkingDay))
            {
                upcomingWeekFirstDay = upcomingWeekFirstDay.AddDays(1);
            }
            if (shifts.Count == 0)
            {
                var currentDate = upcomingWeekFirstDay;
                byte currentSlot = 1;
                var random = new Random();

                for (var i = 0; i < ShiftsPerdayCount * SpanWeeksCount * WorkingDaysPerWeekCount; i++)
                {
                    var shift = new Shift
                    {
                        Date = currentDate,
                        Slot = currentSlot
                    };
                    var added = false;
                    while (!added)
                    {
                        var number = random.Next(0, _engineers.Count);
                        var eng = _engineers[number];
                        shift.EngineerId = eng.Id;
                        if (!IsValidShift(shift, shifts, i)) continue;

                        shifts.Add(shift);
                        added = true;
                    }

                    if (currentSlot >= ShiftsPerdayCount)
                    {
                        currentDate = currentDate.AddDays(1);
                        while ((currentDate.DayOfWeek == DayOfWeek.Saturday) || (currentDate.DayOfWeek == DayOfWeek.Sunday))
                        {
                            currentDate = currentDate.AddDays(1);
                        }
                        currentSlot = 1;
                    }
                    else
                        currentSlot++;
                }
                _context.Shifts.AddRange(shifts);
                await _context.SaveChangesAsync();
            }
            else
            {
                if (shifts[0].Date != upcomingWeekFirstDay)
                {
                    //to span the schedule to start on the first working day of the upcoming week.
                    var daysToAdd = (upcomingWeekFirstDay - shifts[0].Date).TotalDays;
                    if ((daysToAdd / 7) % 2 != 0)
                    {
                        for (int i = 0; i < shifts.Count; i++)
                        {
                            if (i > shifts.Count / 2)
                                shifts[i].Date = shifts[i].Date.AddDays(daysToAdd + 7);
                            else
                                shifts[i].Date = shifts[i].Date.AddDays(daysToAdd - 7);
                        }
                    }
                    else
                    {
                        foreach (var shift in shifts)
                        {
                            shift.Date = shift.Date.AddDays(daysToAdd);
                        }
                    }
                }
            }
            return Ok(_mapper.Map<List<Shift>, List<ShiftResource>>(shifts));
        }
    }
}
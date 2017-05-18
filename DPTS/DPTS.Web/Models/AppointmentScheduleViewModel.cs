using DPTS.Domain.Entities;
using System.Collections.Generic;

namespace DPTS.Web.Models
{
    public class AppointmentScheduleViewModel
    {
        public AppointmentScheduleViewModel()
        {
            SessionOneScheduleSlotModel = new List<ScheduleSlotModel>();
            SessionTwoScheduleSlotModel = new List<ScheduleSlotModel>();
        }
        public IList<ScheduleSlotModel> SessionOneScheduleSlotModel { get; set; }
        public IList<ScheduleSlotModel> SessionTwoScheduleSlotModel { get; set; }
        public AppointmentSchedule AppointmentSchedule { get; set; }
        public string doctorId { get; set; }
    }
    public class ScheduleSlotModel
    {
        public string Slot { get; set; }
        public string Session { get; set; }
        public bool IsBooked { get; set; }
    }
}
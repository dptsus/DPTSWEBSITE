using System;
using System.ComponentModel.DataAnnotations;

namespace DPTS.Domain.Entities
{
    public partial class Schedule
    {
        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        public string DoctorId { get; set; }

        [Required]
        [StringLength(10)]
        public string Day { get; set; }

        public string SessionOneStartTime { get; set; }

        public string SessionOneEndTime { get; set; }

        public string SessionTwoStartTime { get; set; }

        public string SessionTwoEndTime { get; set; }

        public virtual Doctor Doctor { get; set; }
    }
}

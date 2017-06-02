namespace DPTS.Web.Models
{
    public class ConsultEmailViewModel
    {
        public string DoctorName { get; set; }
        public string DoctorId { get; set; }
        public string DoctorAddress { get; set; }
        public string PatientName { get; set; }
        public decimal DoctorEmailCharge { get; set; }
        public string Question { get; set; }
    }
}
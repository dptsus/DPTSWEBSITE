namespace DPTS.Web.Models
{
    public class SuggestedDoctorViewModel
    {
        public string DoctorId { get; set; }
        public string Name { get; set; }
        public string Specailities  { get; set; }
        public PictureModel Picture { get; set; }
        public string AddressLine { get; set; }
        public string Qualification { get; set; }
        public string YearOfExperience { get; set; }
        public int RatingPercentag { get; set; }
        public decimal Fees { get; set; }
    }
}
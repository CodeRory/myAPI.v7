namespace TodoApi.Requests
{
    public class UpdateEmploymentRequest
    {
        //public int Id { get; set; }
        //public int UserId { get; set; } //We need to conect user with employments
        public string? Company { get; set; } //NAME CANT BE REPEATED //MANDATORY
        public int? MonthOfExperince { get; set; }
        public int? Salary { get; set; } //MANDATORY        
        public DateTime? StartDate { get; set; } //MANDATORY
        public DateTime? EndDate { get; set; }
    }
}

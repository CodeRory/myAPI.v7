using TodoApi.Models;

namespace TodoApi.Requests
{
    public class CreateUserRequest
    {
        public CreateUserRequest() //Constructor
        {
            Employments = new List<Employment>();
        }
        //public int Id { get; set; }
        //public Guid UniqueId { get; set; }
        public string? FirstName { get; set; } //MANDATORY
        public string? LastName { get; set; } //MANDATORY
        public int? Age { get; set; } //MANDATORY
        public DateTime? Birthday { get; set; } //MANDATORY
        public Address? Address { get; set; }
        public List<Employment> Employments { get; set; }
    }
}

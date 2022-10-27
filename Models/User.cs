using Microsoft.AspNetCore.Mvc;

namespace TodoApi.Models
{
    public class User
    {
        //We are going to create a constructor in order to avoid the nullable problem with List<Employment> 
        public User()
        {
            Employments = new List<Employment>();
        }
        public int Id { get; set; }      
        public Guid UniqueId { get; set; }
        public string? FirstName { get; set; } //MANDATORY
        public string? LastName { get; set; } //MANDATORY
        public int? Age { get; set; } //MANDATORY
        public DateTime? Birthday { get; set; } //MANDATORY
        public Address? Address { get; set; }
        public List<Employment> Employments { get; set; }
       
    }
}

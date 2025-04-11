using System.ComponentModel.DataAnnotations;

namespace ExamenCSharp.Models {

    public class Intervention 
    {
        [Key]
        public int Id { get; set;}

        [Required]
        public DateTime Reservation { get; set; }
        
        public required List<User> Techniciens {get; set;}

        [Required]
        public required Service Type { get; set; }

        public string? ClientId { get; set; }

        public User? Client {get; set;}
    }

}



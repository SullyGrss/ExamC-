using ExamenCSharp.Models;

namespace ExamenCSharp.DTO {
    public class InterventionDTO
    {
        public int Id { get; set;}
        
        public DateTime Reservation { get; set; }
        
        public required List<UserDTO> Techniciens {get; set;}

        public required ServiceDTO Type { get; set; }

        public required UserDTO Client {get; set;}
    }
}
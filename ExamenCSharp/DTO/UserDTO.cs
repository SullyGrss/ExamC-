using System.ComponentModel.DataAnnotations;

namespace ExamenCSharp.DTO {

    public class UserDTO
    {
        public required string Nom { get; set; } 

        public required string Prenom {get; set; }

        public required string Email { get; set; }

    }

}

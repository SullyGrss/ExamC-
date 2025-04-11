using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ExamenCSharp.Models {

    public class User : IdentityUser
    {
        [Required]
        public required string Nom { get; set; } 

        [Required]
        public required string Prenom {get; set; }

        public List<Intervention>? Interventions{ get; set; }
        

    }

}

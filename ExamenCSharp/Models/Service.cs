using System.ComponentModel.DataAnnotations;

namespace ExamenCSharp.Models {

    public class Service 
    {
        [Key]
        public int Id { get; set;}

        [Required]
        public required string NomService { get; set; }
        
    }

}

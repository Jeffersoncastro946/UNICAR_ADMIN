using System.ComponentModel.DataAnnotations;

namespace UNICAR_ADMIN.Models.DTOS
{
    public class ClienteDto
    {
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "El campo {0} es necesario")]
        [StringLength(100, ErrorMessage ="El Campo {0} no puede exceder los {1} caracteres")]
        public string? NombreCompleto { get; set; }

        [Required(ErrorMessage ="el campo {0} es obligatorio")]
        [RegularExpression(@"^\d{13}$", ErrorMessage = "La identidad debe tener exactamente 13 dígitos numéricos.")]
        public string? Identidad { get; set; }

        [Required(ErrorMessage = "el campo {0} es obligatorio")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "el RTN  debe tener exactamente 14 dígitos numéricos.")]
        public string? Rtn { get; set; }

        [Required(ErrorMessage = "el campo {0} es obligatorio")]
        [RegularExpression(@"^\d{8}$",ErrorMessage ="El telefono debe pospago como Tigo o Claro")]

        public string? Telefono { get; set; }

        [StringLength (100, ErrorMessage = "El campo {0} no puede exceder los {1} caracteres.")]
        public string? Direccion { get; set; }
        [Display(Name = "¿Esta activo?")]
        public bool? Activo { get; set; }

    }
}

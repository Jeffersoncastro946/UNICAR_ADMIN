using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace UNICAR_ADMIN.Models.DTOS
{

    public class CrearVehiculoDTO
    {

        // ---------------------------
        // Datos de fotos del vehículo
        // ---------------------------
        public string? FotoFrontal { get; set; }
        public string? FotoTrasera { get; set; }
        public string? FotoLateralIzquierda { get; set; }
        public string? FotoLateralDerecha { get; set; }
        public string? FotoInterior1 { get; set; }
        public string? FotoInterior2 { get; set; }
        public string? FotoMotor { get; set; }
        public string? FotoExtra1 { get; set; }
        public string? FotoExtra2 { get; set; }
        public string? Miniatura { get; set; }

        // --------------
        // Datos vehículo
        // --------------

        public int VehiculoId { get; set; }

        [Required(ErrorMessage = "el campo {0} es obligaotorio" )]
        public string Marca { get; set; } 


        
        [Required(ErrorMessage = "el campo {0} es obligaotorio")]
        public string Modelo { get; set; } 
        

        [Required(ErrorMessage = "el campo {0} es obligaotorio")]
        [Range(1900, 2025, ErrorMessage = "El año debe estar entre {1} y {2}")]
        public int Anio { get; set; }   


        
        [Required(ErrorMessage = "el campo {0} es obligaotorio")]
        public string? Color { get; set; }      
        

        [Required(ErrorMessage = "el campo {0} es obligaotorio")]
        [StringLength(17, ErrorMessage = "El VIN debe tener exactamente 17 caracteres", MinimumLength = 17)]
        public string VIN { get; set; }                    



        [Required(ErrorMessage = "el campo {0} es obligaotorio")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser un valor positivo ")]
        public decimal Precio { get; set; }   
                        
        public DateTime? FechaIngreso { get; set; }

        [Display(Name = "Es por Consignacion")]
        public bool EsConsignacion { get; set; }

        // ================================
        // 2) ARCHIVOS PARA SUBIR (IFormFile)
        // ================================
        [Display(Name = "Foto frontal")]
        public IFormFile? FotoFrontalFile { get; set; }

        [Display(Name = "Foto trasera")]
        public IFormFile? FotoTraseraFile { get; set; }

        [Display(Name = "Foto lateral izquierda")]
        public IFormFile? FotoLateralIzquierdaFile { get; set; }

        [Display(Name = "Foto lateral derecha")]
        public IFormFile? FotoLateralDerechaFile { get; set; }

        [Display(Name = "Foto interior 1")]
        public IFormFile? FotoInterior1File { get; set; }

        [Display(Name = "Foto interior 2")]
        public IFormFile? FotoInterior2File { get; set; }

        [Display(Name = "Foto motor")]
        public IFormFile? FotoMotorFile { get; set; }

        [Display(Name = "Foto extra 1")]
        public IFormFile? FotoExtra1File { get; set; }

        [Display(Name = "Foto extra 2")]
        public IFormFile? FotoExtra2File { get; set; }

        [Display(Name = "Miniatura")]
        
        public IFormFile? MiniaturaFile { get; set; }


        // ================================
        // 3) PROVEEDOR: dropdown + lista
        // ================================
        [Required(ErrorMessage = "Debes seleccionar un proveedor.")]
        [Display(Name = "Proveedor")]
        public int ProveedorId { get; set; }

        [Display(Name = "Estado del vehiculo")]
        public int estadoId { get; set; }

        [Display(Name = "Lista de proveedores")]
        public IEnumerable<SelectListItem> ListaProveedores { get; set; } = new List<SelectListItem>();

        [Display(Name = "Lista de estados")]
        public IEnumerable<SelectListItem> ListaEstados { get; set; } = new List<SelectListItem>();    





    }
}

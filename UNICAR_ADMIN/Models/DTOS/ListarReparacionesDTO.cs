using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace UNICAR_ADMIN.Models.DTOS
{
    public class ListarReparacionesDTO
    {
        #region VEHICULO
            public int VehiculoId { get; set; }
            public string? Marca { get; set; }
            public string? Modelo { get; set; }
            public int Anio { get; set; }
            public string? Color { get; set; }
            public string? Vin { get; set; }
        #endregion

        #region Reparaciones
            public int ReparacionId { get; set; }
            public string? Descripcion { get; set; }
            public double costo { get; set; }
            public string? ImagenUrl { get; set; }
            public DateTime? FechaInicio { get; set; }
            public DateTime? FechaFin { get; set; }
        #endregion

    }
}

using System;
using System.Collections.Generic;

namespace UNICAR_ADMIN.Models.Renta;

public partial class Reparacione
{
    public int ReparacionId { get; set; }

    public int? VehiculoId { get; set; }

    public string? Descripcion { get; set; }

    public decimal? Costo { get; set; }

    public DateTime? FechaReparacion { get; set; }

    public string? ImagenUrl { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public string? UsuarioCreacion { get; set; }

    public string? UsuarioModificacion { get; set; }

    public virtual Vehiculo? Vehiculo { get; set; }
}

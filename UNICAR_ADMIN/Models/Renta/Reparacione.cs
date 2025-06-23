using System;
using System.Collections.Generic;

namespace UNICAR_ADMIN.Models.Renta;

public partial class Reparacione
{
    public int ReparacionId { get; set; }

    public int VehiculoId { get; set; }

    public string? Descripcion { get; set; }

    public decimal? Costo { get; set; }

    public string? ImagenUrl { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public string? UsuarioCreacion { get; set; }

    public string? UsuarioModificacion { get; set; }

    public bool? Activo { get; set; }

    public DateTime FechaInicial { get; set; }

    public DateTime FechaFinal { get; set; }

    public string Responsable { get; set; } = null!;

    public virtual Vehiculo Vehiculo { get; set; } = null!;
}

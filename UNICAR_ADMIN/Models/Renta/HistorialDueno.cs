using System;
using System.Collections.Generic;

namespace UNICAR_ADMIN.Models.Renta;

public partial class HistorialDueno
{
    public int HistorialId { get; set; }

    public int? VehiculoId { get; set; }

    public string? Nombre { get; set; }

    public string? Identidad { get; set; }

    public DateTime? FechaTransferencia { get; set; }

    public string? Tipo { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public string? UsuarioCreacion { get; set; }

    public string? UsuarioModificacion { get; set; }

    public virtual Vehiculo? Vehiculo { get; set; }
}

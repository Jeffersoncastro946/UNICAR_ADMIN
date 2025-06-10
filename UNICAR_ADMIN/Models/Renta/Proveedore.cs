using System;
using System.Collections.Generic;

namespace UNICAR_ADMIN.Models.Renta;

public partial class Proveedore
{
    public int ProveedorId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? Correo { get; set; }

    public string? Direccion { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public string? UsuarioCreacion { get; set; }

    public string? UsuarioModificacion { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}

using System;
using System.Collections.Generic;

namespace UNICAR_ADMIN.Models.Renta;

public partial class Vendedore
{
    public int VendedorId { get; set; }

    public string? NombreCompleto { get; set; }

    public string? Telefono { get; set; }

    public decimal? Comision { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public string? UsuarioCreacion { get; set; }

    public string? UsuarioModificacion { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<Contrato> Contratos { get; set; } = new List<Contrato>();
}

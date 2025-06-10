using System;
using System.Collections.Generic;

namespace UNICAR_ADMIN.Models.Renta;

public partial class Contrato
{
    public int ContratoId { get; set; }

    public int? VehiculoId { get; set; }

    public int? ClienteId { get; set; }

    public int? VendedorId { get; set; }

    public DateTime? FechaVenta { get; set; }

    public decimal? PrecioVenta { get; set; }

    public string? TipoVenta { get; set; }

    public string? FirmaDocumento { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public string? UsuarioCreacion { get; set; }

    public string? UsuarioModificacion { get; set; }

    public bool? Activo { get; set; }

    public virtual Cliente? Cliente { get; set; }

    public virtual ICollection<Financiamiento> Financiamientos { get; set; } = new List<Financiamiento>();

    public virtual Vehiculo? Vehiculo { get; set; }

    public virtual Vendedore? Vendedor { get; set; }
}

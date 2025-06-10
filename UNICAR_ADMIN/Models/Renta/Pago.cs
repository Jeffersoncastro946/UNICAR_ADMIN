using System;
using System.Collections.Generic;

namespace UNICAR_ADMIN.Models.Renta;

public partial class Pago
{
    public int PagoId { get; set; }

    public int? FinanciamientoId { get; set; }

    public DateTime? FechaPago { get; set; }

    public decimal? Monto { get; set; }

    public string? Estado { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public string? UsuarioCreacion { get; set; }

    public string? UsuarioModificacion { get; set; }

    public virtual Financiamiento? Financiamiento { get; set; }
}

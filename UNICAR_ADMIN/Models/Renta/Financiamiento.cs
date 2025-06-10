using System;
using System.Collections.Generic;

namespace UNICAR_ADMIN.Models.Renta;

public partial class Financiamiento
{
    public int FinanciamientoId { get; set; }

    public int? ContratoId { get; set; }

    public decimal? TasaInteres { get; set; }

    public int? Plazo { get; set; }

    public string? Periodicidad { get; set; }

    public decimal? MontoInicial { get; set; }

    public decimal? MontoFinanciado { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public string? UsuarioCreacion { get; set; }

    public string? UsuarioModificacion { get; set; }

    public virtual Contrato? Contrato { get; set; }

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}

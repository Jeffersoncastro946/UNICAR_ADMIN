using System;
using System.Collections.Generic;

namespace UNICAR_ADMIN.Models.Renta;

public partial class PagosContrato
{
    public int PagoContratoId { get; set; }

    public int ContratoId { get; set; }

    public DateOnly FechaPago { get; set; }

    public decimal MontoPagado { get; set; }

    public string? Observacion { get; set; }

    public bool Activo { get; set; }

    public virtual Contrato Contrato { get; set; } = null!;
}

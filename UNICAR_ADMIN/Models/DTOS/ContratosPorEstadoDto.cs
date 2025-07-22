namespace UNICAR_ADMIN.Models.DTOS
{
    public class ContratosPorEstadoDto
    {
        /* ---------- Filtros ---------- */
        public string? EstadoFiltrado { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        /* ---------- Detalle ---------- */
        public List<Fila> Filas { get; set; } = new();

        /* ---------- Totales globales ---------- */
        public int CantidadContratos => Filas.Count;
        public decimal SumaVentas => Filas.Sum(f => f.PrecioVenta);
        public decimal SumaPagado => Filas.Sum(f => f.MontoPagadoTotal);
        public decimal SumaSaldoPendiente => Filas.Sum(f => f.SaldoPendiente);

        /* ---------- Clase interna: cada fila ---------- */
        public class Fila
        {
            public int ContratoId { get; set; }
            public string Estado { get; set; } = string.Empty;
            public string Cliente { get; set; } = string.Empty;
            public string Vehiculo { get; set; } = string.Empty;
            public DateTime? FechaVenta { get; set; }
            public decimal PrecioVenta { get; set; }           // MontoTotal o PrecioVenta (tú eliges)

            /* --- Info de pagos --- */
            public decimal MontoPagadoTotal { get; set; }         // Sum(Pagos)
            public decimal SaldoPendiente => PrecioVenta - MontoPagadoTotal;
            public List<DetallePago> Pagos { get; set; } = new();

            public class DetallePago
            {
                public int PagoId { get; set; }
                public DateTime FechaPago { get; set; }
                public decimal Monto { get; set; }
                public string Observacion { get; set; } = string.Empty;
            }
        }
    }
}

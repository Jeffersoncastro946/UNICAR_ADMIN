using System;
using System.Collections.Generic;

namespace UNICAR_ADMIN.Models.Renta;

public partial class Vehiculo
{
    public int VehiculoId { get; set; }

    public string Marca { get; set; } = null!;

    public string Modelo { get; set; } = null!;

    public int Anio { get; set; }

    public string? Color { get; set; }

    public string Vin { get; set; } = null!;

    public decimal Precio { get; set; }

    public int? ProveedorId { get; set; }

    public int Estado { get; set; }

    public DateTime? FechaIngreso { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public string? UsuarioCreacion { get; set; }

    public string? UsuarioModificacion { get; set; }

    public bool? EsConsignacion { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<Contrato> Contratos { get; set; } = new List<Contrato>();

    public virtual Estado EstadoNavigation { get; set; } = null!;

    public virtual ICollection<HistorialDueno> HistorialDuenos { get; set; } = new List<HistorialDueno>();

    public virtual ICollection<ImagenesVehiculo> ImagenesVehiculos { get; set; } = new List<ImagenesVehiculo>();

    public virtual Proveedore? Proveedor { get; set; }

    public virtual ICollection<Reparacione> Reparaciones { get; set; } = new List<Reparacione>();
}

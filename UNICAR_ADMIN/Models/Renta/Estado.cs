using System;
using System.Collections.Generic;

namespace UNICAR_ADMIN.Models.Renta;

public partial class Estado
{
    public int EstadoId { get; set; }

    public string Nombre { get; set; } = null!;

    public bool? Estado1 { get; set; }

    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}

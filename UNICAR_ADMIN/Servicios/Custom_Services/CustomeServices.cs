using Microsoft.EntityFrameworkCore;
using UNICAR_ADMIN.Models.DTOS;
using UNICAR_ADMIN.Models.Renta;

namespace UNICAR_ADMIN.Servicios.Custom_Services

{

    public interface ICustomeServices
    {
        Task<ClienteDto> Crear(ClienteDto cliente, string user);
        Task<bool> Actualizar(ClienteDto cliente, string user);
        Task<bool> Eliminar(int clienteId, string user);
        Task<ClienteDto?> ObtenerPorId(int clienteId);
        Task<IEnumerable<ClienteDto>> ObtenerTodos();
        Task<IEnumerable<ContratoViewDto>> ObtenerContratosPorCliente(int clienteId);
    }
    public class CustomeServices : ICustomeServices
    {
        //inyectamos servicios de bd
        private readonly RentaDbContext context;

        public CustomeServices(RentaDbContext contexto)
        {
            this.context = contexto ?? throw new ArgumentNullException(nameof(contexto));
        }
        public async Task<bool> Actualizar(ClienteDto cliente, string user)
        {
            var Cliente_DB = await context.Clientes
                                           .FirstOrDefaultAsync(x => x.ClienteId == cliente.ClienteId)
                                           ?? throw new InvalidOperationException($"No se encontró un cliente para el ID {cliente.ClienteId}");
            // Verifica duplicados con otro cliente
            var existeOtro = await context.Clientes
                                         .AnyAsync(x => x.ClienteId != cliente.ClienteId &&
                                         (x.Identidad == cliente.Identidad || x.Rtn == cliente.Rtn));
            if (existeOtro)
                throw new InvalidOperationException("Ya existe otro cliente con esa Identificación o RTN");
            // Mapear los datos actualizados
            Cliente_DB.Rtn = cliente.Rtn;
            Cliente_DB.Identidad = cliente.Identidad;
            Cliente_DB.Direccion = cliente.Direccion;
            Cliente_DB.Telefono = cliente.Telefono;
            Cliente_DB.UsuarioModificacion = user;
            Cliente_DB.FechaModificacion = DateTime.UtcNow;

            // Guardar cambios
            var guardado = await context.SaveChangesAsync() > 0;
            return guardado;
        }

        public async Task<ClienteDto> Crear(ClienteDto cliente, string user)
        {
            //valdiar que no exista alguien con el mismo identidad y mismo rtn
            var clienteBd = await context.Clientes.Where(x => x.Identidad == cliente.Identidad || x.Rtn==cliente.Rtn).FirstOrDefaultAsync();

            //si existe alguno que devuelva por id o rtn
            if (clienteBd != null) {
                throw new InvalidOperationException("Ya existe un clinete con esa Identificacion o RTN");
            }

            //creamos el cliente en la bd

            var clienteGuardar = new Cliente { 
              NombreCompleto=cliente.NombreCompleto,
              Identidad=cliente.Identidad,
              Rtn=cliente.Rtn,
              Telefono=cliente.Telefono,
              Direccion=cliente.Direccion,  
              FechaCreacion=DateTime.UtcNow,
              UsuarioCreacion=user,
              Activo=true
            };

            //GUARDAMOS TODOS LOS DATOS A LA BD
            context.Clientes.Add(clienteGuardar);
           await context.SaveChangesAsync();
            cliente.ClienteId=clienteGuardar.ClienteId;
          return cliente;

        }

        public async Task<bool> Eliminar(int clienteId, string user )
        {
            var cliente = await context.Clientes.FirstOrDefaultAsync(x => x.ClienteId == clienteId && x.Activo == true) ?? throw new KeyNotFoundException($"Cliente con ID {clienteId} no encontrado.");
            cliente.Activo = false; // Desactivar el cliente en lugar de eliminarlo
                cliente.FechaModificacion = DateTime.UtcNow;
                cliente.UsuarioModificacion = user;

            return await context.SaveChangesAsync() > 0; // Guardar los cambios en la base de datos


        }

        public async Task<ClienteDto?> ObtenerPorId(int clienteId)
        {
            var cliente= await context.Clientes.Where(x=>x.ClienteId == clienteId && x.Activo == true).Select(x => new ClienteDto
            {
                ClienteId = x.ClienteId,
                NombreCompleto = x.NombreCompleto,
                Identidad = x.Identidad,
                Rtn = x.Rtn,
                Telefono = x.Telefono,
                Direccion = x.Direccion,
                Activo = x.Activo
            }).FirstOrDefaultAsync();
            return cliente;
        }

        //public async Task<IEnumerable<ContratoViewDto>> ObtenerPorIdCliente(int clienteId)
        //{
        //    var cliente =await context.Clientes.FirstOrDefaultAsync(x => x.ClienteId == clienteId && x.Activo == true) ?? throw new KeyNotFoundException($"Cliente con ID {clienteId} no encontrado.");
        //    var contrato =await context.Contratos.Where(x => x.ClienteId == clienteId && x.Activo == true).Select(x => new ContratoViewDto
        //    {
        //        ContratoId = x.ContratoId,
        //        Vehiculo = x.Vehiculo != null
        //        ? $"{x.Vehiculo.Marca} {x.Vehiculo.Modelo} {x.Vehiculo.Anio}"
        //        : "Información no encontrada",
        //        Cliente = cliente.NombreCompleto ?? "sin cliente",
        //        //Vendedor = x.Vendedor?.NombreCompleto ?? "sin vendedor"
        //        Vendedor= x.Vendedor != null ? $"{x.Vendedor.NombreCompleto}": "Información no encontrada",
        //        FechaVenta = x.FechaVenta??DateTime.Now,
        //        FechaHaceCuato  = x.FechaVenta.HasValue
        //            ? $"{(DateTime.UtcNow - x.FechaVenta.Value).Days} días"
        //            : "Fecha no disponible",
        //        PrecioVenta = x.PrecioVenta?? 0.0m,
        //        TipoVenta = x.TipoVenta ?? "sin venta",
        //        //Activo = x.Activo
        //    }).ToListAsync();

        //    if (contrato == null)
        //    {
        //        throw new KeyNotFoundException($"No se encontró un contrato para el cliente con ID {clienteId}.");
        //    }
        //    return contrato;
        //}

       
                

        public async Task<IEnumerable<ContratoViewDto>> ObtenerContratosPorCliente(int clienteId)
        {
            // Traigo todo en una misma query, incluyendo cliente y vendedor
            var contratos = await context.Contratos
                .Where(c => c.ClienteId == clienteId && c.Activo==true)
                .Include(c => c.Vehiculo)
                .Include(c => c.Cliente)
                .Include(c => c.Vendedor)
                .Select(x => new ContratoViewDto
                {
                    ContratoId = x.ContratoId,
                    Vehiculo = x.Vehiculo != null
                                         ? $"{x.Vehiculo.Marca} {x.Vehiculo.Modelo} {x.Vehiculo.Anio}"
                                         : "—",
                
                    Vendedor = x.Vendedor != null ? $"{x.Vendedor.NombreCompleto}" : "—",
                    FechaVenta = x.FechaVenta ?? DateTime.Now,  // si no es nullable
                    PrecioVenta = x.PrecioVenta?? 0.0m,
                    TipoVenta = x.TipoVenta?? "sin venta",
                    DiasDesdeVenta =  x.FechaVenta.HasValue
                       ? (int) (DateTime.UtcNow - x.FechaVenta.Value).TotalDays
                        : 0 // Valor predeterminado si FechaVenta es null
                })
                .ToListAsync();

            if (!contratos.Any())
                throw new KeyNotFoundException($"No se encontraron contratos para el cliente {clienteId}.");

            return contratos;
        }


        public async Task<IEnumerable<ClienteDto>> ObtenerTodos()
        {
            IEnumerable<ClienteDto> ListClientes=await context.Clientes.Where(x=>x.Activo==true).Select(x=>new ClienteDto
            {
                ClienteId = x.ClienteId,
                NombreCompleto = x.NombreCompleto,
                Identidad = x.Identidad,
                Rtn = x.Rtn,
                Telefono = x.Telefono,
                Direccion = x.Direccion,
                Activo = x.Activo
            }).ToListAsync();

            return ListClientes;
        }
    }
}

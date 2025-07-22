using Humanizer;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Drawing;
using System.Net.WebSockets;
using UNICAR_ADMIN.Models.DTOS;
//using UNICAR_ADMIN.Models.renta;
using UNICAR_ADMIN.Models.Renta;
using UNICAR_ADMIN.Servicios.LocalImage_Services;

namespace UNICAR_ADMIN.Servicios.Vehiculos_Services
{

    //Interfaz
    public interface IRepositorio_Vehiculo
    {
        #region Vehículo (CRUD)

        // Create
        Task<CrearVehiculoDTO> CrearVehiculo(CrearVehiculoDTO vehiculoDTO, string user);

        // Read
        Task<VehiculoDetalle?> ObtenerDetalleVehiculo(int id);
        Task<ListaVehiculoDTO> ObtenerVehiculosPorContrato(int id);

        // Update
        Task<bool> Editar(CrearVehiculoDTO vehiculoDTO, string user);

        // Delete
        Task<bool> BorrarVehiculo(BorrarVehiculoDTO vehiculoBorrar, string user);

        #endregion


        #region Vehículos (Listados)

        // Obtener todos los vehículos para DataTables
        Task<IEnumerable<Models.DTOS.ListaVehiculoDTO>> ObtenerTodoVehiculo();

        // Obtener listado de VINs para dropdown
        Task<IEnumerable<SelectListItem>> ObtenerVehiculoVIn();

        // Obtener estados para dropdown en creación/edición
        Task<IEnumerable<SelectListItem>> ObtenerEstados();

        #endregion

        #region Reparaciones (CRUD)

        #endregion
        // Read


        #region Reparaciones
        Task<ReparacionesDTO> CrearReparacion(ReparacionesDTO reparacionDTO, string user);
        Task<ReparacionesDTO?> ObtenerDetalleReparacion(int id);
        // Listado de reparaciones agrupado
        Task<IEnumerable<ListarReparacionesDTO>> ListarReparaciones();
        Task<bool> EditarReparacion(ReparacionesDTO reparacionDTO, string user);
        Task<bool> BorrarReparacion(ReparacionesDTO reparacion, string user);
        #endregion
    }


    //implementacion de servicio
    public class Repositorio_Vehiculo : IRepositorio_Vehiculo
    {
        private readonly RentaDbContext contexto;
        private readonly ILocalImageService ServicesImages;


        public Repositorio_Vehiculo(RentaDbContext contexto, ILocalImageService servicesImages)
        {
            this.contexto = contexto;
            ServicesImages = servicesImages;

        }

        #region servicios de vehiculo
        public async Task<CrearVehiculoDTO> CrearVehiculo(CrearVehiculoDTO vehiculoDTO, String user)
        {
            await using var tx = await contexto.Database.BeginTransactionAsync();

            try
            {
                var RUTA = $"{vehiculoDTO.Marca}_{vehiculoDTO.Modelo}_{vehiculoDTO.Anio}_{vehiculoDTO.VIN}";
                var contenedorModeloAñoVin = Path.Combine("Catalogo", RUTA);
                // 1. Guardar el vehículo (sin imágenes)
                var vehiculo = new Vehiculo
                {
                    Marca = vehiculoDTO.Marca,
                    Modelo = vehiculoDTO.Modelo,
                    Anio = vehiculoDTO.Anio,
                    Color = vehiculoDTO.Color,
                    Vin = vehiculoDTO.VIN,
                    Precio = vehiculoDTO.Precio,
                    Estado = vehiculoDTO.estadoId,
                    ProveedorId = vehiculoDTO.ProveedorId,
                    EsConsignacion = vehiculoDTO.EsConsignacion,
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow,
                    FechaIngreso=DateTime.Now,
                    UsuarioCreacion = user

                };


                contexto.Vehiculos.Add(vehiculo);
                await contexto.SaveChangesAsync(); // Ahora tienes el VehiculoId

                // 2. Guardar imágenes si existen
                var imagenes = new ImagenesVehiculo
                {
                    FechaCreacion = DateTime.Now,
                    UsuarioCreacion = user,
                    VehiculoId = vehiculo.VehiculoId
                };

                if (vehiculoDTO.FotoFrontalFile != null)
                    imagenes.FotoFrontal = await ServicesImages.SaveImageAsync(vehiculoDTO.FotoFrontalFile, contenedorModeloAñoVin);

                if (vehiculoDTO.FotoTraseraFile != null)
                    imagenes.FotoTrasera = await ServicesImages.SaveImageAsync(vehiculoDTO.FotoTraseraFile, contenedorModeloAñoVin);

                if (vehiculoDTO.FotoLateralIzquierdaFile != null)
                    imagenes.FotoLateralIzquierda = await ServicesImages.SaveImageAsync(vehiculoDTO.FotoLateralIzquierdaFile, contenedorModeloAñoVin);

                if (vehiculoDTO.FotoLateralDerechaFile != null)
                    imagenes.FotoLateralDerecha = await ServicesImages.SaveImageAsync(vehiculoDTO.FotoLateralDerechaFile, contenedorModeloAñoVin);

                if (vehiculoDTO.FotoInterior1File != null)
                    imagenes.FotoInterior1 = await ServicesImages.SaveImageAsync(vehiculoDTO.FotoInterior1File, contenedorModeloAñoVin);

                if (vehiculoDTO.FotoInterior2File != null)
                    imagenes.FotoInterior2 = await ServicesImages.SaveImageAsync(vehiculoDTO.FotoInterior2File, contenedorModeloAñoVin);

                if (vehiculoDTO.FotoMotorFile != null)
                    imagenes.FotoMotor = await ServicesImages.SaveImageAsync(vehiculoDTO.FotoMotorFile, contenedorModeloAñoVin);

                if (vehiculoDTO.FotoExtra1File != null)
                    imagenes.FotoExtra1 = await ServicesImages.SaveImageAsync(vehiculoDTO.FotoExtra1File, contenedorModeloAñoVin);

                if (vehiculoDTO.FotoExtra2File != null)
                    imagenes.FotoExtra2 = await ServicesImages.SaveImageAsync(vehiculoDTO.FotoExtra2File, contenedorModeloAñoVin);

                if (vehiculoDTO.MiniaturaFile != null)
                    imagenes.Miniatura = await ServicesImages.SaveImageAsync(vehiculoDTO.MiniaturaFile, contenedorModeloAñoVin);

                // Miniatura por defecto si no se subió
                imagenes.Miniatura ??= imagenes.FotoFrontal ?? imagenes.FotoLateralIzquierda ?? imagenes.FotoLateralDerecha;

                contexto.ImagenesVehiculos.Add(imagenes);
                await contexto.SaveChangesAsync();

                await tx.CommitAsync();

                vehiculoDTO.VehiculoId = vehiculo.VehiculoId;
                return vehiculoDTO;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        //detalles
        public async Task<VehiculoDetalle?> ObtenerDetalleVehiculo(int id)
        {
            var Vehiculo = await contexto.Vehiculos.AsNoTracking()
                                             .Include(x => x.EstadoNavigation)
                                             .Include(x => x.ImagenesVehiculos)
                                             .Where(x => x.Activo == true && x.VehiculoId == id)
                                             .Select(v => new VehiculoDetalle
                                             {
                                                 VehiculoId = v.VehiculoId,
                                                 Marca = v.Marca,
                                                 Modelo = v.Modelo,
                                                 Anio = v.Anio,
                                                 Color = v.Color,
                                                 Vin = v.Vin,
                                                 Precio = v.Precio,
                                                 EsConsignacion = v.EsConsignacion ?? false,
                                                 Estado = v.EstadoNavigation.Nombre,
                                                 FotoFrontal = v.ImagenesVehiculos.FirstOrDefault()!.FotoFrontal,
                                                 FotoTrasera = v.ImagenesVehiculos.FirstOrDefault()!.FotoTrasera,
                                                 FotoLateralIzquierda = v.ImagenesVehiculos.FirstOrDefault()!.FotoLateralIzquierda,
                                                 FotoLateralDerecha = v.ImagenesVehiculos.FirstOrDefault()!.FotoLateralDerecha,
                                                 FotoInterior1 = v.ImagenesVehiculos.FirstOrDefault()!.FotoInterior1,
                                                 FotoInterior2 = v.ImagenesVehiculos.FirstOrDefault()!.FotoInterior2,
                                                 FotoMotor = v.ImagenesVehiculos.FirstOrDefault()!.FotoMotor,
                                                 FotoExtra1 = v.ImagenesVehiculos.FirstOrDefault()!.FotoExtra1,
                                                 FotoExtra2 = v.ImagenesVehiculos.FirstOrDefault()!.FotoExtra2,
                                                 Miniatura = v.ImagenesVehiculos.FirstOrDefault()!.Miniatura,
                                                 ProveedorId = v.ProveedorId ?? 0,
                                                 Proveedor = v.Proveedor != null ? v.Proveedor.Nombre : "Sin proveedor",
                                                 EstadoID = v.EstadoNavigation.EstadoId,





                                             }).FirstOrDefaultAsync();
            return Vehiculo;

        }

        public async Task<ImaganesDTO?> GetAllImagenUrlsAsync(int id)
        {
            // Proyectamos únicamente la columna ImagenUrl de todos los productos
            return await contexto.ImagenesVehiculos
                                 .AsNoTracking()
                                 .Where(p => p.VehiculoId == id) // Filtramos por el ID del vehículo
                                 .Select(p =>
                                 new ImaganesDTO
                                 {
                                     FotoFrontal = p.FotoFrontal,
                                     FotoTrasera = p.FotoTrasera,
                                     FotoLateralIzquierda = p.FotoLateralIzquierda,
                                     FotoLateralDerecha = p.FotoLateralDerecha,
                                     FotoInterior1 = p.FotoInterior1,
                                     FotoInterior2 = p.FotoInterior2,
                                     FotoMotor = p.FotoMotor,
                                     FotoExtra1 = p.FotoExtra1,
                                     FotoExtra2 = p.FotoExtra2,
                                     Miniatura = p.Miniatura
                                 })
                                 .FirstOrDefaultAsync();
        }

        //editar
        public async Task<bool> Editar(CrearVehiculoDTO vehiculoDTO, string user)
        {
            var RUTA = $"{vehiculoDTO.Marca}_{vehiculoDTO.Modelo}_{vehiculoDTO.Anio}_{vehiculoDTO.VIN}";
            var contenedorModeloAñoVin = Path.Combine("Catalogo", RUTA);

            await using var tx = await contexto.Database.BeginTransactionAsync();
            try
            {
                var vehiculo = await contexto.Vehiculos.FirstOrDefaultAsync(x => x.VehiculoId == vehiculoDTO.VehiculoId && x.Activo == true);
                if (vehiculo == null)
                    throw new InvalidOperationException("Vehículo no encontrado.");

                vehiculo.Marca = vehiculoDTO.Marca;
                vehiculo.Modelo = vehiculoDTO.Modelo;
                vehiculo.Anio = vehiculoDTO.Anio;
                vehiculo.Color = vehiculoDTO.Color;
                vehiculo.Vin = vehiculoDTO.VIN;
                vehiculo.Precio = vehiculoDTO.Precio;
                vehiculo.Estado = vehiculoDTO.estadoId;
                vehiculo.ProveedorId = vehiculoDTO.ProveedorId;
                vehiculo.EsConsignacion = vehiculoDTO.EsConsignacion;
                vehiculo.FechaModificacion = DateTime.UtcNow;
                vehiculo.UsuarioModificacion = user;

                contexto.Vehiculos.Update(vehiculo);

                var imagenes = await contexto.ImagenesVehiculos.FirstOrDefaultAsync(i => i.VehiculoId == vehiculo.VehiculoId);
                if (imagenes == null)
                    throw new InvalidOperationException("Imágenes del vehículo no encontradas.");

                // Update images only if they exist in DTO
                if (vehiculoDTO.FotoFrontalFile != null)
                {
                    imagenes.FotoFrontal = await ServicesImages.EditImageAsync(
                        vehiculoDTO.FotoFrontalFile,
                        contenedorModeloAñoVin,
                        imagenes.FotoFrontal ?? string.Empty // Ensure non-null value for existingImagePath
                    );
                }

                if (vehiculoDTO.FotoTraseraFile != null)
                {
                    imagenes.FotoTrasera = await ServicesImages.EditImageAsync(
                        vehiculoDTO.FotoTraseraFile,
                        contenedorModeloAñoVin,
                        imagenes.FotoTrasera ?? string.Empty
                    );
                }

                if (vehiculoDTO.FotoLateralIzquierdaFile != null)
                {
                    imagenes.FotoLateralIzquierda = await ServicesImages.EditImageAsync(
                        vehiculoDTO.FotoLateralIzquierdaFile,
                        contenedorModeloAñoVin,
                        imagenes.FotoLateralIzquierda ?? string.Empty
                    );
                }

                if (vehiculoDTO.FotoLateralDerechaFile != null)
                {
                    imagenes.FotoLateralDerecha = await ServicesImages.EditImageAsync(
                        vehiculoDTO.FotoLateralDerechaFile,
                        contenedorModeloAñoVin,
                        imagenes.FotoLateralDerecha ?? string.Empty
                    );
                }

                if (vehiculoDTO.FotoInterior1File != null)
                {
                    imagenes.FotoInterior1 = await ServicesImages.EditImageAsync(
                        vehiculoDTO.FotoInterior1File,
                        contenedorModeloAñoVin,
                        imagenes.FotoInterior1 ?? string.Empty
                    );
                }

                if (vehiculoDTO.FotoInterior2File != null)
                {
                    imagenes.FotoInterior2 = await ServicesImages.EditImageAsync(
                        vehiculoDTO.FotoInterior2File,
                        contenedorModeloAñoVin,
                        imagenes.FotoInterior2 ?? string.Empty
                    );
                }

                if (vehiculoDTO.FotoMotorFile != null)
                {
                    imagenes.FotoMotor = await ServicesImages.EditImageAsync(
                        vehiculoDTO.FotoMotorFile,
                        contenedorModeloAñoVin,
                        imagenes.FotoMotor ?? string.Empty
                    );
                }

                if (vehiculoDTO.FotoExtra1File != null)
                {
                    imagenes.FotoExtra1 = await ServicesImages.EditImageAsync(
                        vehiculoDTO.FotoExtra1File,
                        contenedorModeloAñoVin,
                        imagenes.FotoExtra1 ?? string.Empty
                    );
                }

                if (vehiculoDTO.FotoExtra2File != null)
                {
                    imagenes.FotoExtra2 = await ServicesImages.EditImageAsync(
                        vehiculoDTO.FotoExtra2File,
                        contenedorModeloAñoVin,
                        imagenes.FotoExtra2 ?? string.Empty
                    );
                }

                if (vehiculoDTO.MiniaturaFile != null)
                {
                    imagenes.Miniatura = await ServicesImages.EditImageAsync(
                        vehiculoDTO.MiniaturaFile,
                        contenedorModeloAñoVin,
                        imagenes.Miniatura ?? string.Empty
                    );
                }

                // Default miniatura if not provided
                imagenes.Miniatura ??= imagenes.FotoFrontal ?? imagenes.FotoLateralIzquierda ?? imagenes.FotoLateralDerecha;

                contexto.ImagenesVehiculos.Update(imagenes);
                await contexto.SaveChangesAsync();
                await tx.CommitAsync();

                return true;
            }
            catch (Exception)
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        //borrar
        public async Task<bool> BorrarVehiculo(BorrarVehiculoDTO vehiculoBorrar, string user)
        {
            // 1) Recupera la entidad por su PK
            var vehiculo = await contexto.Vehiculos
                                         .FindAsync(vehiculoBorrar.VehiculoId);
            if (vehiculo == null)
                throw new InvalidOperationException("Vehículo no encontrado.");

            // 2) Marca la entidad como inactiva
            vehiculo.Activo = false;
            vehiculo.FechaModificacion = DateTime.UtcNow;
            vehiculo.UsuarioModificacion = user;



            // 4) Persistir en BD
            await contexto.SaveChangesAsync();
            return true;


        }


        //listados vehiculo y derivados
        public async Task<IEnumerable<Models.DTOS.ListaVehiculoDTO>> ObtenerTodoVehiculo()
        {
            //obtenenemos el listado de vehiculos y su estado que esten activos
            IEnumerable<ListaVehiculoDTO> lista = new List<ListaVehiculoDTO>();
            lista = await contexto.Vehiculos.AsNoTracking()
                                            .Include(x => x.ImagenesVehiculos)
                                            .Include(x => x.EstadoNavigation)
                                            .Where(x => x.Activo == true)
                                            .Select(x => new ListaVehiculoDTO
                                            {
                                                VehiculoId = x.VehiculoId,
                                                Marca = x.Marca,
                                                Modelo = x.Modelo,
                                                Anio = x.Anio,
                                                Color = x.Color,
                                                Vin = x.Vin,
                                                Precio = x.Precio,
                                                Estado = x.EstadoNavigation.Nombre,
                                                EsConsignacion = x.EsConsignacion ?? false,
                                                Miniatura = x.ImagenesVehiculos.Where(img => !string.IsNullOrEmpty(img.Miniatura)).Select(img => img.Miniatura).FirstOrDefault(),
                                            }).ToListAsync();
            return lista;
        }

        public async Task<IEnumerable<SelectListItem>> ObtenerEstados()
        {

            return await contexto.Estados.AsNoTracking()
                 .Where(e => e.Estado1 == true)
                 .Select(e => new SelectListItem
                 {
                     Value = e.EstadoId.ToString(),
                     Text = e.Nombre
                 }).ToListAsync();

        }

        public async Task<IEnumerable<SelectListItem>> ObtenerVehiculoVIn()
        {
            return await contexto.Vehiculos.AsNoTracking()
                 .Where(p => p.Activo == true)
                 .Select(p => new SelectListItem
                 {
                     Value = p.VehiculoId.ToString(),
                     Text = p.Vin
                 }).ToListAsync();
        }

        public async Task<ListaVehiculoDTO> ObtenerVehiculosPorContrato(int id)
        {
            var contrato = await contexto.Contratos
                .Where(x => x.Activo == true && x.ContratoId == id)
                .Select(x => new ContratoDto
                {
                    PrecioVenta=x.PrecioVenta ?? 0.0m,
                    Vendedor =x.Vendedor != null ? x.Vendedor.NombreCompleto : "Sin vendedor",   
                    Cliente = x.Cliente!=null ? x.Cliente.NombreCompleto : "Sin cliente",
                    VehiculoDetalle = x.Vehiculo != null
                        ? new ListaVehiculoDTO //no es una lista ya que al final no se pudo hacer relacion de 1:N
                        {
                            VehiculoId = x.Vehiculo.VehiculoId,

                            Marca = x.Vehiculo.Marca ?? "Sin marca",
                            Modelo = x.Vehiculo.Modelo ?? "Sin modelo",
                            Anio = x.Vehiculo.Anio ,
                            Color = x.Vehiculo.Color ?? "Sin color",
                            Vin = x.Vehiculo.Vin ?? "Sin VIN",
                            Precio=x.Vehiculo.Precio,
                            Estado = x.Vehiculo.EstadoNavigation != null
                                     ? x.Vehiculo.EstadoNavigation.Nombre
                                     : "Desconocido"
                        }
                        : null
                })
                .FirstOrDefaultAsync();

            // Obtener todas las reparaciones
            var reparaciones = await ListarReparaciones();
            // Filtrar solo las del vehículo actual
            var gastos = reparaciones
                .Where(r => r.VehiculoId == contrato?.VehiculoDetalle?.VehiculoId)
                .Sum(r => r.costo);

            //asigno el gasto al vehiculo detalle
            // Replace the problematic line with the following code to avoid using the "null conditional assignment" feature:
            if (contrato?.VehiculoDetalle != null)
            {
                contrato.VehiculoDetalle.TotalGastosMantenimientos = gastos;
            }
            

            return contrato?.VehiculoDetalle?? new ListaVehiculoDTO();
        }

        #endregion


        public async Task<ReparacionesDTO> CrearReparacion(ReparacionesDTO reparacionDTO, string user)
        {
            try
            {
                var ContenedorRuta = "Mantenimientos";

                // Validate if ImagenFile is null before calling SaveImageAsync
                var imagenUrl = reparacionDTO.ImagenFile != null
                    ? await ServicesImages.SaveImageAsync(reparacionDTO.ImagenFile, ContenedorRuta)
                    : null;

                var reparacion = new Reparacione
                {
                    VehiculoId = reparacionDTO.VehiculoId,
                    Descripcion = reparacionDTO.Descripcion,
                    Costo = (decimal?)reparacionDTO.Costo,
                    FechaInicial = reparacionDTO.FechaInicio ?? DateTime.UtcNow,
                    FechaFinal = reparacionDTO.FechaFin ?? DateTime.UtcNow,
                    Responsable = reparacionDTO.Responsable ?? "sin responsable asignado",
                    ImagenUrl = imagenUrl, // Assign the validated image URL
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow,
                    UsuarioCreacion = user
                };

                contexto.Reparaciones.Add(reparacion);
                await contexto.SaveChangesAsync();

                reparacionDTO.ReparacionId = reparacion.ReparacionId;
                return reparacionDTO;
            }
            catch (Exception)
            {
                throw;
            }
        }


        //editar reparacion
        public async Task<bool> EditarReparacion(ReparacionesDTO reparacionDTO, string user)
        {
            var reparacion = await contexto.Reparaciones.FirstOrDefaultAsync(r => r.ReparacionId == reparacionDTO.ReparacionId && r.Activo == true);
            if (reparacion == null)
                throw new InvalidOperationException("Reparación no encontrada.");
            reparacion.Descripcion = reparacionDTO.Descripcion;
            reparacion.Costo = (decimal?)reparacionDTO.Costo;
            reparacion.FechaInicial = reparacionDTO.FechaInicio ?? DateTime.UtcNow;
            reparacion.FechaFinal = reparacionDTO.FechaFin ?? DateTime.UtcNow;
            reparacion.Responsable = reparacionDTO.Responsable ?? "sin responsable asignado";
            // Validate if ImagenFile is null before calling EditImageAsync
            if (reparacionDTO.ImagenFile != null)
            {
                var ContenedorRuta = "Mantenimientos";
                reparacion.ImagenUrl = await ServicesImages.EditImageAsync(reparacionDTO.ImagenFile, ContenedorRuta, reparacion.ImagenUrl ?? string.Empty);
            }
            reparacion.FechaModificacion = DateTime.UtcNow;
            reparacion.UsuarioModificacion = user;
            contexto.Reparaciones.Update(reparacion);
            await contexto.SaveChangesAsync();
            return true;
        }
        //Detalles
        public async Task<ReparacionesDTO?> ObtenerDetalleReparacion(int id)
        {
            ReparacionesDTO? reparacion = new ReparacionesDTO();
            reparacion = await contexto.Reparaciones.AsNoTracking()
                .Where(r => r.ReparacionId == id && r.Activo == true && (r.Vehiculo.Activo == true))
                .Select(r => new ReparacionesDTO
                {
                    ReparacionId = r.ReparacionId,
                    VehiculoId = r.VehiculoId,
                    Descripcion = r.Descripcion ?? string.Empty, // Fix for CS8601: Ensure non-null assignment
                    Costo = r.Costo.HasValue ? (double)r.Costo.Value : 0.0, // Fix for CS8629: Handle nullable decimal
                    FechaInicio = r.FechaInicial,
                    FechaFin = r.FechaFinal,
                    ImagenUrl = r.ImagenUrl ?? string.Empty, // Fix for CS8601: Ensure non-null assignment
                    Vin = r.Vehiculo.Vin ?? string.Empty, // Fix for CS8601: Ensure non-null assignment
                    Marca = r.Vehiculo.Marca ?? string.Empty, // Fix for CS8601: Ensure non-null assignment
                    Modelo = r.Vehiculo.Modelo ?? string.Empty, // Fix for CS8601: Ensure non-null assignment
                    Anio = r.Vehiculo.Anio,
                    Color = r.Vehiculo.Color ?? string.Empty, // Fix for CS8601: Ensure non-null assignment
                    Responsable = r.Responsable ?? "sin responsable"
                }).FirstOrDefaultAsync();

            return reparacion;
        }


        //listado de repaciones y sus derivados
        public async Task<IEnumerable<ListarReparacionesDTO>> ListarReparaciones()
        {
            // Fix for CS0103 and CS1525: Correctly define the lambda expression in the Where clause
            return await contexto.Reparaciones
                                 .Where(r => r.Activo == true && r.Vehiculo.Activo == true) // Ensure the condition is properly defined
                                 .Select(r => new ListarReparacionesDTO
                                 {
                                     VehiculoId = r.VehiculoId,
                                     Marca = r.Vehiculo.Marca,
                                     Modelo = r.Vehiculo.Modelo,
                                     Anio = r.Vehiculo.Anio,
                                     Color = r.Vehiculo.Color,
                                     Vin = r.Vehiculo.Vin,
                                     ReparacionId = r.ReparacionId,
                                     Descripcion = r.Descripcion,
                                     costo = r.Costo.HasValue ? (double)r.Costo.Value : 0.0, // Fix for CS8629
                                     FechaInicio = r.FechaInicial,
                                     FechaFin = r.FechaFinal,
                                     ImagenUrl = r.ImagenUrl
                                 }).ToListAsync();
        }



        //borrar
        //public async Task<bool> BorrarVehiculo(BorrarVehiculoDTO vehiculoBorrar, string user)
        //{
        //    // 1) Recupera la entidad por su PK
        //    var vehiculo = await contexto.Vehiculos
        //                                 .FindAsync(vehiculoBorrar.VehiculoId);
        //    if (vehiculo == null)
        //        throw new InvalidOperationException("Vehículo no encontrado.");

        //    // 2) Marca la entidad como inactiva
        //    vehiculo.Activo = false;
        //    vehiculo.FechaModificacion = DateTime.UtcNow;
        //    vehiculo.UsuarioModificacion = user;



        //    // 4) Persistir en BD
        //    await contexto.SaveChangesAsync();
        //    return true;


        //}
        public async Task<bool> BorrarReparacion(ReparacionesDTO reparacion, string user)
        {

            var dto = await contexto.Reparaciones.FindAsync(reparacion.ReparacionId);

            if (dto == null)
                throw new InvalidOperationException("reparacion no econtrada");

            dto.Activo = false;
            dto.UsuarioModificacion = user;
            dto.FechaModificacion = DateTime.UtcNow;
            await contexto.SaveChangesAsync();

            return true;

        }
    }
}
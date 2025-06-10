using Humanizer;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using UNICAR_ADMIN.Models.DTOS;
//using UNICAR_ADMIN.Models.renta;
using UNICAR_ADMIN.Models.Renta;
using UNICAR_ADMIN.Servicios.LocalImage_Services;

namespace UNICAR_ADMIN.Servicios.Vehiculos_Services
{

    //Interfaz
    public interface IRepositorio_Vehiculo
    {
        Task<CrearVehiculoDTO> CrearVehiculo(CrearVehiculoDTO vehiculoDTO, string user);
        Task<VehiculoDetalle?> ObtenerDetalleVehiculo(int id);
        Task<IEnumerable<SelectListItem>> ObtenerEstados();
        Task<bool> BorrarVehiculo(BorrarVehiculoDTO vehiculoBorrar, string user);
        Task<bool> Editar(CrearVehiculoDTO vehiculoDTO, string user);

        //LISTAR
        public Task<IEnumerable<Models.DTOS.ListaVehiculoDTO>> ObtenerTodoVehiculo();
        Task<IEnumerable<SelectListItem>> ObteberVehiculoVIn();
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

        public async  Task<bool> BorrarVehiculo(BorrarVehiculoDTO vehiculoBorrar, string user)
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
        public async Task<bool> Editar(CrearVehiculoDTO vehiculoDTO, string user)
        {    // 1) Recupera la entidad por su PK
            var vehiculo = await contexto.Vehiculos
                                         .FindAsync(vehiculoDTO.VehiculoId);
            if (vehiculo == null)
                throw new InvalidOperationException("Vehículo no encontrado.");

            // 2) Mapea únicamente los campos que vienen del DTO
            vehiculo.Marca = vehiculoDTO.Marca;
            vehiculo.Modelo = vehiculoDTO.Modelo;
            vehiculo.Anio = vehiculoDTO.Anio;
            vehiculo.Color = vehiculoDTO.Color;
            vehiculo.Vin = vehiculoDTO.VIN;
            vehiculo.Precio = vehiculoDTO.Precio;
            vehiculo.Estado = vehiculoDTO.estadoId;
            vehiculo.ProveedorId = vehiculoDTO.ProveedorId;
            vehiculo.EsConsignacion = vehiculoDTO.EsConsignacion;
            vehiculo.FechaModificacion = DateTime.UtcNow ;
            vehiculo.UsuarioModificacion = user;

            // 3) Indica a EF Core que ha cambiado solo esta entidad
            contexto.Vehiculos.Update(vehiculo);

            // 4) Persistir en BD
            await contexto.SaveChangesAsync();
            return true;


        }

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
                                                 EsConsignacion=v.EsConsignacion ?? false,
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
                                                 ProveedorId=v.ProveedorId ?? 0,
                                                 Proveedor = v.Proveedor != null ? v.Proveedor.Nombre : "Sin proveedor",
                                                 EstadoID = v.EstadoNavigation.EstadoId,
                                                 




                                             }).FirstOrDefaultAsync();
            return Vehiculo;

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

        public async Task<IEnumerable<SelectListItem>> ObteberVehiculoVIn()
        {
            return await contexto.Vehiculos.AsNoTracking()
                 .Where(p => p.Activo == true)
                 .Select(p => new SelectListItem
                 {
                     Value = p.VehiculoId.ToString(),
                     Text = p.Vin
                 }).ToListAsync();
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

        public async Task<CrearVehiculoDTO> CrearVehiculo(CrearVehiculoDTO vehiculoDTO, String user)
        {
            await using var tx = await contexto.Database.BeginTransactionAsync();

            try
            {
                var contenedorModeloAñoVin = $"{vehiculoDTO.Marca}_{vehiculoDTO.Modelo}_{vehiculoDTO.Anio}_{vehiculoDTO.VIN}";

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
                    UsuarioCreacion =user
                    
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
    }
}
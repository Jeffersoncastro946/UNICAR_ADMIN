using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UNICAR_ADMIN.Models.DTOS;
using UNICAR_ADMIN.Models.Renta;

namespace UNICAR_ADMIN.Servicios.Proveedores_Services
{

    public interface IRepositorio_Proveedores
    {
        Task<List<UNICAR_ADMIN.Models.DTOS.ProveedorDTO>> ObtenerProveedores(int? id);
        Task<IEnumerable<SelectListItem>> ObtenerProveedoresSelectList();
    }


    public class Repositorio_Proveedores : IRepositorio_Proveedores
    {
        private readonly RentaDbContext contexto;

        public Repositorio_Proveedores(RentaDbContext contexto)
        {
            this.contexto = contexto;
        }

        //obtener todos los proveedores
        public async Task<List<UNICAR_ADMIN.Models.DTOS.ProveedorDTO>> ObtenerProveedores(int? id = 0)
        {
            //ontener todos los proveedores de la base de datos y mapearlos a ProveedorDTO
            var proveedores = await contexto.Proveedores
                                .Where(p => p.Activo == true && (p.ProveedorId == id || id == 0)) // Filtrar solo proveedores activos y opcional el id 
                                .Select(p => new ProveedorDTO
                                {
                                    ProveedorId = p.ProveedorId,
                                    Nombre = p.Nombre,
                                    Telefono = p.Telefono,
                                    Correo = p.Correo,
                                    Direccion = p.Direccion,
                                    Activo = p.Activo
                                }).ToListAsync();
            return proveedores;
        }
        public async Task<IEnumerable<SelectListItem>> ObtenerProveedoresSelectList()
        {
            return await contexto.Proveedores.AsNoTracking().Where(p=>p.Activo==true)
                                                        .Select (p=>new SelectListItem
                                                                        {Value=p.ProveedorId.ToString(),
                                                                        Text= p.Nombre
                                                        }).ToListAsync();
        }
    }
}
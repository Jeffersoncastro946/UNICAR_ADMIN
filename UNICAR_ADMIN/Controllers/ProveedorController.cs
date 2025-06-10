using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UNICAR_ADMIN.Models.Renta;
using UNICAR_ADMIN.Servicios.Proveedores_Services;

namespace UNICAR_ADMIN.Controllers
{
    public class ProveedorController : Controller
    {
        private readonly IRepositorio_Proveedores repositorio_Proveedores;
        public ProveedorController(IRepositorio_Proveedores repositorio_Proveedores)
        {
            this.repositorio_Proveedores = repositorio_Proveedores ?? throw new ArgumentNullException(nameof(repositorio_Proveedores));

        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerProveedoresSelectItem()
        {
            var ListaProveedores = await repositorio_Proveedores.ObtenerProveedores(0);
            // Convertir la lista de proveedores a una lista de SelectListItem
            var ListaProveedor = ListaProveedores
                                         .Where(p => p.Activo == true) // Filtrar solo proveedores activos   
                                         .Select(p => new SelectListItem
                                         {
                                             Value = p.ProveedorId.ToString(),
                                             Text = p.Nombre
                                         }).ToList();
            return Json(ListaProveedor);
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerProveedoresPorID(int ID)
        {
            var ListaProveedores = await repositorio_Proveedores.ObtenerProveedores(ID);
            // Convertir la lista de proveedores a una lista de SelectListItem

            return Json(ListaProveedores);
        }

    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using UNICAR_ADMIN.Models.DTOS;
using UNICAR_ADMIN.Models.Renta;
using UNICAR_ADMIN.Servicios.Proveedores_Services;
using UNICAR_ADMIN.Servicios.Vehiculos_Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UNICAR_ADMIN.Controllers
{
    [Authorize(Roles = "Admin")]

    public class VehiculoController : Controller
    {
        private readonly IRepositorio_Vehiculo Repositorio;
        private readonly IRepositorio_Proveedores repositorio_Proveedores;
        // Correcting the constructor to properly initialize the field "Repositorio"
        public VehiculoController(IRepositorio_Vehiculo repositorio, IRepositorio_Proveedores repositorio_Proveedores)
        {

            Repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
            this.repositorio_Proveedores = repositorio_Proveedores ?? throw new ArgumentNullException(nameof(repositorio_Proveedores));
        }

        // GET: Vehiculo
        public ActionResult Index()
        {

            return View();
        }
        public async Task<ActionResult> ObtenerVehiculos()
        {
            var ListadoVehiculos = await Repositorio.ObtenerTodoVehiculo();

            return Json(new { data = ListadoVehiculos });
        }

        // GET: Vehiculo/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Vehiculo/Create
        public async Task<ActionResult> CrearVehiculo()
        {
            var vehiculo = new UNICAR_ADMIN.Models.DTOS.CrearVehiculoDTO();
            //cargar los provedores para los vehiculos
            vehiculo.ListaEstados = await Repositorio.ObtenerEstados();

            return View(vehiculo);
        }

        [HttpPost]
        public async Task<ActionResult> CrearVehiculo(CrearVehiculoDTO vehiculo)
        {
            if (!ModelState.IsValid)
            {
                // Si el modelo no es válido, volver a mostrar la vista con los errores
                vehiculo.ListaEstados = await Repositorio.ObtenerEstados();
                return View(vehiculo);
            }
            try
            {
                var user=User.Identity?.Name ?? "desconocido";
                var vehiculoCrear = await Repositorio.CrearVehiculo(vehiculo,user);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                // mensaje amigable que lanzamos desde el repositorio
                ModelState.AddModelError(string.Empty, ex.Message);

                return View(vehiculo);
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditarVehiculo(int id)
        {
            var v = await Repositorio.ObtenerDetalleVehiculo(id);
            if (v == null) return NotFound();

            var dto = new CrearVehiculoDTO
            {
                VehiculoId = v.VehiculoId,
                Marca = v?.Marca ?? "sin marca",
                Modelo = v?.Modelo ?? "Sin modelo",
                Anio = v?.Anio ?? 0,
                Color = v?.Color ?? "sin color",
                VIN = v?.Vin ?? "sin color",
                Precio = v?.Precio ?? 0,
                EsConsignacion = v.EsConsignacion,
                ProveedorId=v.ProveedorId,

                // … resto de mapeo …
                estadoId=v.EstadoID,
     
                ListaEstados = (await Repositorio.ObtenerEstados()),
                ListaProveedores = (await repositorio_Proveedores.ObtenerProveedoresSelectList())
            };

            return PartialView("~/Views/Vehiculo/vistasParciales/_EditarVehiculo.cshtml", dto);
        }
        [HttpPost]
        public async Task<IActionResult> EditarVehiculo(CrearVehiculoDTO vehiculo)
        {
            if (!ModelState.IsValid)
            {
                // Si el modelo no es válido, volver a mostrar la vista con los errores
                vehiculo.ListaEstados = await Repositorio.ObtenerEstados();
                vehiculo.ListaProveedores = (await repositorio_Proveedores.ObtenerProveedoresSelectList());
                // Agrupa todos los mensajes de ModelState
                var errores = ModelState.Values
                               .SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                               .ToList();
                return Json(new
                {
                    success = false,
                    message = string.Join("<br/>", errores)
                });
            }
            try
            {
                var user = User.Identity?.Name ?? "desconocido";
                var vehiculoEditar = await Repositorio.Editar(vehiculo, user);
                return Json(new
                {
                    success = true,
                    message = "Vehículo editado correctamente."
                });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        [HttpGet]
        public async Task<IActionResult> BorrarVehiculo(int id)
        {

            var v = await Repositorio.ObtenerDetalleVehiculo(id);
            if (v == null) return NotFound();

            var dto = new BorrarVehiculoDTO
            {
                VehiculoId = v.VehiculoId,
                Marca = v?.Marca ?? "sin marca",
                Modelo = v?.Modelo ?? "Sin modelo",
                VIN = v?.Vin ?? "sin color"
            };

            return PartialView("~/Views/Vehiculo/vistasParciales/_BorrarVehiculo.cshtml", dto);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarVehiculo(BorrarVehiculoDTO Vehiculo)
        {
            //verificar que exite el vehiculo
            var v = await Repositorio.ObtenerDetalleVehiculo(Vehiculo.VehiculoId);
            if (v == null) return NotFound();

            try
            {
                var user = User.Identity?.Name ?? "desconocido";
                var vehiculoBorrar = await Repositorio.BorrarVehiculo(Vehiculo, user);
                return Json(new
                {
                    success = true,
                    message = "Vehículo Borrado correctamente."
                });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CrearReparacion()
        {
            var reparacion = new ReparacionesDTO();
            //cargar los vehiculos para las reparaciones
            reparacion.listavehiculosVim = await Repositorio.ObteberVehiculoVIn();
            return View(reparacion);
        }
    }
}


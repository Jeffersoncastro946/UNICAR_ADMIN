using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using UNICAR_ADMIN.Models.DTOS;
using UNICAR_ADMIN.Models.Renta;
using UNICAR_ADMIN.Servicios.Proveedores_Services;
using UNICAR_ADMIN.Servicios.Vehiculos_Services;

namespace UNICAR_ADMIN.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VehiculoController : Controller
    {
        private readonly IRepositorio_Vehiculo Repositorio;
        private readonly IRepositorio_Proveedores repositorio_Proveedores;

        public VehiculoController(
            IRepositorio_Vehiculo repositorio,
            IRepositorio_Proveedores repositorio_Proveedores)
        {
            Repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
            this.repositorio_Proveedores = repositorio_Proveedores
                ?? throw new ArgumentNullException(nameof(repositorio_Proveedores));
        }

        #region Vehículos

        // Pantalla principal
        public ActionResult Index()
        {
            return View();
        }

        // DataTables: obtener todos
        public async Task<ActionResult> ObtenerVehiculos()
        {
            var listado = await Repositorio.ObtenerTodoVehiculo();
            return Json(new { data = listado });
        }

        // Detalles vehículo (vista completa)
        public ActionResult Details(int id)
        {
            return View();
        }

        // Crear vehículo (GET)
        [HttpGet]
        public async Task<ActionResult> CrearVehiculo()
        {
            var dto = new CrearVehiculoDTO
            {
                ListaEstados = await Repositorio.ObtenerEstados(),
                ListaProveedores = await repositorio_Proveedores.ObtenerProveedoresSelectList()
            };
            ViewBag.TitleForm = "Registrar Vehículo";
            ViewBag.action = "CrearVehiculo";
            return View(dto);
        }

        // Crear vehículo (POST)
        [HttpPost]
        public async Task<ActionResult> CrearVehiculo(CrearVehiculoDTO vehiculo)
        {
            if (!ModelState.IsValid)
            {
                vehiculo.ListaEstados = await Repositorio.ObtenerEstados();
                vehiculo.ListaProveedores = await repositorio_Proveedores.ObtenerProveedoresSelectList();
                return View(vehiculo);
            }

            try
            {
                var user = User.Identity?.Name ?? "desconocido";
                await Repositorio.CrearVehiculo(vehiculo, user);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                // 1) Detectar si es único por VIN
                if (ex.InnerException?.Message.Contains("UQ_Vehiculo_VIN") ?? false)
                {
                    ModelState.AddModelError(nameof(vehiculo.VIN),
                        "Ya existe un vehículo con ese VIN. Por favor ingresa uno distinto.");
                }
                else
                {
                    // 2) Otro error genérico
                    ModelState.AddModelError("", "Ocurrió un error al guardar el vehículo.");
                }
                return View(vehiculo);

            }
        }

        // Editar vehículo (GET)
        [HttpGet]
        public async Task<IActionResult> EditarVehiculo(int id)
        {
            var v = await Repositorio.ObtenerDetalleVehiculo(id);
            if (v == null) return NotFound();
            ViewBag.TitleForm= "Editar Vehículo";
            ViewBag.action = "EditarVehiculo";
            // Obtén la lista sin seleccionar
            var items = await repositorio_Proveedores.ObtenerProveedoresSelectList();

         

            var dto = new CrearVehiculoDTO
            {
                VehiculoId = v.VehiculoId,
                Marca = v?.Marca ?? "sin marca",
                Modelo = v?.Modelo ?? "Sin modelo",
                Anio = v?.Anio ?? 0,
                Color = v?.Color ?? "sin color",
                VIN = v?.Vin ?? "sin color",
                Precio = v?.Precio ?? 0,
                EsConsignacion = v?.EsConsignacion ?? false,
                
                estadoId=v.EstadoID,
                FotoFrontal=v.FotoFrontal,
                FotoTrasera=v.FotoTrasera,
                FotoLateralDerecha=v.FotoLateralDerecha,
                FotoLateralIzquierda=v.FotoLateralIzquierda,
                FotoInterior1=v.FotoInterior1,
                FotoInterior2=v.FotoInterior2,
                FotoMotor=v.FotoMotor,
                FotoExtra1=v.FotoExtra1,
                FotoExtra2 = v.FotoExtra2,
                Miniatura=v.Miniatura,
                ListaEstados = (await Repositorio.ObtenerEstados()),
                ProveedorId =v.ProveedorId,
                ListaProveedores  = items
            };
            return View("CrearVehiculo", dto);
           
        }

        // Editar vehículo (POST)
        [HttpPost]
        public async Task<IActionResult> EditarVehiculo(CrearVehiculoDTO vehiculo)
        {
            if (!ModelState.IsValid)
            {
                vehiculo.ListaEstados = await Repositorio.ObtenerEstados();
                vehiculo.ListaProveedores = await repositorio_Proveedores.ObtenerProveedoresSelectList();
                var errores = ModelState.Values
                               .SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                               .ToList();
                ViewBag.TitleForm = "Editar Vehículo";
                ViewBag.action = "EditarVehiculo";
                return View("CrearVehiculo",vehiculo);
            }

            try
            {

                var user = User.Identity?.Name ?? "desconocido";
                await Repositorio.Editar(vehiculo, user);
                // Guardas el mensaje de éxito
                TempData["SuccessMessage"] = "Vehículo editado correctamente.";
                return RedirectToAction("Index");
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        // Borrar vehículo (GET)
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
            // Borrar vehículo (POST)
        [HttpPost]
        public async Task<IActionResult> BorrarVehiculo(BorrarVehiculoDTO vehiculo)
        {
            //verificar que exite el vehiculo
            var v = await Repositorio.ObtenerDetalleVehiculo(vehiculo.VehiculoId);
            if (v == null) return NotFound();   

            try
            {
                var user = User.Identity?.Name ?? "desconocido";
                await Repositorio.BorrarVehiculo(vehiculo, user);
                return Json(new { success = true, message = "Vehículo borrado correctamente." });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Reparaciones

        // Vista principal de reparaciones
        public IActionResult ListarReparaciones()
        {
            return View();
        }

        // DataTables: obtener todas las reparaciones
        public async Task<IActionResult> ObtenerListadoReparaciones()
        {
            var listado = await Repositorio.ListarReparaciones();
            return Json(new { data = listado });
        }

        // Crear reparación (GET)
        [HttpGet]
        public async Task<IActionResult> CrearReparacion()
        {
            var dto = new ReparacionesDTO
            {
                ListaVehiculosVin = await Repositorio.ObtenerVehiculoVIn()
            };
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> CrearReparacion(ReparacionesDTO reparacionDto)
        {

            // 1) Validación de DataAnnotations
            if (!ModelState.IsValid)
            {
                //reparacion.ListaVehiculosVin = await Repositorio.ObtenerVehiculoVIn();
                return PartialView("~/Views/Vehiculo/vistasParciales/_EditarReparacion.cshtml", reparacionDto);
            }

            // 2) Validación de negocio: fechas
            if (reparacionDto.FechaFin < reparacionDto.FechaInicio)
            {
              
                ModelState.AddModelError(
                  nameof(reparacionDto.FechaFin),
                  "La fecha de fin no puede ser anterior a la fecha de inicio."
                );
                return PartialView("~/Views/Vehiculo/vistasParciales/_EditarReparacion.cshtml", reparacionDto);
            }


            try
            {
                // 3) Todo OK → guardamos y devolvemos JSON
                await Repositorio.EditarReparacion(reparacionDto, User.Identity?.Name ?? "desconocido");
                return Json(new { success = true, message = "Reparación editada correctamente." });
            }
            catch (Exception ex)
            {

                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("ListarReparaciones"); ;
            }
        }

        // Editar reparación (GET)
        [HttpGet]
        public async Task<IActionResult> EditarReparacion(int id)
        {
            var dto = await Repositorio.ObtenerDetalleReparacion(id);
            if (dto == null) return NotFound();
            return PartialView("~/Views/Vehiculo/vistasParciales/_EditarReparacion.cshtml", dto);
        }

        [HttpPost]
        public async Task<IActionResult> EditarReparacion(ReparacionesDTO reparacion)
        {
            //validar 
            // 1) Validación de DataAnnotations
            if (!ModelState.IsValid)
            {
                return PartialView("~/Views/Vehiculo/vistasParciales/_EditarReparacion.cshtml", reparacion);
            }

            // 2) Validación de negocio: fechas
            if (reparacion.FechaFin < reparacion.FechaInicio)
            {
                reparacion.ListaVehiculosVin = await Repositorio.ObtenerVehiculoVIn();
                ModelState.AddModelError(
                  nameof(reparacion.FechaFin),
                  "La fecha de fin no puede ser anterior a la fecha de inicio."
                );
                return PartialView("~/Views/Vehiculo/vistasParciales/_EditarReparacion.cshtml", reparacion);
            }
            try
            {
                //var user = User.Identity?.Name ?? "desconocido";
                //await Repositorio.EditarReparacion(reparacion, user);
                //TempData["SuccessMessage"] = "Reparación editada correctamente.";
                //return RedirectToAction("ListarReparaciones");
                // 3) Todo OK → guardamos y devolvemos JSON
                await Repositorio.EditarReparacion(reparacion, User.Identity?.Name ?? "desconocido");
                return Json(new { success = true, message = "Reparación editada correctamente." });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return Json(new { success = false, message = ex.Message});
            }
        }


        // Borrar reparación (GET)
        [HttpGet]
        public async Task<IActionResult> BorrarReparacion(int id)
        {
            var v = await Repositorio.ObtenerDetalleReparacion(id);
            if (v == null) return NotFound();

            var dto = new ReparacionesDTO
            {
                ReparacionId = v.ReparacionId,
                VehiculoId = v.VehiculoId,
                Descripcion = v.Descripcion,
                Costo = v.Costo,
                FechaInicio = v.FechaInicio,
                FechaFin = v.FechaFin,
                ImagenUrl = v.ImagenUrl,
                Vin = v.Vin,
                Marca = v.Marca,
                Modelo = v.Modelo
            };
            return PartialView("~/Views/Vehiculo/vistasParciales/_BorrarReparacion.cshtml", dto);
        }

        // Detalle reparación (Partial)
        [HttpGet]
        public async Task<IActionResult> ObtenerDetalleReparacion(int id)
        {
            var dto = await Repositorio.ObtenerDetalleReparacion(id);
            if (dto == null) return NotFound();
            dto.ListaVehiculosVin = await Repositorio.ObtenerVehiculoVIn();
            return PartialView("~/Views/Vehiculo/vistasParciales/_ReparacionCard.cshtml", dto);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarReparacion(ReparacionesDTO reparacion)
        {
            //verificar que exite el vehiculo
            var v = await Repositorio.ObtenerDetalleReparacion(reparacion.ReparacionId);
            if (v == null) return NotFound();

            try
            {
                var user = User.Identity?.Name ?? "desconocido";
                await Repositorio.BorrarReparacion(reparacion, user);
                return Json(new { success = true, message = "Reparacion borrado correctamente." });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion
    }
}

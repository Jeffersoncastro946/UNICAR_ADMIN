using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UNICAR_ADMIN.Models.DTOS;
using UNICAR_ADMIN.Models.Renta;
using UNICAR_ADMIN.Servicios.Custom_Services;

namespace UNICAR_ADMIN.Controllers
{
    public class ClienteController : Controller
    {

        private readonly ICustomeServices customerSevices;
        public ClienteController(ICustomeServices customerSevices)
        {
            this.customerSevices = customerSevices;
        }
        // GET: ClienteController
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerClientes()
        {
           var listar = await customerSevices.ObtenerTodos();
            return Json(new { data=listar});
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerContratosCliente(int id)
        {
            var cliente = await customerSevices.ObtenerContratosPorCliente(id);
            return Json(new { data =cliente });
        }

        // GET: ClienteController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ClienteController/Create
        public ActionResult Crear()
        {
            var cliente = new ClienteDto();
            
            return PartialView("_CrearCliente", cliente);
        }

        [HttpPost]
        public async Task<IActionResult> crear(ClienteDto dto)
        {
            if (!ModelState.IsValid)
            {
                // Reenvía la vista parcial con errores para que AJAX la pinte

                return PartialView("_CrearCliente", dto);
            }

            try
            {
                var cliente = await customerSevices.Crear(dto, User.Identity?.Name ?? "desconocido");
                return Json(new { success = true, message = "Cliente creado correctamente." });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message });
            }
        }


        //GET: ClienteController/Edit/5
        public async Task<ActionResult> EditarCliente(int id)
        {
            //primero verificar que exista ese cliente id
            var clienteDto = await customerSevices.ObtenerPorId(id);
            if (clienteDto == null)
            {
                ModelState.AddModelError(string.Empty,"NO SE ENCONTRO RESULTADO");
                return PartialView("_EditarCliente", clienteDto);
            }
            try
            {

                //si existe el cliente devolvemos la vista parcial con los datos del cliente
                return PartialView("_EditarCliente", clienteDto);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });

            }
  
            //return PartialView("_CrearCliente", clienteDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditarCliente(ClienteDto dto)
        {
            if (!ModelState.IsValid)
            {
               return PartialView("_EditarCliente", dto);
            }

            try
            {
                var cliente = await customerSevices.Actualizar(dto, User.Identity?.Name ?? "desconocido");
                return Json(new { success = true, message = "Cliente Editado correctamente." });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: ClienteController/Delete/5
        public async Task<ActionResult> BorrarCliente(int id)
        {
            //primero verificar que exista ese cliente id
            var clinetebd =await customerSevices.ObtenerPorId(id);
            if (clinetebd == null)
            {
                ModelState.AddModelError(string.Empty, "NO SE ENCONTRO RESULTADO");
               RedirectToAction(nameof(Index));
            }
            try
            {
                //si existe el cliente devolvemos la vista parcial con los datos del cliente
                return PartialView("_BorrarCliente", clinetebd);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }

        // POST: ClienteController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BorrarCliente(ClienteDto Borrar)
        {
            try
            {
                //borrar el cliente
                var clienteBorrar = await customerSevices.Eliminar(Borrar.ClienteId, User.Identity?.Name ?? "desconocido");
                return Json(new { success = true, message = "Cliente Borrado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}

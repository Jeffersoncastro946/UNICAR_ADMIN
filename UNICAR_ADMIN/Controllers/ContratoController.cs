using Microsoft.AspNetCore.Mvc;
using UNICAR_ADMIN.Servicios.Contrato_Services;

namespace UNICAR_ADMIN.Controllers
{
    public class ContratoController : Controller
    {
        private readonly IContratoServices contratoServices;

        public ContratoController(IContratoServices contratoServices)
        {
            this.contratoServices = contratoServices;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerContratos()
        {
            var listadoContratos = await contratoServices.ObtenerTodos();
            return Json(new { data = listadoContratos });
        }
    }
}

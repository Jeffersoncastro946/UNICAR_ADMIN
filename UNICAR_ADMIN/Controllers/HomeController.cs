using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UNICAR_ADMIN.Models;
using UNICAR_ADMIN.Servicios.Vehiculos_Services;

namespace UNICAR_ADMIN.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRepositorio_Vehiculo RepoVehiculo;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;


        public HomeController(ILogger<HomeController> logger, IRepositorio_Vehiculo RepoVehiculo, UserManager<IdentityUser> _userManager,SignInManager<IdentityUser> _signInManager)
        {
            _logger = logger;
            this.RepoVehiculo = RepoVehiculo;
            this._userManager = _userManager ?? throw new ArgumentNullException(nameof(_userManager));
            this._signInManager = _signInManager ?? throw new ArgumentNullException(nameof(_signInManager));
        }

        public async Task<IActionResult> Index()
        {
            var catalogo = (await RepoVehiculo.ObtenerTodoVehiculo()).ToList();
            if (User.Identity?.IsAuthenticated == true)
            {
                // 1) Obtén la entidad user
                var user = await _userManager.GetUserAsync(User);

                // Verifica si el usuario es nulo antes de usarlo
                if (user != null)
                {
                    // 2) Refresca la cookie para que traiga los roles nuevos
                    await _signInManager.RefreshSignInAsync(user);
                    var rol = User.IsInRole("Admin");
                    // 3) Comprueba el rol (case-sensitive)
                    if (User.IsInRole("Admin"))
                    {
                        // ojo al orden de los parámetros: 
                        // primero acción, luego controlador
                        return RedirectToAction("Index", "Vehiculo");
                        // o si tu "Panel" está en AdminController:
                        // return RedirectToAction("Panel", "Admin");
                    }
                }
            }

            // Usuario no autenticado o no Admin
            return View(catalogo);
        }

        public async Task<IActionResult> DetalleVehiculoAsync(int idVehiculo)
        {
            var vehiculo = await RepoVehiculo.ObtenerDetalleVehiculo(idVehiculo);   

            return PartialView("_vehiculoDetalle", vehiculo);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

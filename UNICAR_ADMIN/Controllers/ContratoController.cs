using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using UNICAR_ADMIN.Models.DTOS;
using UNICAR_ADMIN.Models.Renta;
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

        public IActionResult Index2()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerContratos()
        {
            var listadoContratos = await contratoServices.ObtenerTodos();
            return Json(new { data = listadoContratos });
        }
        public async Task<IActionResult> HistorialContratos()
        {
            var listadoContratos = await contratoServices.HistorialContratos();
            return Json(new { data = listadoContratos });
        }


        public async Task<IActionResult> CrearContrato()
        {
            //llenamos los dropdowns y otros datos necesarios
            var contratoDto = new ContratoDto
            {
                ListaCliente = await contratoServices.ObtenerClientesActivos(),
                ListaVendedor = await contratoServices.ObtenerVendedoresActivos(),
                ListaVehiculo = await contratoServices.ObtenerVehiculosActivos(false),
                ListaTipoVenta = new[] {
                    new SelectListItem { Text = "Contado", Value = "Contado" },
                    new SelectListItem { Text = "Financiada", Value = "Financiada" },
                },
                ListaPlazos = new List<SelectListItem>
                {
                    new SelectListItem { Text = "12 meses", Value = "12" },
                    new SelectListItem { Text = "24 meses", Value = "24" },
                    new SelectListItem { Text = "36 meses", Value = "36" },
                    new SelectListItem { Text = "48 meses", Value = "48" },
                    new SelectListItem { Text = "60 meses", Value = "60" }

                },
                ListaTasa = new List<SelectListItem>
                {
                    new SelectListItem { Text = "36%", Value = "36" },
                    new SelectListItem { Text = "46%", Value = "46" }
                },
                FechaVenta = DateTime.UtcNow
            };
            return PartialView("~/Views/Contrato/VistasParciales/_CrearContracto.cshtml", contratoDto);

        }

        [HttpPost]
        public async Task<IActionResult> CrearContrato(ContratoDto contratoDto)
        {

            if (!ModelState.IsValid)
            {
                // Reenvía la vista parcial con errores para que AJAX la pinte
                //return PartialView("~/Views/Contrato/VistasParciales/_CrearContracto.cshtml", contratoDto);
                var errores = ModelState.Values.SelectMany(v => v.Errors).Select(E => E.ErrorMessage).ToList();

                return Json(new { success = false, message = errores });
            }
            try
            {
                var contrato = await contratoServices.CrearAsync(contratoDto, User.Identity?.Name ?? "desconocido");
                return Json(new { success = true, message = "Contrato creado correctamente." });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {

            try
            {
                var contrato = await contratoServices.ObtenerPorId(id);
                return PartialView("~/Views/Contrato/VistasParciales/_EliminarContrato.cshtml", contrato);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EliminarContrato(int ContratoId)
        {
            try
            {
                var resultado = await contratoServices.Eliminar(ContratoId, User.Identity?.Name ?? "desconocido");
                if (resultado)
                {
                    return Json(new { success = true, message = "Contrato eliminado correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo eliminar el contrato." });
                }

            }
            catch (Exception ex)
            {

                return Json(new { succes = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> editar(ContratoDto dto)
        {
            

            try
            {
                //verificamos que el modelo sea correcto
                if (!ModelState.IsValid)
                {
                    return PartialView("~/Views/Contrato/VistasParciales/_EditarContrato.cshtml", dto);
                }

                var contratoEditado = await contratoServices.Actualizar(dto, User.Identity?.Name ?? "desconocido");
                return Json(new { success = true, message = "Actualizado correctamente" });

            }
            catch (Exception ex)
            {

                return Json(new { succes = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> editar(int id)
        {
            try
            {
                //aqui decido que puede editar


                //VERIFICAR QUE EL contrato EXISTA
                var contrato = await contratoServices.ObtenerPorId(id);
                if (contrato == null )
                {
                    //ploblamos los datos de los dropdowns y otros datos necesarios
                    contrato = new ContratoDto
                    {
                        ListaCliente = await contratoServices.ObtenerClientesActivos(),
                        ListaVendedor = await contratoServices.ObtenerVendedoresActivos(),
                        ListaVehiculo = await contratoServices.ObtenerVehiculosActivos(true),
                        ListaTipoVenta = new[] {
                            new SelectListItem { Text = "Contado", Value = "Contado" },
                            new SelectListItem { Text = "Financiada", Value = "Financiada" },
                        },
                        ListaPlazos = new List<SelectListItem>
                        {
                            new SelectListItem { Text = "12 meses", Value = "12" },
                            new SelectListItem { Text = "24 meses", Value = "24" },
                            new SelectListItem { Text = "36 meses", Value = "36" },
                            new SelectListItem { Text = "48 meses", Value = "48" },
                            new SelectListItem { Text = "60 meses", Value = "60" }

                        },
                        ListaTasa = new List<SelectListItem>
                        {
                            new SelectListItem { Text = "36%", Value = "36.00" },
                            new SelectListItem { Text = "46%", Value = "46.00" }
                        },
                        ListarEstadosContrato=new List<SelectListItem>
                        {
                            new SelectListItem { Text = "Activo", Value = "Activo" },
                            new SelectListItem { Text = "Finalizado", Value = "Finalizado" },
                            new SelectListItem { Text = "Cancelado", Value = "Cancelado" },
                            new SelectListItem { Text = "En Mora", Value = "EnMora" }
                        }
                        ,
                        FechaVenta = DateTime.UtcNow
                    };
                    return PartialView("~/Views/Contrato/VistasParciales/_EditarContrato.cshtml", contrato);
                }
                    //armar los dropdows correspodientes
                contrato.ListaCliente = await contratoServices.ObtenerClientesActivos();
                contrato.ListaVendedor = await contratoServices.ObtenerVendedoresActivos();
                contrato.ListaVehiculo = await contratoServices.ObtenerVehiculosActivos(true);
                contrato.ListaTipoVenta = new[] {
                          new SelectListItem("Contado","Contado"),
                          new SelectListItem("Financiada","Financiada"),
                };
                contrato.ListaPlazos = new List<SelectListItem>
                 {
                   new SelectListItem { Text = "12 meses", Value = "12" },
                    new SelectListItem { Text = "24 meses", Value = "24" },
                    new SelectListItem { Text = "36 meses", Value = "36" },
                    new SelectListItem { Text = "48 meses", Value = "48" },
                    new SelectListItem { Text = "60 meses", Value = "60" }
                 };
                contrato.ListaTasa = new List<SelectListItem>
                 {
                     new SelectListItem { Text = "36%", Value = "36.00" },
                     new SelectListItem { Text = "46%", Value = "46.00" }
                 };
                contrato.ListarEstadosContrato = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Activo", Value = "Activo" },
                    new SelectListItem { Text = "Finalizado", Value = "Finalizado" },
                    new SelectListItem { Text = "Cancelado", Value = "Cancelado" },
                    new SelectListItem { Text = "En Mora", Value = "EnMora" }
                };
              

               
                return PartialView("~/Views/Contrato/VistasParciales/_EditarContrato.cshtml", contrato);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }


        }


        [HttpGet]
        public async Task<IActionResult> DetalleContratoParcial(int id)
        {
            try
            {
                var contrato = await contratoServices.ObtenerPorId(id);
                if(contrato.TipoVenta== "Financiada")
                {
                    ViewBag.valor = contrato.PrecioContrato;
                }
                else
                {
                    ViewBag.valor = contrato.PrecioVenta;
                }
                   
                var contratoVehiculo = await contratoServices.ObtenerVehiculosPorContrato(id);
                if (contratoVehiculo == null)
                {
                    return NotFound();
                }
                return PartialView("~/Views/Contrato/VistasParciales/_DetalleContrato.cshtml", contratoVehiculo);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}

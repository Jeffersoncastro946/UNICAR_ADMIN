using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;   // 👈 añade este using
using System.Drawing;
using System.Threading.Tasks;
using UNICAR_ADMIN.Models.DTOS.Reportes;
using UNICAR_ADMIN.Servicios.Reportes_Services;


namespace UNICAR_ADMIN.Controllers
{
    public class ReportesController : Controller
    {
        private readonly IReportes_Services reportesServices;
        private readonly IWebHostEnvironment _env;
        public ReportesController(IReportes_Services reportesServices, IWebHostEnvironment env)
        {
            this.reportesServices = reportesServices;
            _env = env;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ReporteMantenimientos(int vehiculoId)
        {
            var reporte= reportesServices.ObtenerReporteMantenimientos(vehiculoId); // Cambia el ID según sea necesario
      
           
            return View(reporte);
        }

        [HttpGet]
        public async Task<IActionResult> ReporteContratosPdf([FromQuery] FiltroContratosEstadoDto filtro)
        {
            var reporte = await reportesServices.GetReporteAsync(filtro);

            return View(reporte);
        }

        [HttpGet]
        public async Task<IActionResult> ReporteContratosPdfDowload([FromQuery] FiltroContratosEstadoDto filtro)
        {
            var reporte = await reportesServices.GetReporteAsync(filtro);
            var switches =
                          "--header-spacing 5 " +
                            // 35 mm para reservar hueco al header
                          "--margin-bottom 10 " +
                          "--footer-center \"Página [page]/[toPage]\" " +
                          "--footer-font-size 8 --footer-line --footer-spacing 5 "; 
                          
            return new ViewAsPdf("ReporteContratosPdfDowload", reporte)   // ← tu vista Razor
            {
                PageSize = Rotativa.AspNetCore.Options.Size.Letter,
                CustomSwitches = switches,
                FileName = $"ReporteContratos_{DateTime.Now:yyyyMMdd_HHmm}.pdf"
            };
        }
        public IActionResult Filtros()
        {
            return PartialView("/views/Reportes/_FiltrosReporteContratoEstado.cshtml");
        }

        [AllowAnonymous]
        public IActionResult ReporteHeader([FromQuery] FiltroContratosEstadoDto vm)
        {
            return PartialView("_ReporteHeader", vm);
        }
      
        public IActionResult _ReporteHeaderMantenimiento(ReporteVehiculoMantenimientosViewModel vm)
        {
            return PartialView("_ReporteHeader", vm);
        }

        public async Task<IActionResult> ExportarExcelContrato([FromQuery] FiltroContratosEstadoDto filtro)
        {
            var dto = await reportesServices.GetReporteAsync(filtro);


            using var packages = new ExcelPackage();
            var sheet = packages.Workbook.Worksheets.Add("contratos");
            sheet.Cells[1, 1].Value = "ID";
            sheet.Cells[1, 2].Value = "Estado";
            sheet.Cells[1, 3].Value = "Cliente";
            sheet.Cells[1, 4].Value = "Vehículo";
            sheet.Cells[1, 5].Value = "Fecha Venta";
            sheet.Cells[1, 6].Value = "Precio Venta";
            sheet.Cells[1, 7].Value = "Pagado";
            sheet.Cells[1, 8].Value = "Saldo";
            // 4. Rellenar filas
            var row = 2;
           foreach (var item in dto.Filas)
            {
                sheet.Cells[row,1].Value  = item.ContratoId;
                sheet.Cells[row, 2].Value = item.Estado;
                sheet.Cells[row, 3].Value = item.Cliente;
                sheet.Cells[row, 4].Value = item.Vehiculo;
                sheet.Cells[row, 5].Value = item.FechaVenta;
                sheet.Cells[row, 6].Value = item.PrecioVenta;
                sheet.Cells[row, 7].Value = item.MontoPagadoTotal;
                sheet.Cells[row, 8].Value = item.SaldoPendiente;
                row++;
            }
            var headerRange = sheet.Cells[1, 1, 1, 8];
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0x66, 0x39, 0xB3));

            var dataRange = sheet.Cells[1, 1, row - 1, 8];
            dataRange.Style.Border.Top.Style =
            dataRange.Style.Border.Bottom.Style =
            dataRange.Style.Border.Left.Style =
            dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;

            // columna 5 → FechaVenta
            sheet.Column(5).Style.Numberformat.Format = "dd/MM/yyyy";

            // columnas 6–8 → formatos de moneda
            var monedaFormat = "\"L\"#,##0.00";   // o bien @"""L""#,##0.00"            sheet.Column(6).Style.Numberformat.Format = monedaFormat;
            sheet.Column(6).Style.Numberformat.Format = monedaFormat;
            sheet.Column(7).Style.Numberformat.Format = monedaFormat;
            sheet.Column(8).Style.Numberformat.Format = monedaFormat;


            // d) Alternar color de fondo en filas pares
            for (int r = 2; r < row; r += 2)
            {
                var rowRange = sheet.Cells[r, 1, r, 8];
                rowRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                rowRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0xF7, 0xF8, 0xFC));
            }
            //ajsutar columnas
            dataRange.AutoFitColumns();

            var content = packages.GetAsByteArray();

            return File(
               content,
               "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
               $"ReporteContratos_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
            );
        }


        public async Task<IActionResult> InventarioVehiculosDisponibles()
        {
            var lista = await reportesServices.InventarioVehiculos();
            ViewBag.fechaMinima = lista.Min(x => x.FechaIngreso) ?? DateTime.MinValue;
            ViewBag.fechamax = lista.Max(x => x.FechaIngreso) ?? DateTime.MinValue;
            var switches =
              "--enable-local-file-access " +
              "--header-spacing 2 " +
              "--margin-top 0 " +
              "--margin-bottom 10 " +
              "--margin-left 0 " +
              "--margin-right 0 " +
              "--footer-center \"Página [page]/[toPage]\" " +
              "--footer-font-size 8 --footer-line --footer-spacing 5 ";

            return new ViewAsPdf("InventarioVehiculosDisponibles", lista)   // ← tu vista Razor
            {
                PageSize = Rotativa.AspNetCore.Options.Size.Letter,
                //PageMargins = new Margins(0, 10, 0, 10),   // 0 arriba/abajo: ya los da el switch
                CustomSwitches = switches,
                FileName = $"ReporteInventarioVehiculosDisponibles_{DateTime.Now:yyyyMMdd_HHmm}.pdf"
            };
        }
        public async Task<IActionResult> InventarioVehiculosDisponiblesView()
        {
            var lista = await reportesServices.InventarioVehiculos();
           
            ViewBag.fechaMinima = lista.Min(x => x.FechaIngreso) ?? DateTime.MinValue;
            ViewBag.fechamax = lista.Max(x => x.FechaIngreso) ?? DateTime.MinValue;
            var headerVm = new ReporteHeaderViewModel
            {
                Titulo = "Inventario de Vehículos",
                //Subtitulo = $"Agrupado: {(agrupacion == "anio" ? "Año" : "Mes")}",
                FechaEmision = DateTime.Now

            };
            ViewData["header"] = headerVm;
            return View("InventarioVehiculosDisponiblesView", lista);
        }

        public IActionResult VentasPorPeriodo(string agrupacion = "mes")
        {
            // sólo devuelve la vista; el JS en la vista hará fetch al JSON
            ViewBag.Agrupacion = agrupacion.ToLower();
            // Prepara ViewData["Header"] si usas tu partial de encabezado
            var headerVm = new ReporteHeaderViewModel
            {
                Titulo = agrupacion == "anio" ? "Ventas por Año" : "Ventas por Mes",
                Subtitulo = $"Agrupado: {(agrupacion == "anio" ? "Año" : "Mes")}",
                FechaEmision=DateTime.Now

            };
            ViewData["Header"] = headerVm;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> VentasPorPeriodoJson(string agrupacion = "mes")
        {
            var datos = await reportesServices.ObtenerVentasPeriodo(agrupacion);
            return Json(datos);
        }


        [HttpGet]
        public IActionResult IngresosPorEstado(DateTime? fechaInicio, DateTime? fechaFin)
        {
            // 1) Garantizar valores por defecto
            var fInicio = fechaInicio?.Date ?? DateTime.Today.AddMonths(-1).Date;
            var fFin = fechaFin?.Date ?? DateTime.Today.Date;

            // 2) Pasar a la vista
            ViewBag.FechaInicio = fInicio;
            ViewBag.FechaFin = fFin;

            // 3) Preparar ViewModel para el header parcial
            var headerVm = new ReporteHeaderViewModel
            {
                Titulo = "Ingresos Finalizados por Mes",
                Subtitulo = $"({fInicio:dd/MM/yyyy} – {fFin:dd/MM/yyyy})",
                FechaEmision = DateTime.Now,
                // LogoBase64  = ... (carga aquí tu logo si usas base64)
            };
            ViewData["Header"] = headerVm;

            // 4) Renderizar la vista (no enviamos modelo, se obtiene por AJAX)
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> VentasPorEstadoJson(DateTime? fechaInicio, DateTime? fechaFin)
        {
            var fInicio = fechaInicio?.Date ?? DateTime.Today.AddMonths(-1).Date;
            var fFin = fechaFin?.Date ?? DateTime.Today.Date;
            var lista = await reportesServices.ObtenerIngresosPorEstadoAsync(fInicio, fFin);
            return Json(lista);
        }

        // Vista principal (elige año y muestra dashboard)
        [HttpGet]
        public IActionResult ClientesCrecimiento(int? anio)
        {
            int year = anio ?? DateTime.Today.Year;

            var headerVm = new ReporteHeaderViewModel
            {
                Titulo = "Crecimiento de Clientes",
                Subtitulo = $"Año {year}",
                FechaEmision = DateTime.Now
            };
            ViewData["Header"] = headerVm;
            ViewBag.Anio = year;

            return View(); // la vista rellenará por AJAX
        }

        // Endpoint JSON
        [HttpGet]
        public async Task<IActionResult> ClientesCrecimientoJson(int anio)
        {
            var vm = await reportesServices.ObtenerDashboardClientesAsync(anio);
            return Json(vm);
        }


        [HttpGet]
        public IActionResult VehiculosRezago()
        {
            var header = new ReporteHeaderViewModel
            {
                Titulo = "Reporte de vehículos con más de 90 días en inventario",
                Subtitulo = "Vehículos con permanencia extendida en inventario",
                FechaEmision = DateTime.Now
            };
            ViewData["Header"] = header;
            return View(); // la vista consumirá JSON
        }

        // Endpoint JSON
        [HttpGet]
        public async Task<IActionResult> VehiculosRezagoJson()
        {
            var vm = await reportesServices.ObtenerVehiculosRezagadosAsync();
           
            return Json(vm);
        }

        [HttpGet]
        public IActionResult ReparacionesAlCincuentaPerc()
        {
            ViewData["Header"] = new ReporteHeaderViewModel
            {
                Titulo = "Reparaciones al 50% del costo",
                Subtitulo = "Vehículos con reparaciones que alcanzan el 50% del valor de compra",
                FechaEmision = DateTime.Now
            };
            return View(); 
        }

        public async Task<IActionResult> VehiculosReparacionAltaJson()
        {
            var data = await reportesServices.VehiculosReparacionAltaJson();

            return Json(data);

        }
    }
}

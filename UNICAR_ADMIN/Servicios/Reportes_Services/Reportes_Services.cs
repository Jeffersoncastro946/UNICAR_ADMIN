using AspNetCoreGeneratedDocument;
using iText.Commons.Actions.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using UNICAR_ADMIN.Models.DTOS;
using UNICAR_ADMIN.Models.DTOS.Reportes;
using UNICAR_ADMIN.Models.Renta;

namespace UNICAR_ADMIN.Servicios.Reportes_Services
{

    public interface IReportes_Services
    {
        Task<ContratosPorEstadoDto> GetReporteAsync(FiltroContratosEstadoDto filtro);
        Task<List<InventarioVehiculosRPT>> InventarioVehiculos();
        ReporteVehiculoMantenimientosViewModel ObtenerReporteMantenimientos(int vehiculoId);
        Task<List<ReportesVentasDTO>> ObtenerVentasPeriodo(string agrupacion);
        Task<ReporteFinalizadosViewModel> ObtenerIngresosPorEstadoAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<DashboardClientesVM> ObtenerDashboardClientesAsync(int anio);
        Task<ReporteInventarioEnvejecidoVM> ObtenerVehiculosRezagadosAsync();
        Task<ReparacionesResponseDto> VehiculosReparacionAltaJson();
    }

    public class Reportes_Services: IReportes_Services
    {
        private readonly RentaDbContext context;
        public Reportes_Services(RentaDbContext context)
        {
            this.context = context;   
        }
        //estatico
        private static string Bucket(int d) => d switch
        {
            >= 211 => ">210",
            >= 181 => "181–210",
            >= 151 => "151–180",
            >= 121 => "121–150",
            _ => "91–120"
        };


        //reporte man
        public ReporteVehiculoMantenimientosViewModel ObtenerReporteMantenimientos(int vehiculoId)
        {
            var vehiculo = context.Vehiculos
                .Where(v => v.VehiculoId == vehiculoId)
                .Select(v => new ReporteVehiculoMantenimientosViewModel
                {
                    VehiculoId = v.VehiculoId,
                    Marca = v.Marca,
                    Modelo = v.Modelo,
                    Año = v.Anio,
                    Color = v.Color,
                    VIN = v.Vin,
                    Precio = v.Precio,
                    Reparaciones = context.Reparaciones
                        .Where(r => r.VehiculoId == vehiculoId && r.Activo == true)
                        .Select(r => new ReparacionDto
                        {
                            FechaCreacion = r.FechaCreacion.HasValue ? r.FechaCreacion.Value : DateTime.Now,
                            Descripcion = r.Descripcion ?? "Sin descripción",
                            Costo = r.Costo ?? 0m,
                            ImagenUrl = r.ImagenUrl,
                            Responsable = r.Responsable
                        }).OrderBy(r=>r.FechaCreacion).ToList(),
                    fechaInicial=context.Reparaciones.Where(r=>r.VehiculoId==vehiculoId && r.Activo==true).Min(r=>r.FechaInicial),
                    fechaFinal= context.Reparaciones.Where(r => r.VehiculoId == vehiculoId && r.Activo == true).Max(r => r.FechaFinal)
                }).FirstOrDefault();
            return vehiculo ?? new ReporteVehiculoMantenimientosViewModel
            {
                Reparaciones = new List<ReparacionDto>()
            };
        }
        //fin

        //reporte contratos por estado
        public async Task<ContratosPorEstadoDto> GetReporteAsync(FiltroContratosEstadoDto filtro)
        {
            //base del query 
            var q=context.Contratos
                .Include(c => c.Cliente)
                .Include(c => c.Vehiculo)
                .Include(c=>c.PagosContratos.Where(p=> p.Activo == true))
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro.Estado) && filtro.Estado != "Todos")
                q = q.Where(c => c.Estado == filtro.Estado);

            if (filtro.FechaInicio.HasValue)
                q = q.Where(c => c.FechaVenta >= filtro.FechaInicio);

            if (filtro.FechaFin.HasValue)
                q = q.Where(c => c.FechaVenta <= filtro.FechaFin);

            var filas = await q.Select(c => new ContratosPorEstadoDto.Fila
            {
                ContratoId = c.ContratoId,
                Estado = c.Estado,
                Cliente = c.Cliente!.NombreCompleto ?? "sin nombre",
                Vehiculo = c.Vehiculo!.Marca + " " + c.Vehiculo.Modelo,
                FechaVenta =c.FechaVenta,
                PrecioVenta = c.PrecioVenta ?? c.MontoTotal,
                MontoPagadoTotal = c.PagosContratos.Sum(p => p.MontoPagado),
                Pagos = c.PagosContratos.Select(p => new ContratosPorEstadoDto.Fila.DetallePago
                {
                    PagoId = p.PagoContratoId,
                    FechaPago = p.FechaPago.ToDateTime(TimeOnly.MinValue),
                    Monto = p.MontoPagado,
                    Observacion = p.Observacion?? "sin comentario"
                }).ToList()
            })
            .ToListAsync();

            return new ContratosPorEstadoDto
            {
                EstadoFiltrado = filtro.Estado ?? "Todos",
                FechaInicio = filtro.FechaInicio,
                FechaFin = filtro.FechaFin,
                Filas = filas
            };


        }

        //fin 

        public async Task<List<InventarioVehiculosRPT>> InventarioVehiculos()
        {
            var q =  context.Vehiculos.Where(v=>v.Activo==true)
                         .Include(v => v.EstadoNavigation)
                         .Include(v => v.Proveedor);

            var filas = await q.Select(v => new InventarioVehiculosRPT
            {
                CodVehiculo=v.VehiculoId,
                Marca=v.Marca,
                Modelo=v.Modelo,
                FechaIngreso=v.FechaIngreso,
                Estado=v.EstadoNavigation.Nombre,
                Anio=v.Anio,
                color=v.Color??"sin color",
                Vin=v.Vin,
                Proveedor = v.Proveedor != null ? v.Proveedor.Nombre : "Sin proveedor",
                Esconsignacion=v.EsConsignacion ?? false

            }).ToListAsync();
            return filas;
            
        }

        public async Task<List<ReportesVentasDTO>> ObtenerVentasPeriodo(string agrupacion)
        {
            //obtendremos solo la lista de todos los datos
            var datosCrudos = await context.Contratos
                .Where(c => c.Estado != "Cancelado" && c.FechaVenta != null)
                .ToListAsync();

            List<ReportesVentasDTO> lista;
            //segun el filtro que venga del select
            if (agrupacion == "anio")
            {
                lista = datosCrudos.GroupBy(c => c.FechaVenta.Value.Year)
                     .Select(g => new ReportesVentasDTO
                     {
                         Periodo = g.Key.ToString(),
                         contador = g.Count(),

                     })
                     .ToList();
            }
            else
            {
                //crear variable para la cultura del mes
                var dft = CultureInfo.CurrentCulture.DateTimeFormat;
                lista = datosCrudos
                .GroupBy(c => new {
                    Year = c.FechaVenta.Value.Year,
                    Month = c.FechaVenta.Value.Month
                })
                // Paso 1 y 2: Selecciona DTO temporal con Year, Month y contador
                .Select(g => new {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    contador = g.Count()
                })
                // Paso 3: Ordena por Year y Month
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                // Paso 4: Proyecta a ReportesVentasDTO usando GetAbbreviatedMonthName
                .Select(x => new ReportesVentasDTO
                {
                    Periodo = $"{dft.GetAbbreviatedMonthName(x.Month)} {x.Year}",
                    contador = x.contador
                })
                .ToList();
            }
            return lista;
        }

        public async Task<ReporteFinalizadosViewModel> ObtenerIngresosPorEstadoAsync(DateTime fechaInicio ,DateTime fechaFin)
        {
            //hacemos la consulta
            var query =  context.Contratos.
                Where(c => c.Activo == true && c.FechaVenta != null && c.Estado== "Finalizado"
                && (c.FechaVenta.Value.Date>=fechaInicio.Date && c.FechaVenta.Value.Date <=fechaFin.Date)
                );

            //agrupamos por tipo y totales
            var raw = await query.GroupBy(c => new
            {
                year=c.FechaVenta.Value.Year,
                month=c.FechaVenta.Value.Month,
            }).
                Select(g => new 
                {
                  year=g.Key.year,
                  month=g.Key.month,
                  total=g.Sum(c=>c.MontoPagado) //suma de los montos pagados
                }).ToListAsync();
            //mapep y ordenamiento
           var dtf= CultureInfo.CurrentCulture.DateTimeFormat;
            var meses = raw.OrderBy(r=>r.year)
                .ThenBy(r=>r.month)
                .Select(r => new IngresoMesDto
                {
                    Mes = $"{dtf.GetAbbreviatedMonthName(r.month)}-{r.year}",
                    Monto = r.total
                }).ToList();  
            
            var vm = new ReporteFinalizadosViewModel
            {
                TotalFinalizados = meses.Sum(m => m.Monto),
                Meses = meses,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            return vm;
        }

        public async Task<DashboardClientesVM> ObtenerDashboardClientesAsync(int anio)
        {
            // 1. Obtener los clientes del año indicado
            var clienteAnio = await context.Clientes
                .Where(c =>c.FechaCreacion.HasValue && c.FechaCreacion.Value.Year == anio && c.Activo==true)
                .Select(c => new { c.FechaCreacion })
                .ToListAsync();

            //2.Agrupa por mes y cuenta nuevos clientes
           var conteos = clienteAnio.GroupBy(c => c.FechaCreacion!.Value.Month).ToDictionary(g => g.Key, g => g.Count());

            //3.Prepara cultura / idioma para nombres de meses
            var cultura = CultureInfo.CurrentCulture.DateTimeFormat;

            //4.Genera la serie de crecimiento mensual
            var series = new List<ClienteCrecimientoDto>();
            int acumulado = 0;
            ClienteCrecimientoDto? anterior = null;

            for (int i = 1; i <= 12; i++)
            {
                int nuevos = conteos.TryGetValue(i, out var n) ? n : 0; //obtenemos el conteo de ese mes, si no existe, es 0
                acumulado += nuevos; //acumulamos el total
                var dto = new ClienteCrecimientoDto
                {

                    Month = i,
                    Nuevos = nuevos,
                    Year=anio,
                    Acumulado = acumulado,
                    Etiqueta = cultura.GetAbbreviatedMonthName(i).Replace(".", "").ToLowerInvariant(),

                };
                if (anterior != null && anterior.Nuevos > 0 && nuevos > 0)
                {
                    dto.CrecimientoPct = (double)(nuevos - anterior.Nuevos) / anterior.Nuevos * 100.00;
                }
                else
                {
                    dto.CrecimientoPct = null; // así tu JSON nunca tendrá infinity
                }
                series.Add(dto);
                anterior = dto; //actualizamos el anterior para el siguiente ciclo
            }

            //kpis data
            //clientes activos
            int clientesActivos = await context.Clientes.CountAsync(c => c.Activo == true);
            int nuevoAnio = series.Sum(s => s.Nuevos);
            var hoy = DateTime.Today;

            ClienteCrecimientoDto? mesActualDto = (hoy.Year == anio)
                                               ? series.First(s => s.Month == hoy.Month)
                                               : null;

            ClienteCrecimientoDto? mesAnteriorDto = (hoy.Year == anio && hoy.Month > 1)
                                                  ? series.First(s => s.Month == hoy.Month - 1)
                                                  : null;
            // Updated code to fix CS8602: Desreferencia de una referencia posiblemente NULL.
            double? crecimientoActual =
                (mesActualDto != null && mesAnteriorDto != null && mesAnteriorDto.Nuevos > 0)
                    ? (double)(mesActualDto.Nuevos - mesAnteriorDto.Nuevos) / mesAnteriorDto.Nuevos * 100.00
                    : null;


            return new DashboardClientesVM
            {
                
                ClientesActivos = clientesActivos,
                NuevosMesActual = mesActualDto?.Nuevos ?? 0,
                NuevosAnio=nuevoAnio,
                CrecimientoMesActualPct = crecimientoActual ?? 0.00,
                Series = series,
                TasaCancelacionPct = 00.00 //tasa de cancelacion, por ahora no se calcula
            };
        }

        //
        public async Task<ReporteInventarioEnvejecidoVM> ObtenerVehiculosRezagadosAsync()
        {
            var hoy = DateTime.Today;
            
            var totalInventario = await context.Vehiculos.CountAsync(v => v.Activo == true && v.Estado != 3 && v.FechaIngreso != null);


            //obtener los vehiculos
            var query = context.Vehiculos
                .Where(v => v.Activo == true && v.Estado != 3 && v.FechaIngreso != null)
                .Select(v => new
                {
                    v.VehiculoId,
                    v.Vin,
                    v.Marca,
                    v.Modelo,
                    v.Anio,
                    v.Color,
                    v.FechaIngreso,
                    v.Precio,
                    dias = EF.Functions.DateDiffDay(v.FechaIngreso, hoy), //diferencia de dias
                })
                .Where(v => v.dias >= 90); //filtramos por rezago
                
                var lista =await query.ToListAsync();

            var vehiculos = lista.Select(y => new VehiculoRezagoDto
            {
                VehiculoId = y.VehiculoId,
                Vin = y.Vin, //asignamos el id como vin, por ahora
                Marca = y.Marca ?? "sin marca",
                Modelo = y.Modelo ?? "sin modelo",
                Anio = y.Anio,
                Color = y.Color ?? "sin color",
                FechaIngreso = y.FechaIngreso,
                Precio = y.Precio,
                DiasEnStock = y.dias ?? 0,
                Rango = Bucket(y.dias ?? 0) //usamos la funcion estatica para el rango
            }).ToList();

            var grupos = vehiculos
            .GroupBy(v => v.Rango)
            .Select(g => new DistribucionRangoDto
            {
                Rango = g.Key,
                Cantidad = g.Count(),
                Valor = g.Sum(x => x.Precio)
            })
            .ToList();

            int total = vehiculos.Count;
            foreach (var g in grupos)
                g.Porcentaje = total == 0 ? 0 : (double)g.Cantidad / total;

            // Orden visual manual
            string[] orden = { "91–120", "121–150", "151–180", "181–210", ">210" };
            grupos = grupos
                .OrderBy(g => Array.IndexOf(orden, g.Rango))
                .ToList();

            var vm = new ReporteInventarioEnvejecidoVM
            {
                Unidades = total,
                EdadPromedio = vehiculos.Any() ? vehiculos.Average(v => v.DiasEnStock) : 0,
                EdadMaxima = vehiculos.Any() ? vehiculos.Max(v => v.DiasEnStock) : 0,
                ValorTotal = vehiculos.Sum(v => v.Precio),
                PorcInventarioTotal = totalInventario == 0 ? null : (double)total / totalInventario,
                Rangos = grupos,
                Vehiculos = vehiculos
                    .OrderByDescending(v => v.DiasEnStock)
                    .ThenBy(v => v.Marca)
                    .ToList()
            };

            return vm;

        }

        public async Task<ReparacionesResponseDto> VehiculosReparacionAltaJson()
        {
            //hacer la consulta
            var vehiculos = await context.Vehiculos
                .Where(v => v.Activo == true && (v.Estado != 3))
                .Select(v => new
                {
                    v.Vin,
                    v.Marca,
                    v.Modelo,
                    v.Anio,
                    v.Precio,
                    TotalReparaciones = context.Reparaciones.Where(r => r.Activo==true && (r.VehiculoId == v.VehiculoId) ).Sum(r => (decimal?)r.Costo) ?? 0,
                    v.EstadoNavigation.Nombre
                }).ToListAsync();

           
            var vehiculoFinal = vehiculos
                .Where(v=>v.TotalReparaciones>0.5m*v.Precio)
                .Select(v => new VehiculoReparacionDto
            {
                VIN = v.Vin,
                Marca = v.Marca,
                Modelo = v.Modelo,
                Anio = v.Anio,
                PrecioVenta = v.Precio,
                TotalReparaciones = v.TotalReparaciones,
                Estado = v.Nombre,
                Observacion = v.TotalReparaciones > 0.8m * v.Precio
                    ? "¡Reparación excesiva!"
                    : (v.TotalReparaciones > 0.5m *v.Precio ? "Revisión recomendada" : "")

            }).ToList();
            //kpis
            var unidades = vehiculoFinal.Count();
            var costoTotal = vehiculoFinal.Sum(v => v.TotalReparaciones);
            var valorTotal = vehiculoFinal.Sum(v => v.PrecioVenta);
            var porcPromedio = unidades > 0
                ? vehiculoFinal.Average(v => (double)(v.TotalReparaciones / v.PrecioVenta))
                : 0;


            //retornar el response

            var response = new ReparacionesResponseDto
            {
                kpi = new ReparacionesKpiDto
                {
                    Unidades = unidades,
                    CostoTotal = costoTotal,
                    PorcentPromedio = porcPromedio,
                    ValorTotal = valorTotal
                },
                Vehiculos=vehiculoFinal
            };

            return response;
            
        }


    }
}

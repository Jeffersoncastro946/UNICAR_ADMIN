using Humanizer;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;
using UNICAR_ADMIN.Models.DTOS;
using UNICAR_ADMIN.Models.Renta;
using UNICAR_ADMIN.Servicios.Vehiculos_Services;
namespace UNICAR_ADMIN.Servicios.Contrato_Services
{

    public interface IContratoServices
    {
        Task<IEnumerable<ContratoDto>> ObtenerTodos();
        Task<IEnumerable<ContratoDto>> HistorialContratos();
        Task<ContratoDto> ObtenerPorId(int id);
        Task<ContratoDto> CrearAsync(ContratoDto dto, string user);
        Task<bool> Actualizar(ContratoDto dto, string user);
        Task<bool> Eliminar(int id, string user);
        Task<ListaVehiculoDTO> ObtenerVehiculosPorContrato(int id);
        Task<bool> TienePago(int idcontrato);

       Task<string> GenerarPdfAsync(int contratoId);
        Task<IEnumerable<SelectListItem>> ObtenerClientesActivos();
        Task<IEnumerable<SelectListItem>> ObtenerVendedoresActivos();
        Task<IEnumerable<SelectListItem>> ObtenerVehiculosActivos(bool editar);
    }


    public class ContratoServices : IContratoServices
    {
        // Inyectamos el contexto de la base de datos
        private readonly RentaDbContext context;
        private readonly IRepositorio_Vehiculo repositorio_Vehiculo;
        // Inyectamos IWebHostEnvironment si es necesario para manejar archivos o rutas
        private readonly IWebHostEnvironment env;

        public ContratoServices(RentaDbContext _context, IWebHostEnvironment env, IRepositorio_Vehiculo repositorio_Vehiculo)
        {
            context = _context ?? throw new ArgumentNullException(nameof(context));
            this.env = env ?? throw new ArgumentNullException(nameof(env));
            this.repositorio_Vehiculo = repositorio_Vehiculo ?? throw new ArgumentNullException(nameof(repositorio_Vehiculo));
        }

        public async Task<IEnumerable<ContratoDto>> ObtenerTodos()
        {
            var contratos = await context.Contratos
                //.Where(x => x.Activo == true && (x.Cliente != null && x.Cliente.Activo == true)) // Fix for CS0019 and CS8652
                .Where(x => (x.Estado != "Finalizado" && x.Estado != "Cancelado") && x.Activo == true && x.Cliente != null && x.Cliente.Activo == true)
                .Select(x => new ContratoDto
                {
                    ContratoId = x.ContratoId,
                    VehiculoId = x.VehiculoId!.Value,
                    ClienteId = x.ClienteId!.Value,
                    Cliente = x.Cliente != null ? x.Cliente.NombreCompleto : "N/A",
                    VendedorId = x.VendedorId!.Value,
                    Vendedor = x.Vendedor != null ? x.Vendedor.NombreCompleto : "N/A",
                    FechaVenta = x.FechaVenta ?? DateTime.Now, // si no es nullable
                    PrecioVenta = x.PrecioVenta ?? 0.0m,
                    TipoVenta = x.TipoVenta ?? "sin venta",
                    Activo = x.Activo!.Value,
                    EstadoContrato=x.Estado,
                    PrecioContrato=x.MontoTotal,
                    //Pagado=x.PagosContratos.Where(p=>p.Activo==true).Sum(p => (decimal?)p.MontoPagado ?? 0),
                    Pagado = x.PagosContratos.Where(p => p.Activo == true).Sum(p => (decimal?)p.MontoPagado) ?? 0m,
                    UltimoPago = x.PagosContratos
                        .Where(p => p.Activo == true)
                        .OrderByDescending(c => c.FechaPago)
                        .Select(p => (DateTime?)p.FechaPago.ToDateTime(TimeOnly.MinValue))
                        .FirstOrDefault()

                }).ToListAsync(); // Ensure Microsoft.EntityFrameworkCore is imported
            return contratos;
        }
        public async Task<IEnumerable<ContratoDto>> HistorialContratos()
        {
            var contratos = await context.Contratos
                .Where(x => (x.Estado == "Cancelado" || x.Estado == "Finalizado") && x.Activo == true && x.Cliente != null && x.Cliente.Activo == true)
                .Select(x => new ContratoDto
                {
                    ContratoId = x.ContratoId,
                    VehiculoId = x.VehiculoId!.Value,
                    ClienteId = x.ClienteId!.Value,
                    Cliente = x.Cliente != null ? x.Cliente.NombreCompleto : "N/A",
                    VendedorId = x.VendedorId!.Value,
                    Vendedor = x.Vendedor != null ? x.Vendedor.NombreCompleto : "N/A",
                    FechaVenta = x.FechaVenta ?? DateTime.Now, // si no es nullable
                    PrecioVenta = x.PrecioVenta ?? 0.0m,
                    TipoVenta = x.TipoVenta ?? "sin venta",
                    Activo = x.Activo!.Value,
                    EstadoContrato = x.Estado,
                    PrecioContrato = x.MontoTotal,
                    Pagado = x.PagosContratos.Where(p => p.Activo == true).Sum(p => (decimal?)p.MontoPagado) ?? 0m,
                    UltimoPago = x.PagosContratos
                        .Where(p => p.Activo == true)
                        .OrderByDescending(c => c.FechaPago)
                        .Select(p => (DateTime?)p.FechaPago.ToDateTime(TimeOnly.MinValue))
                        .FirstOrDefault()

                }).ToListAsync(); // Ensure Microsoft.EntityFrameworkCore is imported
            return contratos;
        }

        public async Task<ContratoDto> ObtenerPorId(int id)
        {
            var contrato=await context.Contratos.
                Where(c=>c.ContratoId == id && c.Activo == true).Select(x=>new ContratoDto
                {
                    ContratoId=x.ContratoId,
                    vehiculo= x.Vehiculo != null ? $"{x.Vehiculo.Vin}-{x.Vehiculo.Marca} {x.Vehiculo.Modelo} ({x.Vehiculo.Anio})" : "N/A",
                    Cliente= x.Cliente != null ? x.Cliente.NombreCompleto : "N/A",
                    Vendedor= x.Vendedor != null ? x.Vendedor.NombreCompleto : "N/A",
                    TipoVenta = x.TipoVenta ?? "sin venta",
                    FechaVenta = x.FechaVenta ?? DateTime.Now, // si no es nullable
                    PrecioVenta = x.PrecioVenta ?? 0.0m,
                    ClienteId=x.ClienteId ?? 0,
                    VendedorId=x.VendedorId ??0,
                    VehiculoId=x.VehiculoId ??0,
                    //obtener campos financieros
                    TasaAnual = x.TasaAnual,
                    CuotaMensual=x.CuotaMensual,
                    PlazoMeses=x.PlazoMeses,
                    EstadoContrato=x.Estado, //con este si el estado es cancelado automaticamente se elimina el contrarto
                    PrecioContrato = x.MontoTotal,
                    Pagado = x.PagosContratos.Where(p => p.Activo == true).Sum(p => (decimal?)p.MontoPagado) ?? 0m,
                    UltimoPago = x.PagosContratos
                        .Where(p => p.Activo == true)
                        .OrderByDescending(c => c.FechaPago)
                        .Select(p => (DateTime?)p.FechaPago.ToDateTime(TimeOnly.MinValue))
                        .FirstOrDefault()
                }).FirstOrDefaultAsync();
            
            return contrato ?? throw new KeyNotFoundException($"Contrato con ID {id} no encontrado.");
        }


        public async Task<bool> Actualizar(ContratoDto dto, string user)
        {
            var ent = await context.Contratos.FindAsync(dto.ContratoId)
                     ?? throw new KeyNotFoundException("Contrato no encontrado");

            if (await TienePago(dto.ContratoId))
            {
                //listar campos prohibidos en un dictionario
                var CambiosNoPermitidos = new List<(string campo, bool cambio)>
                {
                    ("PrecioVenta", ent.PrecioVenta!=dto.PrecioVenta),
                    ("TipoVenta", ent.TipoVenta!=dto.TipoVenta),
                    ("CuotaMensual", ent.CuotaMensual!=dto.CuotaMensual),
                    ("TasaAnual", ent.TasaAnual!=dto.TasaAnual),
                    ("PlazoMeses", ent.PlazoMeses!=dto.PlazoMeses),
                };

                var cambios = CambiosNoPermitidos.Where(c => c.cambio).ToList(); //si en la tupla hay un true me regresara items que se encuentre
                if (cambios.Any())
                {
                    var campos = string.Join(", ", cambios.Select(c => c.campo));
                    throw new InvalidOperationException($"No se pueden modificar los siguientes campos porque el contrato ya tiene pagos: {campos}.");
                }
                //editar campos vendedor, fecha y estado contrato
                ent.VendedorId = dto.VendedorId;
                ent.FechaVenta = dto.FechaVenta;
                //pagos completos? si es si entonces se podria editar contrato estado a finalizado automaticamente o dejar
                var pagosCompletos = ent.MontoPagado >= ent.MontoTotal;
                var estadosPermitidos = pagosCompletos
                    ? new[] { "Finalizado", "Cancelado" }
                    : new[] { "Cancelado", "EnMora", "Activo" };

                if (!estadosPermitidos.Contains(dto.EstadoContrato))
                {
                    throw new InvalidOperationException($"El estado '{dto.EstadoContrato}' no es válido para este contrato en su situación actual.");
                }
                ent.UsuarioModificacion = user;
                ent.FechaModificacion = DateTime.UtcNow;
                ent.Estado = dto.EstadoContrato ?? "Sin Estado";

            }
            else
            {

                //si no tiene pago se puede cambiar todo expeto cliente y vehiculo
                //se cambio el tipo de venta?

                var estadosPermitido = dto.TipoVenta == "Financiada"
                    ? new[] { "Cancelado", "Activo" }
                    : new[] {  "Activo" };

                if (!estadosPermitido.Contains(dto.EstadoContrato))
                {
                    throw new InvalidOperationException($"El estado '{dto.EstadoContrato}' no es válido para este contrato en su situación actual.");
                }
                ent.Estado = dto.EstadoContrato ?? "Sin Estado";
                ent.FechaVenta = dto.FechaVenta;
                ent.PrecioVenta = dto.PrecioVenta;
                ent.UsuarioModificacion = user;
                ent.FechaModificacion = DateTime.UtcNow;


                //como deberia manejar los valores financiero?
                if (dto.TipoVenta == "Contado")
                {
                    ent.TasaAnual = 0;
                    ent.CuotaMensual = 0;
                    ent.PlazoMeses = 0;
                    ent.MontoTotal = dto.PrecioVenta;
                    ent.TipoVenta = dto.TipoVenta;
                }
                else if (dto.TipoVenta == "Financiada")
                {
                    ent.TasaAnual = dto.TasaAnual ?? 0.0m;
                    ent.CuotaMensual = dto.CuotaMensual ?? 0.0m;
                    ent.PlazoMeses = dto.PlazoMeses ?? 0;
                    ent.MontoTotal = (dto.PlazoMeses ?? 0) * (dto.CuotaMensual ?? 0);
                    ent.TipoVenta = dto.TipoVenta;
                }
            }

            return await context.SaveChangesAsync() > 0;

        }

        public async Task<bool> Eliminar(int id, string user)
        {
            // 1) Buscar el contrato por ID
            var ent = await context.Contratos.FindAsync(id)
                     ?? throw new KeyNotFoundException("Contrato no encontrado");
            ent.Activo = false;
            ent.UsuarioModificacion = user;
            ent.FechaModificacion = DateTime.UtcNow;

            //debo restablecer el estado del vehículo a 1 (disponible)
            var  vehiculo = await context.Vehiculos.FindAsync(ent.VehiculoId)
                     ?? throw new KeyNotFoundException("Vehículo no encontrado");
            vehiculo.Estado = 1; // Restablecer a disponible


            return await context.SaveChangesAsync() > 0;
        }

        public async Task<ContratoDto> CrearAsync(ContratoDto dto, string user)
        {

            //CARGAR DATA DEL VEHICULO PARA CAMBIAR EL ESTADO

            var vehiculo = context.Vehiculos.FirstOrDefault(c => c.VehiculoId == dto.VehiculoId) ?? throw new Exception("Error: no podemos asignar vehículo al contrato"); ;
            // 2) Cambiar estado
            vehiculo.Estado = 3;
            //context.Vehiculos.Update(vehiculo); // <-- Asegura que el cambio se guarde

            //calcular cuantos tiene del monto pagado
            // 2) Mapear DTO a entidad
            var ent = new Contrato
            {
                ClienteId = dto.ClienteId,
                VehiculoId = dto.VehiculoId,
                VendedorId = dto.VendedorId,
                FechaVenta = dto.FechaVenta,
                PrecioVenta = dto.PrecioVenta,
                TipoVenta = dto.TipoVenta,
                FechaCreacion = DateTime.UtcNow,
                UsuarioCreacion = user,
                PlazoMeses = dto.PlazoMeses ?? 0,
                TasaAnual = dto.TasaAnual ?? 0.0m,
                CuotaMensual = dto.CuotaMensual ?? 0.0m,
                MontoPagado = 0.0m, // Inicialmente no hay pagos
                Activo = true,
                MontoTotal= (dto.PlazoMeses ?? 0) * (dto.CuotaMensual ?? 0)
            };

           
            context.Contratos.Add(ent);
            await context.SaveChangesAsync();

            // 4) Actualizar DTO y devolver
            dto.ContratoId = ent.ContratoId;
            return dto;
        }



        public async Task<string> GenerarPdfAsync(int contratoId)
        {
            // 1) Carga del contrato con sus relaciones
            var contrato = await context.Contratos
                .Include(c => c.Cliente)
                .Include(c => c.Vehiculo)
                .Include(c => c.Vendedor)
                .FirstOrDefaultAsync(c => c.ContratoId == contratoId && c.Activo == true)
                ?? throw new KeyNotFoundException($"Contrato {contratoId} no encontrado.");

            // 2) Prepara carpeta de salida
            var carpeta = Path.Combine(env.WebRootPath, "contratos");
            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            // 3) Nombre y ruta del PDF
            var nombrePdf = $"contrato_{contrato.ContratoId}.pdf";
            var rutaPdf = Path.Combine(carpeta, nombrePdf);

            // 4) Creación del documento PDF
            using var writer = new PdfWriter(rutaPdf);
            using var pdf = new PdfDocument(writer);
            var doc = new Document(pdf);

            // Fuentes
            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var fontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var fontItalic = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_OBLIQUE);

            // --- Cabecera ---
            doc.Add(new Paragraph($"Contrato #{contrato.ContratoId}")
                .SetFont(fontBold).SetFontSize(18).SetMarginBottom(15));
            doc.Add(new Paragraph($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}")
                .SetFont(fontItalic).SetFontSize(10).SetMarginBottom(20));

            // --- Datos del Cliente ---
            doc.Add(new Paragraph("Cliente:")
                .SetFont(fontBold).SetFontSize(12));
            doc.Add(new Paragraph($"  {contrato.Cliente?.NombreCompleto}")
                .SetFont(font).SetFontSize(12));
            doc.Add(new Paragraph($"  Identidad: {contrato.Cliente?.Identidad}")
                .SetFont(font).SetFontSize(12));
            doc.Add(new Paragraph($"  RTN:       {contrato.Cliente?.Rtn}")
                .SetFont(font).SetFontSize(12).SetMarginBottom(15));

            // --- Datos del Vehículo ---
            doc.Add(new Paragraph("Vehículo:")
                .SetFont(fontBold).SetFontSize(12));
            var vehInfo = $"{contrato.Vehiculo?.Marca} {contrato.Vehiculo?.Modelo} ({contrato.Vehiculo?.Anio})";
            doc.Add(new Paragraph($"  {vehInfo}")
                .SetFont(font).SetFontSize(12));
            doc.Add(new Paragraph($"  VIN: {contrato.Vehiculo?.Vin}")
                .SetFont(font).SetFontSize(12).SetMarginBottom(15));

            // --- Detalles de la operación ---
            doc.Add(new Paragraph("Detalles:")
                .SetFont(fontBold).SetFontSize(12));
            doc.Add(new Paragraph($"  Tipo:   {contrato.TipoVenta}")
                .SetFont(font).SetFontSize(12));
            doc.Add(new Paragraph($"  Fecha:  {contrato.FechaVenta:dd/MM/yyyy}")
                .SetFont(font).SetFontSize(12));
            doc.Add(new Paragraph($"  Precio: L {contrato.PrecioVenta:N2}")
                .SetFont(font).SetFontSize(12));
            doc.Add(new Paragraph($"  Vendedor: {contrato.Vendedor?.NombreCompleto}")
                .SetFont(font).SetFontSize(12).SetMarginBottom(20));

            // --- Firma ---
            doc.Add(new Paragraph("Firma del Cliente:")
                .SetFont(fontBold).SetFontSize(12));


            doc.Add(new Paragraph("  __________________________")
                    .SetFont(font).SetFontSize(12).SetMarginBottom(20));


            // Cierra el documento
            doc.Close();

            // 5) Devuelve la ruta HTTP relativa
            return $"/contratos/{nombrePdf}";
        }



        //llenaremos un dropdown con los clientes, vendedores y vehiculos activos
        public async Task<IEnumerable<SelectListItem>> ObtenerClientesActivos()
        {
            return await context.Clientes
                .Where(c => c.Activo == true)
                .Select(x => new SelectListItem { Value = x.ClienteId.ToString(), Text = x.NombreCompleto })
                .ToListAsync();
        }
        public async Task<IEnumerable<SelectListItem>> ObtenerVendedoresActivos()
        {
            return await context.Vendedores
                .Where(c => c.Activo == true)
                .Select(x => new SelectListItem { Value = x.VendedorId.ToString(), Text = x.NombreCompleto })
                .ToListAsync();
        }

        // Si necesitas obtener vehículos activos, puedes implementar un método similar
        public async Task<IEnumerable<SelectListItem>> ObtenerVehiculosActivos(bool editar)
        {
            if (editar)
            {
                return await context.Vehiculos.AsNoTracking()
                .Where(v => v.Activo == true &&
                v.Estado == 3)
                .Select(x => new SelectListItem { Value = x.VehiculoId.ToString(), Text = $"{x.Vin}-{x.Marca} {x.Modelo} ({x.Anio})" })
                .ToListAsync();

            }
                return await context.Vehiculos.AsNoTracking()
                    .Where(v => v.Activo == true &&
                    v.Estado != 4 &&
                    v.Estado != 3)
                    .Select(x => new SelectListItem { Value = x.VehiculoId.ToString(), Text = $"{x.Vin}-{x.Marca} {x.Modelo} ({x.Anio})" })
                    .ToListAsync();
        }

        public async Task<ListaVehiculoDTO> ObtenerVehiculosPorContrato(int id)
        {
          var vehiculo=await repositorio_Vehiculo.ObtenerVehiculosPorContrato(id);
            if (vehiculo == null)
            {
                throw new KeyNotFoundException($"Vehículo con contrato {id} no encontrado.");
            }
            return vehiculo;
        }

        public async Task<bool> TienePago(int idContrato) 
        {
            var contratoDetalle = await context.PagosContratos.AnyAsync(x => x.ContratoId == idContrato);
            return contratoDetalle;
        }
    }
}

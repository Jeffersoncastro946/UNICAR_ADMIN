using UNICAR_ADMIN.Models.DTOS;
using UNICAR_ADMIN.Models.Renta;
using Microsoft.EntityFrameworkCore;

/*para el pdf
 */
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Http;
namespace UNICAR_ADMIN.Servicios.Contrato_Services
{

    public interface IContratoServices
    {
        Task<IEnumerable<ContratoDto>> ObtenerTodos();
        Task<ContratoDto> CrearAsync(ContratoDto dto, string user, IFormFile? firmaFile);
        Task<bool> Actualizar(ContratoDto dto, string user);
        Task<bool> Eliminar(int id, string user);
        Task<string?> GuardarFirmaAsync(IFormFile? firmaFile);
        Task<string> GenerarPdfAsync(int contratoId);
    }


    public class ContratoServices: IContratoServices
    {
        // Inyectamos el contexto de la base de datos
        private readonly RentaDbContext context;
        // Inyectamos IWebHostEnvironment si es necesario para manejar archivos o rutas
        private readonly IWebHostEnvironment env;

        public ContratoServices(RentaDbContext _context, IWebHostEnvironment env)
        {
            context = _context ?? throw new ArgumentNullException(nameof(context));
            this.env = env ?? throw new ArgumentNullException(nameof(env));
        }

        public async Task<IEnumerable<ContratoDto>> ObtenerTodos()
        {
            var contratos = await context.Contratos.Where(x => x.Activo == true)
                .Select(x => new ContratoDto
                {
                    ContratoId = x.ContratoId,
                    VehiculoId = x.VehiculoId!.Value,
                    ClienteId = x.ClienteId!.Value,
                    Cliente  = x.Cliente != null ? x.Cliente.NombreCompleto : "N/A",
                    VendedorId = x.VendedorId!.Value,
                    Vendedor = x.Vendedor !=null ? x.Vendedor.NombreCompleto : "N/A",
                    FechaVenta = x.FechaVenta ?? DateTime.Now, // si no es nullable
                    PrecioVenta = x.PrecioVenta ?? 0.0m,
                    TipoVenta = x.TipoVenta ?? "sin venta",
                    FirmaDocumento = x.FirmaDocumento, // si lo necesitas
                    Activo = x.Activo!.Value
                }).ToListAsync(); // Ensure Microsoft.EntityFrameworkCore is imported
            return contratos;
        }



        public async Task<bool> Actualizar(ContratoDto dto, string user)
        {
            var ent = await context.Contratos.FindAsync(dto.ContratoId)
                     ?? throw new KeyNotFoundException("Contrato no encontrado");
            // mapea campos
            ent.VehiculoId = dto.VehiculoId;
            ent.ClienteId = dto.ClienteId;
            ent.VendedorId = dto.VendedorId;
            ent.FechaVenta = dto.FechaVenta;
            ent.PrecioVenta = dto.PrecioVenta;
            ent.TipoVenta = dto.TipoVenta;
            ent.UsuarioModificacion = user;
            ent.FechaModificacion = DateTime.UtcNow;
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Eliminar(int id, string user)
        {
            var ent = await context.Contratos.FindAsync(id)
                     ?? throw new KeyNotFoundException("Contrato no encontrado");
            ent.Activo = false;
            ent.UsuarioModificacion = user;
            ent.FechaModificacion = DateTime.UtcNow;
            return await context.SaveChangesAsync() > 0;
        }


        public async Task<string?> GuardarFirmaAsync(IFormFile? firmaFile)
        {
            // 0) Validación temprana
            if (firmaFile == null || firmaFile.Length == 0)
                return null;

            // 1) Carpeta en wwwroot (inyectada como IWebHostEnvironment env)
            var uploads = Path.Combine(env.WebRootPath, "Firmas");
            Directory.CreateDirectory(uploads);

            // 2) Nombre único y seguro
            //    Evita inyectar rutas o caracteres maliciosos.
            var ext = Path.GetExtension(firmaFile.FileName);
            var fileName = $"firma_{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploads, fileName);

            // 3) Guardar físico de forma asíncrona
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await firmaFile.CopyToAsync(stream);
            }

            // 4) Retornar URL relativa siempre en minúscula para evitar confusiones
            return $"/Firmas/{fileName}".ToLowerInvariant();
        }
        public async Task<ContratoDto> CrearAsync(ContratoDto dto, string user, IFormFile? firmaFile)
        {
            // 1) Guardar la firma y asignar ruta al DTO
            dto.FirmaDocumento = await GuardarFirmaAsync(firmaFile);

            // 2) Mapear DTO a entidad
            var ent = new Contrato
            {
                ClienteId = dto.ClienteId,
                VehiculoId = dto.VehiculoId,
                VendedorId = dto.VendedorId,
                FechaVenta = dto.FechaVenta,
                PrecioVenta = dto.PrecioVenta,
                TipoVenta = dto.TipoVenta,
                FirmaDocumento = dto.FirmaDocumento,
                FechaCreacion = DateTime.UtcNow,
                UsuarioCreacion = user,
                Activo = true
            };

            // 3) Persistir
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

            if (!string.IsNullOrEmpty(contrato.FirmaDocumento))
            {
                var imgPath = Path.Combine(env.WebRootPath, contrato.FirmaDocumento.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(imgPath))
                {
                    var img = new Image(ImageDataFactory.Create(imgPath))
                        .ScaleToFit(200, 100)
                        .SetMarginTop(5)
                        .SetMarginBottom(20);
                    doc.Add(img);
                }
                else
                {
                    doc.Add(new Paragraph("  [Firma no encontrada]")
                        .SetFont(font).SetFontSize(12)
                        .SetFontColor(iText.Kernel.Colors.ColorConstants.RED)
                        .SetMarginBottom(20));
                }
            }
            else
            {
                doc.Add(new Paragraph("  __________________________")
                    .SetFont(font).SetFontSize(12).SetMarginBottom(20));
            }

            // Cierra el documento
            doc.Close();

            // 5) Devuelve la ruta HTTP relativa
            return $"/contratos/{nombrePdf}";
        }

    }
}

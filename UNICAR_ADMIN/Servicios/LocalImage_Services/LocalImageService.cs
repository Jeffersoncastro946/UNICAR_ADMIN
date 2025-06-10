namespace UNICAR_ADMIN.Servicios.LocalImage_Services
{
    public interface ILocalImageService
    {
        Task<string> SaveImageAsync(IFormFile file, string contenedorModeloAñoVin);
    }


    public class LocalImageService: ILocalImageService
    {

        /// <summary>
        /// Servicio para manejar imágenes locales.
        /// </summary>

        private readonly IWebHostEnvironment _env;
        private readonly string _CarpertaRelativa="Catalogo";
        public LocalImageService(IWebHostEnvironment _env) {
            this._env = _env ?? throw new ArgumentNullException(nameof(_env));
        }

       public async Task<string> SaveImageAsync(IFormFile file, string contenedorModeloAñoVin)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("El archivo no puede ser nulo o vacío.", nameof(file));
            }

            // 1. validamos la extension 
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new NotSupportedException($"La extensión {fileExtension} no está permitida. Las extensiones permitidas son: {string.Join(", ", allowedExtensions)}");
            }

            //2. generamos un nombre único para el archivo
            var NombreUnico = $"{Guid.NewGuid()}_{file.FileName}";

            //3. obtenemos la ruta del directorio donde se guardará la imagen
            var RutaFisica = Path.Combine(_env.WebRootPath, _CarpertaRelativa);

            // Si se proporciona un contenedor de modelo, lo agregamos a la ruta
            var carpetaVehiculoFisica = Path.Combine(RutaFisica, contenedorModeloAñoVin);


            //4. aseguramos que el directorio exista
            if (!Directory.Exists(carpetaVehiculoFisica))
            {
                Directory.CreateDirectory(carpetaVehiculoFisica);
            }
            //5. combinamos la ruta del directorio con el nombre del archivo
            var rutaArchivo=Path.Combine(carpetaVehiculoFisica, NombreUnico);
            
            //6. copiamos el archivo de iformfiles al disco  


            using (var stream = new FileStream(rutaArchivo, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return $"/{_CarpertaRelativa}/{contenedorModeloAñoVin}/{NombreUnico}";
        }


    }
}

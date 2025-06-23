using System.IO;

namespace UNICAR_ADMIN.Servicios.LocalImage_Services
{
    public interface ILocalImageService
    {
        Task<string?> SaveImageAsync(IFormFile file, string ContainerFolder);
        Task<string?> EditImageAsync(IFormFile file, string ContainerFolder, string existingImagePath);
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

       //public async Task<string> SaveImageAsync(IFormFile file, string contenedorModeloAñoVin)
       // {
       //     if (file == null || file.Length == 0)
       //     {
       //         throw new ArgumentException("El archivo no puede ser nulo o vacío.", nameof(file));
       //     }

       //     // 1. validamos la extension 
       //     var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
       //     var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
       //     if (!allowedExtensions.Contains(fileExtension))
       //     {
       //         throw new NotSupportedException($"La extensión {fileExtension} no está permitida. Las extensiones permitidas son: {string.Join(", ", allowedExtensions)}");
       //     }

       //     //2. generamos un nombre único para el archivo
       //     var NombreUnico = $"{Guid.NewGuid()}_{file.FileName}";

       //     //3. obtenemos la ruta del directorio donde se guardará la imagen
       //     var RutaFisica = Path.Combine(_env.WebRootPath, _CarpertaRelativa);

       //     // Si se proporciona un contenedor de modelo, lo agregamos a la ruta
       //     var carpetaVehiculoFisica = Path.Combine(RutaFisica, contenedorModeloAñoVin);


       //     //4. aseguramos que el directorio exista
       //     if (!Directory.Exists(carpetaVehiculoFisica))
       //     {
       //         Directory.CreateDirectory(carpetaVehiculoFisica);
       //     }
       //     //5. combinamos la ruta del directorio con el nombre del archivo
       //     var rutaArchivo=Path.Combine(carpetaVehiculoFisica, NombreUnico);
            
       //     //6. copiamos el archivo de iformfiles al disco  


       //     using (var stream = new FileStream(rutaArchivo, FileMode.Create))
       //     {
       //         await file.CopyToAsync(stream);
       //     }
       //     return $"/{_CarpertaRelativa}/{contenedorModeloAñoVin}/{NombreUnico}";
       // }

        public async Task<string?> SaveImageAsync(IFormFile file, string ContainerFolder)
        {
            if(file == null || file.Length == 0) return null;

            // 1. Validamos la extensión del archivo
            ValidateImageExtension(file.FileName);
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            //carperta: wwwroot/{ContainerFolder}/yyyy/MM
            var now = DateTime.UtcNow;
            //Ruta del archivo 
            var folderPath = validarFolder(ContainerFolder, now);

            // Nombre único para el archivo
            var filename = $"{Guid.NewGuid()}{fileExtension}";
            var fullPath = Path.Combine(folderPath, filename);

            using (var stream =new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var relativePath=Path.Combine("/", ContainerFolder, now.ToString("yyyy"),now.ToString("MM"),filename).Replace("\\", "/");
            return relativePath;
        }

        //editar imagenes
        public async Task<string?> EditImageAsync(IFormFile file, string ContainerFolder, string existingImagePath)
        {
            // 1. Validamos la extensión del archivo
            ValidateImageExtension(file.FileName);

            // 2. Obtenemos la ruta del archivo existente
            var existingFilePath = Path.Combine(_env.WebRootPath, existingImagePath.TrimStart('/'));
            if (File.Exists(existingFilePath))
            {
                File.Delete(existingFilePath); // Eliminamos el archivo existente
            }
            // 3. Creamos la nueva carpeta si no existe
            var now = DateTime.UtcNow;
            var folderPath = validarFolder(ContainerFolder, now);
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            // 4. Generamos un nombre único para el nuevo archivo
            var filename = $"{Guid.NewGuid()}{fileExtension}";
            var fullPath = Path.Combine(folderPath, filename);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            // 5. Retornamos la ruta relativa del nuevo archivo
            var relativePath = Path.Combine("/", ContainerFolder, now.ToString("yyyy"), now.ToString("MM"), filename).Replace("\\", "/");
            return relativePath;
        }


        //metodo para validar extensiones de imagenes
        private void ValidateImageExtension(string fileName)
        {
            var ext= Path.GetExtension(fileName).ToLowerInvariant();

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            if (string.IsNullOrEmpty(ext) || !allowedExtensions.Contains(ext))
            {
                throw new NotSupportedException($"La extensión {ext} no está permitida. Las extensiones permitidas son: {string.Join(", ", allowedExtensions)}");
            }
        }

        private string validarFolder(string containerFolder, DateTime now)
        {
            var folder=Path.Combine(_env.WebRootPath, containerFolder, now.ToString("yyyy"), now.ToString("MM"));
            Directory.CreateDirectory(folder); // Aseguramos que la carpeta exista
            return folder;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM2E2GRUPO2
{
    class Utils
    {
        public async Task<string> SaveVideoAsync(Byte[] video)
        {
            try
            {
                string appDataDirectory = FileSystem.AppDataDirectory;
                string filePath = Path.Combine(appDataDirectory, "video.mp4");
                await File.WriteAllBytesAsync(filePath, video);
                Console.WriteLine($"Video guardado en: {filePath}");
                return filePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar video {ex.Message}");
                return null;
            }
        }
    }
}

using System;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace Biobanks.Services
{
    // General helpers for working with images
    public static class ImageService
    {
        public static async Task<MemoryStream> ResizeImageStream(Stream inStream, int maxX, int maxY)
        {
            if (inStream.Length < 1) throw new ArgumentException("The provided Input Stream contains no data");
            
            using var image = await Image.LoadAsync(inStream);
            image.Mutate(x => x.Resize(maxX, maxY));
            
            var outStream = new MemoryStream();
            await image.SaveAsync(outStream, new PngEncoder());
            outStream.Seek(0, SeekOrigin.Begin);

            return outStream;
        }
    }
}

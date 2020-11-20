using System;
using System.IO;

namespace Directory.Services
{
    // General helpers for working with images
    public static class ImageService
    {
        public static MemoryStream ResizeImageStream(Stream inStream, int maxX, int maxY)
        {
            if (inStream.Length < 1) throw new ArgumentException("The provided Input Stream contains no data");

            var outStream = new MemoryStream();

            var job = new ImageResizer.ImageJob(
                inStream,
                outStream,
                new ImageResizer.Instructions($"width={maxX};height={maxY};mode=max"));

            job.Build();

            return outStream;
        }

    }
}

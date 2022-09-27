using System;
using System.IO;
using Imageflow.Fluent;

namespace Biobanks.Services
{
    // General helpers for working with images
    public static class ImageService
    {
        public static  MemoryStream ResizeImageStream(Stream inStream, int maxX, int maxY)
        {
            if (inStream.Length < 1) throw new ArgumentException("The provided Input Stream contains no data");

            var outStream = new MemoryStream();

            using (var b = new ImageJob())
            {
                var job =  b.BuildCommandString(
                    (IBytesSource)inStream,
                    (IOutputDestination)outStream,
                    $"width={maxX};height={maxY};mode=max")
                                .Finish();

            }

            return outStream;
        }
    }
}

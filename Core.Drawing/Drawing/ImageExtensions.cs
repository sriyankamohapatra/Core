using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Sfa.Core.Drawing
{
    /// <summary>
    /// Adds functionality to the image class.
    /// </summary>
    public static class ImageExtensions
    {
        /// <summary>
        /// Gets the total bytes from an image
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this Image image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, ImageFormat.Jpeg);
                return memoryStream.ToArray();
            }
        }
    }
}
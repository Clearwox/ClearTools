using Clear;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Clear.Tools
{
    public static class ImageUtility
    {
        public static byte[] ConvertBitmapToBytes(Bitmap bitmap, ImageFormat format)
        {
            using var stream = new MemoryStream();
            bitmap.Save(stream, format);
            return stream.ToArray();
        }

        public static Image ScaleImage(Image image, int maxWidth, int maxHeight, ImageSizePreference sizePreference = ImageSizePreference.None)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            double ratio = sizePreference switch
            {
                ImageSizePreference.Height => ratioY,
                ImageSizePreference.Width => ratioX,
                _ => Math.Min(ratioX, ratioY),
            };

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            using (var graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }

        public static Image CropImage(Bitmap source, int maxWidth, int maxHeight)
        {
            var bmp = new Bitmap(maxWidth, maxHeight);

            using (var graphics = Graphics.FromImage(bmp))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;

                graphics.DrawImage(source, 0, 0, new Rectangle(0, 0, maxWidth, maxHeight), GraphicsUnit.Pixel);
            }

            return bmp;
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destinationRectangle = new Rectangle(0, 0, width, height);
            var destinationImage = new Bitmap(width, height);

            destinationImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destinationImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphics.DrawImage(image, destinationRectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
            }

            return destinationImage;
        }

        private static (ImageCodecInfo CodecInfo, EncoderParameters Parameters) GetJpegCodecEncoder(int quality)
        {
            if (quality < 0 || quality > 100)
            {
                string error = string.Format("Jpeg image quality must be between 0 and 100, with 100 being the highest quality.  A value of {0} was specified.", quality);
                throw new ArgumentOutOfRangeException(error);
            }

            EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, quality);
            ImageCodecInfo jpegCodec = ImageCodecInfo.GetImageEncoders().First(x => x.MimeType?.ToLower() == "image/jpeg");

            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;

            return (jpegCodec, encoderParams);
        }

        public static void SaveJpegToFile(string path, Image image, int quality)
        {
            var (jpegCodec, encoderParams) = GetJpegCodecEncoder(quality); 
            image.Save(path, jpegCodec, encoderParams);
        }

        public static void SaveJpegToStream(MemoryStream stream, Image image, int quality)
        {
            var (jpegCodec, encoderParams) = GetJpegCodecEncoder(quality); 
            image.Save(stream, jpegCodec, encoderParams);
        }

        public static string ConvertImageToBase64(Image image, ImageFormat format)
        {
            using var ms = new MemoryStream();
            image.Save(ms, format);
            byte[] imageBytes = ms.ToArray();
            return Convert.ToBase64String(imageBytes);
        }

        public static Image ConvertBase64ToImage(string base64)
        {
            byte[] imageBytes = Convert.FromBase64String(base64);
            using var ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            return Image.FromStream(ms, true);
        }
    }
}
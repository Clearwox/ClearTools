using Clear.Tools;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Xunit;

namespace ClearTools.Tests
{
    public class ImageUtilityTests
    {
        [Fact]
        public void ConvertBitmapToBytes_ShouldReturnByteArray()
        {
            using var bitmap = new Bitmap(100, 100);
            var result = ImageUtility.ConvertBitmapToBytes(bitmap, ImageFormat.Png);
            Assert.NotNull(result);
            Assert.IsType<byte[]>(result);
        }

        [Fact]
        public void ScaleImage_ShouldReturnScaledImage()
        {
            using var image = new Bitmap(200, 200);
            var result = ImageUtility.ScaleImage(image, 100, 100);
            Assert.NotNull(result);
            Assert.Equal(100, result.Width);
            Assert.Equal(100, result.Height);
        }

        [Fact]
        public void CropImage_ShouldReturnCroppedImage()
        {
            using var bitmap = new Bitmap(200, 200);
            var result = ImageUtility.CropImage(bitmap, 100, 100);
            Assert.NotNull(result);
            Assert.Equal(100, result.Width);
            Assert.Equal(100, result.Height);
        }

        [Fact]
        public void ResizeImage_ShouldReturnResizedImage()
        {
            using var image = new Bitmap(200, 200);
            var result = ImageUtility.ResizeImage(image, 100, 100);
            Assert.NotNull(result);
            Assert.Equal(100, result.Width);
            Assert.Equal(100, result.Height);
        }

        [Fact]
        public void SaveJpegToFile_ShouldSaveFile()
        {
            using var image = new Bitmap(100, 100);
            var path = Path.Combine(Path.GetTempPath(), "test.jpg");
            ImageUtility.SaveJpegToFile(path, image, 90);
            Assert.True(File.Exists(path));
            File.Delete(path);
        }

        [Fact]
        public void SaveJpegToStream_ShouldSaveToStream()
        {
            using var image = new Bitmap(100, 100);
            using var stream = new MemoryStream();
            ImageUtility.SaveJpegToStream(stream, image, 90);
            Assert.True(stream.Length > 0);
        }

        [Fact]
        public void ConvertImageToBase64_ShouldReturnBase64String()
        {
            using var image = new Bitmap(100, 100);
            var result = ImageUtility.ConvertImageToBase64(image, ImageFormat.Png);
            Assert.NotNull(result);
            Assert.IsType<string>(result);
        }

        [Fact]
        public void ConvertBase64ToImage_ShouldReturnImage()
        {
            using var image = new Bitmap(100, 100);
            var base64 = ImageUtility.ConvertImageToBase64(image, ImageFormat.Png);
            var result = ImageUtility.ConvertBase64ToImage(base64);
            Assert.NotNull(result);
            Assert.IsType<Bitmap>(result);
        }
    }
}

using QRCoder;
using System.Drawing.Imaging;
using System.Drawing;

namespace ZKLT25.API.Helper
{
    public static class QrCodeHelper
    {
        // 生成带有文本的二维码
        public static Bitmap GenerateQrCodeWithText(string qrCodeContent, string overlayText, int qrCodeSize = 300, int totalHeight = 350)
        {
            // 生成二维码
            var qrCodeBitmap = GenerateQrCode(qrCodeContent, 600);

            // 添加文本
            var finalBitmap = AddTextToImage(qrCodeBitmap, overlayText, qrCodeSize, totalHeight);

            return finalBitmap;
        }

        // 生成二维码
        private static Bitmap GenerateQrCode(string content, int desiredSize)
        {
            // 创建二维码数据
            var qrGenerator = new QRCoder.QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(content, QRCoder.QRCodeGenerator.ECCLevel.Q);

            // 计算每个模块的像素大小（忽略安静区）
            int modulesCount = qrCodeData.ModuleMatrix.Count;
            int pixelsPerModule = desiredSize / modulesCount;

            // 生成二维码
            var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeBytes = qrCode.GetGraphic(
                pixelsPerModule,
                Color.Black,  // 注意：使用 ARGB 值
                Color.White,
                false
            );

            // 转换为 Bitmap
            using var ms = new MemoryStream(qrCodeBytes);
            return new Bitmap(ms);
        }

        // 在图片上添加文本
        private static Bitmap AddTextToImage(Bitmap originalImage, string text, int width, int totalHeight)
        {
            // 创建一个新的画布，高度增加以容纳文本
            var bitmapWithText = new Bitmap(width, totalHeight);

            using (var g = Graphics.FromImage(bitmapWithText))
            {
                // 设置高质量渲染
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                // 绘制白色背景
                g.Clear(Color.White);

                // 绘制二维码（居中）
                g.DrawImage(originalImage, 5, 5, 500, 500);

                // 设置文本格式
                using var font = new Font("Microsoft YaHei", 30, FontStyle.Bold);
                using var brush = new SolidBrush(Color.Black);

                // 计算文本位置（垂直居中）
                var textSize = g.MeasureString(text, font);
                float textX = (width - textSize.Width) / 2;
                float textY = 509;  // 文本在二维码下方

                // 绘制文本
                g.DrawString(text, font, brush, textX, textY);
            }

            // 释放原始图像资源
            originalImage.Dispose();

            return bitmapWithText;
        }


        /// <summary>
        /// 将二维码保存为文件
        /// </summary>
        public static void SaveQrCodeToFile(string content, string text, string filePath, ImageFormat format = null)
        {
            format ??= ImageFormat.Png;
            using var bitmap = GenerateQrCodeWithText(content, text, 510, 560);
            bitmap.Save(filePath, format);
        }
    }
}

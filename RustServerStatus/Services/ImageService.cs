using RustServerStatus.Models;
using SkiaSharp;

namespace RustServerStatus.Services;

public class ImageService : IImageService
{
    private const int IMAGE_WIDTH = 640;
    private const int IMAGE_HEIGHT = 160;
    private const string IMAGE_BACKGROUND = "#1b1b1b";

    public byte[] GenerateServerInfoImage(ServerInfo serverInfo)
    {
        var bitmap = new SKBitmap(IMAGE_WIDTH, IMAGE_HEIGHT);

        using (var canvas = new SKCanvas(bitmap))
        {
            canvas.Clear(SKColor.Parse(IMAGE_BACKGROUND));
            canvas.DrawLine(0, 0, 0, 320, new SKPaint
            {
                Color = SKColors.LimeGreen,
                StrokeWidth = 5
            });
            DrawServerInformation(serverInfo, 25, 27, canvas);
        }

        using (var image = SKImage.FromBitmap(bitmap))
        {
            return image.Encode(SKEncodedImageFormat.Png, 100).ToArray();
        }
    }

    private void DrawServerInformation(ServerInfo serverInfo, int startX, int startY, SKCanvas canvas)
    {
        var normalTextPaint = new SKPaint
        {
            Color = SKColors.White,
            StrokeWidth = 5,
            IsAntialias = true,
            TextSize = 16,
            Typeface = SKTypeface.FromFamilyName("Segoe UI")
        };
        const int offsetY = 24 + 5;
        canvas.DrawText($"Address: {serverInfo.Address}", startX, startY, normalTextPaint);
        canvas.DrawText($"Name: {serverInfo.Name}", startX, startY + offsetY * 1, normalTextPaint);
        canvas.DrawText($"Game: {serverInfo.Game}", startX, startY + offsetY * 2, normalTextPaint);
        canvas.DrawText($"Map: {serverInfo.Map}", startX, startY + offsetY * 3, normalTextPaint);
        canvas.DrawText($"Players: {serverInfo.PlayerCount}/{serverInfo.PlayerCapacity} (Bots: {serverInfo.BotCount})",
            startX, startY + offsetY * 4, normalTextPaint);
    }
}
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text;
using CommonLib.Requests;
using System.Drawing.Imaging;

class Program
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    private static void Main(string[] args)
    {
        Console.WriteLine("Fetching all running processes...");
        var processes = Process.GetProcesses();

        var l2m = processes.First(p => p.ProcessName == "Lineage2M");

        Bitmap? previousScreenshot = null;

        while (true)
        {
            var currentScreenshot = TakeScreenshot(l2m);
            //SaveScreenshot(currentScreenshot);
            if (previousScreenshot != null && currentScreenshot != null)
            {
                double similarity = CompareScreenshots(previousScreenshot, currentScreenshot);
                Console.WriteLine($"Screenshot similarity: {similarity * 100:F2}%");

                if (similarity >= 0.95)
                {
                    Console.WriteLine("Screenshots are 95% or more similar.");
                }
                else
                {
                    var imageBytes = BitmapToByteArray(currentScreenshot);
                    _ = ProcessImage(imageBytes, 0, "Central Standard Time");

                }
            }

            previousScreenshot = currentScreenshot;
            Thread.Sleep(60*1000); // Wait for 1 minute
        }
    }

    private static byte[] BitmapToByteArray(Bitmap bitmap)
    {
        using var memoryStream = new MemoryStream();
        bitmap.Save(memoryStream, ImageFormat.Png); // Save the bitmap in PNG format
        return memoryStream.ToArray(); // Convert the stream to a byte array
    }


    private static Bitmap? TakeScreenshot(Process process)
    {
        try
        {
            IntPtr hWnd = process.MainWindowHandle;
            if (hWnd == IntPtr.Zero)
            {
                Console.WriteLine("No main window found for the process.");
                return null;
            }

            if (GetWindowRect(hWnd, out RECT rect))
            {
                // Calculate the width and height of the window
                int width = rect.Right - rect.Left + 525;
                int height = rect.Bottom - rect.Top + 400;

                if (width <= 0 || height <= 0)
                {
                    Console.WriteLine("Invalid window dimensions.");
                    return null;
                }

                // Create a bitmap with the correct dimensions
                var bitmap = new Bitmap(width, height);
                using var graphics = Graphics.FromImage(bitmap);

                graphics.CopyFromScreen(
                    rect.Left + 525,
                    rect.Top + 200,
                    0,
                    0,
                    new Size(width, height),
                    CopyPixelOperation.SourceCopy
                );

                return bitmap;
            }
            else
            {
                Console.WriteLine("Failed to get window rectangle.");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error taking screenshot: {ex.Message}");
            return null;
        }
    }

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);

    private const int SM_CXSCREEN = 0; // Width of the primary screen
    private const int SM_CYSCREEN = 1; // Height of the primary screen

    private static Size ScreenResolution()
    {
        // Get the screen width and height using GetSystemMetrics
        var screenWidth = GetSystemMetrics(SM_CXSCREEN);
        var screenHeight = GetSystemMetrics(SM_CYSCREEN);

        return new Size(screenWidth, screenHeight);
    }
    private static double CompareScreenshots(Bitmap image1, Bitmap image2)
    {
        if (image1.Width != image2.Width || image1.Height != image2.Height)
        {
            Console.WriteLine("Images have different dimensions and cannot be compared.");
            return 0.0;
        }

        var width = image1.Width;
        var height = image1.Height;
        var totalPixels = width * height;
        var similarPixels = 0;

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var pixel1 = image1.GetPixel(x, y);
                var pixel2 = image2.GetPixel(x, y);

                if (AreColorsSimilar(pixel1, pixel2))
                {
                    similarPixels++;
                }
            }
        }

        return (double)similarPixels / totalPixels;
    }

    private static bool AreColorsSimilar(Color color1, Color color2, int tolerance = 10)
    {
        return Math.Abs(color1.R - color2.R) <= tolerance &&
               Math.Abs(color1.G - color2.G) <= tolerance &&
               Math.Abs(color1.B - color2.B) <= tolerance;
    }

    private static async Task<string> ProcessImage(byte[] image, ulong chatId, string timeZone)
    {
        try
        {
            // Prepare the request payload
            var requestData = new RequestData
            {
                Image = image,
                ChatId = chatId,
                TimeZone = timeZone
            };

            var jsonPayload = JsonSerializer.Serialize(requestData);

            using var httpClient = new HttpClient();
            var apiEndpoint = "http://localhost:7112/api/ParseImage";
            //var apiEndpoint = "";

            var response = await httpClient.PostAsync(apiEndpoint, new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);

            return responseContent;
        }
        catch (Exception ex)
        {
            return $"Error processing image: {ex.Message}";
        }
    }
}

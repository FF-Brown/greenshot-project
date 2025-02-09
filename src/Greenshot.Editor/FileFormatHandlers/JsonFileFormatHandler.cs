using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Greenshot.Base.Interfaces;
using Greenshot.Base.Interfaces.Plugin;
using log4net;

namespace Greenshot.Editor.FileFormatHandlers
{
    public class JsonFileFormatHandler : AbstractFileFormatHandler, IFileFormatHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(JsonFileFormatHandler));
        private readonly IReadOnlyCollection<string> _ourExtensions = new [] { ".json" };

        public JsonFileFormatHandler()
        {
            SupportedExtensions[FileFormatHandlerActions.LoadDrawableFromStream] = _ourExtensions;
            SupportedExtensions[FileFormatHandlerActions.LoadFromStream] = _ourExtensions;
            SupportedExtensions[FileFormatHandlerActions.SaveToStream] = _ourExtensions;
        }

        public override bool TrySaveToStream(
            Bitmap bitmap,
            Stream stream,
            string extension,
            ISurface surface = null,
            SurfaceOutputSettings surfaceOutputSettings = null)
        {
            bool didSaveToStream = false;

            if (surface != null)
            {
                try
                {
                    bitmap.Save(stream, ImageFormat.Png);
                    string base64Image = ConvertImageToBase64(bitmap);

                    string version = GetVersion();

                    OutputData output = new(version, base64Image, surface);

                    JsonSerializerOptions options = new();
                    options.WriteIndented = true;
                    options.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                    string outString = JsonSerializer.Serialize(output, options);

                    didSaveToStream = true;
                }
                catch (Exception ex)
                {
                    Log.Error("Couldn't save surface as .json: ", ex);
                }
            }

            return didSaveToStream;
        }

        public override bool TryLoadFromStream(Stream stream, string extension, out Bitmap bitmap)
        {
            bitmap = null;
            return false;
        }

        private string ConvertImageToBase64(Bitmap image)
        {
            return "Image placeholder";
        }

        private string GetVersion()
        {
            Version v = Assembly.GetExecutingAssembly().GetName().Version;

            return $"Greenshot{v.Major:00}.{v.Minor:00}";
        }

        private class OutputData
        {
            public string Version { get; }

            public string Image { get; }

            public ISurface Surface { get; }

            public OutputData(
                string version,
                string image,
                ISurface surface)
            {
                Version = version;
                Image = image;
                Surface = surface;
            }
        }
    }
}

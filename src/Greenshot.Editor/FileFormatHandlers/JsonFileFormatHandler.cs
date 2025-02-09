using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Greenshot.Base.Interfaces;
using Greenshot.Base.Interfaces.Plugin;
using log4net;

namespace Greenshot.Editor.FileFormatHandlers
{
    public class JsonFileFormatHandler : AbstractFileFormatHandler, IFileFormatHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(JsonFileFormatHandler));
        private readonly IReadOnlyCollection<string> _ourExtensions = new[] { ".json" };

        public JsonFileFormatHandler()
        {
            SupportedExtensions[FileFormatHandlerActions.LoadDrawableFromStream] = _ourExtensions;
            SupportedExtensions[FileFormatHandlerActions.LoadFromStream] = _ourExtensions;
            SupportedExtensions[FileFormatHandlerActions.SaveToStream] = _ourExtensions;
        }

        /// <inheritdoc />
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
                    string base64Image = ConvertImageToBase64(bitmap);

                    Version version = Assembly.GetExecutingAssembly().GetName().Version;

                    OutputData output = new(version, base64Image, surface);

                    JsonSerializerOptions options = new()
                    {
                        WriteIndented = true,
                        ReferenceHandler = ReferenceHandler.Preserve,
                    };

                    JsonSerializer.Serialize(stream, output, options);

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

        private string ConvertImageToBase64(Image image)
        {
            using MemoryStream ms = new();
            image.Save(ms, ImageFormat.Png);
            return Convert.ToBase64String(ms.ToArray());
        }

        private class OutputData
        {
            public Version Version { get; }

            public string Image { get; }

            public ISurface Surface { get; }

            public OutputData(
                Version version,
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Greenshot.Base.Core;
using Greenshot.Base.Interfaces;
using Greenshot.Base.Interfaces.Drawing;
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


                    JsonSerializerOptions options = new()
                    {
                        WriteIndented = true,
                        ReferenceHandler = ReferenceHandler.Preserve,
                    };

                    JsonSerializer.Serialize(stream, exportData, options);

                    didSaveToStream = true;
                }
                catch (Exception ex)
                {
                    Log.Error($"Couldn't save surface as {extension}: ", ex);
                }
            }

            return didSaveToStream;
        }

        public override bool TryLoadFromStream(Stream stream, string extension, out Bitmap bitmap)
        {
            bool didLoad = false;
            ExportSurface outputData = null;
            bitmap = null;
            File.AppendAllText(
                @"C:\Users\Nathan\Downloads\greenshotTest.log",
                $"\n~~~~ Begin import\n");

            try
            {
                outputData = JsonSerializer.Deserialize<ExportSurface>(stream);
            }
            catch (Exception ex)
            {
                File.AppendAllText(
                    @"C:\Users\Nathan\Downloads\greenshotTest.log",
                    $"Failed deserializing: {ex.Message}\n");
                File.AppendAllText(
                    @"C:\Users\Nathan\Downloads\greenshotTest.log",
                    $"{ex.InnerException.Message}\n");
            }

            bitmap = null;
            if (outputData != null)
            {
                try
                {
                    var surface = LoadSurface(outputData);
                    bitmap = (Bitmap)surface.GetImageForExport();
                    didLoad = true;
                }
                catch (Exception ex)
                {
                    Log.Error("Couldn't load .greenshot: ", ex);
                    File.AppendAllText(
                        @"C:\Users\Nathan\Downloads\greenshotTest.log",
                        $"Couldn't save surface as .json: {ex.Message}\n");
                }
            }

            return didLoad;
        }

        private string ConvertImageToBase64(Image image)
        {
            using MemoryStream ms = new();
            image.Save(ms, ImageFormat.Png);
            return Convert.ToBase64String(ms.ToArray());
        }

        private Bitmap ConvertBase64ToBitmap(string base64Image)
        {
            byte[] imageBytes = Convert.FromBase64String(base64Image);
            using MemoryStream ms = new(imageBytes);
            return new Bitmap(ms);
        }

        private ISurface LoadSurface(ExportSurface exportData)
        {
            ISurface returnSurface = SimpleServiceProvider.Current.GetInstance<Func<ISurface>>().Invoke();

            Bitmap capture = ConvertBase64ToBitmap(exportData.Image);

            returnSurface.Image = capture;
            returnSurface.Elements.Clear();
            //foreach (var element in exportData.Elements)
            //{
            //    returnSurface.Elements.Add(element);
            //}
            returnSurface.CounterStart = exportData.CounterStart;
            returnSurface.ZoomFactor = exportData.ZoomFactor;
            //returnSurface.FieldAggregator = exportData.FieldAggregator;
            //returnSurface.CurrentDpi = exportData.CurrentDpi;

            return returnSurface;
        }

        public class ExportSurface
        {
            public Version Version { get; set; }

            public string Image { get; set; }

            //public IDrawableContainerList Elements { get; set; }

            public int CounterStart { get; set; }

            public Fraction ZoomFactor { get; set; }

            public IFieldAggregator FieldAggregator { get; set; }

            public int CurrentDpi { get; set; }

            public ExportSurface()
            {

            }

            public ExportSurface(
                Version version,
                string base64Image,
                ISurface surface)
            {
                Version = version;
                Image = base64Image;

                //Elements = surface.Elements;
                CounterStart = surface.CounterStart;
                ZoomFactor = surface.ZoomFactor;
                FieldAggregator = surface.FieldAggregator;
                CurrentDpi = surface.CurrentDpi;
            }

            public Image GetImage()
            {
                byte[] imageBytes = Convert.FromBase64String(Image);
                using MemoryStream ms = new(imageBytes);
                return new Bitmap(ms);
            }
        }
    }
}

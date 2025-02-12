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
using Greenshot.Editor.Drawing;
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
            File.AppendAllText(
                @"C:\Users\Nathan\Downloads\greenshotTest.log",
                $"\n~~~~ Begin export\n");

            bool didSaveToStream = false;

            if (surface != null)
            {
                try
                {
                    string base64Image = ConvertImageToBase64(bitmap);

                    Version version = Assembly.GetExecutingAssembly().GetName().Version;

                    ExportSurface exportData = new(version, base64Image, surface);

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
                    File.AppendAllText(
                        @"C:\Users\Nathan\Downloads\greenshotTest.log",
                        $"Couldn't save surface as .json: {ex.Message}\n");
                }
            }

            File.AppendAllText(
                @"C:\Users\Nathan\Downloads\greenshotTest.log",
                $"Did Save: {didSaveToStream}\n");
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

            JsonSerializerOptions options = new()
            {
                Converters = { new DrawableContainerConverter() },
            };

            try
            {
                outputData = JsonSerializer.Deserialize<ExportSurface>(stream, options);
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
            foreach (var element in exportData.Elements)
            {
                returnSurface.Elements.Add(element);
            }
            returnSurface.CounterStart = exportData.CounterStart;
            //returnSurface.ZoomFactor = exportData.ZoomFactor;
            //returnSurface.FieldAggregator = exportData.FieldAggregator;
            //returnSurface.CurrentDpi = exportData.CurrentDpi;

            return returnSurface;
        }

        private class ExportSurface
        {
            public Version Version { get; set; }

            public string Image { get; set; }

            public IDrawableContainer[] Elements { get; set; }

            public int CounterStart { get; set; }

            //public Fraction ZoomFactor { get; set; }

            //public IFieldAggregator FieldAggregator { get; set; }

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

                Elements = (surface.Elements as DrawableContainerList).AsIDrawableContainerList().ToArray();
                CounterStart = surface.CounterStart;
                //ZoomFactor = surface.ZoomFactor;
                //FieldAggregator = surface.FieldAggregator;
                CurrentDpi = surface.CurrentDpi;
            }

            public Image GetImage()
            {
                byte[] imageBytes = Convert.FromBase64String(Image);
                using MemoryStream ms = new(imageBytes);
                return new Bitmap(ms);
            }
        }

        private class DrawableContainerConverter : JsonConverter<IDrawableContainer>
        {
            public override IDrawableContainer Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                File.AppendAllText(
                    @"C:\Users\Nathan\Downloads\greenshotTest.log",
                    $"Converting IDrawableContainer\n");

                IDrawableContainer container = null;

                using JsonDocument doc = JsonDocument.ParseValue(ref reader);
                var root = doc.RootElement;
                if (!root.TryGetProperty("TypeName", out JsonElement typeElement))
                {
                    throw new JsonException("Missing TypeName property");
                }

                string type = typeElement.GetString();

                List<Type> types = new()
                {
                    typeof(ArrowContainer),
                    typeof(CursorContainer),
                };

                //foreach (var targetType in types)
                //{
                //    if (type == targetType.AssemblyQualifiedName)
                //    {
                //        container == JsonSerializer.Deserialize<typeof(targetType)>(root.GetRawText(), options);
                //    }
                //}

                try
                {
                    container = DeserializeContainer(type, root.GetRawText(), options);
                }
                catch (Exception ex)
                {
                    File.AppendAllText(
                        @"C:\Users\Nathan\Downloads\greenshotTest.log",
                        $"Failed on type: {type}\n");
                    File.AppendAllText(
                        @"C:\Users\Nathan\Downloads\greenshotTest.log",
                        $"Couldn't import .json: {ex.Message}\n");
                }

                return container;
            }

            private IDrawableContainer DeserializeContainer(string type, string jsonText, JsonSerializerOptions options)
            {
                IDrawableContainer container = null;

                if (type == typeof(ArrowContainer).AssemblyQualifiedName)
                {
                    container = JsonSerializer.Deserialize<ArrowContainer>(jsonText, options);
                }
                else if (type == typeof(CropContainer).AssemblyQualifiedName)
                {
                    container = JsonSerializer.Deserialize<CropContainer>(jsonText, options);
                }
                else if (type == typeof(CursorContainer).AssemblyQualifiedName)
                {
                    container = JsonSerializer.Deserialize<CursorContainer>(jsonText, options);
                }
                else if (type == typeof(EllipseContainer).AssemblyQualifiedName)
                {
                    container = JsonSerializer.Deserialize<EllipseContainer>(jsonText, options);
                }
                else if (type == typeof(FilterContainer).AssemblyQualifiedName)
                {
                    container = JsonSerializer.Deserialize<FilterContainer>(jsonText, options);
                }
                else if (type == typeof(FreehandContainer).AssemblyQualifiedName)
                {
                    container = JsonSerializer.Deserialize<FreehandContainer>(jsonText, options);
                }
                else if (type == typeof(HighlightContainer).AssemblyQualifiedName)
                {
                    container = JsonSerializer.Deserialize<HighlightContainer>(jsonText, options);
                }
                else if (type == typeof(IconContainer).AssemblyQualifiedName)
                {
                    container = JsonSerializer.Deserialize<IconContainer>(jsonText, options);
                }
                else if (type == typeof(ImageContainer).AssemblyQualifiedName)
                {
                    container = JsonSerializer.Deserialize<ImageContainer>(jsonText, options);
                }
                else if (type == typeof(LineContainer).AssemblyQualifiedName)
                {
                    container = JsonSerializer.Deserialize<LineContainer>(jsonText, options);
                }
                else if (type == typeof(MetafileContainer).AssemblyQualifiedName)
                {
                    container = JsonSerializer.Deserialize<MetafileContainer>(jsonText, options);
                }
                else if (type == typeof(ObfuscateContainer).AssemblyQualifiedName)
                {
                    container = JsonSerializer.Deserialize<ObfuscateContainer>(jsonText, options);
                }
                else if (type == typeof(RectangleContainer).AssemblyQualifiedName)
                {
                    container = JsonSerializer.Deserialize<RectangleContainer>(jsonText, options);
                }
                else if (type == typeof(SpeechbubbleContainer).AssemblyQualifiedName)
                {
                    container = JsonSerializer.Deserialize<SpeechbubbleContainer>(jsonText, options);
                }
                else if (type == typeof(StepLabelContainer).AssemblyQualifiedName)
                {
                    container = JsonSerializer.Deserialize<StepLabelContainer>(jsonText, options);
                }
                else if (type == typeof(SvgContainer).AssemblyQualifiedName)
                {
                    container = JsonSerializer.Deserialize<SvgContainer>(jsonText, options);
                }
                else if (type == typeof(TextContainer).AssemblyQualifiedName)
                {
                    container = JsonSerializer.Deserialize<TextContainer>(jsonText, options);
                }
                else if (type == typeof(VectorGraphicsContainer).AssemblyQualifiedName)
                {
                    container = JsonSerializer.Deserialize<VectorGraphicsContainer>(jsonText, options);
                }
                else
                {
                    throw new JsonException($"Unrecognized type '{type}'");
                }

                return container;
            }

            public override void Write(
                Utf8JsonWriter writer,
                IDrawableContainer value,
                JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }
    }
}

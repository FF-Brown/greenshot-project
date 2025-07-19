using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using Dapplo.Windows.Common.Structs;
using Greenshot.Base.Core;
using Greenshot.Base.Interfaces;
using Greenshot.Editor.Drawing.Fields;
using Greenshot.Editor.Drawing.Filters.AreaFilters;

namespace Greenshot.Editor.Drawing.Filters.InverseAreaFilters
{
    internal class AreaBrightnessFilter : InverseAreaFilter
    {
        public AreaBrightnessFilter()
        {
            AddField(GetType(), FieldType.BRIGHTNESS, 0.9d);
        }

        public override void Apply(
            Graphics graphics,
            Bitmap applyBitmap,
            IEnumerable<DrawableContainer> containers,
            ISurface parent)
        {
            IEnumerable<DrawableContainer> blurExclusionContainers = containers.Where(c => c.Filters.Any(f => f is BrightnessExclusionArea));
            if (applyBitmap != null && blurExclusionContainers.Any())
            {
                Apply(graphics, applyBitmap, blurExclusionContainers.Select(c => c.Bounds));
            }
        }

        /// <summary>
        /// Implements the Apply code for the Brightness Filet
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="applyBitmap"></param>
        private void Apply(Graphics graphics, Bitmap applyBitmap, IEnumerable<NativeRect> areasToExcludeFromFilters)
        {
            NativeRect applyRect = new(0, 0, applyBitmap.Width, applyBitmap.Height);

            if (applyRect.Width == 0 || applyRect.Height == 0)
            {
                // nothing to do
                return;
            }

            GraphicsState state = graphics.Save();

            graphics.SetClip(applyRect);
            foreach (NativeRect area in areasToExcludeFromFilters)
            {
                graphics.ExcludeClip(area);
            }

            float brightness = GetFieldValueAsFloat(FieldType.BRIGHTNESS);
            using (ImageAttributes ia = ImageHelper.CreateAdjustAttributes(brightness, 1f, 1f))
            {
                graphics.DrawImage(applyBitmap, applyRect, applyRect.X, applyRect.Y, applyRect.Width, applyRect.Height, GraphicsUnit.Pixel, ia);
            }

            graphics.Restore(state);
        }
    }
}

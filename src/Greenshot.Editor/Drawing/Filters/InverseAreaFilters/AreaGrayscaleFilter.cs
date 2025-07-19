using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using Dapplo.Windows.Common.Structs;
using Greenshot.Base.Interfaces;
using Greenshot.Editor.Drawing.Filters.AreaFilters;

namespace Greenshot.Editor.Drawing.Filters.InverseAreaFilters
{
    internal class AreaGrayscaleFilter : InverseAreaFilter
    {
        public override void Apply(
            Graphics graphics,
            Bitmap applyBitmap,
            IEnumerable<DrawableContainer> containers,
            ISurface parent)
        {
            IEnumerable<DrawableContainer> blurExclusionContainers = containers.Where(c => c.Filters.Any(f => f is GrayscaleExclusionArea));
            if (applyBitmap != null && blurExclusionContainers.Any())
            {
                Apply(graphics, applyBitmap, blurExclusionContainers.Select(c => c.Bounds));
            }
        }

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

            ColorMatrix grayscaleMatrix = new ColorMatrix(new[]
            {
                new[]
                {
                    .3f, .3f, .3f, 0, 0
                },
                new[]
                {
                    .59f, .59f, .59f, 0, 0
                },
                new[]
                {
                    .11f, .11f, .11f, 0, 0
                },
                new float[]
                {
                    0, 0, 0, 1, 0
                },
                new float[]
                {
                    0, 0, 0, 0, 1
                }
            });
            using (ImageAttributes ia = new ImageAttributes())
            {
                ia.SetColorMatrix(grayscaleMatrix);
                graphics.DrawImage(applyBitmap, applyRect, applyRect.X, applyRect.Y, applyRect.Width, applyRect.Height, GraphicsUnit.Pixel, ia);
            }

            graphics.Restore(state);
        }
    }
}

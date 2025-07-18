using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Dapplo.Windows.Common.Structs;
using Dapplo.Windows.Gdi32;
using Greenshot.Base.Core;
using Greenshot.Editor.Drawing.Fields;
using Greenshot.Editor.Drawing.Filters.AreaFilters;

namespace Greenshot.Editor.Drawing.Filters.InverseAreaFilters
{
    internal class AreaBlurFilter : InverseAreaFilter
    {
        public AreaBlurFilter()
        {
            AddField(GetType(), FieldType.BLUR_RADIUS, 3);
        }

        public override void Apply(Graphics graphics, Bitmap applyBitmap, IEnumerable<DrawableContainer> containers)
        {
            IEnumerable<DrawableContainer> blurExclusionContainers = containers.Where(c => c.Filters.Any(f => f is BlurExclusionArea));
            if (applyBitmap != null && blurExclusionContainers.Any())
            {
                Apply(graphics, applyBitmap, blurExclusionContainers.Select(c => c.Bounds));
            }
        }

        private void Apply(Graphics graphics, Bitmap applyBitmap, IEnumerable<NativeRect> areasToExcludeFromFilters)
        {
            int blurRadius = GetFieldValueAsInt(FieldType.BLUR_RADIUS);
            NativeRect applyRect = new(0, 0, applyBitmap.Width, applyBitmap.Height);
            if (applyRect.Width == 0 || applyRect.Height == 0)
            {
                return;
            }

            GraphicsState state = graphics.Save();
            graphics.SetClip(applyRect);
            foreach (NativeRect area in areasToExcludeFromFilters)
            {
                graphics.ExcludeClip(area);
            }

            if (GdiPlusApi.IsBlurPossible(blurRadius))
            {
                GdiPlusApi.DrawWithBlur(graphics, applyBitmap, applyRect, null, null, blurRadius, false);
            }
            else
            {
                using IFastBitmap fastBitmap = FastBitmap.CreateCloneOf(applyBitmap, applyRect);
                ImageHelper.ApplyBoxBlur(fastBitmap, blurRadius);
                fastBitmap.DrawTo(graphics, applyRect);
            }

            graphics.Restore(state);
        }

    }
}

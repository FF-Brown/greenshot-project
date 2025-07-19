using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Dapplo.Windows.Common.Structs;
using Dapplo.Windows.Gdi32;
using Greenshot.Base.Core;
using Greenshot.Base.Interfaces;
using Greenshot.Editor.Drawing.Filters.AreaFilters;

namespace Greenshot.Editor.Drawing.Filters.InverseAreaFilters
{
    internal class AreaBlurFilter : InverseAreaFilter
    {
        private int _blurRadius;

        public override void Apply(
            Graphics graphics,
            Bitmap applyBitmap,
            IEnumerable<DrawableContainer> containers,
            ISurface parent)
        {
            _blurRadius = parent.BlurRadius;

            IEnumerable<DrawableContainer> blurExclusionContainers = containers.Where(c => c.Filters.Any(f => f is BlurExclusionArea));
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
                return;
            }

            GraphicsState state = graphics.Save();
            graphics.SetClip(applyRect);
            foreach (NativeRect area in areasToExcludeFromFilters)
            {
                graphics.ExcludeClip(area);
            }

            if (GdiPlusApi.IsBlurPossible(_blurRadius))
            {
                GdiPlusApi.DrawWithBlur(graphics, applyBitmap, applyRect, null, null, _blurRadius, false);
            }
            else
            {
                using IFastBitmap fastBitmap = FastBitmap.CreateCloneOf(applyBitmap, applyRect);
                ImageHelper.ApplyBoxBlur(fastBitmap, _blurRadius);
                fastBitmap.DrawTo(graphics, applyRect);
            }

            graphics.Restore(state);
        }

    }
}

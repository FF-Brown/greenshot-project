using System.Drawing;
using Dapplo.Windows.Common.Structs;
using Greenshot.Base.Interfaces.Drawing;

namespace Greenshot.Editor.Drawing.Filters.AreaFilters
{
    internal class GrayscaleExclusionArea : AbstractFilter
    {
        public GrayscaleExclusionArea(DrawableContainer parent) : base(parent)
        {
        }

        public override void Apply(Graphics graphics, Bitmap applyBitmap, NativeRect rect, RenderMode renderMode)
        {
        }
    }
}

using System.Drawing;
using Dapplo.Windows.Common.Structs;
using Greenshot.Base.Interfaces.Drawing;
using Greenshot.Editor.Drawing.Fields;

namespace Greenshot.Editor.Drawing.Filters.AreaFilters
{
    internal class BrightnessExclusionArea : AbstractFilter
    {
        public BrightnessExclusionArea(DrawableContainer parent) : base(parent)
        {
            AddField(GetType(), FieldType.BRIGHTNESS, 0.9d);
        }

        public override void Apply(Graphics graphics, Bitmap applyBitmap, NativeRect rect, RenderMode renderMode)
        {
        }
    }
}

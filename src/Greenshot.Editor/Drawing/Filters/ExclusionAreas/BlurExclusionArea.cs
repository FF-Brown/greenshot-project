using System.Drawing;
using Dapplo.Windows.Common.Structs;
using Greenshot.Base.Interfaces.Drawing;
using Greenshot.Editor.Drawing.Fields;

namespace Greenshot.Editor.Drawing.Filters.AreaFilters
{
    internal class BlurExclusionArea : AbstractFilter
    {
        public BlurExclusionArea(DrawableContainer parent) : base(parent)
        {
            AddField(GetType(), FieldType.FLAGS, FieldFlag.BLUR);
        }

        public override void Apply(Graphics graphics, Bitmap applyBitmap, NativeRect rect, RenderMode renderMode)
        {
        }
    }
}

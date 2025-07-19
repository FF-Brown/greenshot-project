using System.Collections.Generic;
using System.Drawing;
using Greenshot.Base.Interfaces;
using Greenshot.Editor.Drawing.Fields;

namespace Greenshot.Editor.Drawing.Filters.InverseAreaFilters
{
    internal abstract class InverseAreaFilter : AbstractFieldHolder
    {
        public abstract void Apply(
            Graphics graphics,
            Bitmap applyBitmap,
            IEnumerable<DrawableContainer> containers,
            ISurface parent);
    }
}
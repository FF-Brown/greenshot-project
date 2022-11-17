using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Dapplo.Windows.Common.Extensions;
using Dapplo.Windows.Common.Structs;
using Greenshot.Base.Interfaces;
using Greenshot.Base.Interfaces.Drawing;

namespace Greenshot.Editor.Drawing
{
    public class RightClickIcon : AbstractClickIcon
    {
        public RightClickIcon(ISurface parent) : base(parent)
        {
            lineColor = Color.Black;
            fillColor = Color.Black;
            lineThickness = 0;
            shadow = false;
            Init();
        }

        /// <summary>
        /// Do some logic to make sure all fields are initiated correctly
        /// </summary>
        /// <param name="streamingContext">StreamingContext</param>
        protected override void OnDeserialized(StreamingContext streamingContext)
        {
            base.OnDeserialized(streamingContext);
            Init();
        }

        private void Init()
        {

            CreateDefaultAdorners();

        }

        protected override void InitializeFields()
        {
        }

        public override void Draw(Graphics graphics, RenderMode rm)
        {
            var rect = new NativeRect(Left, Top, Width, Height).Normalize();

            DrawRectangle(rect, graphics, rm, lineThickness, lineColor, fillColor, shadow);
        }

        public override bool ClickableAt(int x, int y)
        {
            var rect = new NativeRect(Left, Top, Width, Height).Normalize();

            return RectangleClickableAt(rect, lineThickness, fillColor, x, y);
        }
    }
}


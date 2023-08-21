/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2021 Thomas Braun, Jens Klingen, Robin Krom
 *
 * For more information see: https://getgreenshot.org/
 * The Greenshot project is hosted on GitHub https://github.com/greenshot/greenshot
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Drawing;
using System.Runtime.Serialization;
using Dapplo.Windows.Common.Extensions;
using Dapplo.Windows.Common.Structs;
using Greenshot.Base.Interfaces;
using Greenshot.Base.Interfaces.Drawing;

namespace Greenshot.Editor.Drawing
{
    /// <summary>
    /// Represents a rectangular shape on the Surface
    /// </summary>
    [Serializable]
    public class RedactionContainer : RectangleContainer
    {
        private readonly Color lineColor = Color.Black;
        private readonly Color fillColor = Color.Black;
        private readonly int lineThickness = 0;
        private readonly bool shadow = false;

        public RedactionContainer(ISurface parent) : base(parent)
        {
            Init();
        }

        /// <summary>
        /// Do some logic to make sure all field are initiated correctly
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
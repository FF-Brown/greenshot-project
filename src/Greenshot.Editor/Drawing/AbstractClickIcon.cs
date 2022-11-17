using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Media.Media3D;
using Dapplo.Windows.Common.Extensions;
using Dapplo.Windows.Common.Structs;
using Greenshot.Base.Interfaces;
using Greenshot.Base.Interfaces.Drawing;

namespace Greenshot.Editor.Drawing;

public abstract class AbstractClickIcon : RectangleContainer
{
    protected Color lineColor;
    protected Color fillColor;
    protected int lineThickness;
    protected bool shadow;

    protected AbstractClickIcon(ISurface parent) : base(parent)
    {
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

    protected void Init()
    {
        CreateDefaultAdorners();
    }

    protected abstract void InitializeFields();

    public abstract void Draw(Graphics graphics, RenderMode rm);


    public abstract bool ClickableAt(int x, int y);

}
    

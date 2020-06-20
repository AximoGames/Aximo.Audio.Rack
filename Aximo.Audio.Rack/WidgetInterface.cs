using System;
using System.Collections.Generic;
using System.Text;
using OpenToolkit.Mathematics;

namespace Aximo.Engine.Audio
{
    public interface IWidgetInterface
    {
        ImageContext RegisterCanvas(Vector2 size);
        Vector2 ModuleSize { get; }
    }
}

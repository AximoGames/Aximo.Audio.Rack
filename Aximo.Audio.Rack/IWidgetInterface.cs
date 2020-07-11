// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
        void SetParameterClickedHandler(Action<AudioParameter> callback);
        IApplicationInterface Application { get; }
    }
}

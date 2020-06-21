// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Aximo.Audio.Rack.JsonModel;
//using System.Media;

namespace Aximo.Engine.Audio
{

    public class AudioModuleDescriptor
    {
        public Type ModuleType { get; internal set; }
        public string Name { get; internal set; }
        public AudioModule CreateInstance() => (AudioModule)Activator.CreateInstance(ModuleType);

        internal AudioModuleDescriptor(Type moduleType)
        {
            ModuleType = moduleType;
            Name = CreateInstance().Name;
        }

    }
}

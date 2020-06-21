// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Aximo.Audio.Rack.JsonModel;

namespace Aximo.Engine.Audio
{

    public class AudioModuleManager
    {
        private static AudioModuleManager _Current;
        public static AudioModuleManager Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = new AudioModuleManager();
                    _Current.RegisterModules(typeof(AudioModuleManager).Assembly);
                }

                return _Current;
            }
        }

        public void RegisterModules(Assembly assembly)
        {
            foreach (var t in assembly.GetTypes())
            {
                if (!t.IsSubclassOf(typeof(AudioModule)))
                    continue;

                if (t.IsAbstract)
                    continue;

                if (t.GetConstructor(Array.Empty<Type>()) == null)
                    continue;

                var descr = new AudioModuleDescriptor(t);
                if (Indexed.TryGetValue(descr.Name, out var oldDescr))
                {
                    Indexed.Remove(descr.Name);
                    All.Remove(oldDescr);
                }

                All.Add(descr);
                Indexed.Add(descr.Name, descr);
            }
        }

        public ICollection<AudioModuleDescriptor> All { get; private set; } = new List<AudioModuleDescriptor>();

        private Dictionary<string, AudioModuleDescriptor> Indexed = new Dictionary<string, AudioModuleDescriptor>();

        public AudioModuleDescriptor Find(string moduleName)
        {
            return Indexed.GetValueOrDefault(moduleName);
        }

        public AudioModule CreateInstance(string moduleName)
        {
            return Find(moduleName)?.CreateInstance();
        }
    }
}

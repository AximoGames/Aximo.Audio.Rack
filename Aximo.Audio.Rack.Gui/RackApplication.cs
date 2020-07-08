// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aximo.Engine;
using Aximo.Engine.Audio;
using Aximo.Engine.Audio.Modules;
using Aximo.Engine.Components.Geometry;
using Aximo.Engine.Components.Lights;
using Aximo.Engine.Components.UI;
using Aximo.Engine.Windows;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;
using SixLabors.ImageSharp;

namespace Aximo.Audio.Rack.Gui
{
    public class RackApplication : Application, IApplicationInterface
    {
        public static Matrix4 BoardTranslationMatrix = Matrix4.CreateScale(1, -1, 1);
        public static readonly string Filename = "c:/users/sebastian/Downloads/275416__georcduboe__ambient-naturept2-squarepurity-2-135bpm.wav";

        public AudioMainRack MainRack;
        public AudioRack SelectedRack;

        public RackApplication()
        {
            CommandLineManager.Current.OnExecuteConsoleCommandLine += ExecuteConsoleCommandLineDelegate;
        }

        private void ExecuteConsoleCommandLineDelegate(ExecuteConsoleCommandLineArgs e)
        {
            if (e.CommandLine == "s")
            {
                MainRack.SaveToFile("/tmp/rack.json");
                e.Handled = true;
            }

            if (e.CommandLine.StartsWith("module add --list"))
            {
                Console.WriteLine("Available Modules:");
                Console.WriteLine();
                foreach (var modDef in AudioModuleManager.Current.All)
                    Console.WriteLine(modDef.Name);
                Console.WriteLine();
                e.Handled = true;
            }
            else if (e.CommandLine.StartsWith("module add "))
            {
                var modName = e.CommandLine.Substring("module add ".Length);
                var modDef = AudioModuleManager.Current.Find(modName);
                if (modDef == null)
                {
                    Console.WriteLine($"Module {modName} not found");
                    return;
                }

                var mod = modDef.CreateInstance();
                MainRack.Dispatch(() =>
                {
                    SelectedRack.AddModule(mod);
                    DispatchUpdater(() =>
                    {
                        AddModule(mod);
                        Console.WriteLine($"Module {modDef.Name} added");
                    });
                });
            }

            if (e.CommandLine.StartsWith("module list"))
            {
                Console.WriteLine("Installed Modules:");
                Console.WriteLine();
                for (var i = 0; i < SelectedRack.Modules.Length; i++)
                {
                    var mod = SelectedRack.Modules[i];
                    Console.WriteLine($"#{i} {mod.Name}");
                }
                Console.WriteLine();
                e.Handled = true;
            }

            if (e.CommandLine.StartsWith("module del"))
            {
                var index = int.Parse(e.CommandLine.Substring("module del".Length));
                var mod = SelectedRack.Modules[index];
                MainRack.Dispatch(() =>
                {
                    SelectedRack.RemoveModule(mod);
                    DispatchUpdater(() =>
                    {
                        RemoveModule(mod);
                        Console.WriteLine($"Module {mod.Name} removed");
                    });
                });
                e.Handled = true;
            }
        }

        private UIFloatingContainer ModuleContainer;

        protected override void SetupScene()
        {
            var camSize = new Vector2(9 * RenderContext.ScreenAspectRatio, 9);

            RenderContext.Camera = new OrthographicCamera(new Vector3(4.5f + ((camSize.X - camSize.Y) / 2f) - 0.5f, -4.5f + 0.5f, 25))
            {
                Size = camSize,
                NearPlane = 1f,
                FarPlane = 100.0f,
                Facing = (float)Math.PI / 2,
                Pitch = -((float)(Math.PI / 2) - 0.00001f),
            };

            SceneContext.AddActor(new Actor(new GridPlaneComponent(10, true)
            {
                Name = "GridPlaneXY",
                RelativeTranslation = new Vector3(0f, 0f, 0.01f),
            }));
            SceneContext.AddActor(new Actor(new CrossLineComponent(10, true)
            {
                Name = "CenterCross",
                RelativeTranslation = new Vector3(0f, 0f, 0.02f),
                RelativeScale = new Vector3(2.0f),
            }));

            ModuleContainer = new UIFloatingContainer()
            {
            };
            SceneContext.AddActor(new Actor(ModuleContainer));

            SceneContext.AddActor(new Actor(new StatsComponent()
            {
                Name = "Stats",
                CustomOrder = 10,
                FontSize = 1,
                Size = new Vector2(10, 10),
            })); ;

            SceneContext.AddActor(new Actor(new DirectionalLightComponent()
            {
                RelativeTranslation = new Vector3(2f, 0.5f, 3.25f),
                Name = "StaticLight",
            }));

            SceneContext.AddActor(new Actor(new CubeComponent()
            {
                Name = "GroundCursor",
                RelativeTranslation = new Vector3(0, 1, 0.05f),
                RelativeScale = new Vector3(1.0f, 1.0f, 0.1f),
            }));

            //SceneContext.AddActor(new Actor(new DebugCubeComponent()
            //{
            //    Name = "Box1",
            //    RelativeRotation = new Vector3(0, 0, 0.5f).ToQuaternion(),
            //    RelativeScale = new Vector3(1),
            //    RelativeTranslation = new Vector3(0, 0, 0.5f),
            //}));

            AudioModuleManager.Current.RegisterModules(typeof(AudioPCMSourceModule).Assembly);
            MainRack = new AudioMainRack();

            //var inMod = new AudioPCMSourceModule();
            //inMod.SetInput(AudioStream.Load(Filename));
            //inMod.OnEndOfStream += () =>
            //{
            //    inMod.Play();
            //};

            //var inMod2 = new AudioPCMSourceModule();
            //inMod2.SetInput(AudioStream.Load(Filename));
            //inMod2.OnEndOfStream += () =>
            //{
            //    inMod2.Play();
            //};

            //var ampMod = new AudioAmplifierModule();
            //var envMod = new AudioADSRModule();
            //var vcoMod = new AudioVCOModule();
            //var vcoMod2 = new AudioVCOModule();
            //var scopeMod = new AudioScopeModule();

            //var outMod = new AudioPCMOpenALSinkModule();
            ////var outFile = new AudioSinkStream();

            //var mixMod = new AudioMix4Module();
            //mixMod.GetParameter("Volume1").SetValue(0.5f);

            //Rack.AddModule(inMod);
            //Rack.AddModule(inMod2);
            //Rack.AddModule(outMod);
            //Rack.AddModule(mixMod);
            //Rack.AddModule(vcoMod);
            //Rack.AddModule(vcoMod2);
            //Rack.AddModule(ampMod);
            //Rack.AddModule(envMod);
            //Rack.AddModule(scopeMod);

            //Rack.AddCable(inMod.GetOutput("Left"), mixMod.GetInput("Left1"));
            //Rack.AddCable(inMod.GetOutput("Right"), mixMod.GetInput("Right1"));
            //Rack.AddCable(inMod2.GetOutput("Left"), mixMod.GetInput("Left2"));
            //Rack.AddCable(inMod2.GetOutput("Right"), mixMod.GetInput("Right2"));
            //Rack.AddCable(mixMod.GetOutput("Left"), outMod.GetInput("Left"));
            //Rack.AddCable(mixMod.GetOutput("Right"), outMod.GetInput("Right"));
            //--
            //rack.AddCable(inMod.GetOutput("Gate"), outMod.GetInput("Gate"));

            //Rack.SaveToFile("/tmp/test.json");
            if (File.Exists("/tmp/rack.json"))
                MainRack.AppendFromFile("/tmp/rack.json");

            MainRack.StartThread();
            //inMod.Play();

            CablesComponent cablesComponent;
            SceneContext.AddActor(new Actor(cablesComponent = new CablesComponent()));

            SwitchRack(MainRack);

            //UISlider debugSlider;
            //FlowContainer.AddComponent(debugSlider = new UISlider()
            //{
            //    Margin = new UIAnchors(),
            //    Padding = new UIAnchors(),
            //    BorderRadius = 0,
            //    Location = new Vector2(0, 50),
            //    Size = new Vector2(50, 4),
            //});

            //var line = new LineComponent(new Vector3(2.5f, 2.5f, 0), new Vector3(7.5f, 7.5f, 0));

            //line.Material = MaterialManager.CreateDefaultScreenLineMaterial();
            //line.Transform = TransformUtil.TransformScreenUnitsToScreenSpace(AudioUnits);

            //SceneContext.AddActor(new Actor(line));

            //cablesComponent.SetCables(MainRack.Cables);
        }

        private List<ModuleComponent> ModuleComponents = new List<ModuleComponent>();
        private void AddModule(AudioModule module)
        {
            ModuleComponent comp;
            ModuleContainer.AddComponent(comp = new ModuleComponent(module, 20)
            {
                Location = new Vector2(CurrentX, CurrentY),
            });
            CurrentX += 20;
            if (CurrentX >= 100)
            {
                CurrentX = 0;
                CurrentY += ModuleComponent.ModuleHeight;
            }
            ModuleComponents.Add(comp);
        }

        private void RemoveModule(AudioModule module)
        {
            var comp = ModuleComponents.FirstOrDefault(comp => comp.Module == module);
            if (comp == null)
                return;

            comp.Detach();
            comp.Dispose();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (CurrentMouseWorldPositionIsValid)
            {
                var cursor = SceneContext.GetActor("GroundCursor")?.RootComponent;
                if (cursor != null)
                    cursor.RelativeTranslation = new Vector3(CurrentMouseWorldPosition.X, CurrentMouseWorldPosition.Y, cursor.RelativeTranslation.Z);
            }
        }

        public static Vector2 AudioUnits = new Vector2(100, 80);

        public override Vector2 GetScreenPixelScale()
        {
            return Vector2.Divide(AudioUnits, ScreenPixelSize.ToVector2());
        }

        internal void PortMouseDown(PortComponent uiport)
        {
            SelectedPort = uiport;
            var cable = uiport.Port.Cables.LastOrDefault();
            SelectedRack.RemoveCable(cable);
        }

        protected override void OnPostMouseUp(MouseButtonArgs e)
        {
            SelectedPort = null;
        }

        private PortComponent SelectedPort;
        internal void PortMouseUp(PortComponent uiport)
        {
            if (SelectedPort != null && SelectedPort != uiport)
            {
                SelectedRack.TryAddCable(SelectedPort.Port, uiport.Port);
                SelectedPort = null;
            }
            SelectedPort = null;
        }

        private float CurrentX = 0;
        private float CurrentY = 0;
        public void SwitchRack(AudioRack rack)
        {
            SelectedRack = rack;
            DispatchUpdater(() =>
            {
                CurrentX = 0;
                CurrentY = 0;
                ModuleContainer.RemoveComponents();

                foreach (var module in rack.Modules)
                    AddModule(module);

                ModuleContainer.UpdateFrame(); // Calculate Port Positions

                SceneContext.Current.Find<CablesComponent>().First().SetCables(rack.Cables);
            });
        }
    }
}

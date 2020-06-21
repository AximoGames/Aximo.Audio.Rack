// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Threading;
using OpenToolkit.Audio.OpenAL;

namespace Aximo.Engine.Audio.Modules
{

    public class AudioPCMOpenALSinkModule : AudioModule
    {
        private static Serilog.ILogger Log = Aximo.Log.ForContext<AudioPCMOpenALSinkModule>();

        /// <summary>
        /// Buffer size in miliseconds
        /// </summary>
        public const int DefaultBufferSizeMiliseconds = 60;
        public const int DefaultBufferCount = 8;
        public const int DefaultFrequency = 44100;
        public const int Bits = 16;
        public const int Channels = 2;

        private int BufferSizeMiliseconds;
        private int BufferCount;
        private int BufferSize;
        private int Frequency;

        public AudioPCMOpenALSinkModule()
        {
            Name = "PCM Sink";
            ConfigureInput(0, "Left");
            ConfigureInput(1, "Right");
            ConfigureInput(2, "Gate");
            InputChannels = new Port[] { Inputs[0], Inputs[1] };

            Init();
        }

        private int[] BufferHandles;
        private int BufferHandleIndex = 0;
        private int SourceHandle;

        private void CheckLibrary()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                var url = "https://github.com/AximoGames/Aximo.redist.OpenAL/raw/a0cdf016869c634c054f416989a1fdedf26d16fe/lib/libopenal.so.1.20.1";
                var fileName = "libopenal.so.1";
                var binPath = Path.Combine(AssetManager.BinDir, fileName);
                Log.Info($"Cannot find {fileName}. Downloading {url}");
                if (!File.Exists(binPath))
                    new WebClient().DownloadFile(url, binPath);
            }
        }

        private void Init()
        {
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

            BufferSizeMiliseconds = DefaultBufferSizeMiliseconds;
            BufferCount = DefaultBufferCount;

            Frequency = DefaultFrequency;
            BufferSize = (int)(((Frequency / 1000f) * BufferSizeMiliseconds) / BufferCount) * Channels;
            BufferHandles = new int[BufferCount];

            Log.Info("Audio Delay: {delay}, Buffers: {buffers}, Frames per Buffer: {samples}", BufferSizeMiliseconds, BufferCount, BufferSize / 2);

            Data1 = new short[BufferSize];
            Data2 = new short[BufferSize];
            Data = Data1;

            CheckLibrary();

            var devices = ALC.GetStringList(GetEnumerationStringList.DeviceSpecifier);
            Console.WriteLine($"Devices: {string.Join(", ", devices)}");

            var device = ALC.OpenDevice(devices.First());
            var con = ALC.CreateContext(device, (int[])null);
            ALC.MakeContextCurrent(con);
            CheckALError();

            Format = GetSoundFormat(Channels, Bits);

            for (var i = 0; i < BufferCount; i++)
                BufferHandles[i] = AL.GenBuffer();

            SourceHandle = AL.GenSource();
            int state;
        }

        private void Dispose()
        {
            AL.SourceStop(SourceHandle);
            AL.DeleteSource(SourceHandle);
            for (var i = 0; i < BufferCount; i++)
                AL.DeleteBuffer(BufferHandles[i]);
        }

        private Port[] InputChannels;

        private short[] Data1;
        private short[] Data2;
        private short[] Data;
        private long DataPosition;

        public long BuffersProcessed = 0;
        private ALFormat Format;

        public static ALFormat GetSoundFormat(int channels, int bits)
        {
            switch (channels)
            {
                case 1: return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2: return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default: throw new NotSupportedException("The specified sound format is not supported.");
            }
        }

        public static void CheckALError()
        {
            ALError error = AL.GetError();
            if (error != ALError.NoError)
            {
                var msg = $"ALError: {AL.GetErrorString(error)}";
                throw new Exception(msg);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public override void Process(AudioProcessArgs e)
        {
            EnsureBuffer();

            if (!Inputs[2].IsConnected || Inputs[2].GetVoltage() >= 0.9f)
                for (var i = 0; i < InputChannels.Length; i++)
                    WritePCMSample(PCMConversion.FloatToShort(InputChannels[i].GetVoltage() / 5f));
            else
                for (var i = 0; i < InputChannels.Length; i++)
                    WritePCMSample(0);
        }

        private void WritePCMSample(short sample)
        {
            Data[DataPosition] = sample;
            DataPosition += 1;
        }

        private void EnsureBuffer()
        {
            if (DataPosition >= BufferSize)
            {
                PresentBuffer(Data);

                if (Data == Data1)
                    Data = Data2;
                else
                    Data = Data1;

                DataPosition = 0;
            }
        }

        private void PresentBuffer(short[] buf)
        {
            var len = buf.Length * Channels;

            int processed;
            int queued;

            while (true)
            {
                AL.GetSource(SourceHandle, ALGetSourcei.BuffersProcessed, out processed);
                AL.GetSource(SourceHandle, ALGetSourcei.BuffersQueued, out queued);

                if (queued < BufferCount || (processed > 0 && queued == BufferCount))
                    break;

                Thread.Sleep((BufferSizeMiliseconds / BufferCount) - 1);
            }

            if (queued == BufferCount && processed == BufferCount)
                Log.Error("AUDIO BUFFER UNDERRUN");

            int nextBufferHandle = 0;
            if (queued >= BufferCount)
                AL.SourceUnqueueBuffers(SourceHandle, 1, ref nextBufferHandle);
            else
                nextBufferHandle = BufferHandles[BufferHandleIndex++];

            CheckALError();
            AL.BufferData(nextBufferHandle, Format, buf, len, Frequency);
            CheckALError();
            AL.SourceQueueBuffers(SourceHandle, 1, ref nextBufferHandle);
            CheckALError();

            int state;
            AL.GetSource(SourceHandle, ALGetSourcei.SourceState, out state);
            if ((ALSourceState)state != ALSourceState.Playing)
                AL.SourcePlay(SourceHandle);

            CheckALError();
            BuffersProcessed++;
        }
    }
}

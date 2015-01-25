using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public interface ISoundManager
    {
        void LoadSound(SoundResource s);
        void PlaySound(SoundResource s);
        void PlaySound(SoundResource s, float volume);
        bool IsSoundPlaying(SoundResource s);
    }

    public class SoundManager
    {
        public static void LoadSound(SoundResource s) { Core.EngineBase.Current.SoundManager.LoadSound(s); }
        public static void PlaySound(SoundResource s) { Core.EngineBase.Current.SoundManager.PlaySound(s); }

        public static bool IsSoundPlaying(SoundResource s) { return Core.EngineBase.Current.SoundManager.IsSoundPlaying(s); }
    }

    public class SoundResource : GameResource 
    {

        public float Volume { get; private set; }

        public SoundResource(string name) : base(new GamePath { Type = PathType.Sounds, Name = name }) { this.Volume = 1f; }

        public SoundResource(string name, float volume) : base(new GamePath { Type = PathType.Sounds, Name = name }) { this.Volume = volume; }

        public SoundResource() : base(GamePath.Undefined) { }
    }
}

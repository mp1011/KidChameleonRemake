using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace Engine.XNA
{
    class XNASound
    {
        public SoundEffect Effect { get; set; }
        public SoundEffectInstance EffectInstance { get; set; }
        public DateTime LastPlayTime { get; set; }
    }

    class XNASoundManager : ISoundManager 
    {
        private Dictionary<string, XNASound> mSoundLibrary = new Dictionary<string, XNASound>();

        public void PlaySound(SoundResource s)
        {
            PlaySound(s, s.Volume);
        }

        public void PlaySound(SoundResource s, float volume)
        {
            if (s == null)
                return;

            var sound = mSoundLibrary.TryGet(s.Path.Name, null);
            if (sound == null)
                return;

            sound.LastPlayTime = DateTime.Now;

            if (sound.EffectInstance == null)
                sound.EffectInstance = sound.Effect.CreateInstance();

            sound.EffectInstance.Volume = volume;
            sound.EffectInstance.Play();
        }


        public void LoadSound(SoundResource s)
        {
            try
            {
                using (var stream = s.OpenFileStream())
                {
                    var sound = SoundEffect.FromStream(stream);
                    mSoundLibrary.AddOrSet(s.Path.Name, new XNASound { Effect = sound });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public bool IsSoundPlaying(SoundResource s)
        {
            var sound = mSoundLibrary.TryGet(s.Path.Name, null);
            return sound != null && IsSoundPlaying(sound);        
        }

        public void StopSound(SoundResource s)
        {
            var sound = mSoundLibrary.TryGet(s.Path.Name, null);
            if (sound != null && sound.EffectInstance != null)
                sound.EffectInstance.Stop();
        }

        private bool IsSoundPlaying(XNASound sound)
        {
            return (DateTime.Now - sound.LastPlayTime) < sound.Effect.Duration;
        }
    }
}

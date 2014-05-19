using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;

namespace ProjectZero.SoundSystem
{
    public partial class SoundRenderer
    {
        private List<SoundSystemHandle> _contents = new List<SoundSystemHandle>();
        private Dictionary<string, SoundHandle> _sounds = new Dictionary<string, SoundHandle>(StringComparer.OrdinalIgnoreCase);

        public SoundHandle RegisterSound(string fileName)
        {
            Debug.Assert(Path.GetExtension(fileName) != string.Empty, "fix xnb");

            // file names is releative to root content directory.
            string path = Path.Combine(ContentManager.RootDirectory, fileName);
            SoundHandle soundEffect;

            if (_sounds.TryGetValue(path, out soundEffect))
            {
                return soundEffect;
            }
            soundEffect  = new SoundEffectHandle(path);
            _contents.Add(soundEffect);
            _sounds.Add(path, soundEffect);

            return soundEffect;
        }

        public void LoadContent()
        {
            foreach (var t in _contents)
            {
                t.Load();
            }
        }

        public void UnloadContent()
        {
            foreach (var t in _contents)
            {
                t.Unload();
            }                  
        }

        private class SoundEffectHandle : SoundHandle
        {
            private readonly string _fileName;
            private SoundEffect _soundEffect;

            private static readonly Func<SoundEffect, int> _sizeGetter;
            private static readonly Func<SoundEffect, float> _rateGetter;
            private static readonly Func<SoundEffect, int> _formatGetter;
            // obs!!!   may need to add to lookup tabels below if more formats are needed.
            // OpenTK.Audio.OpenAL.ALFormat -> number of channels
            private static readonly Dictionary<int, int> _formatToChannels = new Dictionary<int, int>()
            {
                { 4352, 1 },        // Mono8
                { 4353, 1 },        // Mono16
                { 4354, 2 },        // Stero8
                { 4355, 2 }         // Stero16
            };
            // OpenTK.Audio.OpenAL.ALFormat -> bits per sample
            private static readonly Dictionary<int, int> _formatToBits = new Dictionary<int, int>()
            {
                { 4352, 8 },        // Mono8
                { 4353, 8 },        // Mono16
                { 4354, 16 },       // Stero8
                { 4355, 16 }        // Stero16
            };

            static SoundEffectHandle()
            {
                var type = typeof(SoundEffect);
                var sizePropertyInfo = type.GetProperty("Size", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                _sizeGetter = sizePropertyInfo != null ? (Func<SoundEffect, int>)Delegate.CreateDelegate(typeof(Func<SoundEffect, int>), sizePropertyInfo.GetMethod) : _ => 0;
                var ratePropertyInfo = type.GetProperty("Rate", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                _rateGetter = sizePropertyInfo != null ? (Func<SoundEffect, float>)Delegate.CreateDelegate(typeof(Func<SoundEffect, float>), ratePropertyInfo.GetMethod) : _ => 1;
                var formatPropertyInfo = type.GetProperty("Format", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                _formatGetter = formatPropertyInfo != null ? (Func<SoundEffect, int>)Delegate.CreateDelegate(typeof(Func<SoundEffect, int>), formatPropertyInfo.GetMethod) : _ => 0;
            }

            public SoundEffectHandle(string fileName)
            {
                _fileName = fileName;
            }

            public override SoundEffect Sound
            {
                get
                {
                    return _soundEffect;
                }
            }

            private TimeSpan _duration;

            public override TimeSpan Duration
            {
                get
                {
                    return _duration;
                }
            }

            public override void Load()
            {
                try
                {
                    using (var s = File.OpenRead(_fileName))
                    {
                        _soundEffect = SoundEffect.FromStream(s);
                        if (_soundEffect.Duration.TotalMilliseconds <= 0)
                        {
                            CalculateDuration();
                        }
                        else
                        {
                            _duration = _soundEffect.Duration;
                        }
                    }
                }
                catch (Exception)
                {
                    // TODO:    should use default sound here.
                }

            }

            private void CalculateDuration()
            {
                // workaround, this is not implemented in mono games.
                int format = _formatGetter(_soundEffect);
                _duration = new TimeSpan(0, 0, 0, 0,
                    (int)(_sizeGetter(_soundEffect) / (_rateGetter(_soundEffect) * _formatToChannels[format] * _formatToBits[format] / 8f)
                          * 1000f)
                    );
            }

            public override void Unload()
            {
                _soundEffect.Dispose();
                _soundEffect = null;
            }
        }
    }
}

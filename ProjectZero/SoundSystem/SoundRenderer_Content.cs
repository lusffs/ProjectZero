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

        public SoundHandle RegisterSound(string fileName)
        {
            Debug.Assert(Path.GetExtension(fileName) != string.Empty, "fix xnb");

            // file names is releative to root content directory.
            string path = Path.Combine(ContentManager.RootDirectory, fileName);
            SoundEffectHandle soundEffect = new SoundEffectHandle(path);
            _contents.Add(soundEffect);

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

            public override void Load()
            {
                try
                {
                    using (var s = File.OpenRead(_fileName))
                    {
                        _soundEffect = SoundEffect.FromStream(s);
                    }
                }
                catch (Exception)
                {
                    // TODO:    should use default texture here.
                }

            }

            public override void Unload()
            {
                _soundEffect.Dispose();
                _soundEffect = null;
            }
        }
    }
}

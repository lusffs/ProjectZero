using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectZero.SoundSystem
{
    public partial class SoundRenderer
    {
        private List<SoundHandle> _contents = new List<SoundHandle>();

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

            _contents.Clear();            
        }
    }
}

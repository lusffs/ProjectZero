using Microsoft.Xna.Framework;
using ProjectZero.RenderSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectZero.SoundSystem;

namespace ProjectZero.Framework
{
    public enum AnimationDirection
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }
     
    public class Animation
    {
        private TextureHandle _textureHandle;
        private readonly Renderer _renderer;
        private readonly SoundRenderer _soundRenderer;
        private readonly string _fileName;
        private int _tileSize;
        private int _numberOfFramesInAnimation;
        private bool _playing;
        private AnimationDirection _direction;
        private Dictionary<int, SoundHandle> _sounds = new Dictionary<int, SoundHandle>();

        // 
        public Animation(Renderer renderer, SoundRenderer soundRenderer, string fileName)
        {
            _renderer = renderer;
            _fileName = fileName + ".cfg";
            _soundRenderer = soundRenderer;
        }

        public void RegisterContent()
        {
            Load();
        }

        public void ContentLoaded()
        {
            _numberOfFramesInAnimation = _textureHandle.Width / _tileSize;
        }


        private int _currentFrame = 0;

        private SoundHandle _playingSound = null;

        public void Update(Vector2 position, GameTime gameTime, Layer layer)
        {
            if (_playing)
            {
                _currentFrame = (int)(gameTime.TotalGameTime.TotalMilliseconds / 1000f * 12f) % _numberOfFramesInAnimation;
                SoundHandle sound;
                if (_sounds.TryGetValue(_currentFrame, out sound))
                {
                    if (sound != _playingSound)
                    {
                        _soundRenderer.PlaySound(sound);
                        _playingSound = sound;
                    }
                }
                else
                {
                    _playingSound = null;
                }
            }
            else
            {
                _currentFrame = 0;
            }
            _renderer.DrawImage(_textureHandle, position, _tileSize, _tileSize,
                new Rectangle(_currentFrame * _tileSize, (int)_direction * _tileSize, _tileSize, _tileSize), layer);
        }        

        public void Play()
        {
            _playing = true;
        }

        public void Stop()
        {
            _playing = false;
        }

        public bool IsPlaying
        {
            get
            {
                return _playing;
            }
        }

        public int TileSize {  get { return _tileSize; } }

        public AnimationDirection Direction
        {
            get
            {
                return _direction;
            }

            set
            {
                _direction = value;
            }
        }

        private void Load()
        {
            using (var f = File.OpenText(Path.Combine(_renderer.ContentManager.RootDirectory, "animations/", _fileName)))
            {
                var tileSizeLine = f.ReadLine();
                var imageFileName = f.ReadLine();
                _tileSize = int.Parse(tileSizeLine);
                _textureHandle = _renderer.RegisterTexture2D(imageFileName);

                var line = f.ReadLine();
                while (line != null)
                {
                    string[] frameAndFileName = line.Split(' ');
                    int frame = int.Parse(frameAndFileName[0]);
                    _sounds.Add(frame, _soundRenderer.RegisterSound(frameAndFileName[1]));
                    line = f.ReadLine();
                }
            }
        }
    }
}

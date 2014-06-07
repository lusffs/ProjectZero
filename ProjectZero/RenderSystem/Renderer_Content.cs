using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectZero.RenderSystem
{
    public partial class Renderer
    {
        private List<RendererHandle> _contents = new List<RendererHandle>();
        private Dictionary<string, TextureHandle> _texture2d = new Dictionary<string, TextureHandle>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, FontHandle> _fonts = new Dictionary<string, FontHandle>(StringComparer.OrdinalIgnoreCase);
        
        /// <summary>
        /// 1x1 white pixel data.
        /// </summary>
        private Texture2D _whiteTexture;

        private void InitContent(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
            GraphicsDevice.DeviceLost += GraphicsDevice_DeviceLost;
            GraphicsDevice.DeviceReset += GraphicsDevice_DeviceReset;
        }

        private void GraphicsDevice_DeviceReset(object sender, EventArgs e)
        {
            LoadContent((GraphicsDevice)sender);
        }

        private void GraphicsDevice_DeviceLost(object sender, EventArgs e)
        {
            UnloadContent();
        }

        public TextureHandle RegisterTexture2D(string fileName)
        {
            Debug.Assert(Path.GetExtension(fileName) != string.Empty, "fix xnb");

            // file names is releative to root content directory.
            string path = Path.Combine(ContentManager.RootDirectory, fileName);
            TextureHandle texture;
            if (_texture2d.TryGetValue(path, out texture))
            {
                return texture;

            }
            texture = new Texture2DStream(path, _whiteTexture);
            _contents.Add(texture);
            _texture2d.Add(path, texture);

            return texture;
        }

        public FontHandle RegisterFont(string fileName)
        {
            Debug.Assert(Path.GetExtension(fileName) == string.Empty, "xnb file names should never have extension");

            FontHandle font;
            if (_fonts.TryGetValue(fileName, out font))
            {
                return font;
            }
            font = new FontSprite(fileName);
            _contents.Add(font);
            _fonts.Add(fileName, font);

            return font;
        }

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            InitContent(graphicsDevice);

            for (int i = (int)Layer.Map; i < (int)Layer.Last + 1; i++)
            {
                _layers[i] = new SpriteBatch(GraphicsDevice);
            }

            _whiteTexture = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _whiteTexture.SetData(new[] { Color.White });

            foreach (var t in _contents)
            {
                t.Load(GraphicsDevice, ContentManager);
            }
        }

        public void UnloadContent()
        {
            for (int i = (int)Layer.Map; i < (int)Layer.Last + 1; i++)
            {
                _layers[i].Dispose();
                _layers[i] = null;
            }


            _whiteTexture.Dispose();
            _whiteTexture = null;

            foreach (var t in _contents)
            {
                t.Unload();
            }

            GraphicsDevice.Dispose();
            GraphicsDevice = null;
        }

        private class Texture2DStream : TextureHandle
        {
            private readonly string _fileName;
            private Texture2D _texture = null;
            private int _width;
            private int _height;
            private readonly Texture2D _defaultTexture;

            public Texture2DStream(string fileName, Texture2D defaultTexture)
            {
                _fileName = fileName;
                _defaultTexture = defaultTexture;
            }

            public override Texture Texture
            {
                get
                {
                    return _texture;
                }
            }

            public override int Width
            {
                get
                {
                    return _width;
                }
            }

            public override int Height
            {
                get
                {
                    return _height;
                }
            }

            public override void Load(GraphicsDevice graphicsDevice, ContentManager contentManager)
            {
                try
                {
                    using (var s = File.OpenRead(_fileName))
                    {
                        _texture = Texture2D.FromStream(graphicsDevice, s);
                        _width = ((Texture2D)_texture).Width;
                        _height = ((Texture2D)_texture).Height;
                    }
                }
                catch (Exception)
                {
                    _texture = _defaultTexture;
                    _width = _defaultTexture.Width;
                    _height = _defaultTexture.Height;
                }
            }

            public override void Unload()
            {
                if (_defaultTexture == _texture)
                {
                    return;
                }

                _texture.Dispose();
                _texture = null;
            }
        }

        private class FontSprite : FontHandle
        {
            private readonly string _fileName;
            private SpriteFont _font;

            public FontSprite(string fileName)
            {
                _fileName = fileName;
            }


            public override SpriteFont Font
            {
                get
                {
                    return _font;
                }
            }

            public override void Load(GraphicsDevice graphicsDevice, ContentManager contentManager)
            {
                try
                {
                    _font = contentManager.Load<SpriteFont>(_fileName);
                }
                catch (Exception)
                {
                    // TODO:    should use default font here. 
                }
            }

            public override void Unload()
            {
                _font = null;
            }
        }
    }
}

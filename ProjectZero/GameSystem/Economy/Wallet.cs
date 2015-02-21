using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ProjectZero.GameSystem.Entities;
using ProjectZero.RenderSystem;

namespace ProjectZero.GameSystem.Economy
{
    public class Wallet
    {
        public class Transaction
        {
            public BaseTower Tower { get; private set; }

            public Transaction(BaseTower tower)
            {
                Tower = tower;
            }
        }

        private readonly Renderer _renderer;

        private FontHandle _font;

        public int Balance {get;private set;}
        
        public Wallet(Renderer renderer) 
        {
            Balance = 100;
            _renderer = renderer;
        }

        public Transaction Reservation(BaseTower tower)
        {
            if (Balance - tower.Price < 0)
            {
                return null;
            }

            return new Transaction(tower);
        }

        public void Purchase(Transaction transaction)
        {            
            Balance -= transaction.Tower.Price;            
        }
       
        public void Draw(GameTime gameTime)
        {
            string balanceString = string.Format("BALANCE {0}", Balance);
            float scale = 2.0f;
            var balanceSize = _font.Font.MeasureString(balanceString) * scale;
            balanceSize = _renderer.AdjustToVirtual(balanceSize);
            _renderer.DrawString(_font, balanceString, new Vector2(Renderer.ScreenWidth, Renderer.ScreenHeight) - balanceSize, Color.WhiteSmoke, Layer.Last, scale);
        }

        public void ConentLoaded()
        {
            
        }

        public void RegisterContent()
        {
            _font = _renderer.RegisterFont("fonts/console");
        }
    }
}

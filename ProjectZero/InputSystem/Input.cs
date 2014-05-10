using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ProjectZero.InputSystem
{
    public class Input
    {
        private KeyboardState _currentKeyboardState = new KeyboardState();
        private KeyboardState _oldKeyboardState = new KeyboardState();
        private MouseState _currentMouseState = new MouseState();
        private MouseState _oldMouseState = new MouseState();

        public event KeyEventHandler KeyEventHandler;

        public event MouseEventHandler MouseEventHandler;

        public void Frame(KeyboardState currentKeyboardState, MouseState currentMouseState)
        {
            _oldKeyboardState = _currentKeyboardState;
            _currentKeyboardState = currentKeyboardState;
            _oldMouseState = _currentMouseState;
            _currentMouseState = currentMouseState;

            ProcessKeyEvents();
            ProcessMouseEvents();
        }

        private void ProcessMouseEvents()
        {            
            ProcessMouseEvent(MouseButton.Left, _currentMouseState.LeftButton, _oldMouseState.LeftButton, _currentMouseState.X, _currentMouseState.Y);
        }

        private void ProcessMouseEvent(MouseButton button, ButtonState currentState, ButtonState oldState, int x, int y)
        {
            if (currentState == ButtonState.Pressed && oldState == ButtonState.Released)
            {
                OnMouseEvent(new MouseEventArgs(x, y, button, KeyState.Down));                
            }
            else if (currentState == ButtonState.Pressed && oldState == ButtonState.Pressed)
            {
                OnMouseEvent(new MouseEventArgs(x, y, button, KeyState.Pressed));
            }
            else if (currentState == ButtonState.Released && oldState == ButtonState.Pressed)
            {
                OnMouseEvent(new MouseEventArgs(x, y, button, KeyState.Up));
            }
        }

        private void OnMouseEvent(MouseEventArgs e)
        {
            MouseEventHandler handler = MouseEventHandler;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void ProcessKeyEvents()
        {
            // Back is first key and OemClear is last key in enum.
            for (int i = (int)Keys.Back; i < (int)Keys.OemClear; i++)
            {
                Keys k = (Keys)i;

                if (_currentKeyboardState.IsKeyDown(k) && _oldKeyboardState.IsKeyUp(k))
                {
                    OnKeyEvent(new KeyEventArgs(k, KeyState.Down));                    
                }
                else if (_currentKeyboardState.IsKeyDown(k) && _oldKeyboardState.IsKeyDown(k))
                {
                    OnKeyEvent(new KeyEventArgs(k, KeyState.Pressed));
                }
                else if (_currentKeyboardState.IsKeyUp(k) && _oldKeyboardState.IsKeyDown(k))
                {
                    OnKeyEvent(new KeyEventArgs(k, KeyState.Up));
                }
            }
        }

        private void OnKeyEvent(KeyEventArgs e)
        {
            KeyEventHandler handler = KeyEventHandler;

            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}

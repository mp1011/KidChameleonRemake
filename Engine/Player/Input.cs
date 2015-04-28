using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{

    public enum GameKey
    {
        None,
        Up,
        Down,
        Left,
        Right,
        Button1,
        Button2,
        Button3,
        Select,
        Start,
        Editor1,
        Editor2,
        Editor3,
        Editor4
    }

    public interface IGameInputDevice
    {
        bool KeyPressed(GameKey key);
        bool KeyReleased(GameKey key);
        bool KeyDown(GameKey key);
        bool KeyUp(GameKey key);
        int GetFirstPressedKey();
        int GetKeyMapping(GameKey keycode);
        void SetDirectionKeyMapping(GameKey keycode, int keycommand, Direction inputDirection);
        void SetKeyMapping(GameKey keycode, int keycommand);

        Dictionary<int, GameKey> ButtonMappings { get; set; }

        Direction? InputDirection(Orientation orientation);
        void CancelInput();
    }

    public class NullInputDevice : IGameInputDevice
    {

        public bool KeyPressed(GameKey key)
        {
            return false;
        }

        public bool KeyReleased(GameKey key)
        {
            return false;
        }

        public bool KeyDown(GameKey key)
        {
            return false;
        }

        public bool KeyUp(GameKey key)
        {
            return false;
        }

        public int GetFirstPressedKey()
        {
            return 0;
        }

        public int GetKeyMapping(GameKey keycode)
        {
            return 0;
        }

        public void SetDirectionKeyMapping(GameKey keycode, int keycommand, Direction inputDirection)
        {
        }

        public void SetKeyMapping(GameKey keycode, int keycommand)
        {
        }

        public Dictionary<int, GameKey> ButtonMappings
        {
            get
            {
                return new Dictionary<int, GameKey>();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Direction? InputDirection(Orientation orientation)
        {
            throw new NotImplementedException();
        }

        public void CancelInput()
        {
            throw new NotImplementedException();
        }
    }

    public static class IGameInputDeviceExtensions
    {
        public static bool KeyPressed(this IGameInputDevice device, int button)
        {
            var mapping = device.ButtonMappings.TryGet(button, GameKey.None);
            if (mapping == GameKey.None)
                return false;
            return device.KeyPressed(mapping);
        }

        public static bool KeyReleased(this IGameInputDevice device, int button)
        {
            var mapping = device.ButtonMappings.TryGet(button, GameKey.None);
            if (mapping == GameKey.None)
                return false;
            return device.KeyReleased(mapping);
        }

        public static bool KeyDown(this IGameInputDevice device, int button)
        {
            var mapping = device.ButtonMappings.TryGet(button, GameKey.None);
            if (mapping == GameKey.None)
                return false;
            return device.KeyDown(mapping);
        }

        public static bool KeyUp(this IGameInputDevice device, int button)
        {
            var mapping = device.ButtonMappings.TryGet(button, GameKey.None);
            if (mapping == GameKey.None)
                return false;
            return device.KeyUp(mapping);
        }
    }

    public abstract class GenericInputDevice : LogicObject, IGameInputDevice
    {
        private Dictionary<Direction, GameKey> inputDirectionKeys = new Dictionary<Direction, GameKey>();
        private int[] keyMap;
        private bool[,] keyStates = new bool[256, 2];
        private int m_curKeyState;

        private bool CurrentKeyState(GameKey keyCommand) { return keyStates[keyMap[(int)keyCommand], m_curKeyState]; }
        private bool LastKeyState(GameKey keyCommand) { return keyStates[keyMap[(int)keyCommand], m_curKeyState == 0 ? 1 : 0]; }

       
        public Dictionary<int,GameKey> ButtonMappings { get;set;}

        public GenericInputDevice(GameContext ctx)
            : base(LogicPriority.Input, Core.GlobalToken.Instance)
        {
            keyMap = new int[Enum.GetValues(typeof(GameKey)).Length];
            ButtonMappings = new Dictionary<int, GameKey>();
        }

        public void CancelInput() { }

        protected override void Update()
        {
            BeforeUpdateInputState();
            m_curKeyState = m_curKeyState == 0 ? 1 : 0;

            for (int i = 0; i < 256; i++)
                keyStates[i, m_curKeyState] = false;

            foreach (int key in this.GetPressedKeys())
                keyStates[(int)key, m_curKeyState] = true;
        }

        protected abstract IEnumerable<int> GetPressedKeys();

        protected virtual void BeforeUpdateInputState() { }

        #region IGameInputDevice Members

        public void ClearInput()
        {
            for (int i = 0; i < 256; i++)
            {
                keyStates[i, m_curKeyState] = false;
            }
        }

        public int GetFirstPressedKey()
        {
            for (int i = 0; i < 256; i++)
            {
                if (keyStates[i, m_curKeyState] && !keyStates[i, m_curKeyState == 0 ? 1 : 0])
                    return i;
            }

            return 0;
        }

        public bool KeyPressed(GameKey key)
        {
            return CurrentKeyState(key) && !LastKeyState(key);
        }

        public bool KeyDown(GameKey key)
        {
            return CurrentKeyState(key);
        }

        public bool KeyReleased(GameKey key)
        {
            return !CurrentKeyState(key) && LastKeyState(key);
        }

        public bool KeyUp(GameKey key)
        {
            return !CurrentKeyState(key);
        }

        protected virtual Direction? GetInputDirection(Orientation orientation) { return null; }

        public Direction? InputDirection(Orientation orientation)
        {
            var direction = GetInputDirection(orientation);

            if (direction != null)
                return direction;

            bool l = KeyDown(inputDirectionKeys[Direction.Left]), u = KeyDown(inputDirectionKeys[Direction.Up]), r = KeyDown(inputDirectionKeys[Direction.Right]), d = KeyDown(inputDirectionKeys[Direction.Down]);

            if (orientation == Orientation.Horizontal)
            {
                if (r)
                    return Direction.Right;
                else if (l)
                    return Direction.Left;
            }
            else if (orientation == Orientation.Vertical)
            {
                if (u)
                    return Direction.Up;
                else if (d)
                    return Direction.Down;
            }
            else
            {
                if (u && !l && !r)
                    return Direction.Up;
                else if (u && r)
                    return Direction.UpRight;
                else if (r && !u && !d)
                    return Direction.Right;
                else if (d && r)
                    return Direction.DownRight;
                else if (d && !l && !r)
                    return Direction.Down;
                else if (d && l)
                    return Direction.DownLeft;
                else if (l && !u && !d)
                    return Direction.Left;
                else if (u && l)
                    return Direction.UpLeft;
            }

            return null;
        }

        public void SetDirectionKeyMapping(GameKey gameKey, int keycode, Direction inputDirection)
        {
            inputDirectionKeys[inputDirection] = gameKey;
            keyMap[(int)gameKey] = keycode;
        }

        public void SetKeyMapping(GameKey gameKey, int keycode)
        {
            keyMap[(int)gameKey] = keycode;
        }

        public int GetKeyMapping(GameKey gameKey)
        {
            return keyMap[(int)gameKey];
        }

        #endregion
    }

}

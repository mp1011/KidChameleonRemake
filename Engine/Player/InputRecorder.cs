using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class InputRecorder : LogicObject, ISerializable
    {
        private LinkedList<InputEvent> mEvents = new LinkedList<InputEvent>();

        public InputRecorder() : base(LogicPriority.World, Core.GlobalToken.Instance) { }
      
        protected override void Update()
        {
            if (this.Context.FirstPlayer == null)
                return;
            CheckKey(GameKey.Button1);
            CheckKey(GameKey.Button2);
            CheckKey(GameKey.Button3);
            CheckKey(GameKey.Up);
            CheckKey(GameKey.Down);
            CheckKey(GameKey.Left);
            CheckKey(GameKey.Right);
            CheckKey(GameKey.Start);
            CheckKey(GameKey.Select);

            if (this.Context.FirstPlayer.Input.KeyPressed(GameKey.Editor1))
            {
                var path = new GamePath(PathType.Recordings,"Demo");
                System.IO.File.WriteAllText(path.FullPath, Serializer.ToJSON(this));
            }
        }

        private void CheckKey(GameKey key)
        {
            if (this.Context.FirstPlayer.Input.KeyPressed(key))
                mEvents.AddLast(new InputEvent { Frame = Context.CurrentFrameNumber, IsKeyDown = true, Key = key });
            else if (this.Context.FirstPlayer.Input.KeyReleased(key))
                mEvents.AddLast(new InputEvent { Frame = Context.CurrentFrameNumber, IsKeyDown = false, Key = key });
        }

        public object GetSaveModel()
        {
            return mEvents.ToArray();
        }

        public Type GetSaveModelType()
        {
            return typeof(InputEvent[]);
        }

        public void Load(object saveModel)
        {
            var events = (InputEvent[])saveModel;
            mEvents.Clear();
            foreach (var item in events)
                mEvents.AddLast(item);
        }

        public static InputPlayback Playback(GameContext ctx, string name)
        {
            var recording = GameResource<InputRecorder>.Load(new GamePath(PathType.Recordings, name), ctx);
            return new InputPlayback(ctx, recording.mEvents);
        }
    }

    class InputEvent
    {
        public ulong Frame { get; set; }
        public GameKey Key { get; set; }
        public bool IsKeyDown { get; set; }
    }

    public class InputPlayback : GenericInputDevice
    {
        private LinkedList<InputEvent> mEvents;
        private LinkedList<int> mPressedKeys = new LinkedList<int>();

        internal InputPlayback(GameContext ctx, IEnumerable<InputEvent> events)
            : base(ctx)
        {
            mEvents = new LinkedList<InputEvent>(events);


            foreach (var key in Enum.GetValues(typeof(GameKey)))
                this.SetKeyMapping((GameKey)key, (int)key);

            this.SetDirectionKeyMapping(GameKey.Left, (int)GameKey.Left, Direction.Left);
            this.SetDirectionKeyMapping(GameKey.Right, (int)GameKey.Right, Direction.Right);
            this.SetDirectionKeyMapping(GameKey.Up, (int)GameKey.Up, Direction.Up);
            this.SetDirectionKeyMapping(GameKey.Down, (int)GameKey.Down, Direction.Down);

        }

        protected override void BeforeUpdateInputState()
        {
            while (true)
            {
                if (mEvents.First == null)
                    break;
                var evt = mEvents.First.Value;

                if (evt.Frame < Context.CurrentFrameNumber)
                {
                    mEvents.RemoveFirst();
                    continue;
                }
                else if (evt.Frame > Context.CurrentFrameNumber)
                    break;

                if (evt.IsKeyDown)
                    mPressedKeys.AddLast((int)evt.Key);
                else
                    mPressedKeys.Remove((int)evt.Key);

                mEvents.RemoveFirst();
            }
        }

        protected override IEnumerable<int> GetPressedKeys()
        {
            return mPressedKeys;
        }
    }

}

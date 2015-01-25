using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{

    public class ObjectMotion : LogicObject
    {

        #region No Motion
        public static ObjectMotion NoMotion
        {
            get;
            private set;
        }

        private void SetNoMotion(GameContext ctx)
        {
            if (ObjectMotion.NoMotion != null)
                return;

            ObjectMotion.NoMotion = new ObjectMotion(ctx);
        }

        private class ImmobileDummy : IMoveable
        {
            public ObjectMotion MotionManager
            {
                get { return ObjectMotion.NoMotion; }
            }

            public void Move(RGPoint offset)
            {
            }
        }

        #endregion

        private List<IMotionComponent> mMotionComponents = new List<IMotionComponent>();
        private IMoveable mObject;

        private MotionVector mMainVector = new MotionVector();
        public MotionVector Vector
        {
            get
            {
                return mMainVector;
            }
        }

        public Direction Direction { get { return Vector.Direction; } set { Vector.Direction = value; } }
        public float Speed { get { return Vector.Magnitude; } set { Vector.Magnitude = value; } }
        public RGPoint MotionOffset { get { return Vector.MotionOffset; } }

        public ObjectMotion(GameContext ctx, IMoveable obj) : base(LogicPriority.Motion, ctx) { 
            mObject = obj;

            SetNoMotion(ctx);
        }

        private ObjectMotion(GameContext ctx)
            : base(LogicPriority.Motion, ctx)
        {
            mObject = new ImmobileDummy();
        }

        private DirectedMotion mMainMotion;

        public DirectedMotion MainMotion
        {
            get
            {
                if (mMainMotion == null)
                {
                    mMainMotion = new DirectedMotion("main");
                    mMotionComponents.Add(mMainMotion);
                }
                return mMainMotion;
            }
        }

        public void AddComponent(IMotionComponent a)
        {
            mMotionComponents.Add(a);
        }

        protected override void Update()
        {
            var originalVector = this.Vector;

            this.mMainVector = new MotionVector();
            foreach (var c in mMotionComponents)
            {
                if (!c.Inactive)
                {
                    var offset = c.GetOffset(Context);
                    this.mMainVector.Offset(offset);
                }
            }

            mObject.Move(this.Vector.MotionOffset);

            this.Vector.Round(3);
        }

        public void Reset()
        {
            mMainMotion = null;
            mMotionComponents.Clear();
        }

        public void StopMotionInDirection(DirectionFlags dir)
        {
            if (Context.FirstPlayer.Input.KeyDown(GameKey.Editor2) && (this.mObject.ToString().Contains("kid") || this.mObject.ToString().Contains("iron")))
            {
                if (dir.Down || dir.Up)
                    Console.WriteLine("X");
            }

            var thisFlags = new DirectionFlags(this.Direction);
            if ((thisFlags.Up && dir.Up) || (thisFlags.Down && dir.Down))
                this.Vector.MotionOffset = this.Vector.MotionOffset.SetComponent(Orientation.Vertical, 0f);

            if ((thisFlags.Left && dir.Left) || (thisFlags.Right && dir.Right))
                this.Vector.MotionOffset = this.Vector.MotionOffset.SetComponent(Orientation.Horizontal, 0f);

            foreach (var component in mMotionComponents)
                component.StopInDirection(dir);
            
        }

        public void StopAllMotion()
        {
            StopMotionInDirection(new DirectionFlags(true, true, true, true));
        }
    }

}

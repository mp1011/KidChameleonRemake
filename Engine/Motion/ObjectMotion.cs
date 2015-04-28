using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{

    public class ObjectMotion : LogicObject
    {

        #region No Motion

        private static ObjectMotion mNoMotion;
        public static ObjectMotion NoMotion
        {
            get
            {
                return mNoMotion ?? (mNoMotion = new ObjectMotion(Core.GlobalToken.Instance));
            }        
        }

   
        private class ImmobileDummy : IMoveable
        {
            public ObjectMotion MotionManager
            {
                get { return ObjectMotion.NoMotion; }
            }

            public void Move(RGPointI offset)
            {
            }
        }

        #endregion

        private List<IMotionComponent> mMotionComponents = new List<IMotionComponent>();
        private IMoveable mObject;

        private MotionVector mMainVector;
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


        public static ObjectMotion Create<T>(T obj) where T : LogicObject, IMoveable
        {
            var motion = new ObjectMotion(obj, obj);
            motion.mObject = obj;
            return motion;
        }

        public static ObjectMotion Create<T>(ILogicObject owner, T obj) where T : IMoveable
        {
            var motion = new ObjectMotion(owner, obj);
            return motion;
        }

        private ObjectMotion(ILogicObject obj)
            : base(LogicPriority.Motion, obj, RelationFlags.Normal)
        {
            this.mMainVector = new MotionVector();
            this.mObject = new ImmobileDummy();
        }

        private ObjectMotion(ILogicObject owner, IMoveable obj)
            : base(LogicPriority.Motion, owner)
        { 
            mObject = obj;
            this.mMainVector = new MotionVector();
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

        private IntegerX mOffsetX, mOffsetY;

        protected override void OnEntrance()
        {
            mOffsetX = new IntegerX(0);
            mOffsetY = new IntegerX(0);
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

            var motionOffset = this.Vector.MotionOffset;

            mOffsetX.FValue = motionOffset.X;
            mOffsetY.FValue = motionOffset.Y;



            mObject.Move(new RGPointI(mOffsetX.Value, mOffsetY.Value));

        }

        public void Reset()
        {
            mMainMotion = null;
            mMotionComponents.Clear();
        }

        public void StopMotionInDirection(DirectionFlags dir)
        {

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

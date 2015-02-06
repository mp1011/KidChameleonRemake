using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{

    public interface IMotionComponent
    {
        bool Inactive { get; set; }
        RGPoint GetOffset(GameContext context);
        void StopInDirection(DirectionFlags dir);
    }

    public class MotionVector
    {
        private RGPoint mOffset;
        private Direction mDirection;
        private float mMagnitude;

        public RGPoint MotionOffset
        {
            get { return mOffset; }
            set
            {
                mOffset = value;

                var d = MotionOffset.Direction;
                if (d.HasValue)
                    mDirection = d.Value;

                mMagnitude = MotionOffset.Magnitude;
            }
        }

        public Direction Direction
        {
            get { return mDirection; }
            set
            {
                mDirection = value;
                mOffset = RGPoint.Empty.Offset(mDirection, mMagnitude);
            }
        }

        public float Magnitude
        {
            get { return mMagnitude; }
            set
            {
                mMagnitude = value;
                mOffset = RGPoint.Empty.Offset(mDirection, mMagnitude);
            }
        }

        public float GetMagnitudeInDirection(Direction d)
        {
            var unit = RGPoint.Empty.Offset(d, 1f);
            return (float)this.MotionOffset.DotProduct(unit);
        }

        public MotionVector() 
        {
            mOffset = RGPoint.Empty;
        }

        public MotionVector(Direction dir, float speed) : this()
        {
            mDirection = dir;
            Magnitude = speed;
        }

        public void Offset(Direction d, float amount)
        {
            this.MotionOffset = this.MotionOffset.Offset(d, amount);
        }

        public void Offset(RGPoint pt)
        {
            this.MotionOffset = this.MotionOffset.Offset(pt);
        }

        public override string ToString()
        {
            return MotionOffset.ToString();
        }

    }

    public class SpeedInfo
    {
        public float Current { get;set;}
        public float Target { get;set;}
        public float Accel { get;set;}
        public float Decel { get;set;}

        public float Percentage
        {
            get
            {
                if (Target == 0)
                    return 0f;
                else 
                    return Current / Target;
            }
            set
            {
                Current = (Target * value);
            }
        }

        public void Set(float newSpeed)
        {
            this.Current = newSpeed;
            this.Target = newSpeed;
        }

        public void Update()
        {
            bool decel = false;
            bool negate = false;

            if (Current != Target)
            {
                if (Target == 0)
                {
                    decel = true;
                    negate = Current > Target;
                }
                else if (Target > 0)
                {
                    decel = Current > Target;
                    negate = decel;
                }
                else if (Target < 0)
                {
                    decel = Current < Target;
                    negate = !decel;
                }

                float mod = (decel ? this.Decel : this.Accel);
                if (negate)
                    mod *= -1;

                var newSpeed = Current + mod;

                if ((Current < Target && newSpeed > Target) || (Current > Target && newSpeed < Target))
                    Current = Target;
                else
                    Current = newSpeed;
            }    
        }
    }
  
    public class DirectedMotion : IMotionComponent
    {

        private MotionVector mTargetVector;
        public string Description { get; private set; }
      
        public bool DependsOnOriginalSpeed { get { return false; } }

        public bool Inactive
        {
            get;
            set;
        }

        private SpeedInfo mSpeedInfo;
        public SpeedInfo SpeedInfo { get { return mSpeedInfo ?? (mSpeedInfo = new SpeedInfo());}}

        public float Accel { get { return SpeedInfo.Accel;} set {SpeedInfo.Accel = value;}}
        public float Decel { get { return SpeedInfo.Decel;} set {SpeedInfo.Decel = value;}}

        public float Percentage { get { return SpeedInfo.Percentage; } set { SpeedInfo.Percentage = value; } }

        public Direction Direction 
        { 
            get { return mTargetVector.Direction; } 
            set 
            {
                var curVector = new MotionVector(this.Direction, CurrentSpeed);
                CurrentSpeed = curVector.GetMagnitudeInDirection(value);
                mTargetVector.Direction = value; 
            } 
        }

        public float TargetSpeed { get { return mTargetVector.Magnitude; } set { mTargetVector.Magnitude = value; } }

        public float CurrentSpeed { get { return SpeedInfo.Current; } set { SpeedInfo.Current = value; } }

        public void Set(Direction d, float speed)
        {
            this.Direction = d;
            this.CurrentSpeed = speed;
            this.TargetSpeed = speed;
        }

        public void Set(float speed)
        {
            this.CurrentSpeed = speed;
            this.TargetSpeed = speed;
        }

        public DirectedMotion(string description)
        {
            this.Description = description;
            mTargetVector = new MotionVector();
        }

        public void StopInDirection(DirectionFlags flags)
        {
            var thisFlags = new DirectionFlags(this.Direction);

            if (
                (thisFlags.Up && flags.Up) ||
                (thisFlags.Down && flags.Down) ||
                (thisFlags.Left && flags.Left) ||
                (thisFlags.Right && flags.Right))
            {
                CurrentSpeed = 0;
            }

        }


        public RGPoint GetOffset(GameContext context)        
        {
            SpeedInfo.Target = this.TargetSpeed;
            SpeedInfo.Update();
            return RGPoint.Empty.Offset(Direction, CurrentSpeed);      
        }

    }

    public class XYMotion : IMotionComponent
    {
        public bool Inactive { get; set; }
        public string Description { get; private set; }

        private SpeedInfo xSpeed, ySpeed;

        public SpeedInfo XSpeed { get { return xSpeed ?? (xSpeed = new SpeedInfo()); } }
        public SpeedInfo YSpeed { get { return ySpeed ?? (ySpeed = new SpeedInfo()); } }

        public override string ToString()
        {
            return this.Description;
        }

        public void StopInDirection(DirectionFlags flags)
        {
            if (flags.Up && YSpeed.Current < 0)
                YSpeed.Current = 0;

            if (flags.Down && YSpeed.Current > 0)
                YSpeed.Current = 0;

            if (flags.Left && XSpeed.Current < 0)
                XSpeed.Current = 0;

            if (flags.Right && XSpeed.Current > 0)
                XSpeed.Current = 0;

        }

        public RGPoint GetOffset(GameContext context)
        {
            XSpeed.Update();
            YSpeed.Update();
                
            return new RGPoint(XSpeed.Current,YSpeed.Current);
        }

        public XYMotion(string description)
        {
            this.Description = description;
        }
    }

}

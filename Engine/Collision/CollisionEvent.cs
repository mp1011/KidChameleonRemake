using Engine.Collision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class CollisionEvent<T1, T2>
        where T1 : ICollidable
        where T2 : ICollidable
    {
        private T1 mThisPosition;
        private T2 mOtherPosition;

        #region Collision Time Values

        public RGRectangleI ThisCollisionTimeArea { get; private set; }
        public RGRectangleI OtherCollisionTimeArea { get; private set; }

        public RGPoint ThisCollisionTimeSpeed { get; private set; }
        public RGPoint OtherCollisionTimeSpeed { get; private set; }

        #endregion

        public T2 OtherObject { get { return mOtherPosition; } }

        /// <summary>
        /// Combined speed of the two objects
        /// </summary>
        public RGPoint CollisionSpeed { get; private set; }

        public bool IsBlocking { get; private set; }

        public bool OtherTopExposed { get; private set; }
        public bool OtherLeftExposed { get; private set; }
        public bool OtherRightExposed { get; private set; }
        public bool OtherBottomExposed { get; private set; }

        public RGRectangleI ThisArea { get { return mThisPosition.Area; } }
        public RGRectangleI OtherArea { get { return mOtherPosition.Area; } }

        public ObjectType ThisType { get { return mThisPosition.ObjectType; } }
        public ObjectType OtherType { get { return mOtherPosition.ObjectType; } }

        public HitboxType ThisHitboxType { get; private set; }
        public HitboxType OtherHitboxType { get; private set; }

        public RGLine OtherTop { get { if (OtherTopExposed) return OtherArea.TopSide; else return new RGLine(OtherArea.TopLeft, OtherArea.TopLeft); } }
        public RGLine OtherBottom { get { if (OtherBottomExposed) return OtherArea.BottomSide; else return new RGLine(OtherArea.BottomLeft, OtherArea.BottomLeft); } }
        public RGLine OtherLeft { get { if (OtherLeftExposed) return OtherArea.LeftSide; else return new RGLine(OtherArea.TopLeft, OtherArea.TopLeft); } }
        public RGLine OtherRight { get { if (OtherRightExposed) return OtherArea.RightSide; else return new RGLine(OtherArea.TopRight, OtherArea.TopRight); } }

        /// <summary>
        /// Side of the this object that collided
        /// </summary>
        public Side CollisionSide { get; set; }

        public RGPoint SlopeIntersectionPoint { get; set; }
        public bool IsSloped { get { return !SlopeIntersectionPoint.IsEmpty; } }
        public DirectionFlags SlopeDirection { get; set; }

        ///// <summary>
        ///// Either returns this event or its inversion, so that the given object is the primary one. Assumes that the given object is part of this event.
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        //public CollisionEvent<T1,T2> AdjustTo(ICollidable obj)
        //{
        //    if (obj.Equals(mThisPosition))
        //        return this;
        //    else
        //        return this.Invert();
        //}

        //public CollisionEvent Invert()
        //{
        //    var c = new CollisionEvent(mOtherPosition, mThisPosition, true, true, true, true, this.IsBlocking, this.OtherHitboxType,this.ThisHitboxType);
        //    switch (this.CollisionSide)
        //    {
        //        case Side.Left: c.CollisionSide = Side.Right; break;
        //        case Side.Right: c.CollisionSide = Side.Left; break;
        //        case Side.Top: c.CollisionSide = Side.Bottom; break;
        //        case Side.Bottom: c.CollisionSide = Side.Top; break;
        //    }

        //    c.ThisCollisionTimeArea = this.OtherCollisionTimeArea;
        //    c.ThisCollisionTimeSpeed = this.OtherCollisionTimeSpeed;
        //    c.OtherCollisionTimeArea = this.ThisCollisionTimeArea;
        //    c.OtherCollisionTimeSpeed = this.ThisCollisionTimeSpeed;
        //    return c;
        //}

        public CollisionEvent(T1 thisPos, T2 otherPos, bool topExposed, bool leftExposed, bool rightExposed, bool bottomExposed, bool isBlocking, HitboxType thisHitboxType, HitboxType otherHitboxType)
        {
            this.OtherLeftExposed = leftExposed;
            this.OtherRightExposed = rightExposed;
            this.OtherTopExposed = topExposed;
            this.OtherBottomExposed = bottomExposed;

            mThisPosition = thisPos;
            mOtherPosition = otherPos;

            this.IsBlocking = isBlocking;

            this.ThisCollisionTimeArea = ThisArea;
            this.ThisCollisionTimeSpeed = mThisPosition.MotionManager.MotionOffset;
            this.OtherCollisionTimeArea = OtherArea;
            this.OtherCollisionTimeSpeed = mOtherPosition.MotionManager.MotionOffset;

            this.ThisHitboxType = thisHitboxType;
            this.OtherHitboxType = otherHitboxType;
        }

        public void CalcCollisionSpeed(ICollidable obj)
        {
            this.CollisionSpeed = obj.MotionManager.MotionOffset.Offset(this.OtherObject.MotionManager.MotionOffset);
        }

        public RGRectangleI ThisObjectPreviousPosition
        {
            get
            {
                var speed = ThisCollisionTimeSpeed;
                speed = speed.Scale(-1f, -1f);
                return ThisCollisionTimeArea.Offset(speed.ToPointI());
            }
        }
    }
}

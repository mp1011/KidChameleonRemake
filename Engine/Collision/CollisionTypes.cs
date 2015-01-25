using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Collision;

namespace Engine
{
    public static class ICollidableExtensions
    {
        public static void AddCollisionChecks<T>(this T collidable, params ObjectType[] collisionTypes) where T : LogicObject, ICollidable
        {
            CollisionManager<T> m = null;

            if (collisionTypes.Contains(ObjectType.Block))
                m = new Collision.TileCollisionManager<T>(collidable);

            if (collisionTypes.Contains(ObjectType.Border))
                m = new Collision.BorderCollisionmanager<T>(collidable, layer => layer.Location, DirectionFlags.Horizontal);


            var obj = collisionTypes.Where(p => p.Is(ObjectType.Thing)).ToArray();
            if (obj.Length > 0)
            {
                m = new Collision.ObjectCollisionManager<T>(collidable, obj);
                collidable.Context.Listeners.CollisionListener.Register(collidable);
            }

            collidable.CollisionTypes = collisionTypes;
        }

        public static bool CanCollideWith(this ICollidable objectA, ICollidable objectB)
        {
            return objectA.CanCollideWith(objectB, true);
        }

        private static bool CanCollideWith(this ICollidable objectA, ICollidable objectB, bool checkReverse)
        {
            bool canCollide = false;
            foreach (var cType in objectB.CollisionTypes)
            {
                if (objectA.ObjectType.Is(cType))
                {
                    canCollide = true;
                    break;
                }
            }

            if (!canCollide)
                return false;

            if (checkReverse)
                return objectB.CanCollideWith(objectA, false);

            return true;
        }
    }


}

namespace Engine.Collision
{
    public enum Side
    {
        Top,
        Left,
        Bottom,
        Right
    }

    struct CorrectionRec
    {
        public Side Side;
        public RGRectangleI Rec;
    }

    struct CorrectionRecF
    {
        public Side Side;
        public RGRectangle Rec;
    }

    public interface ICollidable : IWithPosition, IMoveable
    {
        LayerDepth LayerDepth { get; }
        ObjectType ObjectType { get; }
        ObjectType[] CollisionTypes { get; set; }
        RGRectangle SecondaryCollisionArea { get; }

        void HandleCollision(CollisionEvent collision, CollisionResponse response);
    }


    public class CollisionEvent
    {
        private ICollidable mThisPosition;
        private ICollidable mOtherPosition;


        #region Collision Time Values

        public RGRectangle ThisCollisionTimeArea { get; private set; }
        public RGRectangle OtherCollisionTimeArea { get; private set; }

        public RGPoint ThisCollisionTimeSpeed { get; private set; }
        public RGPoint OtherCollisionTimeSpeed { get; private set; }

        #endregion

        public ICollidable OtherObject { get { return mOtherPosition; } }

        /// <summary>
        /// Combined speed of the two objects
        /// </summary>
        public RGPoint CollisionSpeed { get; private set; }

        public bool IsBlocking { get; private set; }

        public bool OtherTopExposed { get; private set; }
        public bool OtherLeftExposed { get; private set; }
        public bool OtherRightExposed { get; private set; }
        public bool OtherBottomExposed { get; private set; }

        public RGRectangle ThisArea { get { return mThisPosition.Area; } }
        public RGRectangle OtherArea { get { return mOtherPosition.Area; } }

        public ObjectType ThisType { get { return mThisPosition.ObjectType; } }
        public ObjectType OtherType { get { return mOtherPosition.ObjectType; } }

        public HitboxType ThisHitboxType { get; private set; }
        public HitboxType OtherHitboxType { get; private set; }

        public RGLine OtherTop { get { if (OtherTopExposed) return OtherArea.TopSide; else return new RGLine(OtherArea.TopLeft, OtherArea.TopLeft); } }
        public RGLine OtherBottom { get { if (OtherBottomExposed) return OtherArea.BottomSide; else return new RGLine(OtherArea.BottomLeft, OtherArea.BottomLeft); } }
        public RGLine OtherLeft { get { if (OtherLeftExposed) return OtherArea.LeftSide; else return new RGLine(OtherArea.TopLeft, OtherArea.TopLeft); } }
        public RGLine OtherRight { get { if (OtherRightExposed) return OtherArea.RightSide; else return new RGLine(OtherArea.TopRight, OtherArea.TopRight); } }

        public Side CollisionSide { get; set; }

        public RGPoint SlopeIntersectionPoint { get; set; }
        public bool IsSloped { get { return !SlopeIntersectionPoint.IsEmpty; } }
        public DirectionFlags SlopeDirection { get; set; }

        /// <summary>
        /// Either returns this event or its inversion, so that the given object is the primary one. Assumes that the given object is part of this event.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public CollisionEvent AdjustTo(ICollidable obj)
        {
            if (obj.Equals(mThisPosition))
                return this;
            else
                return this.Invert();
        }

        public CollisionEvent Invert()
        {
            var c = new CollisionEvent(mOtherPosition, mThisPosition, true, true, true, true, this.IsBlocking, this.OtherHitboxType,this.ThisHitboxType);
            switch (this.CollisionSide)
            {
                case Side.Left: c.CollisionSide = Side.Right; break;
                case Side.Right: c.CollisionSide = Side.Left; break;
                case Side.Top: c.CollisionSide = Side.Bottom; break;
                case Side.Bottom: c.CollisionSide = Side.Top; break;
            }

            c.ThisCollisionTimeArea = this.OtherCollisionTimeArea;
            c.ThisCollisionTimeSpeed = this.OtherCollisionTimeSpeed;
            c.OtherCollisionTimeArea = this.ThisCollisionTimeArea;
            c.OtherCollisionTimeSpeed = this.ThisCollisionTimeSpeed;



            return c;
        }

        public CollisionEvent(ICollidable thisPos, ICollidable otherPos, bool topExposed, bool leftExposed, bool rightExposed, bool bottomExposed, bool isBlocking, HitboxType thisHitboxType, HitboxType otherHitboxType)
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
            this.OtherHitboxType =otherHitboxType;
        }

        public void CalcCollisionSpeed(ICollidable obj)
        {
            this.CollisionSpeed = obj.MotionManager.MotionOffset.Offset(this.OtherObject.MotionManager.MotionOffset);
        }

        public RGRectangle ThisObjectPreviousPosition
        {
            get
            {
                var speed = ThisCollisionTimeSpeed;
                speed = speed.Scale(-1f, -1f);
                return ThisCollisionTimeArea.Offset(speed);
            }
        }
    }


}

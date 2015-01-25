using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Engine.Collision
{
    class BorderEdge : ICollidable
    {
        private RGRectangle mArea;
        private Side mSide;

        public BorderEdge(RGRectangle area, Side side)
        {
            mArea = area;
            mSide = side;
        }

        public LayerDepth LayerDepth
        {
            get { return Engine.LayerDepth.Foreground; }
        }

        public ObjectType ObjectType
        {
            get { return ObjectType.Border; }
        }

        public ObjectType[] CollisionTypes
        {
            get;
            set;
        }

        public RGRectangle SecondaryCollisionArea
        {
            get { return RGRectangle.Empty; }
        }

        public void HandleCollision(CollisionEvent collision, CollisionResponse response)
        {
            return;
        }

        public GameContext Context
        {
            get { throw new NotImplementedException(); }
        }

        public RGPoint Location
        {
            get
            {
                return Area.TopLeft;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public RGPoint LocationOffset
        {
            get { return RGPoint.Empty; }
        }

        public RGRectangle Area
        {
            get 
            {
                int thickness = 200;
                switch (mSide)
                {
                    case Side.Left: return RGRectangle.FromXYWH(mArea.Left - thickness, -thickness, thickness, mArea.Height + (thickness * 2));
                    case Side.Right: return RGRectangle.FromXYWH(mArea.Right, -thickness, thickness, mArea.Height + (thickness * 2));
                    case Side.Top: return RGRectangle.FromXYWH(-thickness, -thickness, mArea.Width + (thickness * 2), thickness);
                    case Side.Bottom: return RGRectangle.FromXYWH(-thickness, mArea.Bottom, mArea.Width + (thickness * 2), thickness);
                }

                return RGRectangle.Empty;
            }
        }

        public Direction Direction
        {
            get { throw new NotImplementedException(); }
        }

        public ObjectMotion MotionManager
        {
            get { return ObjectMotion.NoMotion; }
        }

        public void Move(RGPoint offset)
        {
            throw new NotImplementedException();
        }
    }

    class BorderCollisionmanager<T> : CollisionManager<T> where T : LogicObject, ICollidable    
    {

        private Func<Layer, RGRectangle> mGetBounds;
        private DirectionFlags mCheckSides;

        public BorderCollisionmanager(T obj, Func<Layer, RGRectangle> fnGetBounds, DirectionFlags checkSides)
            : base(obj)
        {
            mGetBounds = fnGetBounds;
            mCheckSides = checkSides;
        }


        protected override IEnumerable<CollisionEvent> CheckCollisions(Layer layer)
        {
            var bounds = mGetBounds(layer);
            var hb = this.CollidingObject.Area;

            if (hb.Left < bounds.Left && mCheckSides.Left)
                yield return GetBorderCollisionEvent(bounds, Side.Left);
            if (hb.Top < bounds.Top && mCheckSides.Up)
                yield return GetBorderCollisionEvent(bounds, Side.Top);
            if (hb.Right > bounds.Right && mCheckSides.Right)
                yield return GetBorderCollisionEvent(bounds, Side.Right);
            if (hb.Bottom > bounds.Bottom && mCheckSides.Down)
                yield return GetBorderCollisionEvent(bounds, Side.Bottom);

        }

        private CollisionEvent GetBorderCollisionEvent(RGRectangle bounds, Side side)
        {
            return new CollisionEvent(this.CollidingObject, new BorderEdge(bounds, side), true, true, true, true, true, HitboxType.Primary, HitboxType.Primary);
        }

    }

   
}

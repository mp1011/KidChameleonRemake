using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Engine.Collision
{
    class BorderEdge : ICollidable
    {
        private RGRectangleI mArea;
        private Side mSide;

        public BorderEdge(RGRectangleI area, Side side)
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

        public RGRectangleI SecondaryCollisionArea
        {
            get { return RGRectangleI.Empty; }
        }

        public void HandleCollision(CollisionEvent collision, CollisionResponse response)
        {
            return;
        }

        public GameContext Context
        {
            get { throw new NotImplementedException(); }
        }

        public RGPointI Location
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

        public RGRectangleI Area
        {
            get 
            {
                int thickness = 200;
                switch (mSide)
                {
                    case Side.Left: return RGRectangleI.FromXYWH(mArea.Left - thickness, -thickness, thickness, mArea.Height + (thickness * 2));
                    case Side.Right: return RGRectangleI.FromXYWH(mArea.Right, -thickness, thickness, mArea.Height + (thickness * 2));
                    case Side.Top: return RGRectangleI.FromXYWH(-thickness, -thickness, mArea.Width + (thickness * 2), thickness);
                    case Side.Bottom: return RGRectangleI.FromXYWH(-thickness, mArea.Bottom, mArea.Width + (thickness * 2), thickness);
                }

                return RGRectangleI.Empty;
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

        public void Move(RGPointI offset)
        {
            throw new NotImplementedException();
        }

        private List<ICollisionResponder> mDummyCollection = new List<ICollisionResponder>();
        public ICollection<ICollisionResponder> CollisionResponders
        {
            get { return mDummyCollection; }
        }
    }

    class BorderCollisionmanager<T> : CollisionManager<T> where T : LogicObject, ICollidable    
    {

        private Func<Layer, RGRectangleI> mGetBounds;
        private DirectionFlags mCheckSides;

        public BorderCollisionmanager(T obj, Func<Layer, RGRectangleI> fnGetBounds, DirectionFlags checkSides)
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

        private CollisionEvent GetBorderCollisionEvent(RGRectangleI bounds, Side side)
        {
            return new CollisionEvent(this.CollidingObject, new BorderEdge(bounds, side), true, true, true, true, true, HitboxType.Primary, HitboxType.Primary);
        }

    }

   
}

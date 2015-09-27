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
   
        public BorderEdge(GameContext context, RGRectangleI area, Side side)
        {
            mArea = area;
            mSide = side;
            this.Context = context;
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
            get;
            private set;
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

        public bool Alive
        {
            get { return true; }
        }

        public bool Paused
        {
            get;
            set;
        }

        public ExitCode ExitCode
        {
            get { return ExitCode.StillAlive; }
        }

        public void Kill(ExitCode exitCode)
        {
            throw new NotImplementedException();
        }
    }

    abstract class BorderCollisionmanager<T> : CollisionManager<T> where T : LogicObject, ICollidable    
    {

        private Func<WorldCollisionInfo, RGRectangleI> mGetBounds;
        private DirectionFlags mCheckSides;
        private GameContext mContext;

        public BorderCollisionmanager(T obj, DirectionFlags checkSides)
            : base(obj)
        {
            mCheckSides = checkSides;
            mContext = obj.Context;
        }

        protected abstract RGRectangleI GetBounds(WorldCollisionInfo info);

        protected override IEnumerable<CollisionEvent> CheckCollisions(WorldCollisionInfo info)
        {
            var bounds = GetBounds(info);
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
            return new CollisionEvent(this.CollidingObject, new BorderEdge(mContext,bounds, side), true, true, true, true, true, HitboxType.Primary, HitboxType.Primary);
        }

    }

    class LevelBorderCollisionManager<T> : BorderCollisionmanager<T> where T : LogicObject, ICollidable
    {

        public LevelBorderCollisionManager(T obj, DirectionFlags checkSides)
            : base(obj, checkSides)        {      }


        protected override RGRectangleI GetBounds(WorldCollisionInfo info)
        {
            return info.LevelBounds;
        }
    }

   
}

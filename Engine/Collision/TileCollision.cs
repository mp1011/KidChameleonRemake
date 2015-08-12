using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Collision
{
    public abstract class CollidingTile : ICollidable, ICollisionResponder 
    {
        public TileInstance Tile { get; private set; }
        public TileLayer TileLayer { get; private set; }

        public GameContext Context { get { return TileLayer.Context; } }
        public LayerDepth LayerDepth { get { return TileLayer.Depth; } }

        public ObjectType ObjectType { get { return ObjectType.Block; } }

        public ObjectType[] CollisionTypes { get; set; }

        public RGRectangleI SecondaryCollisionArea { get { return RGRectangleI.Empty; } }

        public RGPointI Location
        {
            get { return Area.TopLeft; }
            set
            {
                throw new NotImplementedException(); 
            }
        }

        public RGRectangleI Area
        {
            get { return Tile.TileArea; }
        }

        public CollidingTile(TileInstance tile, TileLayer layer)
        {
            Tile = tile;
            TileLayer = layer;
            this.CollisionTypes = new ObjectType[] { ObjectType.Thing };
        }

        public ObjectMotion MotionManager
        {
            get { return ObjectMotion.NoMotion; }
        }

        public void BeforeCollision(CollisionEvent collision)
        {
        }

        public abstract void HandleCollision(CollisionEvent collision, CollisionResponse collisionResponse);
       

        public Direction Direction
        {
            get { throw new NotImplementedException(); }
        }


        public void Move(RGPointI offset)
        {
            throw new NotImplementedException();
        }

        public ICollection<ICollisionResponder> CollisionResponders
        {
            get { return new ICollisionResponder[] { this }; }
        }
    }

    class TileCollisionManager<T> : CollisionManager<T> where T : LogicObject, ICollidable
    {
        public TileCollisionManager(T obj)
            : base(obj)
        {
        }

        protected override IEnumerable<CollisionEvent> CheckCollisions(WorldCollisionInfo info)
        {
            if (CollidingObject.CollisionTypes.Contains(ObjectType.Block))     
            {
                RGPointI tilePosStart = info.WorldToTilePoint(this.CollidingObject.Area.TopLeft);
                RGPointI tilePosEnd = info.WorldToTilePoint(this.CollidingObject.Area.BottomRight);

                for (int y = tilePosStart.Y; y <= tilePosEnd.Y; y++)
                    for (int x = tilePosStart.X; x <= tilePosEnd.X; x++)
                    {
                        var tileInfo = info.GetTile(x, y);
                        var tile = tileInfo.Instance;
                        
                        var solid = (tile.TileDef.Flags & TileFlags.Solid) > 0;
                        var sloped = (tile.TileDef.Flags & TileFlags.Sloped) > 0;
                        if (solid || sloped)
                        {
                            var rec = tile.TileArea;

                            if (CollidingObject.Area.CollidesWith(rec))
                            {
                                if (solid)
                                {
                                    //ignore solid tile if there is a sloped tile above it and to either side
                                    var tileAbove = tile.GetAdjacentTile(0,-1).TileDef;
                                    var tileLeft = tile.GetAdjacentTile(-1, 0).TileDef;
                                    var tileRight = tile.GetAdjacentTile(1, 0).TileDef;

                                    if (tileAbove.IsSloped && (tileLeft.IsSloped || tileRight.IsSloped))
                                        continue;

                                    yield return CreateCollisionEvent(tileInfo);

                                }
                                if (sloped)
                                {
                                    var slopeEvent = CreateSlopedTileCollisionEvent(tileInfo);
                                    if (slopeEvent != null)
                                        yield return slopeEvent;
                                }
                            }
                        }
                    }

            }
        }

        private CollisionEvent CreateSlopedTileCollisionEvent(TileCollisionView tileView)
        {
            // Up/Down = The exposed surface
            // Left/Right = The higher side
            var tile = tileView.Instance;
            var rec = tile.TileArea;

            int? yIntercept = tile.TileDef.GetYIntercept(rec, CollidingObject.Location.X);
            if (!yIntercept.HasValue)
                return null;

            var cx = CollidingObject.Location.X;
            var cy = CollidingObject.Location.Y;

            if (cx < rec.Left || cx > rec.Right)
                return null;

            if (cy < yIntercept)
                return null;

            var evt = CreateCollisionEvent(tileView);
            evt.SlopeIntersectionPoint = new RGPoint(cx, yIntercept.Value);
            evt.SlopeDirection = tile.TileDef.Sides;

            if (tile.TileDef.Sides.Up)
                evt.CollisionSide = Side.Top;
            else if (tile.TileDef.Sides.Down)
                evt.CollisionSide = Side.Bottom;
            return evt;
            
        }

        private CollisionEvent CreateCollisionEvent(TileCollisionView tileView)
        {
            var tile = tileView.Instance;
            bool leftExposed = tileView.GetAdjacentTile(-1,0).TileDef.IsPassable;
            bool rightExposed = tileView.GetAdjacentTile(1,0).TileDef.IsPassable;
            bool topExposed = tileView.GetAdjacentTile(0,-1).TileDef.IsPassable;
            bool bottomExposed = tileView.GetAdjacentTile(0,1).TileDef.IsPassable;

            var collidingTile = tile.CreateCollidingTile(tileView.Layer);
            return new CollisionEvent(this.CollidingObject, collidingTile, topExposed, leftExposed, rightExposed, bottomExposed, true, HitboxType.Primary, HitboxType.Primary);
        }
    }
}

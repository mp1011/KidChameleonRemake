using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Collision
{
    public abstract class CollidingTile : ICollidable
    {
        public TileInstance Tile { get; private set; }
        public TileLayer TileLayer { get; private set; }

        public GameContext Context { get { return TileLayer.Context; } }
        public LayerDepth LayerDepth { get { return TileLayer.Depth; } }

        public ObjectType ObjectType { get { return ObjectType.Block; } }

        public ObjectType[] CollisionTypes { get; set; }

        public RGRectangle SecondaryCollisionArea { get { return RGRectangle.Empty; } }

        public RGPoint Location
        {
            get { return Area.TopLeft; }
            set
            {
                throw new NotImplementedException(); 
            }
        }

        public RGRectangle Area
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


        public void Move(RGPoint offset)
        {
            throw new NotImplementedException();
        }


        public RGPoint LocationOffset
        {
            get { return RGPoint.Empty; }
        }
    }

    class TileCollisionManager<T> : CollisionManager<T> where T : LogicObject, ICollidable
    {


        public TileCollisionManager(T obj)
            : base(obj)
        {
        }

        protected override IEnumerable<CollisionEvent> CheckCollisions(Layer layer)
        {
            return GetCollisionsInLayer(layer as TileLayer).ToArray(); 
        }

        private IEnumerable<CollisionEvent> GetCollisionsInLayer(TileLayer layer)
        {
            if (layer != null)
            {
                RGPointI tilePosStart = layer.Map.ScreenToTilePoint(this.CollidingObject.Area.TopLeft);
                RGPointI tilePosEnd = layer.Map.ScreenToTilePoint(this.CollidingObject.Area.BottomRight);

                for (int y = tilePosStart.Y; y <= tilePosEnd.Y; y++)
                    for (int x = tilePosStart.X; x <= tilePosEnd.X; x++)
                    {
                        var tileDef = layer.Map.GetTile(x, y);

                        var solid = (tileDef.Flags & TileFlags.Solid) > 0;
                        var sloped = (tileDef.Flags & TileFlags.Sloped) > 0;
                        if (solid || sloped)
                        {
                            var rec = layer.Map.GetTileLocation(x, y);

                            if (CollidingObject.Area.CollidesWith(rec))
                            {
                                if (solid)
                                {
                                    //ignore solid tile if there is a sloped tile above it and to either side
                                    var tileAbove = layer.Map.GetTile(x, y - 1);
                                    var tileLeft = layer.Map.GetTile(x-1, y);
                                    var tileRight = layer.Map.GetTile(x+1, y);

                                    if (tileAbove.IsSloped && (tileLeft.IsSloped || tileRight.IsSloped))
                                        continue;

                                    yield return CreateCollisionEvent(layer, tileDef, x, y);

                                }
                                if (sloped)
                                {
                                    var slopeEvent = CreateSlopedTileCollisionEvent(layer, tileDef, x, y);
                                    if (slopeEvent != null)
                                        yield return slopeEvent;
                                }
                            }
                        }
                    }

            }
        }

        private CollisionEvent CreateSlopedTileCollisionEvent(TileLayer layer, TileDef tileDef, int tileX, int tileY)
        {
            // Up/Down = The exposed surface
            // Left/Right = The higher side

            var rec = layer.Map.GetTileLocation(tileX, tileY);

            float? yIntercept = tileDef.GetYIntercept(rec, CollidingObject.Location.X);
            if (!yIntercept.HasValue)
                return null;

            var cx = CollidingObject.Location.X;
            var cy = CollidingObject.Location.Y;

            if (cx < rec.Left || cx > rec.Right)
                return null;

            if (cy < yIntercept)
                return null;

            var evt = CreateCollisionEvent(layer, tileDef, tileX, tileY);
            evt.SlopeIntersectionPoint = new RGPoint(cx, yIntercept.Value);
            evt.SlopeDirection = tileDef.Sides;

            if (tileDef.Sides.Up)
                evt.CollisionSide = Side.Top;
            else if (tileDef.Sides.Down)
                evt.CollisionSide = Side.Bottom;
            return evt;
            
        }

        private CollisionEvent CreateCollisionEvent(TileLayer layer, TileDef tileDef, int tileX, int tileY)
        {
            var rec = layer.Map.GetTileLocation(tileX, tileY);

            bool leftExposed = layer.Map.GetTile(tileX - 1, tileY).IsPassable;
            bool rightExposed = layer.Map.GetTile(tileX + 1, tileY).IsPassable;
            bool topExposed = layer.Map.GetTile(tileX, tileY - 1).IsPassable;
            bool bottomExposed = layer.Map.GetTile(tileX, tileY + 1).IsPassable;

            var instance = layer.Map.GetTileAtCoordinates(tileX, tileY);
            var collidingTile = instance.CreateCollidingTile(layer as TileLayer);
            return new CollisionEvent(this.CollidingObject, collidingTile, topExposed, leftExposed, rightExposed, bottomExposed, true, HitboxType.Primary, HitboxType.Primary);
        }
    }
}

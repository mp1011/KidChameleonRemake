using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public interface IWithPosition
    {
        GameContext Context { get; }
        RGPointI Location { get; set; }
        RGRectangleI Area { get; }
        Direction Direction { get; }
    }

    public static class IWithPositionExtensions
    {
        public static void SnapToGround(this IWithPosition obj, Map map)
        {
            obj.SnapToSurface(map, Direction.Down);
        }

        public static void SnapToGround(this IWithPosition obj, TileLayer layer)
        {
            if (layer == null)
                return;
            obj.SnapToGround(layer.Map);
        }

        public static void SnapToSurface(this IWithPosition obj, Map map, Direction surfaceDir)
        {
            var startTile = map.GetTileAtLocation(obj.Area.Center);

            var collideTile = startTile.GetTilesInLine(surfaceDir).FirstOrDefault(p => p.TileDef.IsSolid);

            if (collideTile == null)
                return;

            var x = obj.Location.X;
            var y = obj.Location.Y;

            if (surfaceDir.Equals(Direction.Down))
                y = collideTile.TileArea.Top;
            else
                throw new NotImplementedException();

            obj.Location = new RGPointI(x,y);

        }

    }

    public interface IMoveable 
    {
        ObjectMotion MotionManager { get; }
        void Move(RGPointI offset);
    }

    public interface IMoveableWithPosition : IWithPosition, IMoveable
    {

    }

    public static class IMoveableExtensions
    {
        public static MotionVector GetVelocity(this IMoveable m)
        {
            return m.MotionManager.Vector;
        }

        public static bool IsOnGround(this IWithPosition item, TileLayer layer)
        {
            var groundTile = layer.Map.GetTileAtLocation(item.Location).GetTilesInLine(Direction.Down).Take(2).FirstOrDefault(p => p.TileDef.IsSolid);
            return groundTile != null && (item.Location.Y - groundTile.TileArea.Top) < 1f;
        }
    }

 

}

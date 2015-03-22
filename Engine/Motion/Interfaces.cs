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

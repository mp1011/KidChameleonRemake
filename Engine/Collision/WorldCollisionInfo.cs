using Engine.Collision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class TileCollisionView
    {

        public TileInstance Instance;
        public LayerDepth Depth;
        public TileLayer Layer;
        public WorldCollisionInfo CollisionInfo;

        public TileDef TileDef { get { return Instance.TileDef; } }

        public IEnumerable<TileInstance> GetTilesInLine(Direction d)
        {
            var pt = d.ToPoint();
            return GetTilesInLine(pt.X, pt.Y);
        }

        public IEnumerable<TileInstance> GetTilesInLine(int dx, int dy)
        {
            int x = Instance.TileLocation.X;
            int y = Instance.TileLocation.Y;

            while (true)
            {
                var tile = CollisionInfo.GetTile(x, y);
                if (tile.Instance.TileDef.IsOutOfBounds)
                    break;
                else
                    yield return tile.Instance;

                x += dx;
                y += dy;
            }
        }

        public TileInstance GetAdjacentTile(int xOffset, int yOffset)
        {
            return CollisionInfo.GetTile(Instance.TileLocation.X + xOffset, Instance.TileLocation.Y + yOffset).Instance;
        }
    }

    public class WorldCollisionInfo  
    {

        private TileCollisionView[,] mGrid;
        private TileLayer mForeground;
        private World mWorld;

        public CollidableListener CollisionListener { get; private set; }
        public RGRectangleI LevelBounds { get; private set; }

        public WorldCollisionInfo(World world)
        {
            this.mWorld = world;
            this.CollisionListener = new CollidableListener(world);
            this.mForeground = world.GetLayers(LayerDepth.Foreground).FirstOrDefault(p => p is TileLayer) as TileLayer;
            this.LevelBounds = mForeground.Location;
            FillGrid();
        }

        public TileCollisionView GetTile(int gridX, int gridY)
        {
            if (gridX < 0 || gridY < 0 || gridX >= mForeground.Map.TileDimensions.Width || gridY >= mForeground.Map.TileDimensions.Height)
                return new TileCollisionView { CollisionInfo = this, Instance = mForeground.Map.GetTileAtGridCoordinates(gridX, gridY), Layer = mForeground };
            else
                return mGrid[gridX, gridY];
        }

        public TileCollisionView GetTile(RGPointI worldLocation)
        {
            var gridPt = WorldToTilePoint(worldLocation);
            return GetTile(gridPt.X, gridPt.Y);
        }

        public RGPointI WorldToTilePoint(RGPointI worldPoint)
        {
            var tileset = mForeground.Map.Tileset;
            return new RGPointI((int)(worldPoint.X / tileset.TileSize.Width), (int)(worldPoint.Y / tileset.TileSize.Height));
        }

        private void FillGrid()
        {
            mGrid = new TileCollisionView[mForeground.Map.TileDimensions.Width, mForeground.Map.TileDimensions.Height];

            foreach(var pt in RGPointI.EnumerateGrid(mForeground.Map.TileDimensions))            
                mGrid[pt.X, pt.Y] = FillTile(pt);
            
        }

        private TileCollisionView FillTile(RGPointI gridPoint)
        {
            TileInstance tile;
            foreach (var layer in mWorld.GetLayers().OfType<TileLayer>().Reverse())
            {
                tile = layer.Map.GetTileAtGridCoordinates(gridPoint.X, gridPoint.Y);
                if (tile.TileDef.IsSolid || tile.TileDef.IsSolid || tile.TileDef.IsSpecial)
                    return new TileCollisionView { Layer = layer, Instance = tile, CollisionInfo = this};
            }

            tile = mForeground.Map.GetTileAtGridCoordinates(gridPoint.X, gridPoint.Y);
            return new TileCollisionView { Layer = mForeground, Instance = tile, CollisionInfo = this };
        }

        public void OnTileChanged(int gridX, int gridY)
        {
             mGrid[gridX,gridY] = FillTile(new RGPointI(gridX, gridY));
        }

    }
}

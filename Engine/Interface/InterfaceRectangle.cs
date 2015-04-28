using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class InterfaceRectangle : IDrawable, IWithPosition 
    {
        private TextureResource mTexture;
        private RGSizeI mCellSize;
        private RGPointI mInsideCell;

        public RGSizeI SizeInCells { get; set; }

        private List<BorderPart> mParts = new List<BorderPart>();
        
        public InterfaceRectangle(GameContext ctx, TextureResource texture, RGSizeI cellSize, RGPointI insideCell)
        {
            this.Context = ctx;
            mCellSize = cellSize;
            mTexture = texture;
            mInsideCell = insideCell;
        }

        public void AddBorderPart(RGPointI cell, Direction dir)
        {
            mParts.Add(new BorderPart { Cell = cell, OriginalDirection = dir });
        }

        class BorderPart
        {
            public RGPointI Cell { get; set; }
            public Direction OriginalDirection { get; set; }
        }

        #region Position
        public GameContext Context
        {
            get;
            private set;
        }

        public RGPointI Location
        {
            get;
            set;
        }

        public RGRectangleI Area
        {
            get { return RGRectangleI.Create(this.Location, this.SizeInCells.Scale(mCellSize.Width, mCellSize.Height)); }
        }

        public Direction Direction
        {
            get { return Direction.Right; }
        }
        #endregion
        
        public void Draw(Graphics.Painter p, RGRectangleI canvas)
        {
            for (int x = 0; x < this.SizeInCells.Width; x++)
                for (int y = 0; y < this.SizeInCells.Height; y++)
                    DrawCell(p, canvas, GetCellDirection(x, y), this.Area.TopLeft.Offset(x*mCellSize.Width,y*mCellSize.Height));

        }

        private Direction? GetCellDirection(int x, int y)
        {
            if (x == 0 && y == 0)
                return Direction.UpLeft;
            if (x == 0 && y == this.SizeInCells.Height - 1)
                return Direction.DownLeft;
            if (x == this.SizeInCells.Width - 1 && y == 0)
                return Direction.UpRight;
            if (x == this.SizeInCells.Width - 1 && y == this.SizeInCells.Height - 1)
                return Direction.DownRight;
            else if (x == 0)
                return Direction.Left;
            else if (y == 0)
                return Direction.Up;
            else if (x == this.SizeInCells.Width - 1)
                return Direction.Right;
            else if (y == this.SizeInCells.Height - 1)
                return Direction.Down;
            else 
                return null;
        }

        private void DrawCell(Graphics.Painter p, RGRectangleI canvas, Direction? direction, RGPointI location)
        {
            RenderOptions renderOptions= RenderOptions.Normal;
            RGRectangleI source = RGRectangleI.Empty;

            if (direction.HasValue)
            {
                foreach (var part in mParts)
                {
                    if (part.OriginalDirection == direction)
                    {
                        source = part.Cell.ToGridCell(mCellSize);
                        break;
                    }
                    else if (part.OriginalDirection == direction.Value.Reflect(Orientation.Horizontal))
                    {
                        source = part.Cell.ToGridCell(mCellSize);
                        renderOptions.FlipX = true;
                    }
                    else if (part.OriginalDirection == direction.Value.Reflect(Orientation.Vertical))
                    {
                        source = part.Cell.ToGridCell(mCellSize);
                        renderOptions.FlipY = true;
                    }
                    else if (part.OriginalDirection == direction.Value.Reflect(Orientation.Vertical).Reflect(Orientation.Horizontal))
                    {
                        source = part.Cell.ToGridCell(mCellSize);
                        renderOptions.FlipY = true;
                        renderOptions.FlipX = true;
                    }
                }
            }
            else
                source = mInsideCell.ToGridCell(mCellSize);

            p.Paint(canvas, mTexture,source, RGRectangleI.Create(location, mCellSize), renderOptions);
                    
        }

    }
}

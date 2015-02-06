using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Collision;
using Newtonsoft.Json;

namespace Engine
{
    [Flags]
    public enum TileFlags
    {
        Passable = 0,
        Solid = 1,
        Breakable = 2,
        Special = 4,
        Invisible = 8,
        Sloped = 16,
        OutOfBounds = 512
    }


    public class TileDef : ISerializable
    {
        private static TileDef _blankTile;
        public static TileDef Blank
        {
            get
            {
                if (_blankTile == null)
                {
                    _blankTile = new TileDef(TileFlags.Invisible, 0, 0, RGPoint.Empty, new DirectionFlags());
                    _blankTile.Usage.SingleGroup = "empty";
                }

                return _blankTile;
            }
        }

        private static TileDef _blankSolidTile;
        public static TileDef BlankSolid
        {
            get
            {
                if (_blankSolidTile == null)
                {
                    _blankSolidTile = new TileDef(TileFlags.Invisible | TileFlags.Solid,-1, 0, RGPoint.Empty, new DirectionFlags());
                    _blankSolidTile.Usage.SingleGroup = "empty";
                }

                return _blankSolidTile;
            }
        }

        private static TileDef _oobTile;
        public static TileDef OutOfBounds
        {
            get
            {
                if (_oobTile == null)
                    _oobTile = new TileDef(TileFlags.OutOfBounds, 0, 0, RGPoint.Empty, new DirectionFlags());

                return _oobTile;
            }
        }


        public bool IsPassable { get { return ((this.Flags & TileFlags.Solid) == 0) && ((this.Flags & TileFlags.Sloped) == 0); } }
        public bool IsBlank { get { return (this.Flags & TileFlags.Invisible) > 0; } }
        public bool IsSolid { get { return (this.Flags & TileFlags.Solid) > 0; } }
        public bool IsOutOfBounds { get { return (this.Flags & TileFlags.OutOfBounds) > 0; } }
        public bool IsSloped { get { return (this.Flags & TileFlags.Sloped) > 0; } }

        public RGRectangleI[] SourcePositions { get; private set; }
        public int FrameDuration { get; private set; }

        public TileUsage Usage { get; private set; }

        public DirectionFlags Sides { get; private set; }

        private ulong lastFrameTime = 0;
        private int frameIndex = 0;
        public RGRectangleI SourcePosition
        {
            get
            {
                return SourcePositions[frameIndex];
            }
        }

        public RGPoint DestOffset { get; private set; }

        public void UpdateAnimation(GameContext context)
        {
            if (context.CurrentFrameNumber >= lastFrameTime + (ulong)FrameDuration)
            {
                lastFrameTime = context.CurrentFrameNumber;
                frameIndex++;
                if (frameIndex >= SourcePositions.Length)
                    frameIndex = 0;
            }
        }

        public TileFlags Flags { get; private set; }
        public int TileID { get; private set; }

        public TileDef() 
        {
            this.Usage = new TileUsage();
        }

        public TileDef(TileFlags flags, int id, int frameDuration, RGPoint destOffset, DirectionFlags sides, TileUsage usage, params RGRectangleI[] sources)
        {
            this.Sides = sides;
            this.SourcePositions = sources;
            this.Flags = flags;
            this.TileID = id;
            this.FrameDuration = frameDuration;

            this.Usage = usage;
        }

        public TileDef(TileFlags flags, int id, int frameDuration, RGPoint destOffset, DirectionFlags sides, params RGRectangleI[] sources)
        {
            this.Sides = sides;
            this.SourcePositions = sources;
            this.Flags = flags;
            this.TileID = id;
            this.FrameDuration = frameDuration;

            this.Usage = new TileUsage(); ;
        }

        #region Saving

        private class TileSaveModel
        {
            public int ID;
            public TileFlags Flags;
            public RGRectangleI[] SourcePositions;
            public int FrameDuration;
            public DirectionFlags Sides;
            public string[] SideGroups;
            public string[] Groups;
            public int RandomUsageWeight;
        }

        public object GetSaveModel()
        {
            return new TileSaveModel
            {
                Flags = this.Flags,
                ID = this.TileID,
                SourcePositions = this.SourcePositions,
                FrameDuration = this.FrameDuration,
                Sides = this.Sides,
                Groups = this.Usage.Groups,
                SideGroups = this.Usage.SideGroups,
                RandomUsageWeight = this.Usage.RandomUsageWeight
            };
        }

        public Type GetSaveModelType()
        {
            return typeof(TileSaveModel);
        }

        public void Load(object saveModel)
        {
            var model = saveModel as TileSaveModel;
            this.TileID = model.ID;
            this.Flags = model.Flags;
            this.SourcePositions = model.SourcePositions;
            this.FrameDuration = model.FrameDuration;
            this.Sides = model.Sides;

            this.Usage.SideGroups = model.SideGroups;
            this.Usage.Groups = model.Groups;
            this.Usage.RandomUsageWeight = model.RandomUsageWeight;

            if (this.Usage.Groups.Length == 1 && this.Usage.Groups[0].Contains(","))
                this.Usage.Groups = this.Usage.Groups[0].Split(',');
        }

        #endregion

        public int? GetYIntercept(RGRectangleI tileLocation, int x)
        {
            int yIntercept, yLeft, yRight;

            if (Sides.Left)
            {
                yLeft = tileLocation.Top;
                yRight = tileLocation.Bottom;
            }
            else if (Sides.Right)
            {
                yLeft = tileLocation.Bottom;
                yRight = tileLocation.Top;
            }
            else
                return null;

            var cx = x;
            if (cx < tileLocation.Left || cx > tileLocation.Right)
                return null;

            var yDiff = yRight - yLeft;
            var xPct = (float)(cx - tileLocation.Left) / tileLocation.Width;
            yIntercept = (int)(yLeft + (yDiff * xPct));

            return yIntercept;
        }

        public override string ToString()
        {
            return this.TileID + " " + this.Flags.ToString() + " " + this.Usage.SideGroups.StringJoin(",");
        }
    
    }

    public abstract class TileInstance
    {
        public TileDef TileDef { get; set; }
        public RGPointI TileLocation { get; set; }

        [JsonIgnore]
        public Map Map { get; set; }

        public RGRectangleI TileArea
        {
            get
            {
                if (Map == null)
                    return RGRectangleI.Empty;

                return Map.GetTileLocation(TileLocation.X, TileLocation.Y);
            }
        }

        public TileInstance GetAdjacentTile(int xOff, int yOff)
        {
            var nextTile = TileLocation.Offset(xOff, yOff);
            return this.Map.GetTileAtCoordinates(nextTile.X, nextTile.Y);
        }

        public IEnumerable<TileInstance> GetTilesInLine(Direction d)
        {
            var pt = d.ToPoint();
            return GetTilesInLine(pt.X, pt.Y);
        }

        public IEnumerable<TileInstance> GetTilesInLine(int dx, int dy)
        {
            int x = TileLocation.X;
            int y = TileLocation.Y;

            while (true)
            {
                var tile = Map.GetTileAtCoordinates(x, y);
                if (tile.TileDef.IsOutOfBounds)
                    break;
                else
                    yield return tile;

                x += dx;
                y += dy;
            }
        }

        public TileInstance ReplaceWith(TileDef newTile)
        {
            if (newTile == null)
                return this;

            this.Map.SetTile(this.TileLocation.X, this.TileLocation.Y, newTile.TileID);
            return this.Map.GetTileAtCoordinates(this.TileLocation.X, this.TileLocation.Y);
        }

        public abstract bool IsSpecial { get; }
        public abstract CollidingTile CreateCollidingTile(TileLayer tileLayer);
    }


}

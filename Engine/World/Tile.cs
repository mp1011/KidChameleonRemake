using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Collision;
using Newtonsoft.Json;
using System.ComponentModel;

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


    public class TileDef : ISerializableBaseClass
    {

        #region Defaults

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
                    _blankSolidTile = new TileDef(TileFlags.Invisible | TileFlags.Solid, -1, 0, RGPoint.Empty, new DirectionFlags());
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
      
        #endregion


        #region Editor Only Properties

        [DisplayName("Sides")]
        public EditorDirectionFlags _SidesForEditor
        {
            get
            {
                return Sides.ToEditorFlags();
            }
            set
            {
                this.Sides = new DirectionFlags(value);
            }
        }

        [DisplayName("Flags")]
        public TileFlags _FlagsForEditor { get { return this.Flags; } set { this.Flags = value; } }

        public string Groups
        {
            get { return this.Usage.Groups.StringJoin(","); }
            set
            {
                this.Usage.Groups = value.Split(',').Select(p => p.Trim()).ToArray();
            }
        }

        #endregion

        #region Properties

        private ulong lastFrameTime = 0;
        private int frameIndex = 0;

        [Browsable(false)]
        public int TileID { get; private set; }

        [Browsable(false)]
        public TileFlags Flags { get; private set; }

        [Browsable(false)]
        public bool IsPassable { get { return ((this.Flags & TileFlags.Solid) == 0) && ((this.Flags & TileFlags.Sloped) == 0); } }

        [Browsable(false)]
        public bool IsBlank { get { return (this.Flags & TileFlags.Invisible) > 0; } }

        [Browsable(false)]
        public bool IsSolid { get { return (this.Flags & TileFlags.Solid) > 0; } }

        [Browsable(false)]
        public bool IsOutOfBounds { get { return (this.Flags & TileFlags.OutOfBounds) > 0; } }

        [Browsable(false)]
        public bool IsSloped { get { return (this.Flags & TileFlags.Sloped) > 0; } }

        [Browsable(false)]
        public RGRectangleI[] SourcePositions { get; private set; }

        [Browsable(false)]
        public int FrameDuration { get; private set; }

        [Browsable(false)]
        public TileUsage Usage { get; private set; }

        [Browsable(false)]
        public DirectionFlags Sides { get; private set; }

        #endregion

        public RGRectangleI SourcePosition
        {
            get
            {
                if (SourcePositions.Length == 0)
                    return RGRectangleI.Empty;
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
            public object ExtraData;
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
                RandomUsageWeight = this.Usage.RandomUsageWeight,
                ExtraData = GetSaveModelExtra()
            };
        }

        protected virtual object GetSaveModelExtra()
        {
            return null;
        }

        protected virtual void LoadExtra(object data)
        {

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

            this.LoadExtra(model.ExtraData);
        }


        Type ISerializableBaseClass.GetTargetType()
        {
            var dummy = Engine.Core.GameBase.Current.TileInstanceCreate();
            return dummy.TileDef.GetType();
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

        public void SetValues(int? id, TileFlags? flags, DirectionFlags sides, RGRectangleI? source)
        {
            if (id.HasValue)
                this.TileID = id.Value;

            if (flags.HasValue)
                this.Flags = flags.Value;

            if (sides != null)
                this.Sides = sides;

            if (source.HasValue)
                this.SourcePositions = new RGRectangleI[] { source.Value };
        }
    }

    public abstract class TileInstance
    {
        public abstract TileDef TileDef { get; set; }
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

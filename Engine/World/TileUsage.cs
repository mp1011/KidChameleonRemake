using Engine.Collision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public enum GroupSide
    {
        LeftTop,
        TopLeft,
        TopRight,
        RightTop,
        RightBottom,
        BottomRight,
        BottomLeft,
        LeftBottom
    }

    public static class GroupSideExtensions
    {
        public static GroupSide GetAdjacentSide(this GroupSide side)
        {
            switch (side)
            {
                case GroupSide.LeftTop: return GroupSide.RightTop;
                case GroupSide.TopLeft: return GroupSide.BottomLeft;
                case GroupSide.TopRight: return GroupSide.BottomRight;
                case GroupSide.RightTop: return GroupSide.LeftTop;
                case GroupSide.RightBottom: return GroupSide.LeftBottom;
                case GroupSide.BottomRight: return GroupSide.TopRight;
                case GroupSide.BottomLeft: return GroupSide.TopLeft;
                case GroupSide.LeftBottom: return GroupSide.RightBottom;
            }

            return side;
        }

        public static RGPointI ToOffset(this GroupSide side)
        {
            switch (side)
            {
                case GroupSide.LeftTop: return new RGPointI(-1, 0);
                case GroupSide.TopLeft: return new RGPointI(0, -1);
                case GroupSide.TopRight: return new RGPointI(0, -1);
                case GroupSide.RightTop: return new RGPointI(1, 0);
                case GroupSide.RightBottom: return new RGPointI(1, 0);
                case GroupSide.BottomRight: return new RGPointI(0, 1);
                case GroupSide.BottomLeft: return new RGPointI(0, 1);
                case GroupSide.LeftBottom: return new RGPointI(-1, 0);
            }

            return RGPointI.Empty;
        }
    }

    public class TileUsage
    {
        public string AutomatchGroup { get; set; }

        public ICollection<String> Groups { get; set; }

        private Dictionary<Side, List<TileDef>> mSideMatches;

        public void AddMatch(Side s, TileDef match)
        {
            AddMatches(s, new TileDef[] { match });
        }

        public void AddMatches(Side s, IEnumerable<TileDef> matches)
        {
            var list = mSideMatches.TryGet(s, null);
            if (list == null)
            {
                list = new List<TileDef>();
                mSideMatches.Add(s, list);
            }

            matches = matches.Where(p=> ! list.Any(q=>q.Equals(p)));
            list.AddRange(matches);

            var sorted = list.OrderBy(p => p.TileID).ToArray();
            list.Clear();
            list.AddRange(sorted);
        }

        public void RemoveMatch(Side s, TileDef tile)
        {
            var list = mSideMatches.TryGet(s, null);
            if (list == null)
            {
                list = new List<TileDef>();
                mSideMatches.Add(s, list);
            }

            list.RemoveAll(p => p.Equals(tile));
        }

        public void SyncMatches(IEnumerable<TileDef> matches)
        {
            foreach (var key in mSideMatches.Keys)
            {
                var oldMatches = mSideMatches[key].ToArray();
                mSideMatches[key].Clear();
                mSideMatches[key].AddRange(oldMatches.Select(p => matches.FirstOrDefault(q => q.TileID == p.TileID)).Where(p => p != null));
            }
        }

        public IEnumerable<TileDef> GetMatches(Side s)
        {
            return mSideMatches.TryGet(s, new List<TileDef>());
        }

        public void ClearMatches()
        {
            mSideMatches.Clear();
        }

        public TileUsage()
        {
            this.Groups = new List<string>();
            mSideMatches = new Dictionary<Side, List<TileDef>>();
        }
    }
}

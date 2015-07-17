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

        private Dictionary<GroupSide, string[]> mSideGroups;

        public Dictionary<GroupSide, string[]> SideGroups
        {
            get
            {
                if (mSideGroups == null)
                    mSideGroups = new Dictionary<GroupSide, string[]>();
                return mSideGroups;
            }
        }

        public IEnumerable<string> DistinctGroupNames
        {
            get
            {
                return SideGroups.Values.Where(p => p.NotNullOrEmpty()).SelectMany(p => p).Where(p=>p!="*").Distinct().OrderBy(p => p);
            }
        }

        public int SideCount(string groupName)
        {
            return SideGroups.Values.Where(p => p.Contains(groupName)).Count();
        }

        public int RandomUsageWeight { get; set; }
     
        public TileUsage()
        {
        }

        public bool ContainsGroups(IEnumerable<string> groups)
        {
            return groups.Any(p => this.DistinctGroupNames.Contains(p));
        }

        //public override string ToString()
        //{
        //    return SideGroups.Values.StringJoin(" ");
        //}

    }
}

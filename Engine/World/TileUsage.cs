using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class TileUsage
    {
        /// <summary>
        /// Represents 8 sides, from top left, going clockwise
        /// </summary>
        private  string[] mSideGroups;

        public string[] SideGroups
        {
            get
            {
                if (mSideGroups == null)
                    mSideGroups = new string[8];
                return mSideGroups;
            }
            set
            {
                mSideGroups = value;
            }
        }

        public string[] Groups { get; set; }

        public IEnumerable<string> DistinctGroupNames
        {
            get
            {
                return SideGroups.SelectMany(p => p.NotNull().Split(',')).Where(p=>!String.IsNullOrEmpty(p)).Distinct();
            }
        }

        public string SingleGroup
        {
            get
            {
                if(SideGroups.NotNullOrEmpty() && SideGroups.All(p=> p == SideGroups.First()))
                    return SideGroups.First();
                else
                    return null;
            }
            set
            {
                if (value.IsNullOrEmpty())
                    return;

                this.SideGroups = Enumerable.Range(0, 8).Select(p => value).ToArray(); 
            }
        }

        public string TopLeftGroup { get { return SideGroups.ElementAtOrDefault(0); } set { SideGroups[0] = value; } }
        public string TopRightGroup { get { return SideGroups.ElementAtOrDefault(1); } set { SideGroups[1] = value; } }
        public string RightTopGroup { get { return SideGroups.ElementAtOrDefault(2); } set { SideGroups[2] = value; } }
        public string RightBottomGroup { get { return SideGroups.ElementAtOrDefault(3); } set { SideGroups[3] = value; } }
        public string BottomRightGroup { get { return SideGroups.ElementAtOrDefault(4); } set { SideGroups[4] = value; } }
        public string BottomLeftGroup { get { return SideGroups.ElementAtOrDefault(5); } set { SideGroups[5] = value; } }
        public string LeftBottomGroup { get { return SideGroups.ElementAtOrDefault(6); } set { SideGroups[6] = value; } }
        public string LeftTopGroup { get { return SideGroups.ElementAtOrDefault(7); } set { SideGroups[7] = value; } }

        public int RandomUsageWeight { get; set; }

     
        public TileUsage()
        {
        }

        public bool ContainsGroups(IEnumerable<string> groups)
        {
            return groups.Any(p => this.Groups.Contains(p));
        }

        public override string ToString()
        {
            return SideGroups.StringJoin(" ");
        }

    }
}

using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KidC
{
    class TransformationStats : IFromCSV
    {

        #region Resource

        private static IGameResource<TransformationStats[]> mResource;

        public static void LoadResource(GameContext context)
        {
            mResource = new DevelopmentResource<TransformationStats[]>(new CSVResource<TransformationStats>("Transformations.csv"));
        }

        public static TransformationStats GetStats(Sprite sprite)
        {
            var stats = mResource.GetObject(sprite.Context);
            return stats.FirstOrDefault(p => p.PlayerType.Equals(sprite.ObjectType));
        }

        public static TransformationStats GetStats(GameContext context, ObjectType helmetType)
        {
            var stats = mResource.GetObject(context);
            return stats.FirstOrDefault(p => p.HelmetType.Equals(helmetType));
        }

        #endregion 

        public string HelmetGraphic { get; set; }
        public ObjectType HelmetType { get; set; }
        public ObjectType PlayerType { get; set; }
        public SoundResource TransformSound { get; set; }
       
        public float WalkSpeed { get; set; }
        public float WalkAccel { get; set; }

        public float RunSpeed { get; set; }
        public float RunAccel { get; set; }

        public float StopAccel { get; set; }
        public float AirDecel { get; set; }
        public float TurnAccel { get; set; }
        public float AirTurnAccel { get; set; }
        public float IceMod { get; set; }

        public float CrawlSpeed { get; set; }
        public float CrawlAccel { get; set; }
        public float CrawlDecel { get; set; }

        public float UpHillSpeedMod { get; set; }
        public float DownHillSpeedMod { get; set; }

        public float JumpSpeed { get; set; }
        public float RunJumpSpeedMod { get; set; }

        public ulong LongJumpDuration { get; set; }
        public ulong ShortJumpDuration { get; set; }

        public float SideBounceDecel { get; set; }
        public float SideBounceSpeed { get; set; }
        public float VerticalBounceStrength { get; set; }

        public void FillFromCSV(string[] cells)
        {
            var index = 0;
            this.HelmetGraphic = cells[index];
            this.HelmetType = KCObjectType.FromString(cells[index++]);
            this.PlayerType = KCObjectType.FromString(cells[index++]);
            this.TransformSound = Sounds.FromString(cells[index++]);

            this.WalkSpeed = cells[index++].TryParseFloat(0f);
            this.WalkAccel = cells[index++].TryParseFloat(0f);
            this.RunSpeed = cells[index++].TryParseFloat(0f);
            this.RunAccel = cells[index++].TryParseFloat(0f);
            this.StopAccel = cells[index++].TryParseFloat(0f);
            this.AirDecel = cells[index++].TryParseFloat(0f);
            this.TurnAccel = cells[index++].TryParseFloat(0f);
            this.AirTurnAccel = cells[index++].TryParseFloat(0f);
            this.CrawlSpeed = cells[index++].TryParseFloat(0f);
            this.CrawlAccel = cells[index++].TryParseFloat(0f);
            this.CrawlDecel = cells[index++].TryParseFloat(0f);
            this.UpHillSpeedMod = cells[index++].TryParseFloat(0f);
            this.DownHillSpeedMod = cells[index++].TryParseFloat(0f);
            this.JumpSpeed = cells[index++].TryParseFloat(0f);
            this.RunJumpSpeedMod = cells[index++].TryParseFloat(0f);
            this.LongJumpDuration = (ulong)cells[index++].TryParseInt(0);
            this.ShortJumpDuration = (ulong)cells[index++].TryParseInt(0);
            this.IceMod = cells[index++].TryParseFloat(0f);
            this.SideBounceDecel = cells[index++].TryParseFloat(0f);
            this.SideBounceSpeed = cells[index++].TryParseFloat(0f);
            this.VerticalBounceStrength = cells[index++].TryParseFloat(0f);
        }
    }
}

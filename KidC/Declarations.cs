using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    [EditorVisible]
    public static class KCObjectType
    {
        
        public static ObjectType Player { get { return new ObjectType(ObjectType.CustomIDStart + 1, "Player"); } }
        public static ObjectType Collectable { get { return new ObjectType(ObjectType.CustomIDStart + 2, "Collectable"); } }

        [EditorVisible]
        public static ObjectType JamesKid { get { return new ObjectType(ObjectType.CustomIDStart + 1000, "JamesKid"); } }
        public static ObjectType IronKnight { get { return new ObjectType(ObjectType.CustomIDStart + 1001, "IronKnight"); } }
        public static ObjectType RedStealth { get { return new ObjectType(ObjectType.CustomIDStart + 1002, "RedStealth"); } }

        public static ObjectType Helmet { get { return new ObjectType(ObjectType.CustomIDStart + 2000, "Helmet"); } }
        public static ObjectType IronKnightHelmet { get { return new ObjectType(ObjectType.CustomIDStart + 2001, "Iron Knight Helmet"); } }
        public static ObjectType RedStealthHelmet { get { return new ObjectType(ObjectType.CustomIDStart + 2002, "Red Stealth Helmet"); } }

        public static ObjectType Gem { get { return new ObjectType(ObjectType.CustomIDStart + 3001, "Gem"); } }
        public static ObjectType Puff { get { return new ObjectType(ObjectType.CustomIDStart + 3002, "Puff"); } }

        public static ObjectType Enemy { get { return new ObjectType(ObjectType.CustomIDStart + 4000, "Enemy"); } }

        [EditorVisible]
        public static ObjectType Dragon { get { return new ObjectType(ObjectType.CustomIDStart + 4001, "Dragon"); } }
    }

    public enum PrizeType
    {
        None = 0,
        Gem = ObjectType.CustomIDStart + 3001,
        IronKnightHelmet = ObjectType.CustomIDStart + 2001,
        RedStealthHelmet = ObjectType.CustomIDStart + 2002
    }


    public static class KCAnimation
    {
        public static int Stand { get { return 1; } }
        public static int Walk { get { return 2; } }
        public static int Jump { get { return 3; } }
        public static int Turn { get { return 4; } }
        public static int Fall { get { return 5; } }
        public static int ClimbUp { get { return 6; } }
        public static int ClimbDown { get { return 7; } }
        public static int Crawl { get { return 8; } }
        public static int Flip { get { return 9; } }
 
        public static int TransitionIn { get {return 10; }}
        public static int TransitionOut { get { return 11; } }
        public static int Attack { get { return 12; } }
        public static int AttackAlt { get { return 13; } }
        public static int Hurt { get { return 14; } }
        public static int Dying { get { return 15; } }
        public static int Dead { get { return 16; } }



        public static int IronKnightClimb { get { return 100; } } 

    }

    public static class KCButton
    {
        public static int Jump { get { return 1; } }
        public static int Run { get { return 2; } }
        public static int Special { get { return 3; } }
        public static int Pause { get { return 4; } }
    }
   
}

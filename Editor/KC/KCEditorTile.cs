using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using System.ComponentModel;

namespace Editor.KC
{
    class KCEditorTile : EditorTile
    {
        //[Category("Kid Chameleon")]
        //public KidC.SpecialTile SpecialType { get; set; }

        public override Engine.TileDef CreateTileDef()
        {
            return null;
            //if (SpecialType == KidC.SpecialTile.Normal)
            //    return base.CreateTileDef();
            //else
            //    return new TileDef(Flags, (int)(1000 + this.SpecialType), FrameDuration, RGPoint.Empty, Sides, this.Usage, Image.Region);

        }

        protected override void InitializeExtra()
        {
            //if (this.ID > 1000)
            //    this.SpecialType = (KidC.SpecialTile)(this.ID - 1000);
            //else
            //    this.SpecialType = KidC.SpecialTile.Normal;
        }
    }
}

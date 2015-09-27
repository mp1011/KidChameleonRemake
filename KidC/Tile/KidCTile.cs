using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Collision;
using Engine;
using System.ComponentModel;

namespace KidC
{
    public enum SpecialTile
    {
        Normal=0,
        Rock=10,
        Prize=20,
        Metal=30,
        Rubber=40,
        Platform=50,
        Elevator=60,
        Ice=70,
        Vanishing=80,
        Mushroom=90,
        Teleporter=100,
        Shifter = 110,
        Ghost

    }



    public class KCTileDef : TileDef
    {
        public SpecialTile SpecialType { get; set; }

        public KCTileDef() { }

        public KCTileDef(TileDef baseDef)
        {
            this.SpecialType = SpecialTile.Normal;
            this.SetValues(baseDef.TileID, baseDef.Flags, baseDef.Sides, baseDef.SourcePosition);
        }


        protected override object GetSaveModelExtra()
        {
            return (int)SpecialType;
        }

        protected override void LoadExtra(object data)
        {
            if (data == null)
                SpecialType = SpecialTile.Normal;
            else 
                SpecialType = (SpecialTile)((long)data);
        }
    }

    public class KCTileInstance : TileInstance
    {
        private KCTileDef mTiledef = new KCTileDef();
        [Browsable(false)]
        public override TileDef TileDef
        {
            get
            {
                return mTiledef;
            }
            set
            {
                var d = value as KCTileDef;
                if (d == null)
                    mTiledef = new KCTileDef(value);
                else
                    mTiledef = d;
            }
        }
        [Browsable(false)]
        public KCTileDef KCTileDef { get { return mTiledef; } }

        public PrizeType Prize { get; set; }

        [Browsable(false)]
        public DirectionFlags EffectSides { get; set; }

        [DisplayName("EffectSides")]
        public EditorDirectionFlags _SidesForEditor
        {
            get
            {
                if (EffectSides == null)
                    return EditorDirectionFlags.None;

                return EffectSides.ToEditorFlags();
            }
            set
            {
                this.EffectSides = new DirectionFlags(value);
            }
        }


        public override CollidingTile CreateCollidingTile(TileCollisionView collisionView)
        {
           
            var flags = this.TileDef.Flags;

            switch (this.KCTileDef.SpecialType)
            {
                case SpecialTile.Prize: return new PrizeBlock(this, collisionView);
                case SpecialTile.Rock: return new RockBlock(this, collisionView);
                case SpecialTile.Metal: return new MetalBlock(this, collisionView);
                case SpecialTile.Rubber: return new RubberBlock(this, collisionView);
                case SpecialTile.Ice: return new IceBlock(this, collisionView);
                case SpecialTile.Vanishing: return new VanishingBlock(this, collisionView);
                case SpecialTile.Mushroom: return new MushroomBlock(this, collisionView);
                default: return new PlainTile(this, collisionView);
            }

        }

        [Browsable(false)]
        public override bool IsSpecial
        {
            get { return this.KCTileDef.SpecialType != SpecialTile.Normal; }
        }

        
    
    }

    class PlainTile : CollidingTile
    {
        public PlainTile(TileInstance tile, TileCollisionView collisionView) : base(tile, collisionView) { }

        public override void HandleCollision(CollisionEvent collision, CollisionResponse response)
        {
        }

        protected override void Update()
        {
            this.Kill(Engine.ExitCode.Finished);
        }
    }

}

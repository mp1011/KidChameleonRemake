using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public enum ObjectEntryType
    {
        NearScreen,
        LevelStart
    }


    public class ObjectEntry : LogicObject
    {

        public int SpriteType { get; private set; }
        public RGPoint Location { get; private set; }
        public ObjectEntryType EntryType { get; private set; }
        public Layer Layer { get; private set; }

        private bool mCreated = false;

        public ObjectEntry(GameContext ctx)
            : base(LogicPriority.World, ctx)
        {

        }

        public ObjectEntry(GameContext ctx, int type, RGPoint loc, ObjectEntryType entry, Layer layer)
            : base(LogicPriority.World, ctx)
        {
            this.SpriteType = type;
            this.Location = loc;
            this.EntryType = entry;
            this.Layer = layer;

            mCreated = true;
        }

        protected override void Update()
        {
            //TBD
            if (!mCreated)
            {
                //todo
                if (this.Location.X < 0)
                    this.Kill(ExitCode.Cancelled);
                else
                {
                    mCreated = true;
                   // var sprite = SpriteCreator.Create(this.SpriteType, this.Context, this.Layer, this.Location);
                  //  Context.Listeners.EditorListener.Register(this, sprite);
                }
            }
        }

    }
}

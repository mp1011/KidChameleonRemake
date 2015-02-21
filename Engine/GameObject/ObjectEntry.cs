using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Engine
{
    public enum ObjectEntryType
    {
        LevelStart,
        NearScreen 
    }

    public class ObjectEntry 
    {
        [Browsable(true)]
        public ObjectType SpriteType { get; set; }

        [Browsable(true)]
        public ObjectEntryType EntryType { get; set; }

        [Browsable(false)]
        public RGPointI Location { get; set; }       
    }

    class ObjectEntryEx
    {
        // [Browsable(false )]
        //public Layer Layer { get;  set; }

        //private bool mCreated = false;

        //public ObjectEntryEx(GameContext ctx)
        //    : base(LogicPriority.World, ctx)
        //{

        //}

        //public ObjectEntryEx(GameContext ctx, ObjectType type, RGPointI loc, ObjectEntryType entry, Layer layer)
        //    : base(LogicPriority.World, ctx)
        //{
        //    this.SpriteType = type;
        //    this.Location = loc;
        //    this.EntryType = entry;
        //    this.Layer = layer;

        //    mCreated = true;
        //}

        //protected override void Update()
        //{
        //    //TBD
        //    if (!mCreated)
        //    {
        //        //todo
        //        if (this.Location.X < 0)
        //            this.Kill(ExitCode.Cancelled);
        //        else
        //        {
        //            mCreated = true;
        //           // var sprite = SpriteCreator.Create(this.SpriteType, this.Context, this.Layer, this.Location);
        //          //  Context.Listeners.EditorListener.Register(this, sprite);
        //        }
        //    }
        //}

    }
}

using Newtonsoft.Json;
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

    class ObjectEntryTypeAttribute : Attribute
    {
        public ObjectEntryType EntryType { get; set; }

        public ObjectEntryTypeAttribute(ObjectEntryType t) { this.EntryType = t; }
    }

    public class ObjectEntry : IWithPosition 
    {
        [Browsable(true)]
        public ObjectType SpriteType { get; set; }

        [Browsable(true)]
        public ObjectEntryType EntryType { get; set; }


        private RGPointI mLocation;

        [Browsable(false)]
        public RGPointI Location
        {
            get { return mLocation; }
            set
            {
                mLocation = value;
                if (PlacedObject != null)
                    PlacedObject.Location = value;
            }
        }

        [Browsable(false)]
        [JsonIgnore]
        public Sprite PlacedObject { get; set; }

        public Sprite CreateObject(Layer layer)
        {
            if (this.PlacedObject == null && !this.SpriteType.IsEmpty)
            {
                this.PlacedObject = this.SpriteType.CreateInstance<Sprite>(layer, layer.Context);
                this.PlacedObject.Location = this.Location;
            }

            return this.PlacedObject;

        }

        [Browsable(false)]
        [JsonIgnore]
        public GameContext Context
        {
            get { return PlacedObject.Context; }
        }

        [Browsable(false)]
        [JsonIgnore]
        public RGRectangleI Area
        {
            get { return PlacedObject.Area; }
        }

        [Browsable(false)]
        [JsonIgnore]
        public Direction Direction
        {
            get { return PlacedObject.Direction; }
        }

        public override string ToString()
        {
            return this.EntryType.ToString()+ " " + this.SpriteType.ToString() + " " + this.Location.ToString();
        }

        public void AddToLayer(Layer layer)
        {
            ActiveObjectEntry.Create(this, layer);
        }
    }

    abstract class ActiveObjectEntry : LogicObject 
    {
        private ObjectEntry mObject;
        private Layer mLayer;     
        private bool mCreated = false;

        public ActiveObjectEntry(ObjectEntry entry, Layer layer)
            : base(LogicPriority.World, layer,RelationFlags.DestroyWhenParentDestroyed)
        {
            mLayer = layer;
            mObject = entry;
        }

        protected override void Update()
        {
            if (mObject.PlacedObject == null && ShouldCreateObject())           
                mObject.CreateObject(mLayer);
        }

        protected abstract bool ShouldCreateObject();


        public static ActiveObjectEntry Create(ObjectEntry e, Layer layer)
        {
            return ReflectionHelper.CreateObjectByAttribute<ActiveObjectEntry, ObjectEntryTypeAttribute>(e, t => t.EntryType == e.EntryType,e,layer);
        }
    }

    [ObjectEntryType(ObjectEntryType.LevelStart)]
    class LevelStartObjectEntry : ActiveObjectEntry
    {
        public LevelStartObjectEntry(ObjectEntry entry, Layer layer) : base(entry, layer) { }

        protected override bool ShouldCreateObject()
        {
            return true;
        }
    }


}

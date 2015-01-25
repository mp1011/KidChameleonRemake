using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public abstract class ObjectTypeRelations
    {
        public abstract bool XisY(ObjectType x, ObjectType y);
    }

    public struct ObjectType : IEquatable<ObjectType>
    {
        private string mName;

        #region Standard Types

        public const int CustomIDStart = 1000;

        public static ObjectType None { get { return new ObjectType(0, "None"); } }
        public static ObjectType Thing { get { return new ObjectType(1, "Thing"); } }
        public static ObjectType Block { get { return new ObjectType(2, "Block"); } }
        public static ObjectType Decoration { get { return new ObjectType(3, "Decoration"); } }
        public static ObjectType Border { get { return new ObjectType(4, "Border"); } }

        #endregion

        public int Value { get; private set; }

        public ObjectType(int value, string name)
            : this()
        {
            Value = value;
            mName = name;
        }

        public bool IsEmpty
        {
            get { return this.Value == 0; }
        }

        public bool IsNot(ObjectType other)
        {
            return !this.Is(other);
        }

        public bool Is(ObjectType other)
        {
            return Core.GameBase.Current.Relations.XisY(this, other);
        }

        public bool Equals(ObjectType other)
        {
            return this.Value == other.Value;
        }

        public override string ToString()
        {
            return mName;
        }
    }

}

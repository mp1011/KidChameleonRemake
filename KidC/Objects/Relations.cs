using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace KidC
{
    public class KCObjectRelations : ObjectTypeRelations
    {      
        public KCObjectRelations()
        {
            SetXisY(KCObjectType.Player, ObjectType.Thing);
            SetXisY(KCObjectType.JamesKid, KCObjectType.Player);
            SetXisY(KCObjectType.IronKnight, KCObjectType.Player);
            SetXisY(KCObjectType.RedStealth, KCObjectType.Player);


            SetXisY(KCObjectType.Collectable, ObjectType.Thing);
            SetXisY(KCObjectType.Gem, KCObjectType.Collectable);
            SetXisY(KCObjectType.Puff, ObjectType.Decoration);

            SetXisY(KCObjectType.Helmet, KCObjectType.Collectable);
            SetXisY(KCObjectType.IronKnightHelmet, KCObjectType.Helmet);
            SetXisY(KCObjectType.RedStealthHelmet, KCObjectType.Helmet);

            SetXisY(KCObjectType.Dragon, KCObjectType.Enemy);
        }


        private Dictionary<ObjectType, List<ObjectType>> mParentChildRelations = new Dictionary<ObjectType, List<ObjectType>>();
        private Dictionary<ObjectType, List<ObjectType>> mChildParentRelations = new Dictionary<ObjectType, List<ObjectType>>();

        private List<ObjectType> GetChildren(ObjectType parent)
        {
            List<ObjectType> children;
            if (!mParentChildRelations.TryGetValue(parent, out children))
            {
                children = new List<ObjectType>();
                mParentChildRelations.Add(parent, children);
            }

            return children;
        }

        private List<ObjectType> GetParents(ObjectType child)
        {
            List<ObjectType> parents;
            if (!mChildParentRelations.TryGetValue(child, out parents))
            {
                parents = new List<ObjectType>();
                mChildParentRelations.Add(child, parents);
            }

            return parents;
        }


        private void SetXisY (ObjectType x, ObjectType y)
        {         
            GetChildren(y).Add(x);
            GetParents(x).Add(y);

            foreach (var grandParent in GetParents(y))
                SetXisY(x, grandParent);
        }

        public override bool XisY(ObjectType x, ObjectType y)
        {
            return x.Equals(y) || GetChildren(y).Contains(x);
        }
    }
}

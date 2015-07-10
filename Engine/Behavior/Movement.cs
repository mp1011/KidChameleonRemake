using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class SimpleMover : LogicObject 
    {
        private MotionVector mVector;
        private IWithPosition mObject;

        public SimpleMover(ILogicObject owner, IWithPosition obj, Direction d, float speed):base(LogicPriority.World,owner)
        {
            mVector = new MotionVector(d, speed);
            mObject = obj;
        }

        protected override void Update()
        {
            mObject.Location = mObject.Location.Offset(mVector.MotionOffset);
        }
    }
}

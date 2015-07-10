using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace KidC
{
    public class WalkController : TriggeredController<Direction?>
    {
        public bool WalksOffLedges { get; set; }
        public float WalkSpeed { get; set; }
        public float WalkAccel { get; set; }

        protected override bool AllowRetrigger { get { return true; } }

        public WalkController(Sprite sprite, float speed, float accel)
            : base(sprite)
        {
            this.WalkSpeed = speed;
            this.WalkAccel = accel;
        }

        public void Walk(Direction d)
        {
            Sprite.MotionManager.MainMotion.Direction = d;
            Sprite.MotionManager.MainMotion.TargetSpeed = WalkSpeed;
            Sprite.Direction = d;
            Sprite.MotionManager.MainMotion.Accel = WalkAccel;
            Sprite.SetAnimation(KCAnimation.Walk);
        }

        public void Stop()
        {
            Sprite.MotionManager.MainMotion.TargetSpeed = 0;
            Sprite.MotionManager.MainMotion.Accel = WalkAccel;
            Sprite.SetAnimation(KCAnimation.Stand);
        }

        protected override void OnEntrance()
        {
            Walk(Sprite.Direction);
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override Switch OnTriggered(Direction? dir)
        {
            if (this.Paused)
                return Switch.Off;

            if (dir.HasValue)
                this.Walk(dir.Value);
            else
                this.Stop();

            return Switch.Off;
        }

        protected override void HandleCollisionEx(Engine.Collision.CollisionEvent cEvent, CollisionResponse response)
        {
            if (cEvent.OtherType.Is(ObjectType.Border))
            {
                if (this.Sprite.Direction == Direction.Left && cEvent.OtherArea.Left < this.Sprite.X)
                    Walk(Direction.Right);
                else if (this.Sprite.Direction == Direction.Right && cEvent.OtherArea.Right > this.Sprite.X)
                    Walk(Direction.Left);
            }

            base.HandleCollisionEx(cEvent, response);
        }
    }
}

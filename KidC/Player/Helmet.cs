﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace KidC
{

    /// <summary>
    /// Handles the differences between helmets
    /// </summary>
    class HelmetController : SpriteBehavior 
    {
        public ObjectType PlayerType { get; private set; }
        public SoundResource TransformSound { get; private set; }

        public HelmetController(Sprite s, ObjectType playerType, SoundResource sound)
            : base(s)
        {
             this.PlayerType = playerType;
             this.TransformSound = sound;
        }

        protected override void HandleCollisionEx(Engine.Collision.CollisionEvent cEvent, CollisionResponse response)
        {
            if (cEvent.OtherType.Is(KCObjectType.Player))
                response.AddInteraction(new PlayerPicksUpHelmet(), this);    
        }

    }

    class TransformationController : TriggeredController<HelmetController>
    {
        private int mDefaultMaxHealth;
        private bool mPlayIntroAnimation = false;
        private bool mTransformToKidOnExit = false;

        private HelmetController mPickedUpHelmet;
        private RGPointI mLockPosition = RGPointI.Empty;
        protected override bool AllowRetrigger { get { return false; } }
    
        public TransformationController(Sprite s, int maxHealth)
            : base(s) 
        {
            this.mDefaultMaxHealth = maxHealth;
        }

        //Player picks up a helmet, which triggers this controller
        protected override void HandleCollisionEx(Engine.Collision.CollisionEvent cEvent, CollisionResponse response)
        {
            if (cEvent.OtherType.Is(KCObjectType.Helmet))
                response.AddInteraction(new PlayerPicksUpHelmet(), this);
        }

        //Upon being triggered, we play the sound for the new helmet and begin the transition out animation
        protected override Switch OnTriggered(HelmetController state)
        {
            mPickedUpHelmet = state;
            mLockPosition = this.Sprite.Location;
            this.Sprite.LockDirection(this.Sprite.Direction, this);

            if (state != null)
            {
                SoundManager.PlaySound(mPickedUpHelmet.TransformSound);
                this.Sprite.SetAnimation(KCAnimation.TransitionOut,this,true);
            }

            return Switch.On;
        }

        protected override Switch OnTriggerUpdate(HelmetController state)
        {
            if (this.Sprite.CurrentAnimationKey == KCAnimation.TransitionOut)
            {
                this.Sprite.Location = mLockPosition;
                this.Sprite.MotionManager.StopAllMotion();
                if (this.Sprite.CurrentAnimation.Finished)
                    return Switch.Off;
                else
                    return Switch.On;
            }
            if (this.Sprite.CurrentAnimationKey == KCAnimation.TransitionIn)
            {
                this.Sprite.Location = mLockPosition;
                if (this.Sprite.CurrentAnimation.Finished)
                    return Switch.Off;
                else
                    return Switch.On;
            }
            else
                return Switch.Off;
         
           

        }

        protected override void OnTriggerStop()
        {
            this.Sprite.ReleaseAnimationKeyLock(this);
            this.Sprite.UnlockDirection(this);

            if(mPickedUpHelmet != null)
                DoTransform();
        }

        protected override void OnEntrance()
        {
            this.Context.GetStats().MaxHealth = mDefaultMaxHealth;
            this.Context.GetStats().CurrentHealth = mDefaultMaxHealth;

            if (mPlayIntroAnimation)
            {
                this.Sprite.SetAnimation(KCAnimation.TransitionIn,this,true);
                if (this.Sprite.CurrentAnimationKey == KCAnimation.TransitionIn)
                    this.Trigger(null);
            }
        }

        protected override void OnExit()
        {
            if (this.ExitCode == Engine.ExitCode.Destroyed && mTransformToKidOnExit)
            {
                var transform = this.Sprite.Copy(false, false);
                transform.SetSingleAnimation(this.Sprite.GetAnimation(KCAnimation.TransitionOut).Animation);
                new DestroyWhenAnimationFinished(transform);
                new CreateObjectWhenDestroyed(transform, KCObjectType.JamesKid, RGPointI.Empty,s=>
                    {
                        SoundManager.PlaySound(Sounds.Bummer);
                        var hitCtl = s.GetBehavior<PlayerHitController>();
                        hitCtl.Trigger(new HitInfo { Damage = 0 });                        
                    });
            }
        }

        private void DoTransform()
        {
            this.Sprite.Kill(Engine.ExitCode.Removed);
            var newCharacter = ObjectFactory.CreateSprite(mPickedUpHelmet.PlayerType, this.Sprite.DrawLayer, this.Context);

            var ctl = newCharacter.GetBehavior<TransformationController>();
            ctl.mPlayIntroAnimation = true;
            ctl.mTransformToKidOnExit = true;

            newCharacter.Sprite.Location = this.Sprite.Location;
            newCharacter.Sprite.Direction = this.Sprite.Direction;
            this.Sprite.DrawLayer.AddObject(newCharacter.Sprite);
        }

     

       
    }

    class PlayerPicksUpHelmet : Interaction<TransformationController, HelmetController>
    {
        protected override void DoAction(TransformationController controller1, HelmetController controller2)
        {
            controller1.Trigger(controller2);
        }
    }
}

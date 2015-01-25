using System;
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

    class TransformationController : SpriteBehavior
    {
        private int mDefaultMaxHealth;
        private bool mPlayIntroAnimation = false;
        private HelmetController mPickedUpHelmet;

        public TransformationController(Sprite s, int maxHealth) : base(s) 
        {
            this.mDefaultMaxHealth = maxHealth;
        }

        public void OnHelmetPickedUp(HelmetController ctl)
        {
            mPickedUpHelmet = ctl;
        }

        protected override void OnEntrance()
        {
            this.Context.GetStats().MaxHealth = mDefaultMaxHealth;
            this.Context.GetStats().CurrentHealth = mDefaultMaxHealth;

            if (mPlayIntroAnimation)
            {               
                this.Sprite.CurrentAnimationKey = KCAnimation.TransitionIn;
                if (this.Sprite.CurrentAnimationKey == KCAnimation.TransitionIn)
                    this.Sprite.PauseOtherBehaviors(this);
            }
        }

        protected override void Update()
        {
            if (this.Sprite.CurrentAnimationKey == KCAnimation.TransitionIn)
            {
                this.Sprite.MotionManager.StopAllMotion();
                if (this.Sprite.CurrentAnimation.Finished)                
                    this.Sprite.ResumeAllBehaviors();                     
                return;
            }

            if (this.Sprite.CurrentAnimationKey == KCAnimation.TransitionOut)
            {
                this.Sprite.MotionManager.StopAllMotion();
                if (this.Sprite.CurrentAnimation.Finished)
                    this.Sprite.ResumeAllBehaviors();


                if (this.Sprite.CurrentAnimation.Finished || this.Sprite.CurrentAnimationKey != KCAnimation.TransitionOut)
                    DoTransform();

                return;
            }

            if (mPickedUpHelmet != null)
            {
                this.Sprite.CurrentAnimationKey = KCAnimation.TransitionOut;
                SoundManager.PlaySound(mPickedUpHelmet.TransformSound);

                if(this.Sprite.CurrentAnimationKey == KCAnimation.TransitionOut)
                    this.Sprite.PauseOtherBehaviors(this);
                else 
                    DoTransform();
            }
            
        }

        private void DoTransform()
        {
            this.Sprite.Kill(Engine.ExitCode.Removed);
            var newCharacter = ObjectFactory.CreateSprite(mPickedUpHelmet.PlayerType, this.Sprite.DrawLayer, this.Context);

            var transformCtl = newCharacter.GetBehavior<TransformationController>();
            transformCtl.mPlayIntroAnimation = true;
            newCharacter.Location = this.Sprite.Location;
            this.Sprite.DrawLayer.AddObject(newCharacter);
        }

        protected override void HandleCollisionEx(Engine.Collision.CollisionEvent cEvent, CollisionResponse response)
        {
            if (cEvent.OtherType.Is(KCObjectType.Helmet))
               response.AddInteraction(new PlayerPicksUpHelmet(),this);            
        }
    }

    class PlayerPicksUpHelmet : Interaction<TransformationController, HelmetController>
    {
        protected override void DoAction(TransformationController controller1, HelmetController controller2)
        {
            controller1.OnHelmetPickedUp(controller2);
        }
    }
}

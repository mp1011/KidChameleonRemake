using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public abstract class SceneTransition : LogicObject , ITriggerable<WorldInfo>
    {
        private WorldInfo mNextWorld;
        private TransitionState mCurrentState = TransitionState.None;

        private ulong mStartFrame;

        public int OutDuration { get { return 60; } }
        public int InDuration { get { return 60; } }

        protected SceneTransition(World current) : base(LogicPriority.World, current) { }

        public int CurrentStateDuration
        {
            get
            {
                if (mCurrentState == TransitionState.In)
                    return InDuration;
                else if (mCurrentState == TransitionState.Out)
                    return OutDuration;
                else
                    return 0;
            }
        }

        enum TransitionState
        {
            None,
            Out,
            In
        }

        protected override void Update()
        {

            if (mCurrentState == TransitionState.None)
                return;

            var statePercentage = (float)(Context.CurrentFrameNumber - mStartFrame) / (float)this.CurrentStateDuration;
            if (statePercentage >= 1f)
            {
                if (mCurrentState == TransitionState.Out)
                {
                    Context.CurrentWorld.Kill(Engine.ExitCode.Finished);
                    mStartFrame = Context.CurrentFrameNumber;
                    mCurrentState = TransitionState.In;
                    Context.SetWorld(mNextWorld.CreateWorld(this.Context));
                }
                else
                {
                    this.Kill(Engine.ExitCode.Finished);
                }
            }
            else
                UpdateTransition(statePercentage);

        }

        public void Trigger(WorldInfo arg)
        {
            mNextWorld = arg;
            mCurrentState = TransitionState.Out;
            mStartFrame = this.Context.CurrentFrameNumber;
        }

        public bool Triggered
        {
            get { return mNextWorld != null; }
        }

        private void UpdateTransition(float percentage)
        {
            if (mCurrentState == TransitionState.Out)
                DoTransitionOut(percentage);
            else if (mCurrentState == TransitionState.In)
                DoTransitionIn(percentage);
        }

        protected abstract void DoTransitionIn(float percentage);
        protected abstract void DoTransitionOut(float percentage);
    }



    public class FadeoutSceneTransition : SceneTransition 
    {

        public FadeoutSceneTransition(World current) : base(current) { }

        private WorldInfo mNewInfo;
        private RGColor mOriginalSkyColor;

        public FadeoutSceneTransition(World current, WorldInfo newWorld)
            : base(current)
        {
            mNewInfo = newWorld;
        }


        protected override void OnEntrance()
        {
            if (mNewInfo != null)
                this.Trigger(mNewInfo);

            mOriginalSkyColor = this.Context.CurrentWorld.BackgroundColor;
            base.OnEntrance();
        }

        protected override void DoTransitionIn(float percentage)
        {
            this.Context.CurrentWorld.ExtraRenderInfo.FadeColor = RGColor.Black.Fade(RGColor.White, percentage);           
        }

        protected override void DoTransitionOut(float percentage)
        {
            this.Context.CurrentWorld.ExtraRenderInfo.FadeColor = RGColor.White.Fade(RGColor.Black, percentage);
            this.Context.CurrentWorld.BackgroundColor = mOriginalSkyColor.Fade(RGColor.Black, percentage);
        }



    }
}

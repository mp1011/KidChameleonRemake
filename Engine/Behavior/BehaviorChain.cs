using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{

    public static class BehaviorChainExtensions
    {
        public static T ContinueWith<T>(this ILogicObject o, T newBehavior) where T : ILogicObject
        {
            new OnExitWaiter(o, newBehavior);
            return newBehavior;
        }

        public static ILogicObject ContinueWithMulti(this ILogicObject o, params ILogicObject[] newBehavior) 
        {
            return o.ContinueWith(new OnAllFinishedWaiter(o, newBehavior));
        }
    }

    class OnAllFinishedWaiter : LogicObject
    {
        private ILogicObject[] mObjects;
   
        public OnAllFinishedWaiter(ILogicObject owner, params ILogicObject[] objects)
            : base(LogicPriority.World, owner, RelationFlags.None)
        {
            mObjects = objects;
            foreach (var obj in objects)
                obj.Paused = true;
        }

        protected override void Update()
        {
            foreach (var obj in mObjects)
                obj.Paused = false;

            foreach (var obj in mObjects)
                if (obj.Alive)
                    return;


            this.Kill(Engine.ExitCode.Finished);
        }


    }

    public class OnExitWaiter : LogicObject
    {
        private ILogicObject mWaitObject;
        private ILogicObject mContinuation;

        public OnExitWaiter(ILogicObject waitObject, ILogicObject continuation)
            : base(LogicPriority.Behavior, waitObject, RelationFlags.Normal)
        {
            mWaitObject = waitObject;
            mContinuation = continuation;
            mContinuation.Paused = true;
        }

        protected override void OnExit()
        {
            mContinuation.Paused = false;
        }
    }

    public class PlaySound : ILogicObject
    {

        private SoundResource mSound;
        private bool mPlayedSound=false;

        public PlaySound(SoundResource sound)
        {
            mSound = sound;
            
        }

        public bool Alive
        {
            get 
            {
                return this.ExitCode == Engine.ExitCode.StillAlive; 
            }
        }

        private bool mPaused = true;
        public bool Paused
        {
            get
            {
                return mPaused;
            }
            set
            {
                if (!value && !mPlayedSound)
                {
                    SoundManager.PlaySound(mSound);
                    mPlayedSound = true;
                }

                mPaused = value;
            }
        }

        public ExitCode ExitCode
        {
            get
            {
                if (mPlayedSound && !SoundManager.IsSoundPlaying(mSound))
                    return ExitCode.Finished;
                else
                    return ExitCode.StillAlive;
            }
        }

        public void Kill(ExitCode exitCode)
        {           
        }

        public GameContext Context
        {
            get { return Core.GlobalToken.Instance.Context; }
        }
    }

    public class DelayWaiter : LogicObject
    {
        private ulong mStartTime;
        private float mWaitSeconds;
        private bool mWasPaused;

        public DelayWaiter(ILogicObject owner, float seconds)
            : base(LogicPriority.Behavior, owner)
        {
            mStartTime = this.Context.CurrentFrameNumber;
            mWaitSeconds = seconds;
        }

        protected override void Update()
        {
            if (this.Paused != mWasPaused)
            {
                mStartTime = Context.CurrentFrameNumber;
                mWasPaused = this.Paused;
            }

            if (this.Context.ElapsedSecondsSinceF(mStartTime) >= mWaitSeconds)
                this.Kill(Engine.ExitCode.Finished);
        }
    }


 }

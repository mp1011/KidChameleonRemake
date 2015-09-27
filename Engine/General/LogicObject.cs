using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public enum LogicPriority
    {
        Input = 0,
        Motion = 1,
        Collision = 2,
        AfterCollision = 3,
        Behavior = 4,
        World = 5
    }

    public class Timer
    {
        private GameContext mContext;

        public Timer(GameContext ctx) { mContext = ctx; }


        public bool OnInterval(ulong startTime, int frames)
        {
            var currentFrame = mContext.CurrentFrameNumber;
            if (currentFrame <= startTime)
                return false;

            if (frames == 0)
                return true;

            return (currentFrame - startTime) % (ulong)frames == 0;
        }

        public bool OnInterval(int frames)
        {
            return OnInterval(0, frames);
        }
    }


    class LoopManager
    {

        private LinkedList<LogicObject>[] mObjects = new LinkedList<LogicObject>[6];

        private LinkedList<LogicObject> mAddedObjects = new LinkedList<LogicObject>();

        public LoopManager()
        {
            for (int i = 0; i < Enum.GetValues(typeof(LogicPriority)).Length; i++)
                mObjects[i] = new LinkedList<LogicObject>();
        }

        public void AddObject(LogicObject obj)
        {
            mAddedObjects.AddLast(obj);
        }

        public void Update()
        {
            for (int i = 0; i < mObjects.Length; i++)
            {
                foreach (var obj in mObjects[i])
                    obj.UpdateX();
            }

            foreach (var obj in mAddedObjects)
                mObjects[(int)obj.Priority].AddLast(obj);

            mAddedObjects.Clear();

            for (int i = 0; i < mObjects.Length; i++)
            {
                foreach (var o in mObjects[i])
                {
                    if (!o.Alive)
                        o.Kill(ExitCode.Default);
                }

                mObjects[i].RemoveAll(p => !p.Alive);
            }            
        }
    }

    public enum ExitCode
    {
        StillAlive = 0,
        Removed = 1,
        Destroyed = 2,
        Collected = 3,
        Cancelled = 4,
        Finished = 5,
        Default = 100
    }

    public enum RelationFlags
    {
        None = 0,
        DestroyWhenParentDestroyed = 1,
        PauseWhenParentPaused = 2,
        Normal = 3,        
    }

    public interface ILogicObject
    {
        bool Alive { get; }
        bool Paused { get; set; }
        ExitCode ExitCode { get; }
        void Kill(ExitCode exitCode);
        GameContext Context { get; }
    }

    public class GeneralLogicObject : ILogicObject
    {

        private ILogicObject mParent;

        public GeneralLogicObject(ILogicObject parent)
        {
            this.mParent = parent;
        }

        public bool Alive
        {
            get
            {
                return mParent.Alive && this.ExitCode == Engine.ExitCode.StillAlive;
            }          
        }

        private bool mPaused;
        public bool Paused
        {
            get { return mPaused || this.mParent.Paused; }
            set { mPaused=value;}
        }

        public ExitCode ExitCode
        {
            get;
            private set;
        }

        public void Kill(ExitCode exitCode)
        {
            this.ExitCode = exitCode;
        }

        public GameContext Context
        {
            get { return mParent.Context; }
        }

        public void Pause()
        {
            this.Paused = true;
        }
        public void Resume()
        {
            this.Paused = false;
        }
    }

    public abstract class LogicObject : ILogicObject 
    {

        private ILogicObject Parent { get; set; }

        private bool mAlive = false;
        public bool Alive
        {
            get
            {
                if ((mRelationFlags & RelationFlags.DestroyWhenParentDestroyed) == 0)
                    return mAlive;

                return mAlive && (Parent == null || Parent.Alive);
            }
            private set
            {
                mAlive = value;
                if (value)
                    mLastUnpauseTime = mContext.CurrentFrameNumber;
            }
        }

        private ulong mLastUnpauseTime = 0;
        private bool mPaused = false;
        public bool Paused
        {
            get
            {
                if ((mRelationFlags & RelationFlags.PauseWhenParentPaused) == 0)
                    return mPaused;

                return mPaused || (Parent != null && Parent.Paused);
            }
            set
            {
                if (mPaused && !value)
                    mLastUnpauseTime = mContext.CurrentFrameNumber;

                mPaused = value;
               
            }
        }

        public bool Started { get { return StartFrame != 0; } }

        public ulong StartFrame { get; private set; }

        public ulong TimeActive { get { return Context.CurrentFrameNumber - mLastUnpauseTime; } }

        public ulong Age { get { return Context.CurrentFrameNumber - StartFrame; } }

        public ExitCode ExitCode { get; private set; }

        private GameContext mContext;

        public LogicPriority Priority { get; private set; }
        public int ID { get; private set; }

        public GameContext Context { get { return mContext; } }

        private RelationFlags mRelationFlags;

        protected LogicObject(LogicPriority priority, ILogicObject parent, RelationFlags relationFlags)
        {
            this.mRelationFlags = relationFlags;
            this.Parent = parent;
            UpdateContext(priority, parent.Context);
        }

        protected LogicObject(LogicPriority priority, ILogicObject parent)
        {
            this.mRelationFlags = RelationFlags.Normal;
            this.Parent = parent;
            UpdateContext(priority, parent.Context);
        }

        //protected LogicObject(LogicPriority priority, GameContext context, bool bindToCurrentWorld)
        //{
        //    this.mRelationFlags = RelationFlags.Normal;
        //    UpdateContext(priority, context);
        //}


        protected void UpdateContext(LogicPriority priority, GameContext ctx)
        {
            this.Priority = priority;
            this.mContext = ctx;
            this.ID = GetNextID();
            this.Alive = true;
            ctx.AddObject(this);
        }


        public void Kill(ExitCode exitCode)
        {
            if (exitCode == Engine.ExitCode.Default)
            {
                if (this.Parent != null && !this.Parent.Alive)
                    exitCode = this.Parent.ExitCode;
                else
                    exitCode = Engine.ExitCode.Removed;
            }

            if (this.ExitCode != Engine.ExitCode.StillAlive)
                return;

            this.ExitCode = exitCode;
            this.Alive = false;
            this.OnExit();
        }

        private object mLock = new object();
        private static int NextID = 0;
        private int GetNextID()
        {
            int ret = 0;
            lock (mLock)
            {
                NextID++;
                ret = NextID;
            }

            return ret;
        }
        private bool mRanUpdate = false;

        private bool mWasPaused = false;

        public void UpdateX() //todo, naming
        {
            if (this.Paused)
            {
                mWasPaused = true;
                return;
            }

            if (mWasPaused)
            {
                mWasPaused = false;
                OnResume();
            }
            
            if (!mRanUpdate)
            {
                mRanUpdate = true;
                this.StartFrame = Context.CurrentFrameNumber;
                this.OnEntrance();
            }

            this.Update();
        }

        protected virtual void OnResume() { }

        protected virtual void Update()
        {
        }

        protected virtual void OnEntrance() { }
        protected virtual void OnExit() { }


        public void Pause()
        {
            this.Paused = true;
        }

        public void Resume()
        {
            this.Paused = false;
        }

        public void ChangeParent(ILogicObject newParent)
        {
            this.Parent = newParent;
        }
    }


    



}

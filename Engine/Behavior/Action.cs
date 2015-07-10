using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public abstract class Action : ILogicObject 
    {
        protected Action(ILogicObject parent)
        {
            this.Paused = true;
            this.Context = parent.Context;
        }

        public void Start()
        {
            this.ExitCode = DoAction();
        }

        protected abstract ExitCode DoAction();

        public bool Alive
        {
            get { return this.ExitCode == Engine.ExitCode.StillAlive; }
        }

        private bool mPaused;
        public bool Paused
        {
            get
            {
                return mPaused;
            }
            set
            {
                if (mPaused && !value && this.ExitCode == Engine.ExitCode.StillAlive)                
                    this.ExitCode = DoAction();
                
                mPaused = value;
            }
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
            get;
            private set;
        }
    }

    public class GenericAction : Action
    {
        private Func<GameContext, ExitCode> fnAction;

        public GenericAction(ILogicObject owner, Func<GameContext, ExitCode> action, bool startNow)  : base(owner)
        {
            fnAction = action;
            if (startNow)
                Start();
        }

        protected override ExitCode DoAction()
        {
            return fnAction(this.Context);
        }
    }
}

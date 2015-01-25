using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{

    class BehaviorChain : SpriteBehavior
    {
        private LinkedList<SpriteBehavior> mBehaviors;

        public BehaviorChain(Sprite s, params SpriteBehavior[] behaviors)
            : base(s, RelationFlags.DestroyWhenParentDestroyed)
        {
            mBehaviors = new LinkedList<SpriteBehavior>(behaviors);

            foreach (var behavior in mBehaviors.Skip(1))
                behavior.Pause();
        }

        protected override void Update()
        {
            var currentBehavior = mBehaviors.FirstOrDefault();
            if (!currentBehavior.Alive)
            {

                if (currentBehavior.ExitCode != ExitCode.Finished)
                {
                    this.Kill(currentBehavior.ExitCode);

                    foreach (var behavior in mBehaviors.Skip(1))
                        behavior.Kill(currentBehavior.ExitCode);

                    return;
                }

                mBehaviors.RemoveFirst();

                currentBehavior = mBehaviors.FirstOrDefault();
                if (currentBehavior == null)
                {
                    this.Kill(ExitCode.Finished);
                    return;
                }
                else
                    currentBehavior.Resume();
            }
        }
    }

}

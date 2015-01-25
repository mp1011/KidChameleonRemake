using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class CollisionResponse
    {
        public bool ShouldContinueHandling { get; set;}
        public bool ShouldBlock { get; set;}
        public float BounceModifier { get; set;}
      
        public RGPoint CorrectionVector { get; set; }
        public RGPoint? NewLocation { get; set; }

        public ExitCode DestroyType { get; set; }

        private List<IInteraction> mInteractions = new List<IInteraction>();
        
        public CollisionResponse()
        {
            ShouldContinueHandling = true;
            DestroyType = ExitCode.StillAlive;
        }

        public CollisionResponse(CollisionResponse original)
        {
            mInteractions = original.mInteractions;
            DestroyType = ExitCode.StillAlive;
            ShouldContinueHandling = true;
        }

        public bool ShouldDestroy { get { return this.DestroyType != ExitCode.StillAlive; } }

        public void AddInteraction<T>(T interaction, object controller) where T:IInteraction 
        {
            foreach (var i in mInteractions.OfType<T>())
            {
                if (i.Register(controller))
                    return;
            }

            mInteractions.Add(interaction);
            interaction.Register(controller);
        }



        public CollisionResponse Clone()
        {
            return new CollisionResponse(){ ShouldBlock = this.ShouldBlock, ShouldContinueHandling = this.ShouldContinueHandling, BounceModifier = this.BounceModifier, mInteractions = this.mInteractions};
        }
    }
}

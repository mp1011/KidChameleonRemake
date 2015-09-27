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
      
        public RGPointI CorrectionVector { get; set; }
        public RGPointI? NewLocation { get; set; }

        public ExitCode DestroyType { get; set; }

        public CollisionResponse()
        {
            ShouldContinueHandling = true;
            DestroyType = ExitCode.StillAlive;
        }

        public CollisionResponse(CollisionResponse original)
        {
            DestroyType = ExitCode.StillAlive;
            ShouldContinueHandling = true;
        }

        public bool ShouldDestroy { get { return this.DestroyType != ExitCode.StillAlive; } }

        public CollisionResponse Clone()
        {
            return new CollisionResponse(){ ShouldBlock = this.ShouldBlock, ShouldContinueHandling = this.ShouldContinueHandling, BounceModifier = this.BounceModifier};
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Engine.Collision
{
    public abstract class CollisionManager<T> : LogicObject where T : LogicObject, ICollidable
    {
        protected T CollidingObject { get; private set; }

        protected LayerDepth LayerDepth { get { return CollidingObject.LayerDepth; } }

        public CollisionManager(T obj)
            : base(LogicPriority.Collision, obj, RelationFlags.DestroyWhenParentDestroyed)
        {
            this.CollidingObject = obj;
        }

        protected abstract IEnumerable<CollisionEvent> CheckCollisions(Layer layer);

        private IEnumerable<CollisionEvent> CheckCollisions()
        {
            if (CollidingObject.Location.X >= 0 && CollidingObject.Location.Y >= 0)
            {
                foreach (var layer in Context.CurrentWorld.GetLayers(this.LayerDepth))
                {
                    foreach (var collisionEvent in CheckCollisions(layer))
                        yield return collisionEvent;
                }
            }
        }

        protected override void Update()
        {

            bool onSlope = false;

            foreach (var collisionEvent in this.CheckCollisions())
            {
                collisionEvent.CalcCollisionSpeed(this.CollidingObject);

                CollisionResponse response;

                if (collisionEvent.IsSloped)
                {
                    onSlope = true;
                    response = HandleSlopeCollision(collisionEvent);
                }
                else if (collisionEvent.IsBlocking && !onSlope) //once on a slope, we don't check normal collisions anymore
                    response = HandleBlockingCollision(collisionEvent);
                else
                    response = new CollisionResponse();

                CollidingObject.HandleCollision(collisionEvent, response);
                collisionEvent.OtherObject.HandleCollision(collisionEvent.Invert(), new CollisionResponse(response));

              // if (!response.ShouldContinueHandling)
                   // return;

                if (response.ShouldBlock)
                {
                    if (response.NewLocation.HasValue)
                        CollidingObject.Location = response.NewLocation.Value;
                    else if (!response.CorrectionVector.IsEmpty)
                        CollidingObject.Location = CollidingObject.Location.Offset(response.CorrectionVector);
                }
              
                if (response.ShouldDestroy)
                    CollidingObject.Kill(response.DestroyType);
            }
        }

        private CollisionResponse HandleBlockingCollision(CollisionEvent collisionEvent)
        {
            var originalArea = CollidingObject.Area;
            var originalCArea = collisionEvent.OtherArea;

            var collisionLocation = RGRectangleI.Create(CollidingObject.Location, CollidingObject.Area.Size);

            var coA = CollidingObject.Area;
            var oA = collisionEvent.OtherArea;

            var leftCorrection = new CorrectionRec { Side = Side.Right, Rec = collisionEvent.OtherLeftExposed ? RGRectangleI.FromXYWH(oA.Left - coA.Width, coA.Y, coA.Width, coA.Height) : RGRectangleI.Empty };
            var rightCorrection = new CorrectionRec { Side = Side.Left, Rec = collisionEvent.OtherRightExposed ? RGRectangleI.FromXYWH(oA.Right, coA.Y, coA.Width, coA.Height) : RGRectangleI.Empty };
            var upCorrection = new CorrectionRec { Side = Side.Bottom, Rec = collisionEvent.OtherTopExposed ? RGRectangleI.FromXYWH(coA.X, oA.Top - coA.Height, coA.Width, coA.Height) : RGRectangleI.Empty };
            var downCorrection = new CorrectionRec { Side = Side.Top, Rec = collisionEvent.OtherBottomExposed ? RGRectangleI.FromXYWH(coA.X, oA.Bottom, coA.Width, coA.Height) : RGRectangleI.Empty };

            var possibleRectangles = new List<CorrectionRec>();
            var m = CollidingObject.MotionManager.Vector.MotionOffset;
            if (m.X >= 0)
                possibleRectangles.Add(leftCorrection);
            if (m.X <= 0)
                possibleRectangles.Add(rightCorrection);

            if (m.Y >= 0)
                possibleRectangles.Add(upCorrection);
            if (m.Y <= 0)
                possibleRectangles.Add(downCorrection);

            possibleRectangles = possibleRectangles.Where(p => !p.Rec.IsEmpty).ToList();

            CorrectionRec correctionRectangle = new CorrectionRec { Rec = RGRectangleI.Empty };

            if (possibleRectangles.Count == 0)
                return new CollisionResponse { ShouldBlock = false };
            else if (possibleRectangles.Count == 1)
                correctionRectangle = possibleRectangles[0];
            else
            {
                var collidedArea = this.GetCollisionLocation(collisionEvent);
                if (collidedArea.IsEmpty)
                {
                    collidedArea = originalArea;
                    // continue;
                }

                correctionRectangle = possibleRectangles.MinElement(p => p.Rec.Center.GetDistanceTo(collidedArea.Center));
            }

            var correctionVector = correctionRectangle.Rec.TopLeft.Difference(CollidingObject.Area.TopLeft);
      
            collisionEvent.CollisionSide = correctionRectangle.Side;

            var collisionResponse = new CollisionResponse() { CorrectionVector = correctionVector, ShouldBlock = true };
            var finalX = CollidingObject.Location.X + correctionVector.X;
            var finalY = CollidingObject.Location.Y + correctionVector.Y;

            collisionResponse.NewLocation = new RGPointI(finalX, finalY);
            return collisionResponse;
        }

        private CollisionResponse HandleSlopeCollision(CollisionEvent collisionEvent)
        {
            var correctionVector = new RGPointI(CollidingObject.Location.X-collisionEvent.SlopeIntersectionPoint.X, CollidingObject.Location.Y - collisionEvent.SlopeIntersectionPoint.Y);
            return new CollisionResponse { ShouldBlock=true, CorrectionVector = correctionVector, NewLocation = new RGPointI(CollidingObject.Location.X, collisionEvent.SlopeIntersectionPoint.Y) };
        }

        private RGPointI GetCorrectionVector(RGRectangleI collidingRec, RGPointI pt, RGPoint motionVector, RGLine hSide, RGLine vSide)
        {

            var motionLine = new RGLine(pt, pt.Offset(motionVector.Reverse())).ExtendB(motionVector.Magnitude);

            if (hSide.Length > 0f)
            {
                var collisionPoint = motionLine.GetIntersectionPoint(hSide.Extend());
                if (!collisionPoint.IsInfinity)
                {
                    return collisionPoint.Difference(pt);
                }
            }

            if (vSide.Length > 0f)
            {
                var collisionPoint = motionLine.GetIntersectionPoint(vSide.Extend());
                if (!collisionPoint.IsInfinity)
                {
                    var dist = hSide.PointA.Y - collisionPoint.Y;
                    var pt2 = pt.Offset(0, (int)dist);
                    if (collidingRec.Contains(pt2))
                        return collisionPoint.Difference(pt);
                }
            }

            return RGPointI.Empty;

        }

        private RGRectangleI GetCollisionLocationI(CollisionEvent cEvent)
        {
            return GetCollisionLocation(cEvent);
        }

        private RGRectangleI GetCollisionLocation(CollisionEvent cEvent)
        {
            var correctionVector = CollidingObject.MotionManager.Vector.MotionOffset.Reverse().ToPointI();

            var closeRec = CollidingObject.Area; 
            var farRec = closeRec.Offset(correctionVector);

            while (closeRec.CollidesWith(cEvent.OtherArea) && !farRec.CollidesWith(cEvent.OtherArea))
            {
                var difference = farRec.TopLeft.Difference(closeRec.TopLeft);
                if (difference.Magnitude <= 1)
                    return farRec;

                difference = difference.Scale(.5f, .5f);
                if (difference.IsEmpty)
                    return farRec;

                var mid = closeRec.Offset(difference);

                if (mid.CollidesWith(cEvent.OtherArea))
                    closeRec = mid;
                else
                    farRec = mid;
            }

            return farRec;

        }

    }

}
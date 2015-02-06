using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class AnimationFrame : IHasHitboxes 
    {
        public RGRectangleI Source;

        public RGRectangleI HitBox = RGRectangleI.Empty;

        public RGRectangleI SecondaryHitbox { get; set; }

        public RGPointI Origin;

        RGRectangleI IHasHitboxes.PrimaryHitbox { get { return this.HitBox; } set { this.HitBox = value; } }

    }

    public class DirectedAnimation 
    {
        public Direction Direction { get; private set; }
        private List<AnimationFrame> mFrames = new List<AnimationFrame>();

        public IEnumerable<AnimationFrame> Frames { get { return mFrames; } }

        public bool FlipX { get; private set; }
        public bool FlipY { get; private set; }

        public int FrameDuration { get; set; }

        public DirectedAnimation() { }

        public DirectedAnimation(Direction dir)
        {
            this.Direction = dir;
        }

        public AnimationFrame AddFrame()
        {
            var frame = new AnimationFrame();
            mFrames.Add(frame);
            return frame;
        }

        public AnimationFrame AddFrame(AnimationFrame frame)
        {
            mFrames.Add(frame);
            return frame;
        }

        public AnimationFrame GetFrame(int index)
        {
            return mFrames[index];
        }

        public DirectedAnimation Copy(Direction dir, bool flipX, bool flipY)
        {
            var newDir = new DirectedAnimation();
            newDir.Direction = dir;
            newDir.FlipX = flipX;
            newDir.FlipY = flipY;
            newDir.FrameDuration = FrameDuration;
            newDir.mFrames = new List<AnimationFrame>();
            newDir.mFrames.AddRange(this.Frames);
            return newDir;
        }
    }

    public class Animation
    {
        private TextureResource mTexture;
        private List<DirectedAnimation> mAnimations = new List<DirectedAnimation>();
        public IEnumerable<DirectedAnimation> Animations { get { return mAnimations; } }

        public TextureResource Texture { get { return mTexture; } }
        public bool Looping { get; private set;}

        public Animation()
        {
        }

        public Animation(TextureResource texture)
        {
            mTexture = texture;
        }

        public Animation(SpriteSheet spriteSheet, Direction dir, params int[] frames) : this(spriteSheet,dir,true,frames)
        {
        }

        public Animation(SpriteSheet spriteSheet, Direction dir, bool looping, params int[] frames)
        {
            this.Looping = looping;
            mTexture = spriteSheet.Image;
            AddDirectedAnimation(spriteSheet, dir, frames);
        }

        public void SetFrameDuration(int duration)
        {
            foreach (var a in mAnimations)
                a.FrameDuration = duration;
        }

        public DirectedAnimation GetDirectedAnimation(Direction dir)
        {
            var anim = mAnimations.FirstOrDefault(p => p.Direction == dir);
            if (anim != null)
                return anim;

            anim = mAnimations.FirstOrDefault(p => dir.Reflect(Orientation.Horizontal) == p.Direction);
            if (anim != null)
            {
                anim = anim.Copy(dir, true, false);
                mAnimations.Add(anim);
            }

            return mAnimations[0];
        }

        public DirectedAnimation AddDirectedAnimation(Direction dir)
        {
            var anim = new DirectedAnimation(dir);
            mAnimations.Add(anim);
            return anim;
        }


        public DirectedAnimation AddDirectedAnimation(SpriteSheet spriteSheet, Direction dir, params int[] frames)
        {
            var anim = this.AddDirectedAnimation(dir);
            foreach (int frameNum in frames)
                anim.AddFrame(spriteSheet.Frames[frameNum]);

            return anim;
        }
    }

    public class SpriteAnimation : LogicObject, IDrawable, IHasTextureInfo
    {
        private IWithPosition mSprite;
        private Animation mAnimation;

        private int mCurrentFrame = 0;

        public Animation Animation { get { return mAnimation;}}

        public DirectedAnimation CurrentDirectedAnimation { get { return mAnimation.GetDirectedAnimation(mSprite.Direction); } }
        public AnimationFrame CurrentDirectedAnimationFrame { get { return this.CurrentDirectedAnimation.GetFrame(mCurrentFrame); } }

        public bool Looping { get { return mAnimation.Looping; } }
        public bool Finished { get; private set; }

        public float SpeedModifier { get; set; }

        public SpriteAnimation(IWithPosition sprite, Animation animation, RenderOptions options)
            : base(LogicPriority.Behavior, sprite.Context)
        {
            mSprite = sprite;
            mAnimation = animation;
            SpeedModifier = 1f;
            mRenderOptions = options;
        }

        public void Reset()
        {
            mCurrentFrame = 0;
            Finished = false;
        }

        public void SetFrameDuration(int duration)
        {
            mAnimation.SetFrameDuration(duration);
        }

        private ulong lastFrameBeginTime;
        protected override void Update()
        {
            if (Context.Timer.OnInterval(lastFrameBeginTime, (int)(CurrentDirectedAnimation.FrameDuration / SpeedModifier)))
            {
                lastFrameBeginTime = this.Context.CurrentFrameNumber;
                mCurrentFrame++;
            }

            if (mCurrentFrame >= CurrentDirectedAnimation.Frames.Count())
            {
                if (Looping)
                    mCurrentFrame = 0;
                else
                {
                    mCurrentFrame = CurrentDirectedAnimation.Frames.Count() - 1;
                    Finished = true;
                }
            }
            else
                Finished = false;
        }

        #region IDrawable
        public TextureResource Texture
        {
            get { return mAnimation.Texture; }
        }

        public RGRectangleI SourceRec
        {
            get { return this.CurrentDirectedAnimationFrame.Source; }
        }

        public RGRectangleI DestinationRec
        {
            get
            {

                var rec = this.SourceRec;
                var frame = this.CurrentDirectedAnimationFrame;

                return AdjustFrameRectangle(RGRectangleI.FromXYWH(0,0,frame.Source.Width,frame.Source.Height));
            }
        }

        private RGRectangleI AdjustFrameRectangle(RGRectangleI rec)
        {
            int offX, offY;
            var frame = this.CurrentDirectedAnimationFrame;

            if (RenderOptions.FlipX)
                offX = (frame.Source.Width - frame.Origin.X) - (frame.Source.Width - rec.Right);
            else
                offX = frame.Origin.X - rec.Left;

            if(RenderOptions.FlipY)
                offY = (frame.Source.Height - frame.Origin.Y) - (frame.Source.Height - rec.Bottom);
            else
                offY = frame.Origin.Y - rec.Top;

            return RGRectangleI.FromXYWH(mSprite.Location.X - offX, mSprite.Location.Y - offY, rec.Width, rec.Height);          
        }

        public RGRectangleI CollisionRec
        {
            get
            {
                return GetCollisionRec(HitboxType.Primary);
            }
        }

        public RGRectangleI SecondaryCollisionRec
        {
            get
            {
                return GetCollisionRec(HitboxType.Secondary);
            }
        }


        private RGRectangleI GetCollisionRec(HitboxType t)
        {
            var frame = this.CurrentDirectedAnimationFrame;
            var rec = frame.GetHitbox(t);
            if (rec.IsEmpty)
            {
                if (t == HitboxType.Primary)
                    rec = frame.Source;
                else
                    return RGRectangleI.Empty;
            }

            return AdjustFrameRectangle(rec);
        }

        private RenderOptions mRenderOptions;
        public RenderOptions RenderOptions
        {
            get
            {
                mRenderOptions.FlipX = this.CurrentDirectedAnimation.FlipX;
                return mRenderOptions;
            }
        }
        #endregion


        public void Draw(Graphics.Painter p, RGRectangleI canvas)
        {
            this.DrawTexture(p, canvas);
        }
    }
}

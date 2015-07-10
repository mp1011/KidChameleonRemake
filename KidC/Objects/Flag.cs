using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace KidC
{
    class Flag
    {
        public static SpriteCreationInfo Create(GameContext context, Layer layer)
        {
            var flag = new Sprite(context, layer, KCObjectType.Flag);

            var spriteSheet = SpriteSheet.Load("flag", context);
            flag.SetSingleAnimation(new Animation(spriteSheet,Direction.Right, 0, 1, 2, 3, 4, 5));
            flag.CurrentAnimation.SetFrameDuration(8);
            flag.AddCollisionChecks(KCObjectType.Player);
            flag.OnCollision(KCObjectType.Player, new EndLevel(flag));
            return new SpriteCreationInfo(flag);
        }
    }

    class EndLevel : LogicObject
    {
        private Sprite mFlag;
        public EndLevel(Sprite flag) : base(LogicPriority.World,flag) 
        {
            mFlag = flag;
        }

        protected override void OnResume()
        {

            //pause sprites
            ILogicObject action = new GenericAction(this, ctx =>
                {
                    foreach (var sprite in ctx.Listeners.SpriteListener.GetObjects())
                    {
                        if (sprite.ObjectType.IsNot(KCObjectType.Flag))
                        {
                            sprite.RemoveCollisionType(ObjectType.Block);
                            sprite.Pause();
                        }
                    }

                    return Engine.ExitCode.Finished;
                },true);

            action = action.ContinueWith(new DelayWaiter(this, 1.0f));

            //dissolve background
            action =  action.ContinueWithMulti(Context.CurrentWorld.GetLayers(LayerDepth.Background).
                Select(layer=>CreateTextureDissolve(layer)).ToArray());

            //hide flag
            action = action.ContinueWith(new DelayWaiter(this,2f).ContinueWith(new GenericAction(this, ctx=> 
            { mFlag.CurrentAnimation.RenderOptions.Visible=false; return Engine.ExitCode.Finished;},false)));


            //vr background
            action = action.ContinueWithMulti(
                    new VRBackground(this),
                    new FadeSky(Context.CurrentWorld, RGColor.Black, 60));

            action= action.ContinueWith(new VRBlockReplacer(Context.CurrentWorld));

            action = action.ContinueWithMulti(
                new SpriteDissolver(this),
                new HUDMoveout(this));

            action = action.ContinueWith(new EndOfLevelScoreTally(Context.CurrentWorld));

            var nextScene = new SplashScreenInfo(new GameResource<WorldInfo>(new GamePath(PathType.Maps, "test"), typeof(KCWorldInfo)));
            action = action.ContinueWith(new DelayWaiter(this, 2f));
            action = action.ContinueWith(new FadeoutSceneTransition(Context.CurrentWorld, nextScene));

        }

        private TextureDissolve CreateTextureDissolve(Layer layer)
        {
            var d = new TextureDissolve(this, layer.ExtraRenderInfo, new RGSizeI(2, 2), new RGSizeI(8, 8));
            d.FrameDelay = 1;
            d.CellsPerFrame = 3;
            d.Pause();
            return d;
        }
    }


    class VRBackground : LogicObject, IDrawableRemovable 
    {
        private LinkedList<SimpleAnimation> mBackgrounds;
        private Layer mLayer;

        public VRBackground(ILogicObject owner)
            : base(LogicPriority.World, owner)
        {

            mBackgrounds = new LinkedList<SimpleAnimation>();

            int frameDuration = 2;
            mBackgrounds.AddLast(new SimpleAnimation("vrbackground1", frameDuration, this.Context, 0, 1, 2, 3, 4, 5));
            mBackgrounds.AddLast(new SimpleAnimation("vrbackground2", frameDuration, this.Context, 0, 1, 2, 3, 4));
            mBackgrounds.AddLast(new SimpleAnimation("vrbackground3", frameDuration, this.Context, 0, 1, 2, 3,4));


          
        }

        protected override void OnResume()
        {
            if (mLayer == null)
            {
                mLayer = Context.CurrentWorld.AddLayer(new FixedLayer(Context.CurrentWorld, LayerDepth.Background));
                mLayer.AddObject(this);
            }

            foreach (var g in mBackgrounds)
            {
                g.Looping = false;
                g.ChangeParent(this);
                g.CornerPosition = RGPointI.Empty;           
            }
        }

        protected override void Update()
        {
            var g = mBackgrounds.First;
            if (g != null)
            {
                if (g.Value.Finished)
                {
                    if (g.Next != null)
                    {
                        mBackgrounds.RemoveFirst();
                        mBackgrounds.First.Value.ResetAnimation();
                    }
                    else
                    {
                        mLayer.AddObject(g.Value);
                        g.Value.ChangeParent(mLayer);
                        mBackgrounds.RemoveFirst();
                        this.Kill(Engine.ExitCode.Finished);
                    }
                }
            }
        }

        public void Draw(Engine.Graphics.Painter p, RGRectangleI canvas)
        {
          var g = mBackgrounds.First;
          if (g != null)           
                g.Value.Draw(p, canvas);
        }
    }

    class VRBlockReplacer : LogicObject 
    {
        enum VRTileType
        {
            Empty=1,
            Special=2,
            SlopeDR=3,
            Solid=4,
            SlopeUR=5,
            SlopeUL=6,
            SlopeDL=7
        }

        private Map mForeground;
        private TileLayer mVRLayer;
        private int mBlocksPerTick = 6;
        private RGRectangleI mGridRange;

        public VRBlockReplacer(World owner) : base(LogicPriority.World, owner) 
        {
            mForeground = owner.GetLayers(LayerDepth.Foreground).OfType<TileLayer>().FirstOrDefault().Map;

            mVRLayer = new TileLayer(owner,new Map(owner.Context, 
                new GameResource<TileSet>("vrblocks", PathType.Tilesets),mForeground.TileDimensions.Width, mForeground.TileDimensions.Height),
                RGPointI.Empty, LayerDepth.Foreground);

            owner.AddLayer(mVRLayer);
        }

        private RGPointI mCursor;


        protected override void OnResume()
        {
            var topLeft = mForeground.ScreenToTilePoint(Context.ScreenLocation.TopLeft);
            var bottomRight = mForeground.ScreenToTilePoint(Context.ScreenLocation.BottomRight);
            mGridRange = RGRectangleI.Create(topLeft, bottomRight);

            mCursor = mGridRange.TopLeft;
        }

        protected override void Update()
        {            
            var count = mBlocksPerTick;
            while (--count > 0)
            {
                mVRLayer.Map.SetTile(mCursor.X, mCursor.Y, (int)GetVRTile(mForeground.GetTile(mCursor.X, mCursor.Y)));
                mForeground.SetTile(mCursor.X, mCursor.Y, TileDef.Blank.TileID);

                mCursor.X++;
                if (mCursor.X >= mGridRange.Right)
                {
                    mCursor.Y++;
                    mCursor.X = mGridRange.Left;
                }

                if (mCursor.Y >= mGridRange.Bottom)
                    this.Kill(Engine.ExitCode.Finished);
            }                    
        }

        private VRTileType GetVRTile(TileDef original)
        {
            if (original.IsBlank)
                return VRTileType.Empty;
            else if ((original as KCTileDef).SpecialType != SpecialTile.Normal)
                return VRTileType.Special;
            else if (original.IsSloped)
            {
                if (original.Sides.Up && original.Sides.Right)
                    return VRTileType.SlopeUR;
                if (original.Sides.Up && original.Sides.Left)
                    return VRTileType.SlopeUL;
                if (original.Sides.Down && original.Sides.Right)
                    return VRTileType.SlopeDR;
                if (original.Sides.Down && original.Sides.Left)
                    return VRTileType.SlopeDL;
            }
            else if(original.IsSolid)
                return VRTileType.Solid;

            return VRTileType.Empty;
        }

    }

    class SpriteDissolver : LogicObject
    {
        public SpriteDissolver(ILogicObject parent) : base(LogicPriority.World, parent) { }

        private List<TextureDissolveIntoRows> mBreakups;
        protected override void OnResume()
        {
            mBreakups = new List<TextureDissolveIntoRows>();
            foreach (var sprite in this.Context.Listeners.SpriteListener.GetObjects())
            {
                if (sprite.ObjectType.IsNot(KCObjectType.Flag))
                {
                    var b = new TextureDissolveIntoRows(this, sprite.CurrentAnimation.RenderOptions, sprite.CurrentAnimation.DestinationRec.Width);
                    mBreakups.Add(b);
                }
            }
        }

        protected override void Update()
        {
            if (mBreakups.All(p => p.ExitCode == Engine.ExitCode.Finished))
                this.Kill(Engine.ExitCode.Finished);
        }
        
    }

    class HUDMoveout : LogicObject
    {
        private HUD mHUD;

        public HUDMoveout(ILogicObject parent) : base(LogicPriority.World, parent) 
        {
            mHUD = Context.CurrentMapHUD();
        }

        protected override void OnResume()
        {
            int moveoutSpeed = 2;

            new SimpleMover(this, mHUD.Clock, Direction.Left, moveoutSpeed);
            new SimpleMover(this, mHUD.HealthGuage, Direction.Left, moveoutSpeed);
            new SimpleMover(this, mHUD.GemsCounter, Direction.Right, moveoutSpeed);
            new SimpleMover(this, mHUD.LivesCounter, Direction.Right, moveoutSpeed);

            new DestroyWhenOutOfFrame<Clock>(mHUD.Clock, true);
            new DestroyWhenOutOfFrame<HealthGuage>(mHUD.HealthGuage, true);
            new DestroyWhenOutOfFrame<Counter>(mHUD.GemsCounter, true);
            new DestroyWhenOutOfFrame<Counter>(mHUD.LivesCounter, true);
        }

        protected override void Update()
        {
            if(!mHUD.Clock.Alive && !mHUD.HealthGuage.Alive && !mHUD.GemsCounter.Alive && !mHUD.LivesCounter.Alive)
                this.Kill(Engine.ExitCode.Finished);
        }
    }

    class EndOfLevelScoreTally : LogicObject
    {
        private World mWorld;
        private bool mIsPlayingSound;

        public EndOfLevelScoreTally(World world) : base(LogicPriority.World, world) 
        {
            mWorld = world;
        }

        private List<ScoreTally> mText = new List<ScoreTally>();

        protected override void OnEntrance()
        {
            int columnFromBottom=1;

            mText.Add(new ScoreTally(mWorld, mWorld, "score", 0, columnFromBottom++));
            foreach (var tracker in this.Context.GetBonusTrackers())
            {
                if(tracker.GetCurrentValue() > 0)
                mText.Add(new ScoreTally(mWorld, mWorld, tracker.Name,tracker.GetCurrentValue(), columnFromBottom++));
            }

            var screenWidth = mWorld.ScreenLayer.Location.Width;
            var screenHeight = mWorld.ScreenLayer.Location.Height;

            var continuesLabel = new GameText(mWorld, FontManager.GetBigFontGreen(this.Context), "continues", new RGPointI(20, screenHeight - 20), 200);
            mWorld.ScreenLayer.AddObject(continuesLabel);
            MovingText.MoveIn(continuesLabel, mWorld.ScreenLayer, Direction.Left, 200);


            var continuesValue = new GameText(mWorld, FontManager.GetBigFontWhite(this.Context), "2", new RGPointI(0, screenHeight - 20), screenWidth / 2, Alignment.Far, Alignment.Near);
            mWorld.ScreenLayer.AddObject(continuesValue);
            MovingText.MoveIn(continuesValue, mWorld.ScreenLayer, Direction.Left, 200);

        }

        protected override void Update()
        {
            if (this.TimeActive < 180)
                return;

            if (!mIsPlayingSound)
            {
                mIsPlayingSound = true;
                SoundManager.PlaySound(Sounds.ScoreCounter);
            }

            var addTo = mText.First();
            var takeFrom = mText.Skip(1).FirstOrDefault(p => p.Amount > 0);
            if (takeFrom == null)
            {
                this.Kill(Engine.ExitCode.Finished);
                SoundManager.StopSound(Sounds.ScoreCounter);
                return;
            }
     
            takeFrom.TransferTo(addTo, 50);            
        }



        class ScoreTally : LogicObject 
        {
            private MovingText mScoreLabel;
            private MovingText mScoreValue;

            public string Name { get; private set; }
            public int Amount { get; private set; }

            private GameText mScoreText;

            public ScoreTally(ILogicObject owner, World world, string name, int initialValue, int columnFromBottom)
                : base(LogicPriority.World, owner)
            {
                this.Name = name;
                this.Amount = initialValue;

                int yPos = world.ScreenLayer.Location.Height - ((columnFromBottom +1)* 25);
                int xOffset = 200 + (columnFromBottom * 50);

                var screenWidth = world.ScreenLayer.Location.Width;
                var midPoint = (screenWidth / 2) + 35;

                var labelText = new GameText(this, FontManager.GetBigFontGreen(this.Context), name, new RGPointI(0, yPos), midPoint, Alignment.Far, Alignment.Near);
                world.ScreenLayer.AddObject(labelText);
                mScoreLabel = MovingText.MoveIn(labelText, world.ScreenLayer, Direction.Left, xOffset);

                mScoreText = new GameText(this, FontManager.GetBigFontWhite(this.Context), initialValue.ToString(), new RGPointI((midPoint) + 50, yPos), 50, Alignment.Far, Alignment.Near);
                world.ScreenLayer.AddObject(mScoreText);
                mScoreValue = MovingText.MoveIn(mScoreText, world.ScreenLayer, Direction.Right, xOffset);
            }

            public void TransferTo(ScoreTally other, int amount)
            {
                var transferAmount = Math.Min(this.Amount, amount);
                this.Amount -= amount;
                other.Amount += amount;

                mScoreText.Text = this.Amount.ToString();
                other.mScoreText.Text = other.Amount.ToString();

            }
        }

    }
}


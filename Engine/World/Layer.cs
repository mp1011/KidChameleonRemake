using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Engine
{
    public enum LayerDepth
    {
        FarBackground = 0,
        Background = 100,
        Foreground = 200,
        CloseForeground = 300,
        Screen = 400
    }

    public abstract class Layer : LogicObject, IDrawable
    {
        private List<IDrawableRemovable> mSprites = new List<IDrawableRemovable>();

        public RGPointI Position
        {
            get
            {
                return Location.TopLeft;
            }
            set
            {
                Location = RGRectangleI.FromXYWH(value.X, value.Y, Location.Width, Location.Height);
            }
        }

        public RGRectangleI Location { get; private set; }

        public RGPoint ParallaxSpeed { get; protected set; }

        public LayerDepth Depth { get; private set; }

        public int LayerID { get; private set; }

        protected bool mRepeatH, mRepeatV;

        public void SetLayerID(World w)
        {
            this.LayerID = w.GetNextFreeLayerID();
        }
      
        public void AddObject(IDrawableRemovable s)
        {
            if(s!= null && mSprites.IndexOf(s) == -1)
                mSprites.Add(s);
        }

        public void RemoveObject(IDrawableRemovable s)
        {
            mSprites.Remove(s);
        }

        public void BringObjectToFront(IDrawableRemovable s)
        {
            if (mSprites.IndexOf(s) == mSprites.Count - 1)
                return;

            RemoveObject(s);
            AddObject(s);
        }

        protected Layer(GameContext ctx, RGRectangleI location, LayerDepth depth)
            : base(LogicPriority.World, ctx)
        {
            this.Depth = depth;
            this.Location = location;
            this.ParallaxSpeed = new RGPoint(1f, 1f);
        }

        protected override void Update()
        {
            if (mSprites.Any(p => !p.Alive))
                mSprites = mSprites.Where(p => p.Alive).ToList();

            UpdateEx();
        }

        protected virtual void UpdateEx() { }

        protected virtual void DrawLayer(Graphics.Painter painter, RGRectangleI canvas, RGPointI origin) { }

        public void Draw(Graphics.Painter painter, RGRectangleI canvas)
        {
            canvas.X = (int)Math.Floor(canvas.X * ParallaxSpeed.X);
            canvas.Y = (int)Math.Floor(canvas.Y * ParallaxSpeed.Y);



            var cursor = this.Location.TopLeft;

            DrawLayer(painter, canvas, cursor);

            if (mRepeatH)
            {
                while (cursor.X + this.Location.Width > canvas.Left)
                {
                    cursor = cursor.Offset(-this.Location.Width, 0);
                    DrawLayer(painter, canvas, cursor);
                }
                cursor = this.Location.TopLeft;

                while (cursor.X < canvas.Right)
                {
                    cursor = cursor.Offset(this.Location.Width, 0);
                    DrawLayer(painter, canvas, cursor);
                }
                cursor = this.Location.TopLeft;
            }

            if (mRepeatV)
            {
                throw new NotImplementedException();
            }

            foreach (var sprite in mSprites)
                painter.Paint(canvas, sprite);
        }

        public RGPointI ScreenPointToLayerPoint(RGPointI screenPoint)
        {
            if (this.ParallaxSpeed.X == 0 && this.ParallaxSpeed.Y == 0)
                return screenPoint;

            if (this.ParallaxSpeed.X != 1f || this.ParallaxSpeed.Y != 1f)
                throw new NotImplementedException(); //todo

            return this.Context.ScreenLocation.TopLeft.Offset(screenPoint);
        }


        public RGPointI LayerPointToScreenPoint(RGPointI layerPoint)
        {
            if (this.ParallaxSpeed.X == 0 && this.ParallaxSpeed.Y == 0)
                return layerPoint;

            if (this.ParallaxSpeed.X != 1f || this.ParallaxSpeed.Y != 1f)
                throw new NotImplementedException(); //todo

            var offset = this.Context.ScreenLocation.TopLeft;
            return layerPoint.Offset(-1 * offset.X, -1 * offset.Y);
        }



        public void PositionBelow(Layer previousLayer)
        {
            float top = 0;

            if (previousLayer != null)
                top = previousLayer.Location.Bottom;

            this.Position = new RGPointI(this.Position.X, top);
        }

    }

    public class FixedLayer : Layer
    {
        public FixedLayer(GameContext ctx, LayerDepth depth)
            : base(ctx, RGRectangleI.FromXYWH(0, 0, ctx.Engine.GameSize.Width, ctx.Engine.GameSize.Height), depth)
        {
            this.ParallaxSpeed = RGPoint.Empty;
        }

        public FixedLayer(GameContext ctx) : base(ctx, RGRectangleI.Empty, LayerDepth.Background) { }
    }

    public class ImageLayer : Layer
    {
        private SimpleGraphic mGraphic;

        public ImageLayer(GameContext ctx, SimpleGraphic graphic) : base(ctx, RGRectangleI.FromXYWH(0,0, graphic.SourceRec.Width,graphic.SourceRec.Height), LayerDepth.Background) 
        {
            mGraphic = graphic;
        }

        protected override void DrawLayer(Graphics.Painter painter, RGRectangleI canvas, RGPointI origin)
        {
            mGraphic.CornerPosition = origin;
            mGraphic.Draw(painter,canvas);
        }


        public static ImageLayer CreateRepeatingHorizontal(GameContext ctx, SimpleGraphic graphic, float xSpeed)
        {
            return CreateRepeatingHorizontal(ctx, graphic, xSpeed, RGPointI.Empty);
        }

        public static ImageLayer CreateRepeatingHorizontal(GameContext ctx, SimpleGraphic graphic, float xSpeed, RGPointI position)
        {
            var layer = new ImageLayer(ctx, graphic);
            layer.ParallaxSpeed = new RGPoint(xSpeed, 1f);
            layer.mRepeatH = true;
            layer.Position = position;
            return layer;
        }

    }

    public class TileLayer : Layer
    {
        public Map Map { get; private set; }

        public TileLayer(GameContext ctx) : base(ctx, RGRectangleI.Empty, LayerDepth.Background) { }

        public TileLayer(GameContext ctx, Map map, RGPointI location, LayerDepth depth)
            : base(ctx, RGRectangleI.Create(location, map.Size), depth)
        {
            Map = map;
        }

        protected override void UpdateEx()
        {
            Map.Tileset.UpdateAnimation(this.Context);
        }

        protected override void DrawLayer(Graphics.Painter painter, RGRectangleI canvas, RGPointI location)
        {
            Map.Draw(painter, canvas,location);
        } 
    }

}

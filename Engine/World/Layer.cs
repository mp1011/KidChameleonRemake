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

        public RGPoint Position
        {
            get
            {
                return Location.TopLeft;
            }
            set
            {
                Location = RGRectangle.FromXYWH(value.X, value.Y, Location.Width, Location.Height);
            }
        }

        public RGRectangle Location { get; private set; }

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

        protected Layer(GameContext ctx, RGRectangle location, LayerDepth depth)
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

        protected virtual void DrawLayer(Graphics.Painter painter, RGRectangle canvas, RGPoint origin) { }

        public void Draw(Graphics.Painter painter, RGRectangle canvas)
        {
            canvas.X = (float)Math.Floor(canvas.X * ParallaxSpeed.X);
            canvas.Y = (float)Math.Floor(canvas.Y * ParallaxSpeed.Y);



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

        public RGPoint ScreenPointToLayerPoint(RGPoint screenPoint)
        {
            if (this.ParallaxSpeed.X == 0 && this.ParallaxSpeed.Y == 0)
                return screenPoint;

            if (this.ParallaxSpeed.X != 1f || this.ParallaxSpeed.Y != 1f)
                throw new NotImplementedException(); //todo

            return this.Context.ScreenLocation.TopLeft.Offset(screenPoint);
        }


        public RGPoint LayerPointToScreenPoint(RGPoint layerPoint)
        {
            if (this.ParallaxSpeed.X == 0 && this.ParallaxSpeed.Y == 0)
                return layerPoint;

            if (this.ParallaxSpeed.X != 1f || this.ParallaxSpeed.Y != 1f)
                throw new NotImplementedException(); //todo

            var offset = this.Context.ScreenLocation.TopLeft;
            return layerPoint.Offset(-1 * offset.X, -1 * offset.Y);
        }


    }

    public class FixedLayer : Layer
    {
        public FixedLayer(GameContext ctx, LayerDepth depth)
            : base(ctx, RGRectangle.FromXYWH(0, 0, ctx.Engine.GameSize.Width, ctx.Engine.GameSize.Height), depth)
        {
            this.ParallaxSpeed = RGPoint.Empty;
        }

        public FixedLayer(GameContext ctx) : base(ctx, RGRectangle.Empty, LayerDepth.Background) { }
    }

    public class ImageLayer : Layer
    {
        private SimpleGraphic mGraphic;

        public ImageLayer(GameContext ctx, SimpleGraphic graphic) : base(ctx, RGRectangle.FromXYWH(0,0, graphic.SourceRec.Width,graphic.SourceRec.Height), LayerDepth.Background) 
        {
            mGraphic = graphic;
        }

        protected override void DrawLayer(Graphics.Painter painter, RGRectangle canvas, RGPoint origin)
        {
            mGraphic.CornerPosition = origin;
            mGraphic.Draw(painter,canvas);
        }


        public static ImageLayer CreateRepeatingHorizontal(GameContext ctx, SimpleGraphic graphic, float xSpeed)
        {
            var layer = new ImageLayer(ctx, graphic);
            layer.ParallaxSpeed = new RGPoint(xSpeed, 1f);
            layer.mRepeatH = true;
            return layer;
        }

    }

    public class TileLayer : Layer
    {
        public Map Map { get; private set; }

        public TileLayer(GameContext ctx) : base(ctx, RGRectangle.Empty, LayerDepth.Background) { }

        public TileLayer(GameContext ctx, Map map, RGPoint location, LayerDepth depth)
            : base(ctx, RGRectangle.Create(location, map.Size), depth)
        {
            Map = map;
        }

        protected override void UpdateEx()
        {
            Map.Tileset.UpdateAnimation(this.Context);
        }

        protected override void DrawLayer(Graphics.Painter painter, RGRectangle canvas, RGPoint location)
        {
            Map.Draw(painter, canvas,location);
        } 
    }

}

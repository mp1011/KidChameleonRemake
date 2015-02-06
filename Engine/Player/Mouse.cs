using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Input
{
    enum MouseButton
    {
        Left,
        Right
    }

    public abstract class MouseInput : GenericInputDevice
    {
        private RGPointI mPreviousPosition;

        public RGPointI Position { get; private set; }

        public RGLine MotionLine { get { return new RGLine(mPreviousPosition, Position); } }

        public MouseInput(GameContext ctx)
            : base(ctx)
        {
            this.SetKeyMapping(GameKey.Button1, 1);
            this.SetKeyMapping(GameKey.Button2, 2);
        }

        public RGPointI PositionInLayer(Layer layer)
        {
            return layer.ScreenPointToLayerPoint(this.Position);
        }

        protected override void BeforeUpdateInputState()
        {
            mPreviousPosition = this.Position;
            this.Position = GetMousePosition();
        }

        protected abstract RGPointI GetMousePosition();

        protected override Direction? GetInputDirection(Orientation orientation)
        {
            if (MotionLine.Length == 0f)
                return null;
            return this.MotionLine.Angle;
        }

        public bool UserClickedOn(GameKey button, IWithPosition obj)
        {
            return KeyPressed(button) && obj.Area.Contains(this.Position);
        }
    }

    public class Cursor : LogicObject, IDrawableRemovable, IWithPosition
    {

        private SimpleGraphic mCursorGraphic;
        private Layer mLayer;

        public Cursor(GameContext ctx, Layer layer)
            : base(LogicPriority.World, ctx)
        {
            layer.AddObject(this);
            mLayer = layer;
          //TBD  mCursorGraphic = new SimpleGraphic(GameResource.SpriteSheet, RGRectangle.FromTLBR(46, 272, 53, 279), RGRectangle.FromTLBR(46, 318, 53, 325));
        }

        protected override void Update()
        {
            mLayer.BringObjectToFront(this);

            this.Location = Context.Mouse.Position;

            if (Context.Mouse.KeyDown(GameKey.Button1))
                mCursorGraphic.SourceIndex = 1;
            else
                mCursorGraphic.SourceIndex = 0;
        }

        public RGPointI Location
        {
            get;
            set;
        }

        public RGRectangleI Area
        {
            get { return RGRectangleI.Create(Location.Offset(-8, -8), new RGSizeI(16, 16)); }
        }

        public Direction Direction
        {
            get { return (Direction)(Context.Mouse.InputDirection(Orientation.None) ?? Direction.Right); }
        }

        public void Draw(Graphics.Painter painter, RGRectangleI canvas)
        {
            mCursorGraphic.Position = this.Location;
            painter.Paint(canvas, mCursorGraphic);
        }


        public RGPointI LocationOffset
        {
            get { return new RGPointI(8, 8); }
        }
    }

}

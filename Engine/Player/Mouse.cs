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
        private RGPoint mPreviousPosition;

        public RGPoint Position { get; private set; }

        public RGLine MotionLine { get { return new RGLine(mPreviousPosition, Position); } }

        public MouseInput(GameContext ctx)
            : base(ctx)
        {
            this.SetKeyMapping(GameKey.Button1, 1);
            this.SetKeyMapping(GameKey.Button2, 2);
        }

        public RGPoint PositionInLayer(Layer layer)
        {
            return layer.ScreenPointToLayerPoint(this.Position);
        }

        protected override void BeforeUpdateInputState()
        {
            mPreviousPosition = this.Position;
            this.Position = GetMousePosition();
        }

        protected abstract RGPoint GetMousePosition();

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

        public RGPoint Location
        {
            get;
            set;
        }

        public RGRectangle Area
        {
            get { return RGRectangle.Create(Location.Offset(-8f, -8f), new RGSize(16f, 16f)); }
        }

        public Direction Direction
        {
            get { return (Direction)(Context.Mouse.InputDirection(Orientation.None) ?? Direction.Right); }
        }

        public void Draw(Graphics.Painter painter, RGRectangle canvas)
        {
            mCursorGraphic.Position = this.Location;
            painter.Paint(canvas, mCursorGraphic);
        }


        public RGPoint LocationOffset
        {
            get { return new RGPoint(8, 8); }
        }
    }

}

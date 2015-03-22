using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine;

namespace Editor
{

    enum MouseActionType
    {
        None,
        MouseDown,
        Move,
        Click,
        DoubleClick,
        RectangleSelection
    }

    class ImageEventArgs : EventArgs
    {
        public EditorPoint Point { get; private set; }
        public MouseButtons Buttons { get; private set; }
        public MouseActionType Action { get; private set; }

        public ImageEventArgs(EditorPoint pt, MouseButtons buttons, MouseActionType action)
        {
            this.Action = action;
            this.Point = pt;
            this.Buttons = buttons;
        }
    }

    class DrawRectangleEventArgs : ImageEventArgs
    {
        public EditorPoint Point2 { get; private set; }

        public RGRectangleI ImageRectangle
        {
            get
            {
                if (Point2 == null)
                    return RGRectangleI.Empty;

                return RGRectangleI.Create(Point.ImagePoint, Point2.ImagePoint);
            }
        }

        public DrawRectangleEventArgs(EditorPoint firstPoint, MouseButtons buttons)
            : base(firstPoint, buttons, MouseActionType.RectangleSelection)
        {
        }

        public void SetSecondPoint(EditorPoint secondPoint)
        {
            this.Point2 = secondPoint;
        }

    }

}

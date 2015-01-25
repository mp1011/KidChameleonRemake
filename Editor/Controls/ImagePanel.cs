using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine;
using Editor.Forms;
using System.Drawing.Drawing2D;

namespace Editor
{
    abstract class ImagePanel
    {
        public const float ZoomMin = .5f;
        public const float ZoomMax = 5;

        private Control mControl;

        public ImagePanel(Control control)
        {          
            mControl = control;
        }

        protected void SetupEvents()
        {
            var control = this.DrawingSurface;
            control.Paint += new System.Windows.Forms.PaintEventHandler(this.ImagePanel_Paint);
            control.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ImagePanel_MouseDown);
            control.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ImagePanel_MouseMove);
            control.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ImagePanel_MouseUp);
            control.Resize += new System.EventHandler(this.ImagePanel_Resize);
        }

        private RGPointI mPan = RGPointI.Empty;

        public RGPointI Pan
        {
            get { return mPan; }
            set
            {
                mPan = value;
                this.RecalcDrawArea();
                foreach (var item in mOverlayItems)
                {
                    foreach (var pt in item.GetPoints())
                    {
                        pt.RecalcClientPoint();
                    }
                }
            }
        }

        private float mZoom = 1f;
        public float Zoom
        {
            get
            {
                return mZoom;
            }
            set
            {
                if (value < ZoomMin)
                    mZoom = ZoomMin;
                else if (value > ZoomMax)
                    mZoom = ZoomMax;
                else
                    mZoom = value;

                this.RecalcDrawArea();
                foreach (var item in mOverlayItems)
                {
                    foreach (var pt in item.GetPoints())
                    {
                        pt.RecalcClientPoint();
                    }
                }
            }
        }
  
        public RGSizeI Size { get { return new RGSizeI(mControl.Width, mControl.Height); } }

        #region Overlay

        private List<IDrawable> mOverlayItems = new List<IDrawable>();
        public void AddOverlayItem(IDrawable i)
        {
            if (!mOverlayItems.Contains(i))
                mOverlayItems.Add(i);
            RefreshImage();
        }

        public void RemoveOverlayItem(IDrawable i)
        {
            mOverlayItems.Remove(i);
            RefreshImage();
        }

        public void ClearOverlay()
        {
            mOverlayItems.Clear();
            RefreshImage();
        }

        public void RecalcOverlayClientPositions()
        {
            foreach (var point in mOverlayItems.SelectMany(p=>p.GetPoints()))         
                point.RecalcClientPoint();
        }

        #endregion

        #region Drawing

        public RGRectangleI CurrentDrawArea
        {
            get;
            private set;
        }

        public abstract RGSizeI WorkingImageSize { get; }

        private Bitmap mWorkingImage;
        private Bitmap WorkingImage
        {
            get
            {
                if (mWorkingImage == null || mWorkingImage.Width != WorkingImageSize.Width || mWorkingImage.Height != WorkingImageSize.Height)
                {
                    frmLog.AddLine("Recreating working image -" + WorkingImageSize.Width + "x" + WorkingImageSize.Height);
                    mCurrentChangeHashcode = 0;
                    mWorkingImage = new Bitmap(WorkingImageSize.Width, WorkingImageSize.Height);
                }
                return mWorkingImage;
            }
        }

        private Bitmap mWorkingImageCtl;
        private Bitmap WorkingImageCtl
        {
            get
            {
                if (mWorkingImageCtl == null || mWorkingImageCtl.Width != this.mControl.Width || mWorkingImageCtl.Height != this.mControl.Height)
                    mWorkingImageCtl = new Bitmap(this.mControl.Width, this.mControl.Height);
                return mWorkingImageCtl;
            }
        }

        private int mCurrentChangeHashcode;
        public void RefreshImage()
        {
            frmLog.AddLine("Refresh Image -" + WorkingImageSize.Width + "x" + WorkingImageSize.Height);
            Graphics gWorking=null, gWorking2, gControl=null;
            try
            {

                int newChangeHashcode = this.GetWorkingImageChangeHashCode();

                if (newChangeHashcode == 0 || newChangeHashcode != mCurrentChangeHashcode)
                {
                    frmLog.AddLine("Redrawing working image - " + WorkingImageSize.Width + "x" + WorkingImageSize.Height);
                    gWorking = Graphics.FromImage(WorkingImage);
                    gWorking.SetBlockyScaling();
                    gWorking.Clear(Color.White);

                    if (this.DrawWorkingImage(gWorking))
                        mCurrentChangeHashcode = newChangeHashcode;
                    else
                        return;
                }

                gWorking2 = Graphics.FromImage(WorkingImageCtl);
                gWorking2.SetBlockyScaling();
                DisplayImage(gWorking2);
                gWorking2.Dispose();

                gControl = DrawingSurface.CreateGraphics();
                gControl.SetBlockyScaling();
                gControl.DrawImage(WorkingImageCtl, Point.Empty);

                if(gWorking != null) 
                    gWorking.Dispose();
                gControl.Dispose();

                frmLog.AddLine("Change hash code = " + mCurrentChangeHashcode + " -" + WorkingImageSize.Width + "x" + WorkingImageSize.Height);
            }
            catch (Exception e)
            {
                frmLog.AddLine(e.Message);
                //if (!e.Message.ToLower().Contains("disposed object"))
                //    throw e;
            }
            finally
            {
                if (gWorking != null)
                    gWorking.Dispose();
                if (gControl != null)
                    gControl.Dispose();

            }
        }

        protected abstract int GetWorkingImageChangeHashCode();

        protected abstract bool DrawWorkingImage(Graphics gWorking);

        public void DisplayImage(Graphics gControl)
        {          
            var eraser = new SolidBrush(Color.White);
            gControl.FillRectangle(eraser, new Rectangle(0, 0, DrawingSurface.Width, DrawingSurface.Height));

            RecalcDrawArea();

            DrawImageToClient(gControl, this.WorkingImage, this.CurrentDrawArea);

            foreach (var item in mOverlayItems.ToArray())
                item.DrawToClient(gControl);
        }

        private void RecalcDrawArea()
        {
            var drawSurface = RGRectangleI.FromXYWH(0, 0, DrawingSurface.Width * this.Zoom, DrawingSurface.Height * this.Zoom);
            drawSurface = RGRectangleI.FromXYWH(0, 0, this.WorkingImage.Width, this.WorkingImage.Height).ScaleWithin(drawSurface);
            drawSurface = drawSurface.Offset(Pan.X, Pan.Y);
            this.CurrentDrawArea = drawSurface;

            this.CurrentDrawArea = AdjustDrawArea(drawSurface);
        }

        protected virtual RGRectangleI AdjustDrawArea(RGRectangleI drawArea)
        {
            return drawArea;
        }

        private void DrawImageToClient(Graphics gControl, Bitmap src, RGRectangleI destinationArea)
        {
            RGRectangleI srcArea = RGRectangleI.FromXYWH(0,0,src.Width,src.Height);
            gControl.DrawImage(src, destinationArea.ToSystemRec(), srcArea.ToSystemRec(), GraphicsUnit.Pixel);
        }

        #endregion

        #region Events

        public delegate void MouseActionEventHandler(object sender, ImageEventArgs e);
        public event MouseActionEventHandler MouseAction;

        private void ImagePanel_MouseMove(object sender, MouseEventArgs e)
        {          
            if (this.DrawRectangle)
            {
                ResizeRectangle(e.X, e.Y);
                RefreshImage();
            }
            else if(MouseAction != null)
                MouseAction(this, new ImageEventArgs(this.CreateEditorPoint(e.X, e.Y), e.Button, MouseActionType.Move));
        }


        private void ImagePanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.DrawRectangle)
            BeginRectangle(e.X, e.Y);
            else if (MouseAction != null)
                MouseAction(this, new ImageEventArgs(this.CreateEditorPoint(e.X, e.Y), e.Button, MouseActionType.MouseDown));
        }


        private void ImagePanel_Resize(object sender, EventArgs e)
        {
            RefreshImage();
        }

        private void ImagePanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (MouseAction != null)
            {
                if (this.DrawRectangle)
                {
                    ResizeRectangle(e.X, e.Y);
                    MouseAction(this, mDrawRectangleEventArgs);
                    mDrawRectangleEventArgs = null;
                    this.RefreshImage();
                }
                else
                    MouseAction(this, new ImageEventArgs(this.CreateEditorPoint(e.X, e.Y), e.Button, MouseActionType.Click));
            }
        }

        private void ImagePanel_Paint(object sender, PaintEventArgs e)
        {
            DisplayImage(e.Graphics);
        }



        #endregion

        #region Rectangle

        private DrawRectangleEventArgs mDrawRectangleEventArgs = null;
        private OverlayRectangle mDrawRecOverlay = null;

        private bool mDrawRectangle = false;

        public bool DrawRectangle
        {
            get { return mDrawRectangle; }
            set
            {
                mDrawRectangle = value;
                if (!value)
                    ClearRectangle();
            }
        }


        private void BeginRectangle(int clientX, int clientY)
        {
            mDrawRectangleEventArgs = new DrawRectangleEventArgs(this.CreateEditorPoint(clientX, clientY), MouseButtons.None);

            if (mDrawRecOverlay == null)
            {
                mDrawRecOverlay = new OverlayRectangle();
                mDrawRecOverlay.Pen = new Pen(Color.Red);
            }

            AddOverlayItem(mDrawRecOverlay);
            mDrawRecOverlay.Area = EditorRectangleFromImageRec(mDrawRectangleEventArgs.ImageRectangle);
        }

        private void ResizeRectangle(int x, int y)
        {
            if (mDrawRectangleEventArgs != null)
            {
                mDrawRectangleEventArgs.SetSecondPoint(this.CreateEditorPoint(x, y));
                mDrawRecOverlay.Area = EditorRectangleFromImageRec(mDrawRectangleEventArgs.ImageRectangle);
            }
        }

        public EditorRectangle EditorRectangleFromImageRec(RGRectangleI ImageRec)
        {
            return new EditorRectangle()
            {
                TopLeft = CreateEditorPointFromImagePoint(ImageRec.TopLeft.X, ImageRec.TopLeft.Y),
                BottomRight = CreateEditorPointFromImagePoint(ImageRec.BottomRight.X, ImageRec.BottomRight.Y)
            };
        }

        public void ClearRectangle()
        {
            if (mDrawRecOverlay != null)
                mOverlayItems.Remove(mDrawRecOverlay);
            mDrawRecOverlay = null;
            mDrawRectangleEventArgs = null;
        }

        #endregion

        protected virtual Control DrawingSurface
        {
            get
            {
                return mControl;
            }
        }

        protected abstract EditorPoint CreateEditorPoint(int clientX, int clientY);
        protected abstract EditorPoint CreateEditorPointFromImagePoint(int imageX, int imageY);
    }


}

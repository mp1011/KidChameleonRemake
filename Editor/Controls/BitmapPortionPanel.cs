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

namespace Editor
{
    partial class BitmapPortionPanelUserControl : UserControl
    {
        public delegate void ImageSelectedEventHandler(object sender, ImageSelectedEventArgs e);
        public event ImageSelectedEventHandler ImageSelectionChanged;


        public bool DisplayPortionOnly { get { return ImagePanel.DisplayPortionOnly; } set { ImagePanel.DisplayPortionOnly = value; RefreshImage(); } }
        public DrawRectangleType DrawRectangle { get { return ImagePanel.DrawRectangle; } set { ImagePanel.DrawRectangle = value; } }

        public bool EnableSelector { get; set; }

        public bool CenterOnOrigin { get { return ImagePanel.CenterOnOrigin; } set { ImagePanel.CenterOnOrigin = value; } }

        public RGPointI? DisplayOrigin { get; set; }

        public BitmapPortionPanel ImagePanel { get; private set; }

        public BitmapPortion Image
        {
            get { return ImagePanel.Image; }
            set { ImagePanel.Image = value; }
        }


        public int SelectionIndex { get; private set; }
        private static int mNextSelectionIndex;

        private bool mSelected = false;
        public bool Selected
        {
            get { return mSelected; }
            set
            {
                if (value && !mSelected)
                {
                    this.SelectionIndex = mNextSelectionIndex;
                    mNextSelectionIndex++;
                }

                if (mSelected = value)
                {
                    this.BackColor = Color.DarkSeaGreen;
                }
                else
                {
                    this.BackColor = Color.DarkGoldenrod;
                }

                this.Refresh();
                
                if(ImageSelectionChanged != null)
                    ImageSelectionChanged(this, new ImageSelectedEventArgs(value, Control.ModifierKeys == Keys.Shift, Control.ModifierKeys == Keys.Control));

            }
        }

        private bool mAllowSelection;
        public void SetAllowSelection(bool value)
        {
            mAllowSelection = value;
            AdjustBorderSize();
        }

        private void AdjustBorderSize()
        {
            if (mAllowSelection)
            {
                int pad = 4;
                pnlImage.SetBounds(pad, pad, this.Width - pad * 2, this.Height - pad * 2);
            }
            else
                pnlImage.SetBounds(0, 0, this.Width, this.Height);
         
        }

        public BitmapPortionPanelUserControl()
        {
            InitializeComponent();
            this.ImagePanel = new BitmapPortionPanel(this.pnlImage);
        }

        public BitmapPortionPanelUserControl(bool allowSelection)
        {
            InitializeComponent();
            this.ImagePanel = new BitmapPortionPanel(this.pnlImage);
            this.SetAllowSelection(allowSelection);
        }
 
        #region Common

        public void ClearOverlay()
        {
            ImagePanel.ClearOverlay();
        }

        public void RefreshImage()
        {
            ImagePanel.RefreshImage();
        }

        public void AddOverlayItem(IDrawable item)
        {
            ImagePanel.AddOverlayItem(item);
        }

        #endregion
      
        private void BitmapPortionPanelUserControl_Resize(object sender, EventArgs e)
        {
            ImagePanel.RecalcOverlayClientPositions();
            this.AdjustBorderSize();
        }

        private void pnlImage_Click(object sender, EventArgs e)
        {
            this.Selected = !this.Selected;
        }
    
    }

    class BitmapPortionPanel : ImagePanel
    {
        public BitmapPortionPanel(Control ctl) : base(ctl) 
        {
            this.SetupEvents();
        }

        private bool mDisplayPortionOnly;
        public bool DisplayPortionOnly
        {
            get { return mDisplayPortionOnly; }
            set
            {
                mDisplayPortionOnly = value;
                this.RecalcOverlayClientPositions(); 
            }
        }
     
        private BitmapPortion mImage;
        public BitmapPortion Image
        {
            get { return mImage; }
            set
            {
                mImage = value;
                RefreshImage();                
            }
        }

        public bool CenterOnOrigin { get; set; }

        #region Overrides

        public override RGSizeI WorkingImageSize
        {
            get
            {
                if (this.Image == null)
                    return this.Size;
                else if (this.DisplayPortionOnly && !this.Image.Region.Size.IsZero)
                    return this.Image.Region.Size;
                else
                    return this.Image.Size;
            }
        }

        protected override bool DrawWorkingImage(Graphics gWorking)
        {
            if (this.Image == null)
                return false;

            if (this.DisplayPortionOnly)
                gWorking.DrawImage(this.Image.Image, new Rectangle(Point.Empty, this.Image.Region.Size.ToSystemSize()), this.Image.Region.ToSystemRec(), GraphicsUnit.Pixel);
            else
                gWorking.DrawImage(this.Image.Image, Point.Empty);

            return true;

        }

        protected override RGRectangleI AdjustDrawArea(RGRectangleI drawArea)
        {
            if(!CenterOnOrigin)
                return base.AdjustDrawArea(drawArea);

            var controlCenter = this.Size.Width / 2;

            var scale = drawArea.Width / WorkingImageSize.Width;

            var originPoint = this.Image.Origin.X * scale;
            return RGRectangleI.FromXYWH(controlCenter - originPoint, drawArea.Y, drawArea.Width, drawArea.Height);            
        }


        protected override EditorPoint CreateEditorPoint(int clientX, int clientY)
        {
            return BitmapPortionPoint.FromClientPoint(clientX, clientY, this);
        }

        protected override EditorPoint CreateEditorPointFromImagePoint(int imageX, int imageY)
        {
            return BitmapPortionPoint.FromImagePoint(imageX, imageY, this);
        }

        protected override int GetWorkingImageChangeHashCode()
        {
            return 0;
        }

        protected RGRectangleI DrawImageToClient(Graphics gControl, Bitmap src, Size size)
        {
            throw new NotImplementedException();
            //float scale = 10f;
            //if (!CenterOnOrigin)
            //    return base.DrawImageToClient(gControl, src, size);

            //var dest = new RGPointI(size.Width / 2, size.Height - 16);

            //var dx = dest.X - this.Image.Origin.X;
            //var dy = dest.Y - this.Image.Origin.Y;

            //var destRec = new Rectangle(dest.X, dest.Y, (int)(src.Width*scale), (int)(src.Height*scale));

            //destRec.X -= (int)(this.Image.Origin.X * scale);
            //destRec.Y -= (int)(this.Image.Origin.Y * scale);

            //gControl.DrawImage(src, destRec);

            //return RGRectangleI.FromTLBR(destRec.Top, destRec.Left, destRec.Bottom, destRec.Right);
        }
        #endregion
    }

    class ImageSelectedEventArgs : EventArgs 
    {
        public bool Selected { get; private set; }
        public bool Shift { get; private set; }
        public bool Control { get; private set; }

        public ImageSelectedEventArgs(bool isSelected, bool shiftKeyPressed, bool controlKeyPressed)
        {
            this.Selected = isSelected;
            this.Shift = shiftKeyPressed;
            this.Control = controlKeyPressed;
        }
    }
}

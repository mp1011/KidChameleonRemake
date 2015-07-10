using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class TextureCell
    {
        public RGRectangleI Src { get; set; }
        public RGRectangleI Dest { get; set; }
    }

    public interface ITextureBreakup
    {
        IEnumerable<TextureCell> GetVisibleCells(RGRectangleI src, RGRectangleI dest);       
    }

    public class GridTextureBreakup : ITextureBreakup 
    {
        private RGSizeI mCellSize;
        private bool[,] mMask;

        public GridTextureBreakup(RGSizeI cellSize, RGSizeI maskSize)
        {
            mCellSize = cellSize;
            mMask = new bool[maskSize.Width, maskSize.Height];

            for(int y = 0; y < maskSize.Height;y++)
                for(int x =0; x < maskSize.Width;x++)
                    mMask[x,y]=true;
        }

        //test only
        public GridTextureBreakup()
        {
            mCellSize = new RGSizeI(8,8);
            var maskSize = new RGSizeI(2, 2);
            mMask = new bool[maskSize.Width, maskSize.Height];

            mMask[0, 0] = true;
            mMask[1, 1] = true;

            mMask[1, 0] = true;
            mMask[0, 1] = true;

        }

        public void SetCell(int x, int y, bool value)
        {
            mMask[x, y] = false;
        }

        public IEnumerable<TextureCell> GetVisibleCells(RGRectangleI src, RGRectangleI dest)
        {
            int maskX = 0, maskY = 0;

            int x = 0, y = 0;
            while(y < src.Height)
            {
                y = maskY * mCellSize.Height;
                x = 0;
                maskX=0;
                while (x < src.Width)
                {
                    x = maskX * mCellSize.Width;
            
                    if(mMask[maskX % mMask.GetLength(0),maskY % mMask.GetLength(1)])
                    {
                        var r = new TextureCell
                        {
                            Src = RGRectangleI.FromTLBR(src.Y + y, src.X + x, 
                            Math.Min(src.Bottom, src.Y +y+ mCellSize.Height), 
                            Math.Min(src.Right, src.X +x+ mCellSize.Width)),
  
                            Dest = RGRectangleI.FromTLBR(dest.Y + y, dest.X + x, 
                            Math.Min(dest.Bottom, dest.Y + y + mCellSize.Height), 
                            Math.Min(dest.Right, dest.X +x+ mCellSize.Width))

                        };
                    
                        yield return r;
                    }

                    maskX++;
                }

                maskY++;
            }
        }
    }


    public class TextureDissolve : LogicObject
    {
        public int FrameDelay { get; set; }
        public int CellsPerFrame { get; set; }

        private ulong mLastUpdateFrame;
        private GridTextureBreakup mTextureBreakup;
        private LinkedList<RGPointI> mCells;

        public TextureDissolve(ILogicObject owner, StackedRenderInfo renderInfo, RGSizeI cellSize, RGSizeI maskSize)
            : base(LogicPriority.World,owner)
        {
            mTextureBreakup = new GridTextureBreakup(cellSize, maskSize);
            renderInfo.TextureBreakup = mTextureBreakup;

            mCells = new LinkedList<RGPointI>();
            for (int y = 0; y < maskSize.Height; y++)
                for (int x = 0; x < maskSize.Width; x++)
                    mCells.AddLast(new RGPointI(x, y));
        }

        protected override void OnResume()
        {
            mLastUpdateFrame = Context.CurrentFrameNumber;
        }

        protected override void Update()
        {
            if (Context.ElapsedFramesSince(mLastUpdateFrame) < FrameDelay)
                return;

            mLastUpdateFrame = Context.CurrentFrameNumber;

            int cells = CellsPerFrame;
            while (cells-- > 0)
            {
                var cell = mCells.RandomItem();
                mCells.Remove(cell);
                mTextureBreakup.SetCell(cell.X, cell.Y, false);
            }

            if (mCells.IsNullOrEmpty())
                this.Kill(Engine.ExitCode.Finished);
        }
    }

    public class TextureDissolveIntoRows : LogicObject
    {
        private GridTextureBreakup mTextureBreakup;
        private int dissolveIndex;
        private int mMaskHeight = 8;
        public TextureDissolveIntoRows(ILogicObject owner, RenderOptions renderInfo, int textureWidth):base(LogicPriority.World,owner)
        {
            mTextureBreakup = new GridTextureBreakup(new RGSizeI(textureWidth, 1), new RGSizeI(textureWidth, mMaskHeight));
            renderInfo.TextureBreakup = mTextureBreakup;
        }

        protected override void Update()
        {
            if (Context.Timer.OnInterval(2))
            {
                mTextureBreakup.SetCell(0, dissolveIndex, false);
                dissolveIndex++;
                if (dissolveIndex >= mMaskHeight)
                    this.Kill(Engine.ExitCode.Finished);
            }
        }
    }
}

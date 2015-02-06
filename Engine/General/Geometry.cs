using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{

    public static class GeometryUtil
    {

        public static float FixRad(this float num)
        {
            return num.Fix(0, (float)Math.PI * 2f);
        }

        public static float FixDeg(this float num)
        {
            return num.Fix(0f, 360f);
        }

        public static double Fix(this double num, double min, double max)
        {
            while (num < min)
                num += (max - min);

            while (num >= max)
                num -= (max - min);

            return num;
        }

        public static double FixRad(this double num)
        {
            return num.Fix(0, Math.PI * 2.0);
        }

        public static double FixDeg(this double num)
        {
            return num.Fix(0, 360.0);
        }

        public static double GetLineAngleR(RGPointI source, RGPointI target)
        {
            return GetLineAngleR(new RGPoint(source.X, source.Y), new RGPoint(target.X, target.Y));
        }

        /// <summary>
        /// Returns the angle between the two points in radians
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static double GetLineAngleR(RGPoint source, RGPoint target)
        {
            double a, b;
            double angle;

            a = target.X - source.X;
            b = target.Y - source.Y;

            if (a == 0 && b == 0)
                return 0;

            angle = Math.Atan(Math.Abs(b) / Math.Abs(a));

            if (a >= 0 && b >= 0)
            {
                angle *= -1;
            }
            else if (a < 0 && b >= 0)
            {
                angle += Math.PI;
            }
            else if (a >= 0 && b < 0)
            {

            }
            else if (a < 0 && b < 0)
            {
                angle = Math.PI - angle;
            }
            return angle;

        }

        /// <summary>
        /// Converts an angle in radians to a point
        /// </summary>
        /// <param name="angle">Angle in radians</param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static RGPoint AngleToXY(double angle, float length)
        {
            double a, b;

            double t = Math.Tan(angle);

            double c2 = length * length;
            double t2 = t * t;

            a = Math.Sqrt(c2 / (t2 + 1));
            b = Math.Sqrt(c2 - a * a);

            if (angle < (Math.PI / 2))
            {
                b *= -1;
            }
            if (angle >= (Math.PI / 2) && angle < Math.PI)
            {
                a *= -1;
                b *= -1;
            }
            else if (angle >= Math.PI && angle < ((Math.PI / 2) + Math.PI))
            {
                a *= -1;
            }
            else if (angle >= ((Math.PI / 2) + Math.PI))
            {
                ;
            }

            var f = 1f;
            if (length < 0)
                f = -1f;


            return new RGPoint((float)Math.Round(a, 4) * f, (float)Math.Round(b, 4) * f);
        }

        //http://paulbourke.net/geometry/lineline2d/Helpers.cs
        public static bool CheckLineIntersection(RGPoint a1, RGPoint a2, RGPoint b1, RGPoint b2)
        {
            // Denominator for ua and ub are the same, so store this calculation
            double d =
               (b2.Y - b1.Y) * (a2.X - a1.X)
               -
               (b2.X - b1.X) * (a2.Y - a1.Y);

            //n_a and n_b are calculated as seperate values for readability
            double n_a =
               (b2.X - b1.X) * (a1.Y - b1.Y)
               -
               (b2.Y - b1.Y) * (a1.X - b1.X);

            double n_b =
               (a2.X - a1.X) * (a1.Y - b1.Y)
               -
               (a2.Y - a1.Y) * (a1.X - b1.X);

            // Make sure there is not a division by zero - this also indicates that
            // the lines are parallel.  
            // If n_a and n_b were both equal to zero the lines would be on top of each 
            // other (coincidental).  This check is not done because it is not 
            // necessary for this implementation (the parallel check accounts for this).
            if (d == 0)
                return false;

            // Calculate the intermediate fractional point that the lines potentially intersect.
            double ua = n_a / d;
            double ub = n_b / d;


            // PointF ptIntersection = new PointF();
            // The fractional point will be between 0 and 1 inclusive if the lines
            // intersect.  If the fractional calculation is larger than 1 or smaller
            // than 0 the lines would need to be longer to intersect.
            if (ua >= 0d && ua <= 1d && ub >= 0d && ub <= 1d)
            {
                // ptIntersection.X = (float)(a1.X + (ua * (a2.X - a1.X)));
                // ptIntersection.Y = (float)(a1.Y + (ua * (a2.Y - a1.Y)));
                return true;
            }
            return false;
        }

        public static RGPoint GetLineIntersection(RGPointI a1, RGPointI a2, RGPointI b1, RGPointI b2)
        {       
            // Denominator for ua and ub are the same, so store this calculation
            double d =
               (b2.Y - b1.Y) * (a2.X - a1.X)
               -
               (b2.X - b1.X) * (a2.Y - a1.Y);

            //n_a and n_b are calculated as seperate values for readability
            double n_a =
               (b2.X - b1.X) * (a1.Y - b1.Y)
               -
               (b2.Y - b1.Y) * (a1.X - b1.X);

            double n_b =
               (a2.X - a1.X) * (a1.Y - b1.Y)
               -
               (a2.Y - a1.Y) * (a1.X - b1.X);

            // Make sure there is not a division by zero - this also indicates that
            // the lines are parallel.  
            // If n_a and n_b were both equal to zero the lines would be on top of each 
            // other (coincidental).  This check is not done because it is not 
            // necessary for this implementation (the parallel check accounts for this).
            if (d == 0)
                return new RGPoint(float.PositiveInfinity, float.PositiveInfinity);

            // Calculate the intermediate fractional point that the lines potentially intersect.
            double ua = n_a / d;
            double ub = n_b / d;


            // PointF ptIntersection = new PointF();
            // The fractional point will be between 0 and 1 inclusive if the lines
            // intersect.  If the fractional calculation is larger than 1 or smaller
            // than 0 the lines would need to be longer to intersect.
            if (ua >= 0d && ua <= 1d && ub >= 0d && ub <= 1d)
            {
                RGPoint intersection = new RGPoint((float)(a1.X + (ua * (a2.X - a1.X))),
                (float)(a1.Y + (ua * (a2.Y - a1.Y))));
                return intersection;
            }
            return new RGPoint(float.PositiveInfinity, float.PositiveInfinity);
        }

    }

    public enum Orientation
    {
        None,
        Horizontal,
        Vertical,
        Diagonal
    }

    public enum RotationType
    {
        None,
        Clockwise,
        Counterclockwise
    }

    public struct Direction
    {
        public static Direction Right = Direction.FromAngle(0);
        public static Direction UpRight = Direction.FromAngle(45);
        public static Direction Up = Direction.FromAngle(90);
        public static Direction UpLeft = Direction.FromAngle(135);
        public static Direction Left = Direction.FromAngle(180);
        public static Direction DownLeft = Direction.FromAngle(225);
        public static Direction Down = Direction.FromAngle(270);
        public static Direction DownRight = Direction.FromAngle(315);
        public static Direction Default = Direction.FromAngle(0);

        private double degrees;

        private Direction(double deg) { degrees = deg.FixDeg(); }

        public static Direction FromAngle(double angle) { return new Direction(angle); }
        public static Direction FromRad(double angle) { return new Direction(angle * (180.0 / Math.PI)); }

        public double Radians { get { return degrees * (Math.PI / 180.0); } }

        public double Degrees { get { return degrees; } }

        public Direction Reverse()
        {
            return Direction.FromAngle(this.degrees + 180.0);
        }

        public RGPoint ToPoint(float magnitude)
        {
            return GeometryUtil.AngleToXY((double)this.Radians, magnitude);
        }

        /// <summary>
        /// Returns a point where the x and y values are -1, 0, or 1.
        /// </summary>
        /// <returns></returns>
        public RGPointI ToPoint()
        {
            var pt = this.ToPoint(1f);
            int x=0, y=0;
            if (pt.X < 0)
                x = -1;
            if (pt.X > 0)
                x = 1;
            if (pt.Y < 0)
                y = -1;
            if (pt.Y > 0)
                y = 1;

            return new RGPointI(x, y);
        }

        public static bool operator ==(Direction d1, Direction d2)
        {
            return d1.degrees == d2.degrees;
        }

        public static bool operator !=(Direction d1, Direction d2)
        {
            return d1.degrees != d2.degrees;
        }

        public override bool Equals(object obj)
        {
            try
            {
                Direction d = (Direction)obj;
                return d.degrees == this.degrees;
            }
            catch
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return (int)(degrees * 1000);
        }

        public override string ToString()
        {
            return "{" + this.Degrees + "'}";
        }

        public Direction RotateR(RotationType rotation, double degreesRad)
        {
            if (rotation == RotationType.Clockwise)
                return Direction.FromRad(this.Radians - degreesRad);
            else
                return Direction.FromRad(this.Radians + degreesRad);
        }

        public Direction RotateD(RotationType rotation, float degrees)
        {
            return RotateR(rotation, (float)(degrees * (Math.PI / 180f)));
        }

        public Direction Reflect(Orientation plane)
        {
            Direction planeDir = Direction.Right;

            if (plane == Orientation.Vertical)
                planeDir = Direction.Right;
            else if (plane == Orientation.Horizontal)
                planeDir = Direction.Up;
            else if (plane == Orientation.None)
                return this.RotateD(RotationType.Clockwise, 180);

            var distance = AngleDifferenceR(planeDir, RotationType.Counterclockwise);

            Direction reflectedDir = this.RotateR(RotationType.Counterclockwise, distance * 2);

            return reflectedDir;
        }

        public Direction RotateTowards(Direction other, float degrees)
        {
            var dir = this.RotateD(this.ClosestRotation(other), degrees);
            if (dir.ClosestRotation(other) != this.ClosestRotation(other))
                dir = other;
            return dir;
        }

        public RotationType ClosestRotation(Direction targetDirection)
        {
            double distCW, distCCW;

            distCW = (this.Degrees - targetDirection.Degrees).FixDeg();

            distCCW = (targetDirection.Degrees - this.Degrees).FixDeg();

            if (distCW < distCCW)
                return RotationType.Clockwise;
            else
                return RotationType.Counterclockwise;
        }


        public double AngleDifferenceR(Direction other, RotationType rotation)
        {
            if (rotation == RotationType.Clockwise)
                return (this.Radians - other.Radians).FixRad();
            else if (rotation == RotationType.Counterclockwise)
                return (other.Radians - this.Radians).FixRad();
            else
                return this.AngleDifferenceR(other);
        }

        public double AngleDifferenceR(Direction other)
        {
            double distCW, distCCW;

            distCW = (this.Radians - other.Radians).FixRad();
            distCCW = (other.Radians - this.Radians).FixRad();

            return Math.Min(distCW, distCCW);
        }

        public double AngleDifference(Direction other)
        {
            double distCW, distCCW;

            distCW = (this.Degrees - other.Degrees).FixDeg();
            distCCW = (other.Degrees - this.Degrees).FixDeg();

            return Math.Min(distCW, distCCW);
        }

    }

    public struct RGPoint : ISerializable 
    {
        public static RGPoint Empty { get { return new RGPoint(0, 0); } }

        private float mX, mY;
        public float X { get { return mX; } }
        public float Y { get { return mY; } }

        public RGPoint(float pX, float pY)
        {
            mX = pX;
            mY = pY;
        }

        public override int GetHashCode()
        {
            return (int)(mX * 7777 + mY);
        }

        public override bool Equals(object obj)
        {
            var otherPoint = (RGPoint)obj;
            return otherPoint.X == this.X && otherPoint.Y == this.Y;
        }

        public Direction? Direction
        {
            get
            {
                if (this.IsEmpty)
                    return null;

                return Engine.Direction.FromRad((double)GeometryUtil.GetLineAngleR(RGPoint.Empty, this));
            }
        }

        public float Magnitude
        {
            get
            {
                return (float)Math.Sqrt(X * X + Y * Y);
            }
        }

        public Direction GetDirectionTo(RGPoint other)
        {
            return Engine.Direction.FromRad(Util.GetLineAngleR(this, other));
        }

        public float GetDistanceTo(RGPoint other)
        {
            float x = this.X - other.X;
            float y = this.Y - other.Y;
            return (float)Math.Sqrt(x * x + y * y);
        }

        public RGPoint Offset(float x, float y)
        {
            return new RGPoint(X + x, Y + y);
        }

        public RGPoint Offset(RGPoint other)
        {
            return this.Offset(other.X, other.Y);
        }

        public RGPoint Offset(RGPointI other)
        {
            return this.Offset(other.X, other.Y);
        }

        public RGPoint Offset(Direction d, float distance)
        {
            return this.Offset(d.ToPoint(distance));
        }

        public bool IsEmpty
        {
            get { return this.X == 0 && this.Y == 0; }
        }

        public bool IsInfinity
        {
            get { return (float.IsPositiveInfinity(this.X) || float.IsPositiveInfinity(this.Y) || float.IsNegativeInfinity(this.X) || float.IsNegativeInfinity(this.Y)); }
        }

        public RGPoint Difference(RGPoint other)
        {
            return new RGPoint(this.X - other.X, this.Y - other.Y);
        }

        public override string ToString()
        {
            return "{X:" + this.X + ",Y:" + this.Y + "}";
        }

        public RGPoint Reverse()
        {
            if (!this.Direction.HasValue)
                return this;

            return GeometryUtil.AngleToXY(this.Direction.Value.Reverse().Radians, this.Magnitude);
        }

        public RGPoint Round(int decimals)
        {
            return new RGPoint((float)Math.Round(this.X, decimals), (float)Math.Round(this.Y, decimals));
        }

        public RGPoint Scale(float dx, float dy)
        {
            return new RGPoint(this.X * dx, this.Y * dy);
        }

        public RGPointI ToGridPoint(RGPoint gridPosition, RGSize gridCellSize)
        {
            return new RGPointI((this.X - gridPosition.X) / gridCellSize.Width, (this.Y - gridPosition.Y) / gridCellSize.Height);
        }


        public object GetSaveModel()
        {
            return new RGPointSave  { X = this.X, Y = this.Y };
        }

        public Type GetSaveModelType() { return typeof(RGPointSave); }


        public void Load(object saveModel)
        {
            var m = saveModel as RGPointSave;
            mX = m.X;
            mY = m.Y;
        }

        private class RGPointSave
        {
            public float X;
            public float Y;
        }

        public RGPointI ToPointI()
        {
            return new RGPointI(this.X, this.Y);
        }

        public float GetComponent(Orientation o)
        {
            if (o == Orientation.Horizontal)
                return this.X;
            else if (o == Orientation.Vertical)
                return this.Y;
            else
                return this.Magnitude;
        }

        public RGPoint SetComponent(Orientation o, float value)
        {
            if (o == Orientation.Horizontal)
                return new RGPoint(value, this.Y);
            else if (o == Orientation.Vertical)
                return new RGPoint(this.X, value);
            else if (this.Direction.HasValue)
                return RGPoint.Empty.Offset(this.Direction.Value, value);
            else
                return this;
        }

        public double DotProduct(RGPoint other)
        {
            if(!this.Direction.HasValue || !other.Direction.HasValue)
                return 0f;

            var angleDifference = this.Direction.Value.AngleDifferenceR(other.Direction.Value);
            return this.Magnitude * Util.Cosine(angleDifference);
        }
    }

    public struct RGPointI
    {
        public static RGPointI Empty { get { return new RGPointI(0, 0); } }
        public static RGPointI Min { get { return new RGPointI(Int32.MinValue, Int32.MinValue); } }

        public int X;
        public int Y;

        public bool IsInfinity
        {
            get { return (float.IsPositiveInfinity(this.X) || float.IsPositiveInfinity(this.Y) || float.IsNegativeInfinity(this.X) || float.IsNegativeInfinity(this.Y)); }
        }

        public bool IsEmpty
        {
            get { return this.X == 0 && this.Y == 0; }
        }

        public Direction GetDirectionTo(RGPointI other)
        {
            return Engine.Direction.FromRad(Util.GetLineAngleR(this, other));
        }


        public float GetDistanceTo(RGPointI other)
        {
            float x = this.X - other.X;
            float y = this.Y - other.Y;
            return (float)Math.Sqrt(x * x + y * y);
        }

        public RGPointI(int pX, int pY)
        {
            this.X = pX;
            this.Y = pY;
        }

        public RGPointI(float pX, float pY)
        {
            this.X = (int)pX;
            this.Y = (int)pY;
        }

        public Direction Direction
        {
            get
            {
                return Direction.FromRad((float)GeometryUtil.GetLineAngleR(RGPointI.Empty, this));
            }
        }

        public float Magnitude
        {
            get
            {
                return (float)Math.Sqrt(X * X + Y * Y);
            }
        }

        public RGPointI Offset(int x, int y)
        {
            return new RGPointI(X + x, Y + y);
        }

        public RGPointI Offset(RGPointI other)
        {
            return this.Offset(other.X, other.Y);
        }

        public RGPointI Offset(RGPoint other)
        {
            return this.Offset((int)other.X, (int)other.Y);
        }

        public RGPointI Offset(Direction d, int distance)
        {
            var pt = d.ToPoint(distance);
            return this.Offset((int)pt.X, (int)pt.Y);
        }

        public RGPointI Offset(Direction d, float distance)
        {
            var pt = d.ToPoint(distance);
            return this.Offset((int)pt.X, (int)pt.Y);
        }

        //TBD - replace with next fn
        public RGPointI Translate(RGRectangleI originalArea, RGRectangleI newArea)
        {
            var x = this.X - originalArea.X;
            var y= this.Y - originalArea.Y;
            double xScale = (double)newArea.Width / (double)originalArea.Width;
            double yScale = (double)newArea.Height / (double)originalArea.Height;

            return new RGPointI(newArea.X + (int)(x * xScale), newArea.Y + (int)(y * yScale));
        }

        public RGPointI Scale(RGSizeI originalSize, RGSizeI newSize)
        {
            var x = this.X;
            var y = this.Y;
            float xScale = (float)newSize.Width / (float)originalSize.Width;
            float yScale = (float)newSize.Height / (float)originalSize.Height;

            return new RGPointI(x * xScale,y * yScale);
        }


        public RGPointI Scale(float dx, float dy)
        {
            return new RGPointI(this.X * dx, this.Y * dy);
        }

        public RGPointI Scale(float scale)
        {
            return Scale(scale, scale);
        }

        public RGPointI RelativeTo(RGPointI other)
        {
            return new RGPointI(this.X - other.X, this.Y - other.Y);
        }

        public RGRectangle ToGridCell(RGSize gridCellSize)
        {
            return RGRectangle.FromXYWH(this.X * gridCellSize.Width, this.Y * gridCellSize.Height, gridCellSize.Width, gridCellSize.Height);
        }

        public RGPoint ToPointF() { return new RGPoint(this.X, this.Y); }

        public override string ToString()
        {
            return "{X:" + this.X + ",Y:" + this.Y + "}";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RGPointI))
                return false;

            var other = (RGPointI)obj;
            return this.X == other.X && this.Y == other.Y;
        }

        public RGPointI Difference(RGPointI other)
        {
            return new RGPointI(this.X - other.X, this.Y - other.Y);
        }
    }

    public struct RGLine
    {
        public static RGLine Empty { get { return new RGLine(RGPointI.Empty, RGPointI.Empty); } }

        public bool IsEmpty { get { return PointA.IsEmpty && PointB.IsEmpty; } }

        public RGPointI PointA { get; private set; }
        public RGPointI PointB { get; private set; }

        public Direction Angle { get { return Direction.FromRad(GeometryUtil.GetLineAngleR(PointA, PointB)); } }

        public float Length { get { return PointA.GetDistanceTo(PointB); } }

        public RGLine(RGPointI a, RGPointI b)
            : this()
        {
            PointA = a;
            PointB = b;
        }

        public RGLine(RGPointI src, Direction dir, float distance)
            : this()
        {
            PointA = src;
            PointB = src.Offset(dir, distance);
        }
    
        public RGPointI GetIntersectionPoint(RGLine other)
        {
            return GeometryUtil.GetLineIntersection(this.PointA, this.PointB, other.PointA, other.PointB).ToPointI();
        }


        public RGLine GetIntersectingLine(RGRectangleI rec)
        {
            var lines = new RGLine[] { new RGLine(rec.TopLeft, rec.TopRight), new RGLine(rec.TopLeft, rec.BottomLeft), new RGLine(rec.BottomLeft, rec.BottomRight), new RGLine(rec.TopRight, rec.BottomRight) };

            var thisLine = this;
            var line = lines.FirstOrDefault(p => !thisLine.GetIntersectionPoint(p).IsInfinity);

            return line;
        }

        public RGPoint ToVector()
        {
            return new RGPoint(PointB.X - PointA.X, PointB.Y - PointA.Y);
        }

        public IEnumerable<RGPointI> GetIntersectionPoints(RGRectangleI other)
        {
            var top = this.GetIntersectionPoint(new RGLine(other.TopLeft, other.TopRight));
            var left = this.GetIntersectionPoint(new RGLine(other.TopLeft, other.BottomLeft));
            var bottom = this.GetIntersectionPoint(new RGLine(other.BottomLeft, other.BottomRight));
            var right = this.GetIntersectionPoint(new RGLine(other.TopRight, other.BottomRight));

            RGPointI[] pt = new RGPointI[] { top, left, bottom, right };
            return pt.Where(p => !p.IsInfinity);
        }

        public RGLine Extend(float length)
        {
            return new RGLine(PointA.Offset(Angle.Reverse(), length / 2f), PointB.Offset(Angle, length / 2f));
        }

        public RGLine ExtendB(float length)
        {
            return new RGLine(PointA, PointB.Offset(Angle, length));
        }

        public RGLine Extend()
        {
            return Extend(1000);
        }
    }

    public struct RGSize
    {
        public static RGSize Empty { get { return new RGSize(0, 0); } }

        public float Width;
        public float Height;

        public RGSize(float pWidth, float pHeight)
        {
            this.Width = pWidth;
            this.Height = pHeight;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RGSize))
                return false;

            var other = (RGSize)obj;
            return this.Width == other.Width && this.Height == other.Height;
        }
    }

    public struct RGSizeI
    {
        public static RGSizeI Empty { get { return new RGSizeI(0, 0); } }

        public int Width;
        public int Height;

        public RGSizeI(int pWidth, int pHeight)
        {
            this.Width = pWidth;
            this.Height = pHeight;
        }

        public RGSizeI(float pWidth, float pHeight)
        {
            this.Width = (int)pWidth;
            this.Height = (int)pHeight;
        }


        public override bool Equals(object obj)
        {
            if (!(obj is RGSizeI))
                return false;

            var other = (RGSizeI)obj;
            return this.Width == other.Width && this.Height == other.Height;
        }

        public override int GetHashCode()
        {
            return Width * Height;
        }

        public bool IsZero { get { return this.Width == 0 || this.Height == 0; } }

        public RGSize ToSizeF() { return new RGSize((float)this.Width, (float)this.Height); }
    }

    public struct RGRectangle : ISerializable 
    {
        public static RGRectangle Empty { get { return RGRectangle.FromTLBR(0, 0, 0, 0); } }

        public RGSize Size { get { return new RGSize(this.Width, this.Height); } }

        public bool IsEmpty { get { return this.Width == 0 && this.Height == 0; } }

        public float Left;
        public float Top;
        public float Right;
        public float Bottom;

        public float X { get { return Left; } set { Left = value; } }
        public float Y { get { return Top; } set { Top = value; } }
        public float Width { get { return Right - Left; } set { Right = Left + value; } }
        public float Height { get { return Bottom - Top; } set { Bottom = Top + value; } }

        public RGPoint TopLeft { get { return new RGPoint(Left, Top); } }
        public RGPoint TopRight { get { return new RGPoint(Right, Top); } }
        public RGPoint BottomRight { get { return new RGPoint(Right, Bottom); } }
        public RGPoint BottomLeft { get { return new RGPoint(Left, Bottom); } }

        public RGPoint Center { get { return new RGPoint(X + (Width / 2), Y + (Height / 2)); } }
   
        public override string ToString()
        {
            return "{L:" + this.Left + ",T:" + this.Top + ",R:" + this.Right + ",B:" + this.Bottom + "}";
        }

        public static RGRectangle FromTLBR(float top, float left, float bottom, float right)
        {
            RGRectangle rec = new RGRectangle();
            rec.Left = left;
            rec.Top = top;
            rec.Bottom = bottom;
            rec.Right = right;
            return rec;
        }

        public static RGRectangle FromXYWH(float x, float y, float width, float height)
        {
            RGRectangle rec = new RGRectangle();
            rec.X = x;
            rec.Y = y;
            rec.Width = width;
            rec.Height = height;
            return rec;
        }

        public static RGRectangle Create(RGPoint pt, RGSize size)
        {
            RGRectangle rec = new RGRectangle();
            rec.X = pt.X;
            rec.Y = pt.Y;
            rec.Width = size.Width;
            rec.Height = size.Height;
            return rec;
        }

        public RGRectangle Round()
        {
            return RGRectangle.FromTLBR((float)Math.Round(this.Top, 1), (float)Math.Round(this.Left, 1), (float)Math.Round(this.Bottom, 1), (float)Math.Round(this.Right, 1));
        }
        public bool Contains(RGPoint point)
        {
            return point.X >= this.Left && point.X <= this.Right && point.Y >= this.Top && point.Y <= this.Bottom;
        }

        public bool CollidesWith(RGRectangle other)
        {
           // return CollidesWithIgnoreEdges(other);
            return (this.Bottom >= other.Top && this.Top <= other.Bottom && this.Right >= other.Left && this.Left <= other.Right);
        }

        public bool CollidesWithIgnoreEdges(RGRectangle other)
        {
            return (this.Bottom > other.Top && this.Top < other.Bottom && this.Right > other.Left && this.Left < other.Right);
        }

        public RGRectangle Offset(float x, float y)
        {
            return RGRectangle.FromXYWH(this.X + x, this.Y + y, this.Width, this.Height);
        }

        public RGRectangle Offset(RGPoint pt)
        {
            return RGRectangle.FromXYWH(this.X + pt.X, this.Y + pt.Y, this.Width, this.Height);
        }

        public RGRectangle Expand(float amount)
        {
            return RGRectangle.FromTLBR(this.Top - amount, this.Left - amount, this.Bottom + amount, this.Right + amount);
        }

        public RGPoint GetOppositePoint(RGPoint pt, Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
            {
                if (pt.X == this.Left)
                    return new RGPoint(this.Right, pt.Y);
                else
                    return new RGPoint(this.Left, pt.Y);
            }
            else if (orientation == Orientation.Vertical)
            {
                if (pt.X == this.Top)
                    return new RGPoint(pt.X, this.Bottom);
                else
                    return new RGPoint(pt.X, this.Top);
            }

            throw new Exception("Invalid orientation");

        }

        public RGPoint? GetClosestSurfacePoint(RGPoint src)
        {
            bool lr = (src.Y >= this.Top && src.Y <= this.Bottom);
            bool ud = (src.X >= this.Left && src.X <= this.Right);

            if (src.X < this.Left && lr)
                return new RGPoint(this.Left, src.Y);

            if (src.X > this.Right && lr)
                return new RGPoint(this.Right, src.Y);

            if (src.Y < this.Top && ud)
                return new RGPoint(src.X, this.Top);

            if (src.Y > this.Bottom && ud)
                return new RGPoint(src.X, this.Bottom);

            return null;
        }

        public RGRectangle Floor()
        {
            var rx = (float)Math.Floor(this.X);
            var ry = (float)Math.Floor(this.Y);
            var rw = (float)Math.Floor(this.Width);
            var rh = (float)Math.Floor(this.Height);
            return RGRectangle.FromXYWH(rx, ry, rw, rh);

        }

        public RGRectangleI ToRecI()
        {
            var rx = (float)Math.Floor(this.X);
            var ry = (float)Math.Floor(this.Y);
            var rw = (float)Math.Floor(this.Width);
            var rh = (float)Math.Floor(this.Height);
            return RGRectangleI.FromXYWH(rx, ry, rw, rh);
        }

        private class RGRectangleSaveModel
        {
            public float X;
            public float Y;
            public float Width;
            public float Height;
        }

        public object GetSaveModel()
        {
            return new RGRectangleSaveModel { X = this.X, Y = this.Y, Width = this.Width, Height = this.Height };
        }

        public Type GetSaveModelType() { return typeof(RGRectangleSaveModel); }

        public void Load(object saveModel)
        {
            var m = saveModel as RGRectangleSaveModel;
            this.X = m.X;
            this.Y = m.Y;
            this.Width = m.Width;
            this.Height = m.Height;                
        }
    }

    public struct RGRectangleI : ISerializable 
    {
        public static RGRectangleI Empty { get { return RGRectangleI.FromTLBR(0, 0, 0, 0); } }

        public RGSizeI Size { get { return new RGSizeI(this.Width, this.Height); } }

        public bool IsEmpty { get { return this.Width == 0 && this.Height == 0; } }

        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public int X { get { return Left; } set { Left = value; } }
        public int Y { get { return Top; } set { Top = value; } }
        public int Width { get { return Right - Left; } set { Right = Left + value; } }
        public int Height { get { return Bottom - Top; } set { Bottom = Top + value; } }

        public RGPointI TopLeft { get { return new RGPointI(Left, Top); } }
        public RGPointI TopRight { get { return new RGPointI(Right, Top); } }
        public RGPointI BottomRight { get { return new RGPointI(Right, Bottom); } }
        public RGPointI BottomLeft { get { return new RGPointI(Left, Bottom); } }

        public RGLine TopSide { get { return new RGLine(this.TopLeft, this.TopRight); } }
        public RGLine LeftSide { get { return new RGLine(this.TopLeft, this.BottomLeft); } }
        public RGLine RightSide { get { return new RGLine(this.TopRight, this.BottomRight); } }
        public RGLine BottomSide { get { return new RGLine(this.BottomLeft, this.BottomRight); } }


        public RGPoint Center { get { return new RGPoint(X + (Width / 2), Y + (Height / 2)); } }

        public override string ToString()
        {
            return "{L:" + this.Left + ",T:" + this.Top + ",R:" + this.Right + ",B:" + this.Bottom + "}";
        }

        public static RGRectangleI FromTLBR(float top, float left, float bottom, float right)
        {
            RGRectangleI rec = new RGRectangleI();
            rec.Left = (int)left;
            rec.Top = (int)top;
            rec.Bottom = (int)bottom;
            rec.Right = (int)right;
            return rec;
        }

        public RGPointI? GetClosestSurfacePoint(RGPointI src)
        {
            bool lr = (src.Y >= this.Top && src.Y <= this.Bottom);
            bool ud = (src.X >= this.Left && src.X <= this.Right);

            if (src.X < this.Left && lr)
                return new RGPointI(this.Left, src.Y);

            if (src.X > this.Right && lr)
                return new RGPointI(this.Right, src.Y);

            if (src.Y < this.Top && ud)
                return new RGPointI(src.X, this.Top);

            if (src.Y > this.Bottom && ud)
                return new RGPointI(src.X, this.Bottom);

            return null;
        }

        public static RGRectangleI FromTLBR(int top, int left, int bottom, int right)
        {
            RGRectangleI rec = new RGRectangleI();
            rec.Left = left;
            rec.Top = top;
            rec.Bottom = bottom;
            rec.Right = right;
            return rec;
        }

        public static RGRectangleI FromXYWH(float x, float y, float width, float height)
        {
            RGRectangleI rec = new RGRectangleI();
            rec.X = (int)Math.Round(x,2);
            rec.Y = (int)Math.Round(y,2);
            rec.Width = (int)Math.Round(width,2);
            rec.Height = (int)Math.Round(height, 2);
            return rec;
        }

        public static RGRectangleI FromXYWH(int x, int y, int width, int height)
        {
            RGRectangleI rec = new RGRectangleI();
            rec.X = x;
            rec.Y = y;
            rec.Width = width;
            rec.Height = height;
            return rec;
        }

        public static RGRectangleI Create(RGPointI pt, RGSizeI size)
        {
            RGRectangleI rec = new RGRectangleI();
            rec.X = pt.X;
            rec.Y = pt.Y;
            rec.Width = size.Width;
            rec.Height = size.Height;
            return rec;
        }

        public static RGRectangleI Create(RGPointI pt1, RGPointI pt2)
        {
            RGRectangleI rec = new RGRectangleI();

            try
            {
                rec.X = Math.Min(pt1.X, pt2.X);
                rec.Y = Math.Min(pt1.Y, pt2.Y);
                rec.Width = Math.Abs(pt1.X - pt2.X);
                rec.Height = Math.Abs(pt1.Y - pt2.Y);
                return rec;
            }
            catch
            {
                return RGRectangleI.Empty;
            }
        }


        public RGRectangle ToRecF() { return RGRectangle.FromXYWH(this.X, this.Y, this.Width, this.Height); }


        public bool Contains(RGPointI point)
        {
            return point.X >= this.Left && point.X < this.Right && point.Y >= this.Top && point.Y < this.Bottom;
        }

        public bool CollidesWith(RGRectangleI other)
        {
            return (this.Bottom >= other.Top && this.Top <= other.Bottom && this.Right >= other.Left && this.Left <= other.Right);
        }

        public bool CollidesWith(RGRectangle other)
        {
            return (this.Bottom > other.Top && this.Top < other.Bottom && this.Right > other.Left && this.Left < other.Right);
        }

        public RGRectangleI Offset(int x, int y)
        {
            return RGRectangleI.FromXYWH(this.X + x, this.Y + y, this.Width, this.Height);
        }

        public RGRectangleI Offset(float x, float y)
        {
            return RGRectangleI.FromXYWH(this.X + x, this.Y + y, this.Width, this.Height);
        }


        public RGRectangleI Offset(RGPointI pt)
        {
            return RGRectangleI.FromXYWH(this.X + pt.X, this.Y + pt.Y, this.Width, this.Height);
        }

        public RGRectangle Expand(int amount)
        {
            return RGRectangle.FromTLBR(this.Top - amount, this.Left - amount, this.Bottom + amount, this.Right + amount);
        }

        public RGRectangleI Translate(RGRectangleI originalArea, RGRectangleI newArea)
        {
            var x = this.X - originalArea.X;
            var y = this.Y - originalArea.Y;
            float xScale = (float)newArea.Width / (float)originalArea.Width;
            float yScale = (float)newArea.Height / (float)originalArea.Height;

            return RGRectangleI.FromXYWH(newArea.X + (x * xScale), newArea.Y + (y * yScale),
               this.Width * xScale, this.Height * yScale);
        }

        public RGRectangleI CenterWithin(RGRectangleI container)
        {
            return RGRectangleI.FromXYWH((container.Width - this.Width) / 2, (container.Height - this.Height) / 2, this.Width, this.Height);
        }

        public bool Contains(RGRectangleI other)
        {
            return other.Left >= this.Left && other.Right <= this.Right && other.Top >= this.Top && other.Bottom <= this.Bottom;
        }

        /// <summary>
        /// Creates a new rectangle that does not go beyond the boundries of the containing rectangle.
        /// </summary>
        /// <param name="containingRectangle"></param>
        /// <returns></returns>
        public RGRectangleI CropWithin(RGRectangleI containingRectangle)
        {
            return RGRectangleI.FromTLBR(
                Math.Max(containingRectangle.Top, this.Top),
                Math.Max(containingRectangle.Left, this.Left),
                Math.Min(containingRectangle.Bottom, this.Bottom),
                Math.Min(containingRectangle.Right, this.Right));
        }

        public RGRectangleI Scale(float x, float y)
        {
            return RGRectangleI.FromXYWH(this.X, this.Y, this.Width * x, this.Height * y);
        }

        /// <summary>
        /// Scales this rectangle as large as it can get without going outside of the container, without changing its aspect ratio.
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public RGRectangleI ScaleWithin(RGRectangleI container)
        {
            var ratio = (float)this.Height / (float)this.Width;

            var scaledArea = RGRectangleI.FromXYWH(container.X, container.Y, container.Width, (int)(container.Width * ratio));
            if (scaledArea.Height > container.Height)
                scaledArea = RGRectangleI.FromXYWH(scaledArea.X, scaledArea.Y, (int)(container.Height / ratio), container.Height);

            return scaledArea;
        }

        #region Saving

        private class RGRectangleISaveModel
        {
            public int X;
            public int Y;
            public int Width;
            public int Height;
        }

        public object GetSaveModel()
        {
            return new RGRectangleISaveModel { X = this.X, Y = this.Y, Width = this.Width, Height = this.Height };
        }

        public Type GetSaveModelType() { return typeof(RGRectangleISaveModel); }

        public void Load(object saveModel)
        {
            var m = saveModel as RGRectangleISaveModel;
            this.X = m.X;
            this.Y = m.Y;
            this.Width = m.Width;
            this.Height = m.Height;
        }

        #endregion

    }

    public struct RGColor
    {
        public static RGColor White { get { return RGColor.FromRGB(255, 255, 255); } }
        public static RGColor Black { get { return RGColor.FromRGB(0, 0, 0); } }

        public byte Red;
        public byte Green;
        public byte Blue;

        public static RGColor FromRGB(byte r, byte g, byte b)
        {
            var c = new RGColor();
            c.Red = r;
            c.Green = g;
            c.Blue = b;
            return c;
        }
    }

    public class DirectionFlags
    {
        public static DirectionFlags Horizontal { get { return new DirectionFlags(false, false, true, true); } }
        public static DirectionFlags Vertical { get { return new DirectionFlags(true, true, false, false); } }
        public static DirectionFlags None { get { return new DirectionFlags(false, false, false, false); } }


        public bool Up, Down, Left, Right;

        public DirectionFlags() { }

        public DirectionFlags(bool up, bool down, bool left, bool right)
        {
            this.Up = up; 
            this.Down = down;
            this.Left = left;
            this.Right = right;
        }

        public DirectionFlags(Direction d)
        {
            var offset = RGPoint.Empty.Offset(d, 10f);

            Up = offset.Y < 0;
            Down = offset.Y > 0;
            Left = offset.X < 0;
            Right = offset.X > 0;
        }

        public override bool Equals(object obj)
        {
            var other = obj as DirectionFlags;
            if (other == null)
                return (this == null);

            return this.Up == other.Up && this.Down == other.Down && this.Right == other.Right && this.Left == other.Left;
        }

        public override string ToString()
        {
            if (!Up && !Down && !Left && !Right)
                return "None";
            else if (Up && Down && Left && Right)
                return "All";

            return (Up ? "Up " : "") +
                (Down ? "Down " : "") +
                (Left ? "Left " : "") +
                (Right ? "Right " : "");
        }

        public DirectionFlags Invert()
        {
            return new DirectionFlags(!this.Up, !this.Down, !this.Left, !this.Right);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine;

namespace Editor
{

    public class RGColorEditor : System.Drawing.Design.ColorEditor
    {
        public RGColorEditor() 
        {
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            RGColor color = (RGColor)value;
            var systemColor = color.ToSystemColor();

            var newSystemColor = (Color)base.EditValue(context, provider, systemColor);
            return new RGColor { Red = newSystemColor.R, Green = newSystemColor.G, Blue = newSystemColor.B };

        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            RGColor color = (RGColor)e.Value;
            var systemColor = color.ToSystemColor();
            
            var e2 = new PaintValueEventArgs(e.Context,systemColor,e.Graphics,e.Bounds);
            base.PaintValue(e2);
        }
    }
}

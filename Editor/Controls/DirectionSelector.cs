using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine;
using Engine.Collision;

namespace Editor
{
    public partial class DirectionSelector : UserControl
    {

        private bool mAllowDiagonals=true;

        public bool AllowDiagonals { get { return mAllowDiagonals; } set { mAllowDiagonals = value; } }

        public delegate void DirectionChangedEventHandler(DirectionSelector sender, EventArgs args);
        public event DirectionChangedEventHandler DirectionChanged;
  
        public DirectionSelector()
        {
            InitializeComponent();
        }

        public Direction SelectedDirection
        {
            get
            {
                if (IsOnlyChecked(chkRight))
                    return Direction.FromAngle(0);
                if (IsOnlyChecked(chkRight,chkUp))
                    return Direction.FromAngle(45);
                if (IsOnlyChecked(chkUp))
                    return Direction.FromAngle(90);
                if (IsOnlyChecked(chkUp,chkLeft))
                    return Direction.FromAngle(135);
                if (IsOnlyChecked(chkLeft))
                    return Direction.FromAngle(180);
                if (IsOnlyChecked(chkLeft,chkDown))
                    return Direction.FromAngle(225);
                if (IsOnlyChecked(chkDown))
                    return Direction.FromAngle(270);
                if (IsOnlyChecked(chkDown,chkRight))
                    return Direction.FromAngle(315);

                return Direction.FromAngle(0);
            }
        }

        public Side SelectedSide
        {
            get
            {
                var f = this.Flags;
                if (f.Left)
                    return Side.Left;
                if (f.Right)
                    return Side.Right;
                if (f.Up)
                    return Side.Top;
                if (f.Down)
                    return Side.Bottom;
                else
                    return Side.None;
            }
        }

        public DirectionFlags Flags
        {
            get
            {
                var flags = new DirectionFlags();
                flags.Left = chkLeft.Checked;
                flags.Right = chkRight.Checked;
                flags.Up = chkUp.Checked;
                flags.Down = chkDown.Checked;
                return flags;
            }
        }

        private bool IsOnlyChecked(params CheckBox[] checkBoxes)
        {
            var otherCheckboxes = this.Controls.OfType<CheckBox>().Where(p=> !checkBoxes.Contains(p)).ToArray();

            return checkBoxes.All(p => p.Checked) && otherCheckboxes.All(p => !p.Checked);
        }

        private void chkRight_CheckedChanged(object sender, EventArgs e)
        {
            if (chkRight.Checked && !AllowDiagonals)
                { chkUp.Checked = false; chkDown.Checked = false; }

            if(chkRight.Checked) chkLeft.Checked = false;
            RefreshDirection();
        }

        private void chkUp_CheckedChanged(object sender, EventArgs e)
        {

            if (chkUp.Checked && !AllowDiagonals)
                { chkLeft.Checked = false; chkRight.Checked = false; }

            if(chkUp.Checked) chkDown.Checked = false;
            RefreshDirection();
        }

        private void chkLeft_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLeft.Checked && !AllowDiagonals)
                { chkUp.Checked = false; chkDown.Checked = false; }

            if(chkLeft.Checked) chkRight.Checked = false;
            RefreshDirection();
        }

        private void chkDown_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDown.Checked && !AllowDiagonals)
                { chkLeft.Checked = false; chkRight.Checked = false; }

            if(chkDown.Checked) chkUp.Checked = false;
            RefreshDirection();
        }


        private void RefreshDirection()
        {
            txtDegrees.Text = this.SelectedDirection.ToString();
            if (DirectionChanged != null)
                DirectionChanged(this, new EventArgs());
        }

    }
}

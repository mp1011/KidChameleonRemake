using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine;

namespace Editor
{
    public partial class DirectionSelector : UserControl
    {
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
            if(chkRight.Checked) chkLeft.Checked = false;
            RefreshDirection();
        }

        private void chkUp_CheckedChanged(object sender, EventArgs e)
        {
            if(chkUp.Checked) chkDown.Checked = false;
            RefreshDirection();
        }

        private void chkLeft_CheckedChanged(object sender, EventArgs e)
        {
            if(chkLeft.Checked) chkRight.Checked = false;
            RefreshDirection();
        }

        private void chkDown_CheckedChanged(object sender, EventArgs e)
        {
            if(chkDown.Checked) chkUp.Checked = false;
            RefreshDirection();
        }


        private void RefreshDirection()
        {
            txtDegrees.Text = this.SelectedDirection.ToString();
        }

    }
}

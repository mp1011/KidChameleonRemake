using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Editor.Forms
{
    public partial class frmLog : Form
    {
        private static LinkedList<string> mNewLines = new LinkedList<string>();

        public static void AddLine(string text)
        {
            if (String.IsNullOrEmpty(text))
                return;

            lock(mNewLines)
            {
                mNewLines.AddLast(text);
            }
        }

        public frmLog()
        {
            mNewLines.Clear();
            InitializeComponent();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
        }

        private void AddLineInner(string text)
        {
            if (String.IsNullOrEmpty(txtLog.Text))
                txtLog.Text = text;
            else
                txtLog.Text += Environment.NewLine + text;

            txtLog.SelectionStart = txtLog.Text.Length - 1;
            txtLog.ScrollToCaret();
        }

        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            lock (mNewLines)
            {
                while (true)
                {
                    var newLine = mNewLines.First;
                    if(newLine == null)
                        return;

                    AddLineInner(newLine.Value);
                    mNewLines.RemoveFirst();
                }
            }
        }

    }
}

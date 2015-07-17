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
    public partial class TileGroupAdder : UserControl
    {

        public delegate void NamesChangedEventHandler(TileGroupAdder sender, string[] names);
        public event NamesChangedEventHandler NamesAdded;
        public event NamesChangedEventHandler NamesRemoved;

        public TileGroupAdder MainGroup { get; set; }

        private string CurrentName
        {
            get
            {
                if (MainGroup == null)
                    return string.Empty;
                else
                    return MainGroup.Names.FirstOrDefault().NotNull();
            }
        }

        public string[] Names
        {
            get
            {
                return textbox.Text.NotNull().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim()).WhereNotNullOrEmpty().ToArray();               
            }
            set
            {              
                textbox.Text = value.StringJoin(", ");                
            }
        }
        
        public TileGroupAdder()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddName(CurrentName);
            if (NamesAdded != null)
                NamesAdded(this, this.Names);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            RemoveName(CurrentName);
            if (NamesRemoved != null)
                NamesRemoved(this, this.Names);
        }

        public void AddName(string name)
        {
            if (!this.Names.Contains(name))
            {
                var names = this.Names.ToList();
                names.Add(name);
                this.Names = names.ToArray();
            }
        }

        public void RemoveName(string name)
        {
            if (this.Names.Contains(name))
            {
                var names = this.Names.ToList();
                names.Remove(name);
                this.Names = names.ToArray();
            }
        }
    }
}

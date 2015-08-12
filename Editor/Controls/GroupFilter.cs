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
    public partial class GroupFilter : UserControl
    {
        private const string Uncategorized = "(none)";
        private TileSet mTileset;
        private TilePanelUserControl mPanel;

        private GroupFilterRow[] EditorRows
        {
            get
            {
                return (GroupFilterRow[])grid.DataSource;
            }
            set
            {
                grid.DataSource = value;
            }
        }

 
        public GroupFilter()
        {
            InitializeComponent();
        }

        internal void LoadData(TilePanelUserControl panel)
        {
            mTileset = panel.Tileset;
            mPanel = panel;
            List<string> groups = mTileset.GetTiles().SelectMany(p => p.Usage.Groups).Distinct().OrderBy(p => p).ToList();
            groups.Add(Uncategorized);
            var rows = groups.Select(p=>new GroupFilterRow(p)).ToArray();
            grid.DataSource=rows;
        }

        private void grid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var row = this.EditorRows[e.RowIndex];

            if (e.ColumnIndex == 1) // exclusive
            {
                foreach (var otherRow in EditorRows)
                {
                    if (row.Exclusive)
                    {
                        otherRow.Visible = otherRow.Equals(row);
                        otherRow.Exclusive = otherRow.Equals(row);
                    }
                    else
                    {
                        otherRow.Visible = true;
                    }
                }
            }

            RefreshGridFilter();
        }
       
        private void RefreshGridFilter()
        {
            bool anySelected = EditorRows.Any(p => p.Visible);
            var filteredTilset = new TileSet(mTileset.Texture, mTileset.TileSize, mTileset.GetTiles().Where(p => IncludeGroup(p.Usage)));
            mPanel.SetFromTileset(filteredTilset);
            grid.Refresh();
        }

        private bool IncludeGroup(string group)
        {
            var row = EditorRows.FirstOrDefault(p => p.Group == group);
            if (row == null)
                return false;

            return row.Visible;
        }

        private bool HideGroup(string group)
        {
            var row = EditorRows.FirstOrDefault(p => p.Group == group);
            if (row == null)
                return false;

            return row.Hide;
        }

        private bool IncludeGroup(TileUsage usage)
        {    
            if(usage.Groups.IsNullOrEmpty())
                return !HideGroup(Uncategorized) && IncludeGroup(Uncategorized);
            else 
                return !usage.Groups.Any(p=> HideGroup(p)) && usage.Groups.Any(p => IncludeGroup(p));
        }

        private void grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
   
    }

    class GroupFilterRow
    {
        public string Group { get; private set; }
        public bool Exclusive { get; set; }
        public bool Visible { get; set; }
        public bool Hide { get; set; }

        public GroupFilterRow(string group)
        {
            this.Group = group;
            this.Visible = true;
            this.Exclusive = false;
        }

    }
}

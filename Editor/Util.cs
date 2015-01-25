using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Engine;
using System.IO;
using Editor.Forms;

namespace Editor
{
    class FileDialog
    {
        public static T ShowOpenDialog<T>(string path, string extension, Func<string,T> onFileSelected)
        {
            try
            {
                var ofd = new OpenFileDialog();
                ofd.InitialDirectory = path;

                var extensions = extension.Split(';');
                ofd.DefaultExt = extensions[0];
                ofd.Filter = typeof(T).Name + " Files | " + String.Join(";", extensions.Select(ext=> "*." + ext).ToArray());
                if (ofd.ShowDialog() == DialogResult.OK)
                    return onFileSelected(ofd.FileName);
                else                
                    return default(T);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
                return default(T);
            }
        }

        public static T ShowSaveDialog<T>(string path, string extension, Func<string, T> onFileSelected)
        {
            try
            {
                var ofd = new SaveFileDialog();
                ofd.InitialDirectory = path;
                ofd.DefaultExt = extension;
                ofd.Filter = typeof(T).Name + " Files|*." + extension;
                if (ofd.ShowDialog() == DialogResult.OK)
                    return onFileSelected(ofd.FileName);
                else
                    return default(T);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
                return default(T);
            }
        }

        public static string ShowSaveDialog<T>(PathType path, T item)
        {
            var json = Serializer.ToJSON(item);
            return FileDialog.ShowSaveDialog<string>(path.GetFolder(), path.GetExtension(), selectedFile => { File.WriteAllText(selectedFile, json); return Path.GetFileNameWithoutExtension(selectedFile); });
        }

        public static void ShowSaveDialog<T>(PathType path, Func<string, T> save)
        {
            FileDialog.ShowSaveDialog<bool>(path.GetFolder(), path.GetExtension(), selectedPath =>
                {
                    var item = save(selectedPath);
                    var json = Serializer.ToJSON(item);

                    if (File.Exists(selectedPath))
                        File.Delete(selectedPath);
                    File.WriteAllText(selectedPath, json);
                    BackupManager.CreateBackup(selectedPath);
                    return true;
                });
        }

        public static T ShowLoad<T>(PathType path) where T : new()
        {
            return FileDialog.ShowOpenDialog(path.GetFolder(),path.GetExtension(), selectedFile => Serializer.FromJson<T>(File.ReadAllText(selectedFile)));
        }

    }

    static class Extensions
    {
        public static void SetBlockyScaling(this Graphics g)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
        }

        public static bool PixelEquals(this Color a, Color b)
        {
            if (a.A == 0 && b.A == 0)
                return true;

            return a.Equals(b);
        }

        public static Rectangle ToSystemRec(this RGRectangleI rec)
        {
            return new Rectangle(rec.X, rec.Y, rec.Width, rec.Height);
        }

        public static Point ToSystemPoint(this RGPointI pt)
        {
            return new Point(pt.X, pt.Y);
        }

        public static Size ToSystemSize(this RGSizeI size)
        {
            return new Size(size.Width, size.Height);
        }

        public static void AddScroll(this ScrollBar sb, int value)
        {

            int newValue = sb.Value + value;
            if (newValue < sb.Minimum)
                newValue = sb.Minimum;
            if (newValue > sb.Maximum)
                newValue = sb.Maximum;

            sb.Value = newValue;

        }

        public static int Width<T>(this T[,] grid)
        {
            return grid.GetLength(0);
        }

        public static int Height<T>(this T[,] grid)
        {
            return grid.GetLength(1);
        }

        public static void AddDistinct<T>(this List<T> items, T newItem)
        {
            foreach (var item in items)
            {
                if (item.Equals(newItem))
                    return;
            }

            items.Add(newItem);
        }


        public static void AddRangeDistinct<T>(this List<T> items, IEnumerable<T> newItems, IEqualityComparer<T> comparer)
        {
            items.AddRange(newItems);
            var distinctItems = items.Distinct(comparer).ToArray();
            items.Clear();
            items.AddRange(distinctItems);
        }

     
        public static Bitmap CloneImage(this Bitmap source)
        {
            var copy = new Bitmap(source.Width, source.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var g = Graphics.FromImage(copy);
            g.DrawImage(source, Point.Empty);
            g.Dispose();
            return copy;
        }

        public static Bitmap GetImage(this TextureResource resource)
        {
            var editorResource = resource as EditorTextureResource;
            if (editorResource != null)
                return editorResource.GetBitmap();

            var fileStream = System.IO.File.OpenRead(resource.Path.FullPath);
            var bmp = new Bitmap(fileStream);
            fileStream.Close();
            return bmp;
        }

        public static bool IsTransparent(this Color c)
        {
            return c.A == 0;
        }


        public static void SaveDebugImage(this Bitmap img, string name)
        {
            string path = Paths.DebugPath + Path.DirectorySeparatorChar + name + DateTime.Now.Ticks + ".bmp";
            img.Save(path);
        }

        public static bool GetItemChecked(this CheckedListBox box, int index, ItemCheckEventArgs e)
        {
            if (index == e.Index)
                return e.NewValue == CheckState.Checked;
            else
                return box.GetItemChecked(index);
        }

        public static IEnumerable<T> GetCheckedItems<T>(this CheckedListBox box, ItemCheckEventArgs e) where T:class
        {
            for (int i = 0; i < box.Items.Count; i++)
            {
                if (box.GetItemChecked(i, e))
                    yield return box.Items[i] as T;
            }
        }

    }
}

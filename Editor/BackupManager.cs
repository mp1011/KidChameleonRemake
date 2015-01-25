using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Engine;

namespace Editor
{
    static class BackupManager
    {
        public const int NumBackups = 10;

        public static void CreateBackup(string file)
        {
            var backupName = GetBackupFilename(file);
            File.Copy(file, backupName);
        }

        private static string GetBackupFilename(string file)
        {
            var path = Path.Combine(Path.GetDirectoryName(file), "Backups");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = Path.Combine(path, Path.GetFileNameWithoutExtension(file));
            var fileInfo = Enumerable.Range(0,NumBackups).Select(i=> new FileInfo(path + "_bak" + i + Path.GetExtension(file))).ToArray();

            var freeFile = fileInfo.FirstOrDefault(p => !p.Exists);

            if (freeFile != null)
                return freeFile.FullName;

            var oldestFile = fileInfo.MinElement(p => p.LastWriteTime.Ticks);
            oldestFile.Delete();
            return oldestFile.FullName;
        }
    }
}

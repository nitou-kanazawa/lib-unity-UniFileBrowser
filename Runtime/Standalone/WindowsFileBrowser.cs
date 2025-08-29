#if UNITY_STANDALONE_WIN || UNITY_EDITOR
using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Ookii.Dialogs;

namespace UniFileBrowser.Standalone
{
    internal sealed class WindowWrapper : IWin32Window
    {
        private IntPtr _hwnd;
        public IntPtr Handle => _hwnd;

        public WindowWrapper(IntPtr handle) { _hwnd = handle; }
    }


    /// <summary>
    /// 
    /// </summary>
    internal sealed class WindowsFileBrowser : IStandaloneFileBrowser
    {
        /// <inheritdoc/>
        public string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool multiselect)
        {
            using var fd = new VistaOpenFileDialog { Title = title };
            if (extensions != null)
            {
                fd.Filter = GetFilterFromFileExtensionList(extensions);
                fd.FilterIndex = 1;
            }
            else
            {
                fd.Filter = string.Empty;
            }
            fd.Multiselect = multiselect;
            if (!string.IsNullOrEmpty(directory))
            {
                fd.FileName = GetDirectoryPath(directory);
            }

            var res = fd.ShowDialog(new WindowWrapper(NativeMethods.GetActiveWindow()));
            var filenames = res == DialogResult.OK ? fd.FileNames : new string[0];
            return filenames;
        }

        /// <inheritdoc/>
        public string[] OpenFolderPanel(string title, string directory, bool multiselect)
        {
            using var fd = new VistaFolderBrowserDialog { Description = title };
            if (!string.IsNullOrEmpty(directory))
            {
                fd.SelectedPath = GetDirectoryPath(directory);
            }

            var res = fd.ShowDialog(new WindowWrapper(NativeMethods.GetActiveWindow()));
            var filenames = res == DialogResult.OK ? new[] { fd.SelectedPath } : new string[0];
            return filenames;
        }

        /// <inheritdoc/>
        public string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions)
        {
            using var fd = new VistaSaveFileDialog { Title = title };

            var finalFilename = "";
            if (!string.IsNullOrEmpty(directory))
            {
                finalFilename = GetDirectoryPath(directory);
            }
            if (!string.IsNullOrEmpty(defaultName))
            {
                finalFilename += defaultName;
            }

            fd.FileName = finalFilename;
            if (extensions != null)
            {
                fd.Filter = GetFilterFromFileExtensionList(extensions);
                fd.FilterIndex = 1;
                fd.DefaultExt = extensions[0].Extensions[0];
                fd.AddExtension = true;
            }
            else
            {
                fd.DefaultExt = string.Empty;
                fd.Filter = string.Empty;
                fd.AddExtension = false;
            }

            var res = fd.ShowDialog(new WindowWrapper(NativeMethods.GetActiveWindow()));
            var filename = res == DialogResult.OK ? fd.FileName : "";
            return filename;
        }


        #region Private Method

        // .NET Framework FileDialog Filter format
        // https://msdn.microsoft.com/en-us/library/microsoft.win32.filedialog.filter
        private static string GetFilterFromFileExtensionList(ExtensionFilter[] extensions)
        {
            var filterString = "";
            foreach (var filter in extensions)
            {
                filterString += filter.Name + "(";

                foreach (var ext in filter.Extensions)
                {
                    filterString += "*." + ext + ",";
                }

                filterString = filterString.Remove(filterString.Length - 1);
                filterString += ") |";

                foreach (var ext in filter.Extensions)
                {
                    filterString += "*." + ext + "; ";
                }

                filterString += "|";
            }
            filterString = filterString.Remove(filterString.Length - 1);
            return filterString;
        }

        private static string GetDirectoryPath(string directory)
        {
            var directoryPath = Path.GetFullPath(directory);
            if (!directoryPath.EndsWith("\\"))
            {
                directoryPath += "\\";
            }
            if (Path.GetPathRoot(directoryPath) == directoryPath)
            {
                return directory;
            }
            return Path.GetDirectoryName(directoryPath) + Path.DirectorySeparatorChar;
        }
        #endregion


        private static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern IntPtr GetActiveWindow();
        }
    }
}
#endif

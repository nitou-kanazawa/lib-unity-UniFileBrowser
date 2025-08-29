using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UniFileBrowser.Standalone
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class LinuxFileBrowser : IStandaloneFileBrowser
    {

        public LinuxFileBrowser()
        {
            NativeMethods.DialogInit();
        }

        /// <inheritdoc/>
        public string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool multiselect)
        {
            var paths = Marshal.PtrToStringAnsi(NativeMethods.DialogOpenFilePanel(
                title,
                directory,
                GetFilterFromFileExtensionList(extensions),
                multiselect));
            return paths.Split((char)28);
        }

        /// <inheritdoc/>
        public string[] OpenFolderPanel(string title, string directory, bool multiselect)
        {
            var paths = Marshal.PtrToStringAnsi(NativeMethods.DialogOpenFolderPanel(
                title,
                directory,
                multiselect));
            return paths.Split((char)28);
        }

        /// <inheritdoc/>
        public string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions)
        {
            return Marshal.PtrToStringAnsi(NativeMethods.DialogSaveFilePanel(
                title,
                directory,
                defaultName,
                GetFilterFromFileExtensionList(extensions)));
        }


        private static string GetFilterFromFileExtensionList(ExtensionFilter[] extensions)
        {
            if (extensions == null)
            {
                return "";
            }

            var filterString = "";
            foreach (var filter in extensions)
            {
                filterString += filter.Name + ";";

                foreach (var ext in filter.Extensions)
                {
                    filterString += ext + ",";
                }

                filterString = filterString.Remove(filterString.Length - 1);
                filterString += "|";
            }
            filterString = filterString.Remove(filterString.Length - 1);
            return filterString;
        }


        private static class NativeMethods
        {
            [DllImport("StandaloneFileBrowser")]
            public static extern void DialogInit();
            
            [DllImport("StandaloneFileBrowser")]
            public static extern IntPtr DialogOpenFilePanel(string title, string directory, string extension, bool multiselect);
            
            [DllImport("StandaloneFileBrowser")]
            public static extern void DialogOpenFilePanelAsync(string title, string directory, string extension, bool multiselect, AsyncCallback callback);
            
            [DllImport("StandaloneFileBrowser")]
            public static extern IntPtr DialogOpenFolderPanel(string title, string directory, bool multiselect);
            
            [DllImport("StandaloneFileBrowser")]
            public static extern void DialogOpenFolderPanelAsync(string title, string directory, bool multiselect, AsyncCallback callback);
            
            [DllImport("StandaloneFileBrowser")]
            public static extern IntPtr DialogSaveFilePanel(string title, string directory, string defaultName, string extension);

            [DllImport("StandaloneFileBrowser")]
            public static extern void DialogSaveFilePanelAsync(string title, string directory, string defaultName, string extension, AsyncCallback callback);
        }
    }
}

#if UNITY_EDITOR
using System;
using UnityEditor;

namespace UniFileBrowser.Standalone
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class EditorFileBrowser : IStandaloneFileBrowser
    {
        /// <inheritdoc/>
        public string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool multiselect)
        {
            string path = (extensions == null)
                ? EditorUtility.OpenFilePanel(title, directory, "")
                : EditorUtility.OpenFilePanelWithFilters(title, directory, GetFilterFromFileExtensionList(extensions));

            return string.IsNullOrEmpty(path) ? new string[0] : new[] { path };
        }

        /// <inheritdoc/>
        public string[] OpenFolderPanel(string title, string directory, bool multiselect)
        {
            var path = EditorUtility.OpenFolderPanel(title, directory, "");
            return string.IsNullOrEmpty(path) ? new string[0] : new[] { path };
        }


        /// <inheritdoc/>
        public string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions)
        {
            var ext = (extensions != null) ? extensions[0].Extensions[0] : "";
            var name = string.IsNullOrEmpty(ext) ? defaultName : defaultName + "." + ext;
            return EditorUtility.SaveFilePanel(title, directory, name, ext);
        }


        #region Private Method

        private static string[] GetFilterFromFileExtensionList(ExtensionFilter[] extensions)
        {
            var filters = new string[extensions.Length * 2];
            for (int i = 0; i < extensions.Length; i++)
            {
                filters[(i * 2)] = extensions[i].Name;
                filters[(i * 2) + 1] = string.Join(",", extensions[i].Extensions);
            }
            return filters;
        }
        #endregion
    }
}
#endif

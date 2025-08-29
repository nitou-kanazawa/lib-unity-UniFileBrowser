using System;

namespace UniFileBrowser.Standalone
{
    /// <summary>
    /// 
    /// </summary>
    public interface IStandaloneFileBrowser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="directory"></param>
        /// <param name="extensions">拡張子</param>
        /// <param name="multiselect"></param>
        /// <returns></returns>
        string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool multiselect);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="directory"></param>
        /// <param name="multiselect"></param>
        /// <returns></returns>
        string[] OpenFolderPanel(string title, string directory, bool multiselect);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="directory"></param>
        /// <param name="defaultName"></param>
        /// <param name="extensions"></param>
        /// <returns></returns>
        string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IAsyncStandaloneFileBrowser
    {
        void OpenFilePanelAsync(string title, string directory, ExtensionFilter[] extensions, bool multiselect, Action<string[]> callback);
        void OpenFolderPanelAsync(string title, string directory, bool multiselect, Action<string[]> callback);
        void SaveFilePanelAsync(string title, string directory, string defaultName, ExtensionFilter[] extensions, Action<string> callback);
    }
}

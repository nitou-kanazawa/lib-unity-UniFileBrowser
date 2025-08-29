using UnityEngine;

namespace UniFileBrowser.Standalone
{
    internal sealed class MacFileBrowser : IStandaloneFileBrowser
    {
        public string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool multiselect)
        {
            throw new System.NotImplementedException();
        }

        public string[] OpenFolderPanel(string title, string directory, bool multiselect)
        {
            throw new System.NotImplementedException();
        }

        public string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions)
        {
            throw new System.NotImplementedException();
        }


    }
}

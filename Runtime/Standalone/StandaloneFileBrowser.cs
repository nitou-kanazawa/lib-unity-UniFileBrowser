namespace UniFileBrowser.Standalone
{
    /// <summary>
    /// </summary>
    public static class StandaloneFileBrowser
    {
        private static readonly IStandaloneFileBrowser FileBrowser;

        static StandaloneFileBrowser()
        {
#if UNITY_EDITOR
            FileBrowser = new EditorFileBrowser();
#elif UNITY_STANDALONE_OSX
            FileBrowser = new MacFileBrowser();
#elif UNITY_STANDALONE_WIN
            FileBrowser = new WindowsFileBrowser();
#elif UNITY_STANDALONE_LINUX
            FileBrowser = new LinuxFileBrowser();
#endif
        }

        public static string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool multiselect)
        {
            return GetBrowser().OpenFilePanel(title, directory, extensions, multiselect);
        }

        public static string[] OpenFolderPanel(string title, string directory, bool multiselect)
        {
            return GetBrowser().OpenFolderPanel(title, directory, multiselect);
        }

        public static string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions)
        {
            return GetBrowser().SaveFilePanel(title, directory, defaultName, extensions);
        }

        private static IStandaloneFileBrowser GetBrowser()
        {
            if (FileBrowser == null)
                throw new System.PlatformNotSupportedException(
                    "StandaloneFileBrowser is not supported on the current platform.");
            return FileBrowser;
        }
    }
}
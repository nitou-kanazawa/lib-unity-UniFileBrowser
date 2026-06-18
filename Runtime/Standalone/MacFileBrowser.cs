using System;

namespace UniFileBrowser.Standalone
{
    /// <summary>
    ///     macOS standalone 向けの実装。
    ///     現時点では未対応であり、各メソッドは <see cref="NotSupportedException" /> を投げる。
    /// </summary>
    internal sealed class MacFileBrowser : IStandaloneFileBrowser
    {
        private const string NotSupportedMessage =
            "macOS standalone is not supported by UniFileBrowser yet.";

        public string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool multiselect)
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        public string[] OpenFolderPanel(string title, string directory, bool multiselect)
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        public string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions)
        {
            throw new NotSupportedException(NotSupportedMessage);
        }
    }
}

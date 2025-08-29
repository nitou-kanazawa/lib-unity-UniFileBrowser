
namespace UniFileBrowser
{
    public struct ExtensionFilter
    {
        public string Name;
        public string[] Extensions;

        public ExtensionFilter(string filterName, params string[] filterExtensions)
        {
            Name = filterName;
            Extensions = filterExtensions;
        }

        public int GetExtensionCount()
        {
            return Extensions?.Length ?? 0;
        }


        #region Static

        public static readonly ExtensionFilter Images = new("Images", "png", "jpg", "jpeg", "gif", "bmp", "webp");
        public static readonly ExtensionFilter Text = new("Text", "txt", "csv", "json", "xml");
        public static readonly ExtensionFilter Audio = new("Audio", "mp3", "wav", "ogg", "m4a", "aac");
        public static readonly ExtensionFilter Video = new("Video", "mp4", "avi", "mov", "wmv", "mkv", "webm");
        public static readonly ExtensionFilter All = new("All", "*");
        #endregion
    }
}

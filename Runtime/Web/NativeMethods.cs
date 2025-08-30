using System;
using System.Runtime.InteropServices;

namespace UniFileBrowser.Web {
    public static class NativeMethods {
        [DllImport("__Internal")]
        public static extern void OpenFileDialog(string filter, int taskId, Action<int, string> action);

        //[DllImport("__Internal")]
        //public static extern void SaveFile(string filename, byte[] byteArray, int byteArraySize, int taskId, Action<int, string> action);
    }
}
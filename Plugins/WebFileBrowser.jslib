var FileBrowser = {

    /**
     * ファイル選択ダイアログを開く
     * @param {number} filterPtr - ファイルフィルタ文字列へのポインタ (例: ".png,.jpg")
     * @param {number} taskId - タスクID
     * @param {number} callback - Unity側のコールバック関数ポインタ
     * @returns {void}
    */
    OpenFileDialog: function (filterPtr, taskId, callback) {

        // Create copy of message because it might be deleted before callback is run
        var filter = UTF8ToString(filterPtr);

        const fileInputId = "file-input-" + taskId;
        var fileInput = document.getElementById(fileInputId);

        // Delete if element exist
        if (fileInput) {
            document.body.removeChild(fileInput);
        }

        // Create new element
        fileInput = document.createElement('input');
        fileInput.setAttribute('id', fileInputId);
        fileInput.setAttribute('type', 'file');
        fileInput.setAttribute('style', 'display:none; visibility:hidden;');

        if (filter) {
            fileInput.setAttribute('accept', filter);
        }

        // クリック時の処理：同じファイルの再選択を可能にするため値をクリア
        fileInput.onclick = function (event) {
            event.target.value = null;
        };

        // ファイル選択時の処理：選択されたファイルをUnityに通知
        fileInput.onchange = function (event) {
            try {
                if (!event.target.files || event.target.files.length === 0) {
                    throw new Error('No file selected');
                }

                // JS文字列をUTF8文字列に変換して確保
                var file = event.target.files[0];
                var fileUrl = URL.createObjectURL(file);
                var buffer = stringToNewUTF8(fileUrl);

                // コールバック呼び出し（taskIdとbufferを渡す）
                {{{ makeDynCall('vii', 'callback') }}} (taskId, buffer);

            } catch (error) {
                console.error('File selection error:', error);
                // エラー時は空文字列を返す
                var errorBuffer = stringToNewUTF8("");
                {{{ makeDynCall('vii', 'callback') }}} (taskId, errorBuffer);
            } finally {
                _free(buffer);
                URL.revokeObjectURL(fileUrl);

                if (fileInput.parentNode) {
                    document.body.removeChild(fileInput);
                }
            }
        }

        fileInput.oncancel = function () {
            var buffer = stringToNewUTF8("");
            {{{ makeDynCall('vii', 'callback') }}} (taskId, buffer);
            _free(buffer);

            // Remove after file selected
            if (fileInput.parentNode) {
                document.body.removeChild(fileInput);
            }
        };

        document.body.appendChild(fileInput);

        // ファイル選択ダイアログを即座に開く
        fileInput.click();
    }


}


mergeInto(LibraryManager.library, FileBrowser)
var FileBrowser = {

    /**
     * �t�@�C���I���_�C�A���O���J��
     * @param {number} filterPtr - �t�@�C���t�B���^������ւ̃|�C���^ (��: ".png,.jpg")
     * @param {number} taskId - �^�X�NID
     * @param {number} callback - Unity���̃R�[���o�b�N�֐��|�C���^
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

        // �N���b�N���̏����F�����t�@�C���̍đI�����\�ɂ��邽�ߒl���N���A
        fileInput.onclick = function (event) {
            event.target.value = null;
        };

        // �t�@�C���I�����̏����F�I�����ꂽ�t�@�C����Unity�ɒʒm
        fileInput.onchange = function (event) {
            try {
                if (!event.target.files || event.target.files.length === 0) {
                    throw new Error('No file selected');
                }

                // JS�������UTF8������ɕϊ����Ċm��
                var file = event.target.files[0];
                var fileUrl = URL.createObjectURL(file);
                var buffer = stringToNewUTF8(fileUrl);

                // �R�[���o�b�N�Ăяo���itaskId��buffer��n���j
                {{{ makeDynCall('vii', 'callback') }}} (taskId, buffer);

            } catch (error) {
                console.error('File selection error:', error);
                // �G���[���͋󕶎����Ԃ�
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

        // �t�@�C���I���_�C�A���O�𑦍��ɊJ��
        fileInput.click();
    }


}


mergeInto(LibraryManager.library, FileBrowser)
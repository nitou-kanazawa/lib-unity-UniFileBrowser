using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AOT;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UniFileBrowser.Web
{
    /// <summary>
    /// </summary>
    public static class WebFileBrowser
    {
        // 進行中のタスクを管理するためのDictionary
        private static readonly Dictionary<int, UniTaskCompletionSource<string>> PendingTasks = new();

        private static int _taskIdCounter;

        /// <summary>
        ///     ファイル選択ダイアログを開く（ExtensionFilter配列版）
        /// </summary>
        /// <param name="filters">ファイルフィルタ配列</param>
        /// <param name="cancellationToken">キャンセレーショントークン</param>
        /// <returns>選択されたファイルのURL。キャンセルまたは選択なしの場合は空文字列</returns>
        public static UniTask<string> OpenFileDialogAsync(ExtensionFilter[] filters, CancellationToken cancellationToken = default)
        {
            var taskId = ++_taskIdCounter;
            var completionSource = new UniTaskCompletionSource<string>();

            PendingTasks[taskId] = completionSource;

            // キャンセレーション処理
            if (cancellationToken.CanBeCanceled)
                cancellationToken.Register(() =>
                {
                    if (PendingTasks.Remove(taskId, out var source))
                        source.TrySetCanceled(cancellationToken);
                });

            var filterString = ConvertExtensionFiltersToString(filters);

            try
            {
                NativeMethods.OpenFileDialog(filterString, taskId, Callback);
            }
            catch (Exception ex)
            {
                PendingTasks.Remove(taskId);
                completionSource.TrySetException(ex);
            }

            return completionSource.Task;
        }

        /// <summary>
        ///     ファイル選択ダイアログを開く（単一ExtensionFilter版）
        /// </summary>
        /// <param name="filter">ファイルフィルタ</param>
        /// <param name="cancellationToken">キャンセレーショントークン</param>
        /// <returns>選択されたファイルのURL。キャンセルまたは選択なしの場合は空文字列</returns>
        public static UniTask<string> OpenFileDialogAsync(ExtensionFilter filter, CancellationToken cancellationToken = default)
        {
            return OpenFileDialogAsync(new[] { filter }, cancellationToken);
        }

        /// <summary>
        ///     ファイル選択ダイアログを開く（フィルタなし版）
        /// </summary>
        /// <param name="cancellationToken">キャンセレーショントークン</param>
        /// <returns>選択されたファイルのURL。キャンセルまたは選択なしの場合は空文字列</returns>
        public static UniTask<string> OpenFileDialogAsync(CancellationToken cancellationToken = default)
        {
            return OpenFileDialogAsync(Array.Empty<ExtensionFilter>(), cancellationToken);
        }

        /// <summary>
        ///     ExtensionFilter配列をHTML accept属性用の文字列に変換
        /// </summary>
        /// <param name="filters">ExtensionFilter配列</param>
        /// <returns>HTML accept属性用の文字列（例：".png,.jpg,.gif"）</returns>
        private static string ConvertExtensionFiltersToString(ExtensionFilter[] filters)
        {
            if (filters == null || filters.Length == 0)
                return string.Empty;

            var extensions = filters
                             .Where(f => f.extensions != null && f.extensions.Length > 0)
                             .SelectMany(f => f.extensions)
                             .Where(ext => !string.IsNullOrEmpty(ext))
                             .Select(ext => ext.StartsWith(".") ? ext : "." + ext)
                             .Distinct()
                             .ToArray();

            return string.Join(",", extensions);
        }

        /// <summary>
        ///     JavaScript側からのコールバック処理
        /// </summary>
        /// <param name="taskId">タスクID</param>
        /// <param name="message">結果メッセージ</param>
        [MonoPInvokeCallback(typeof(Action<int, string>))]
        public static void Callback(int taskId, string message)
        {
            Debug.Log($"C# callback received: taskId={taskId}, message=\"{message}\"");

            if (PendingTasks.TryGetValue(taskId, out var completionSource))
            {
                PendingTasks.Remove(taskId);

                // 空の文字列またはnullの場合はキャンセル扱い
                if (string.IsNullOrEmpty(message))
                    completionSource.TrySetResult(string.Empty);
                else
                    completionSource.TrySetResult(message);
            }
            else
            {
                Debug.LogWarning($"No pending task found for taskId: {taskId}. Task may have been already completed or cancelled.");
            }
        }

        /// <summary>
        ///     すべての進行中タスクをキャンセル（アプリケーション終了時などに使用）
        /// </summary>
        public static void CancelAllPendingTasks()
        {
            foreach (var kvp in PendingTasks) kvp.Value.TrySetCanceled();
            PendingTasks.Clear();
        }
    }
}
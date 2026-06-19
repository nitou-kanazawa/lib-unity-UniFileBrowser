# CLAUDE.md

このファイルは Claude Code がこのリポジトリで作業する際の指針です。

## プロジェクト概要

UniFileBrowser は Unity 向けのネイティブファイルブラウザ（UPM パッケージ `jp.nitou.unifilebrowser`）。
各プラットフォームのネイティブダイアログを呼び出してファイル/フォルダ選択・保存を行う。

## ⚠️ ビルド・実行に関する重要な制約

- **この CI / コンテナ環境では Unity が無いためコンパイル・実行・テストはできない。**
  変更の検証は「ロジックの精読」と「静的チェック（後述）」に留めること。
- C# スクリプトは `UnityEngine` / `UnityEditor` 等に依存しており、Unity 無しでは `dotnet build` も通らない。
- 実機・各プラットフォームでの最終確認はメンテナが Unity Editor / 各ビルドで行う。

## プラットフォーム対応とコードの所在

| プラットフォーム | 実装 | 依存 |
|---|---|---|
| Windows (Standalone) | `Runtime/Standalone/WindowsFileBrowser.cs` | `Ookii.Dialogs`, `System.Windows.Forms`（`Plugins/`） |
| Linux (Standalone) | `Runtime/Standalone/LinuxFileBrowser.cs` | ネイティブ `Plugins/Linux/x86_64/libStandaloneFileBrowser.so` |
| macOS (Standalone) | `Runtime/Standalone/MacFileBrowser.cs` | **未対応**（呼ぶと `NotSupportedException`） |
| Editor | `Runtime/Standalone/EditorFileBrowser.cs` | `UnityEditor.EditorUtility` |
| Web (WebGL) | `Runtime/Web/WebFileBrowser.cs` + `Plugins/WebFileBrowser.jslib` | `UniTask`, ブラウザ `<input type=file>` |

- エントリポイント: `Runtime/Standalone/StandaloneFileBrowser.cs`（`#if` でプラットフォーム実装を選択）。
- 共通型: `Runtime/ExtensionFilter.cs`。

## コーディング規約・注意点

- **`.jslib` および全ソースは UTF-8 で保存する。** 過去に Shift-JIS 由来の文字化けが混入した実績あり。
  （`.github/workflows/encoding-check.yml` で機械的に検査している。）
- `ExtensionFilter[]` は `null` だけでなく **長さ0 / 要素の `extensions` が空** のケースも必ずガードする
  （`GetExtensionCount()` を使う）。空配列での `string.Remove(-1)` 例外は実際に起きたバグ。
- ネイティブ相互運用（`Marshal.PtrToStringAnsi` 等）の戻り値 `null` をガードする。
- `.meta` ファイルは Unity が管理する。新規ファイル追加時は対応する `.meta` の扱いに注意。
- フォーマットは `.editorconfig`（Rider）に従う。

## PR 運用

- **各修正は `master` 起点の独立 PR にする。** stacked PR は base 付け替えに依存し、取りこぼし事故の原因になる
  （実際に発生済み）。複数の関連修正は1本にまとめるか、独立 PR にすること。
- **PR には必ずラベルを付ける。** 内容に応じて最低1つ付与すること
  （バグ修正 → `bug` / 機能追加・改善 → `enhancement` / ドキュメント・規約・CI → `documentation`）。
- PR は作成のみ。マージはメンテナが行う。

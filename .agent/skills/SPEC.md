---
name: TextSpeedReader 專案規範
description: 開始任何任務前必讀，所有修改必須符合本規範
---

# 專案規範

## 架構概覽

### 主要表單
`FormTextSpeedReader` 是主視窗，以 **partial class** 分拆至以下檔案：

| 檔案 | 職責 |
|------|------|
| `FormTextSpeedReader.cs` | 欄位宣告、表單初始化、視窗大小/佈局事件 |
| `FormTextSpeedReader.FileBrowser.cs` | TreeView / ListView 的填充、選取、刷新邏輯 |
| `FormTextSpeedReader.FileIO.cs` | 檔案儲存、另存新檔、重新命名、編碼修正、繁簡轉換 |
| `FormTextSpeedReader.TextActions.cs` | RichTextBox 文字處理（移除斷行、排序、縮排、插入標記等） |
| `FormTextSpeedReader.Search.cs` | 尋找 / 取代對話框的開啟與重用 |
| `FormTextSpeedReader.DragDrop.cs` | 拖曳進入驗證與拖放處理 |
| `FormTextSpeedReader.WebBrowser.cs` | WebBrowser 預覽模式的樣式套用 |

### 輔助類別

| 類別 | 說明 |
|------|------|
| `AppSettings` | INI 格式設定讀寫，儲存至 `.\TextSpeedReader_Settings.ini` |
| `FileSystemManager` | 支援的副檔名清單、RecentReadList 的讀寫 |
| `DisplayManager` | 字體大小清單與 WebBrowser 縮放比例清單，提供「取得下一個較大/較小值」方法 |
| `JTextFileLib` | **Singleton**（`JTextFileLib.Instance()`），封裝 UDE 編碼偵測與檔案讀寫 |
| `ListViewColumnSorter` | 實作 `IComparer`，支援 ListView 欄位排序 |

### 對話框表單

| 表單 | 用途 |
|------|------|
| `FormFindReplace` | 尋找 / 取代（兩種模式共用一個表單，以 Hide/Show 重用） |
| `FormOptions` | 應用程式選項設定 |
| `FormRenameInput` | 通用重新命名輸入框（自動選取檔名不含副檔名的部分） |
| `FormSaveConfirm` | 切換檔案時詢問是否儲存目前修改（不儲存 / 另存新檔 / 儲存） |
| `FormFileOverwriteConfirm` | 目標檔案已存在時詢問處理方式（取消 / 覆蓋 / 另存新檔） |
| `FormReCodeFileName` | 編碼修正預覽（檔名或文字片段），回傳修正後名稱與編碼組合 |
| `FormReCodeFull` | 批量編碼修正預覽（目錄全量版），只回傳使用者選定的編碼組合 |

### 程式碼編碼
- utf-8(無BOM)程式碼 

---

## 介面設計

- 主視窗採左右分割（SplitContainer）：左側為 TreeView + ListView；右側為 RichTextBox 或 WebBrowser。
- 工具列（ToolStrip）放置所有文字操作按鈕；下拉式按鈕（DropDownButton）用於儲存類群組。
- 狀態列（StatusStrip）顯示目前游標位置、字數、以及操作狀態訊息。
- 字型下拉選單（ToolStripComboBox）列出系統字型。
- 對話框一律以 `ShowDialog(this)` 開啟，設定 `Owner` 確保置中與焦點管理。
- `FormFindReplace` 特例：以 `Hide()` / `Show()` 重用，關閉按鈕改為隱藏（覆寫 `OnFormClosing`）。

---

## 資料儲存

- **設定檔**：本地 INI 格式，路徑 `.\TextSpeedReader_Settings.ini`（相對於執行檔）。
- **設定項目**：
  - `AutoOpenLastDirectory`：啟動時自動開啟上次目錄。
  - `KeepFontSize`：切換檔案時保持字體大小。
  - `NewLineStartJudgment` / `NewLineEndJudgment`：段落判定字串。
  - `AddSpaceChrCount`：行首縮排空格數。
  - `InsertBeginingText` / `InsertEndText`：行首 / 尾插入字串。
  - `AnnotationBegin` / `AnnotationEnd`：行號標記的開頭 / 結尾。
  - `HistoryFiles` / `HistoryDirectories`：最近使用紀錄，各保留最多 10 筆（MRU 順序）。
- **閱讀紀錄**：以 `FileSystemManager` 管理，記錄「完整路徑 + 上次閱讀字元位置」。
- **沒有雲端資料庫**：所有資料均為本地檔案。
- **臨時檔案**：WebBrowser 載入 GB2312/GBK HTML 時會建立 GUID 命名的臨時 `.html`，關閉時清理。

---

## 支援的檔案格式

### 文字編輯器模式（RichTextBox）
`.txt`, `.cs`, `.yaml`, `.js`, `.py`, `.md`, `.css`, `.json`

### 網頁預覽模式（WebBrowser）
`.html`, `.htm`

新增支援格式時，需同步修改 `FileSystemManager`（副檔名清單）與 `SaveFileDialog` 的 Filter 字串。

---

## 關鍵實作慣例

### 批次文字修改（防止重入與閃爍）
所有對 `richTextBoxText.Text` 的大範圍替換，必須遵循以下模式：
```csharp
SuspendDrawing();
richTextBoxText.TextChanged -= RichTextBoxText_TextChanged;
richTextBoxText.SelectionChanged -= RichTextBoxText_SelectionChanged;
try
{
    richTextBoxText.SelectAll();
    richTextBoxText.SelectedText = result;
    // 恢復游標位置...
}
finally
{
    richTextBoxText.TextChanged += RichTextBoxText_TextChanged;
    richTextBoxText.SelectionChanged += RichTextBoxText_SelectionChanged;
    ResumeDrawing();
    UpdateStatusLabel();
}
```
- `SuspendDrawing` / `ResumeDrawing` 使用 Win32 `WM_SETREDRAW` 訊息防止閃爍。
- 事件取消訂閱是為了避免在替換過程中觸發「文件已修改」標誌或狀態列更新的重入。

### TreeView 惰性載入
- 子節點初始放置一個文字為 `"Dummy"` 的假節點。
- 在 `BeforeExpand` 事件中偵測到 Dummy 節點時，才實際載入子目錄。
- `HasSubDirectories()` 方法用於判斷目錄是否有子目錄（決定是否放置 Dummy）。

### 檔案讀取（JTextFileLib Singleton）
- 使用 `JTextFileLib.Instance().ReadTxtFile(path, ref text, showError)` 讀取檔案。
- 使用 `JTextFileLib.Instance().SaveTxtFile(path, content, isAppend)` 儲存檔案。
- 內部使用 UDE 函式庫自動偵測編碼，並支援 BOM 識別。
- **不要**直接使用 `File.ReadAllText` 讀取使用者的文字檔案（除非確知編碼）。

### 檔案刪除
- 刪除檔案或目錄一律使用 `Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile/DeleteDirectory` 並指定 `RecycleOption.SendToRecycleBin`，不直接呼叫 `File.Delete`。

### 繁簡轉換
- 繁→簡：`ZhConverter.HantToHans(text)`
- 簡→繁（台灣用詞）：`ZhConverter.HansToTW(text, true)`
- 使用 OpenCCNET 函式庫，需 `using OpenCCNET;`。

### 設定存取
- 透過 `appSettings` 欄位（型別 `AppSettings`）存取所有設定值。
- 修改設定後呼叫 `appSettings.SaveSettings()` 寫入 INI 檔案。

---

## 檔案命名規則

- 主表單的 partial class 檔案：`FormTextSpeedReader.{職責}.cs`（例如 `FormTextSpeedReader.FileIO.cs`）。
- 對話框表單：`Form{功能名稱}.cs`（例如 `FormSaveConfirm.cs`）。
- 設定檔：`TextSpeedReader_Settings.ini`（固定，不可更改）。
- 批次轉換輸出：原檔名 + `_簡體1.txt` 或 `_繁體.txt`。
- 序號分割輸出：原檔名 + `-001`、`-002` ... 三位數序號 + 原副檔名。

---

## 禁止事項

- **禁止**在 `partial class` 之外新增表單的事件處理邏輯（保持職責分離）。
- **禁止**直接修改 `richTextBoxText.Text = ...`（應使用 `SelectAll + SelectedText =`），否則會清除 Undo 歷史並跳過閃爍防護。
- **禁止**在批次文字操作中省略事件取消訂閱，避免修改標誌誤觸發。
- **禁止**在未確認無副作用的情況下呼叫 `File.Delete`，應使用資源回收筒刪除。
- **禁止**在 `JTextFileLib` 中硬寫死路徑（過去曾有 Bug：append 分支寫到硬寫死的測試路徑，已修正）。
- **禁止**新增未被任何程式碼呼叫的函式（死碼）。

---

## 常見維護任務

### 新增支援副檔名
1. 在 `FileSystemManager` 的副檔名清單中加入。
2. 在 `FormTextSpeedReader.FileBrowser.cs` 的開啟邏輯中，確認該副檔名會進入 RichTextBox 模式。
3. 在 `SaveFileDialog.Filter` 字串中加入對應的選項（`FileIO.cs` 有多處）。

### 新增文字處理功能
1. 在 `FormTextSpeedReader.TextActions.cs` 中新增 `private void` 方法。
2. 遵循「批次文字修改」慣例（SuspendDrawing + 事件取消訂閱）。
3. 在工具列或選單中連結對應的 Click 事件。

### 新增設定項目
1. 在 `AppSettings` 中新增屬性，並在 `LoadSettings` / `SaveSettings` 中加入對應的 INI 讀寫。
2. 在 `FormOptions` 的建構子中填入初始值，在 `buttonOK_Click` 中回寫。

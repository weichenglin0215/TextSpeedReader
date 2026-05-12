using System.Collections;
using System.Windows.Forms;

/// <summary>
/// ListView 欄位排序器，實作 IComparer 介面。
/// 支援數字與字串的混合排序：純數字的儲存格以數值大小比較，
/// 其他情況以不分大小寫的字串方式比較。
/// 使用方式：將此物件指定給 ListView.ListViewItemSorter，
/// 並在欄位標頭點擊事件中切換 SortColumn 和 Order，再呼叫 ListView.Sort()。
/// </summary>
public class ListViewColumnSorter : IComparer
{
    // 目前作為排序依據的欄位索引（0 為第一欄）
    private int ColumnToSort;
    // 目前的排序方向（升序/降序/無排序）
    private SortOrder OrderOfSort;
    // 字串比較工具，忽略大小寫
    private CaseInsensitiveComparer ObjectCompare;

    /// <summary>
    /// 建構子：預設排序第一欄（索引 0），無排序方向。
    /// </summary>
    public ListViewColumnSorter()
    {
        ColumnToSort = 0;
        OrderOfSort = SortOrder.None;
        ObjectCompare = new CaseInsensitiveComparer();
    }

    /// <summary>
    /// 比較兩個 ListViewItem 在指定排序欄位中的值。
    /// 若兩個值皆為合法數字，以數值大小比較；否則以字串（不分大小寫）比較。
    /// 當只有其中一個是數字時，數字排在字串之前。
    /// </summary>
    /// <param name="x">第一個 ListViewItem 物件。</param>
    /// <param name="y">第二個 ListViewItem 物件。</param>
    /// <returns>
    /// 依排序方向回傳比較結果：
    /// 負數表示 x 排在 y 前面，正數表示 y 排在 x 前面，0 表示相等。
    /// </returns>
    public int Compare(object x, object y)
    {
        ListViewItem listviewX = (ListViewItem)x;
        ListViewItem listviewY = (ListViewItem)y;

        // 取得要比較的欄位文字
        string sx = listviewX.SubItems[ColumnToSort].Text;
        string sy = listviewY.SubItems[ColumnToSort].Text;

        // 嘗試將文字解析為數值
        bool isNumericX = double.TryParse(sx, out double dx);
        bool isNumericY = double.TryParse(sy, out double dy);

        int compareResult;
        if (isNumericX && isNumericY)
            compareResult = dx.CompareTo(dy);       // 兩者皆為數字，以數值比較
        else if (isNumericX)
            compareResult = -1;                     // 只有 x 是數字，x 排前面
        else if (isNumericY)
            compareResult = 1;                      // 只有 y 是數字，y 排前面
        else
            compareResult = ObjectCompare.Compare(sx, sy); // 兩者皆為字串，字串比較

        // 根據排序方向決定最終結果
        switch (OrderOfSort)
        {
            case SortOrder.Ascending:
                return compareResult;
            case SortOrder.Descending:
                return -compareResult;
            default:
                return 0;
        }
    }

    /// <summary>設定或取得作為排序依據的欄位索引（0 為第一欄）。</summary>
    public int SortColumn
    {
        set { ColumnToSort = value; }
        get { return ColumnToSort; }
    }

    /// <summary>設定或取得排序方向（Ascending/Descending/None）。</summary>
    public SortOrder Order
    {
        set { OrderOfSort = value; }
        get { return OrderOfSort; }
    }
}

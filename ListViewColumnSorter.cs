using System;
using System.Collections;
using System.Windows.Forms;

/// <summary>
/// 提供ListView欄位排序功能，支援數字與字串排序。
/// </summary>
public class ListViewColumnSorter : IComparer
{
    /// <summary>
    /// 要排序的欄位索引。
    /// </summary>
    private int ColumnToSort;

    /// <summary>
    /// 排序方式 (升序或降序)。
    /// </summary>
    private SortOrder OrderOfSort;

    /// <summary>
    /// 字串比較工具，忽略大小寫。
    /// </summary>
    private CaseInsensitiveComparer ObjectCompare;

    /// <summary>
    /// 建構子，預設排序第一欄，無排序方式。
    /// </summary>
    public ListViewColumnSorter()
    {
        ColumnToSort = 0;
        OrderOfSort = SortOrder.None;
        ObjectCompare = new CaseInsensitiveComparer();
    }

    /// <summary>
    /// 實作IComparer介面的比較方法。
    /// </summary>
    /// <param name="x">第一個比較的ListViewItem</param>
    /// <param name="y">第二個比較的ListViewItem</param>
    /// <returns>比較結果(小於0表示x在y前，大於0表示y在x前，0表示相等)</returns>
    public int Compare(object x, object y)
    {
        int compareResult;
        ListViewItem listviewX, listviewY;

        // 將物件轉型為ListViewItem
        listviewX = (ListViewItem)x;
        listviewY = (ListViewItem)y;

        // 取得要比較的文字
        string sx = listviewX.SubItems[ColumnToSort].Text;
        string sy = listviewY.SubItems[ColumnToSort].Text;

        double dx, dy;

        // 判斷是否為數值型態
        bool isNumericX = double.TryParse(sx, out dx);
        bool isNumericY = double.TryParse(sy, out dy);

        if (isNumericX && isNumericY)
        {
            // 若兩者皆為數值，則以數值大小進行比較
            compareResult = dx.CompareTo(dy);
        }
        else if (isNumericX)
        {
            // 只有x為數字時，將數字排在前面
            compareResult = -1;
        }
        else if (isNumericY)
        {
            // 只有y為數字時，將數字排在前面
            compareResult = 1;
        }
        else
        {
            // 若兩者皆非數值，則以字串方式比較 (忽略大小寫)
            compareResult = ObjectCompare.Compare(sx, sy);
        }

        // 根據指定的排序方式決定結果
        if (OrderOfSort == SortOrder.Ascending)
        {
            // 升序排序
            return compareResult;
        }
        else if (OrderOfSort == SortOrder.Descending)
        {
            // 降序排序
            return -compareResult;
        }
        else
        {
            // 無排序
            return 0;
        }
    }

    /// <summary>
    /// 設定或取得排序欄位索引。
    /// </summary>
    public int SortColumn
    {
        set { ColumnToSort = value; }
        get { return ColumnToSort; }
    }

    /// <summary>
    /// 設定或取得排序方式。
    /// </summary>
    public SortOrder Order
    {
        set { OrderOfSort = value; }
        get { return OrderOfSort; }
    }
}

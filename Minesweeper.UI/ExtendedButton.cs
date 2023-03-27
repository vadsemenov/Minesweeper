using System.Windows.Forms;

namespace Minesweeper.UI;

internal class ExtendedButton : Button
{
    public int RowCount { get; set; }

    public int ColumnCount { get; set; }

    public ExtendedButton(int rowCount, int columnCount)
    {
        RowCount = rowCount;
        ColumnCount = columnCount;
    }
}
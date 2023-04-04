namespace Minesweeper.UI;

internal class ExtendedButton : Button
{
    public int RowIndex { get; set; }

    public int ColumnIndex { get; set; }

    public ExtendedButton(int rowIndex, int columnIndex)
    {
        RowIndex = rowIndex;
        ColumnIndex = columnIndex;
    }
}
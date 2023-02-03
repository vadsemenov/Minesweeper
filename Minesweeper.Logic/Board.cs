using System;

namespace Minesweeper.Logic;

public class Board
{
    public Cell[,] Field { get; set; }
    public int MineAmount { get; private set; }

    public Board(int rowsAmount, int columnAmount, int mineAmount)
    {
        Field = new Cell[rowsAmount, columnAmount];

        MineAmount = mineAmount;

        // InsertMines() 
    }
}

public record Cell
{
    public CellContent Content { get; set; }
    public CellStatus Status { get; set; }
}

public enum CellStatus
{
    CloseCell = 0,
    OpenCell = 1
}

public enum CellContent
{
    EmptyCell = 0,
    Mine = 1
}
using System;

namespace Minesweeper.Logic;

public class Board
{
    public Cell[,] Field { get; set; }
    public int MineAmount { get; private set; }

    public Board(int rowsAmount, int columnAmount, int mineAmount )
    {
        Field = new Cell[rowsAmount,columnAmount];

        MineAmount = mineAmount;

        // InsertMines() 
    }

}

public record Cell
{
    public CellStatus Status { get; set;}

}

public enum CellStatus
{
    EmptyCell = 0,

}

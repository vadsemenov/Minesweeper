using System;
using System.Text;

namespace Minesweeper.Logic;

public class Board
{
    public int[,] Field { get; private set; }

    public int MineAmount { get; private set; }

    private readonly Random _random = new();

    public Board(int rowsAmount, int columnAmount, int mineAmount)
    {
        Field = new int[rowsAmount, columnAmount];

        MineAmount = mineAmount;

        GenerateField();
    }

    public void RegenerateField()
    {
        ClearField();
        PlaceMines();
        PlaceDigits();
    }

    private void ClearField()
    {
        var rowsAmount = Field.GetLength(0);
        var columnsAmount = Field.GetLength(1);

        for (int i = 0; i < rowsAmount; i++)
        {
            for (int j = 0; j < columnsAmount; j++)
            {
                Field[i, j] = 0;
            }
        }
    }

    private void GenerateField()
    {
        PlaceMines();
        PlaceDigits();
    }

    private void PlaceDigits()
    {
        var rowsAmount = Field.GetLength(0);
        var columnsAmount = Field.GetLength(1);

        for (int i = 0; i < rowsAmount; i++)
        {
            for (int j = 0; j < columnsAmount; j++)
            {
                if (Field[i, j] == 0)
                {
                    PlaceDigitsInsideField(i, j, rowsAmount, columnsAmount);
                }
            }
        }
    }

    private void PlaceDigitsInsideField(int i, int j, int rowsAmount, int columnsAmount)
    {
        var neighboringMinesAmount = 0;



        for (var k = i - 1; k <= i + 1; k++)
        {
            for (var l = j - 1; l <= j + 1; l++)
            {
                if (k < 0 || l < 0 || k > rowsAmount - 1 || l > columnsAmount - 1)
                {
                    continue;
                }

                if (Field[k,l] == 9)
                {
                    neighboringMinesAmount++;
                }
            }
        }

        Field[i, j] = neighboringMinesAmount;
    }

    public void PlaceMines()
    {
        var mineCount = MineAmount;

        while (mineCount > 0)
        {
            var rowCount = _random.Next(0, Field.GetLength(0));
            var columnCount = _random.Next(0, Field.GetLength(0));

            if (Field[rowCount, columnCount] != 9)
            {
                Field[rowCount, columnCount] = 9;
                mineCount--;
            }
        }
    }

    public override string ToString()
    {
        var rowsAmount = Field.GetLength(0);
        var columnsAmount = Field.GetLength(1);

        var stringBuilder = new StringBuilder();

        for (int i = 0; i < rowsAmount; i++)
        {
            for (int j = 0; j < columnsAmount; j++)
            {
                stringBuilder.Append(Field[i, j] + " ");
            }

            stringBuilder.AppendLine();
        }

        return stringBuilder.ToString();
    }
}

public record Cell
{
    public CellContent Content { get; set; }
    // public CellStatus Status { get; set; }
}

// public enum CellStatus
// {
//     CloseCell = 0,
//     OpenCell = 1
// }

public enum CellContent
{
    EmptyCell = 0,
    NearOneMine = 1,
    NearTwoMine = 2,
    NearThreeMine = 3,
    NearFourMine = 4,
    NearFiveMine = 5,
    NearSixMine = 6,
    NearSevenMine = 7,
    NearEightMine = 8,
    Mine = 9
}
using System;
using System.Collections.Generic;
using System.Text;
using Minesweeper.Logic.Enums;

namespace Minesweeper.Logic;

public class Game
{
    public Cell[,] Field { get; private set; }
    public int RowsAmount { get; private set; }
    public int ColumnsAmount { get; private set; }

    public int MineAmount { get; private set; }

    public GameStatus GameStatus { get; private set; }

    private readonly Random _random = new(1234);

    public Game(int rowsAmount, int columnsAmount, int mineAmount)
    {
        Field = new Cell[rowsAmount, columnsAmount];

        RowsAmount = rowsAmount;
        ColumnsAmount = columnsAmount;

        MineAmount = mineAmount;

        GameStatus = GameStatus.Run;

        GenerateNewField();
    }

    public Game(GameDifficulty gameDifficulty)
    {
        switch (gameDifficulty)
        {
            case GameDifficulty.Easy:
                RowsAmount = 9;
                ColumnsAmount = 9;
                MineAmount = 10;
                break;
            case GameDifficulty.Normal:
                RowsAmount = 12;
                ColumnsAmount = 12;
                MineAmount = 20;
                break;
            case GameDifficulty.Hard:
                RowsAmount = 20;
                ColumnsAmount = 20;
                MineAmount = 55;
                break;
        }

        Field = new Cell[RowsAmount, ColumnsAmount];

        GameStatus = GameStatus.Run;

        GenerateNewField();
    }

    public void TrySetOrRemoveFlag(Cell cell)
    {
        if (cell.Status == CellStatus.OpenedCell)
        {
            return;
        }

        if (cell.Status == CellStatus.Flag)
        {
            cell.Status = CellStatus.NotOpenedCell;
            return;
        }

        cell.Status = CellStatus.Flag;
    }

    public GameStatus TryOpenCell(int rowCount, int columnCount)
    {
        if (rowCount < 0 || columnCount < 0 || rowCount >= Field.GetLength(1) || columnCount >= Field.GetLength(0))
        {
            return GameStatus;
        }

        if (Field[rowCount, columnCount].Status == CellStatus.OpenedCell)
        {
            return GameStatus;
        }

        if (Field[rowCount, columnCount].Status == CellStatus.Flag)
        {
            return GameStatus;
        }

        if (Field[rowCount, columnCount].CellContent == CellContent.Mine)
        {
            Field[rowCount, columnCount].CellContent = CellContent.ExplodedMine;

            GameStatus = GameStatus.Lose;

            OpenAllCells();

            return GameStatus;
        }

        Field[rowCount, columnCount].Status = CellStatus.OpenedCell;

        OpenCellsInBreadth(rowCount, columnCount);

        if (CheckWinGameStatus())
        {
            OpenAllCells();
            GameStatus = GameStatus.Win;
        }

        return GameStatus;
    }

    private bool CheckWinGameStatus()
    {
        for (var i = 0; i < RowsAmount; i++)
        {
            for (var j = 0; j < ColumnsAmount; j++)
            {
                if (Field[i, j].Status != CellStatus.OpenedCell && Field[i, j].CellContent != CellContent.Mine)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void OpenCellsInBreadth(int rowCount, int columnCount)
    {
        var visitQueue = new Queue<(int, int)>();
        var visited = new List<(int, int)>();

        var currentCoord = (rowCount, columnCount);

        visitQueue.Enqueue(currentCoord);

        while (visitQueue.Count > 0)
        {
            currentCoord = visitQueue.Dequeue();

            if (visited.Contains((currentCoord.rowCount, currentCoord.columnCount)))
            {
                continue;
            }

            visited.Add(currentCoord);

            var currentCell = Field[currentCoord.rowCount, currentCoord.columnCount];

            if (currentCell.CellContent == CellContent.Empty && currentCell.Status != CellStatus.Flag)
            {
                currentCell.Status = CellStatus.OpenedCell;

                AddNeighboringCellsToQueue(currentCoord.rowCount, currentCoord.columnCount, visitQueue, visited);
            }
        }
    }

    private void AddNeighboringCellsToQueue(int rowCount, int columnCount, Queue<(int, int)> visitQueue, List<(int, int)> visited)
    {
        for (var i = rowCount - 1; i <= rowCount + 1; i++)
        {
            for (var j = columnCount - 1; j <= columnCount + 1; j++)
            {
                if (i == rowCount && j == columnCount)
                {
                    continue;
                }

                if (i < 0 || j < 0 || i >= RowsAmount || j >= ColumnsAmount)
                {
                    continue;
                }

                if (visited.Contains((i, j)))
                {
                    continue;
                }

                visitQueue.Enqueue((i, j));

                if (Field[i, j].CellContent != CellContent.Mine)
                {
                    Field[i, j].Status = CellStatus.OpenedCell;
                }
            }
        }
    }

    private void OpenAllCells()
    {
        for (var i = 0; i < RowsAmount; i++)
        {
            for (var j = 0; j < ColumnsAmount; j++)
            {
                Field[i, j].Status = CellStatus.OpenedCell;
            }
        }
    }

    public void GenerateNewField()
    {
        CreateField();
        PlaceMines();
        PlaceDigits();
    }

    private void CreateField()
    {
        for (var i = 0; i < RowsAmount; i++)
        {
            for (var j = 0; j < ColumnsAmount; j++)
            {
                Field[i, j] = new Cell
                {
                    CellContent = CellContent.Empty,
                    Status = CellStatus.NotOpenedCell
                };
            }
        }
    }

    private void PlaceMines()
    {
        var mineCount = MineAmount;

        while (mineCount > 0)
        {
            var rowCount = _random.Next(0, Field.GetLength(0));
            var columnCount = _random.Next(0, Field.GetLength(0));

            if (Field[rowCount, columnCount].CellContent != CellContent.Mine)
            {
                Field[rowCount, columnCount].CellContent = CellContent.Mine;
                mineCount--;
            }
        }
    }

    private void PlaceDigits()
    {
        for (var i = 0; i < RowsAmount; i++)
        {
            for (var j = 0; j < ColumnsAmount; j++)
            {
                if (Field[i, j].CellContent == CellContent.Empty)
                {
                    PlaceDigitsInsideField(i, j);
                }
            }
        }
    }

    private void PlaceDigitsInsideField(int i, int j)
    {
        var neighboringMinesAmount = 0;

        for (var k = i - 1; k <= i + 1; k++)
        {
            for (var l = j - 1; l <= j + 1; l++)
            {
                if (k < 0 || l < 0 || k > RowsAmount - 1 || l > ColumnsAmount - 1)
                {
                    continue;
                }

                if (Field[k, l].CellContent == CellContent.Mine)
                {
                    neighboringMinesAmount++;
                }
            }
        }

        Field[i, j].CellContent = (CellContent)neighboringMinesAmount;
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        for (var i = 0; i < RowsAmount; i++)
        {
            for (var j = 0; j < ColumnsAmount; j++)
            {
                if (Field[i, j].CellContent == CellContent.Mine)
                {
                    stringBuilder.Append("*" + " ");
                }
                else
                {
                    stringBuilder.Append((int)Field[i, j].CellContent + " ");
                }
            }

            stringBuilder.AppendLine();
        }

        return stringBuilder.ToString();
    }

    public string FieldView()
    {
        var stringBuilder = new StringBuilder();

        for (var i = 0; i < RowsAmount; i++)
        {
            for (var j = 0; j < ColumnsAmount; j++)
            {
                stringBuilder.Append((int)Field[i, j].Status + " ");
            }

            stringBuilder.AppendLine();
        }

        return stringBuilder.ToString();
    }
}
﻿using System;
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

    private readonly Random _random = new();

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
        for (var y = 0; y < RowsAmount; y++)
        {
            for (var x = 0; x < ColumnsAmount; x++)
            {
                if (Field[y, x].Status != CellStatus.OpenedCell && Field[y, x].CellContent != CellContent.Mine)
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

        var currentCoordinate = (rowCount, columnCount);

        visitQueue.Enqueue(currentCoordinate);

        while (visitQueue.Count > 0)
        {
            currentCoordinate = visitQueue.Dequeue();

            if (visited.Contains((currentCoordinate.rowCount, currentCoordinate.columnCount)))
            {
                continue;
            }

            visited.Add(currentCoordinate);

            var currentCell = Field[currentCoordinate.rowCount, currentCoordinate.columnCount];

            if (currentCell.CellContent == CellContent.Empty && currentCell.Status != CellStatus.Flag)
            {
                currentCell.Status = CellStatus.OpenedCell;

                AddNeighboringCellsToQueue(currentCoordinate.rowCount, currentCoordinate.columnCount, visitQueue, visited);
            }
        }
    }

    private void AddNeighboringCellsToQueue(int rowCount, int columnCount, Queue<(int, int)> visitQueue, List<(int, int)> visited)
    {
        for (var y = rowCount - 1; y <= rowCount + 1; y++)
        {
            for (var x = columnCount - 1; x <= columnCount + 1; x++)
            {
                if (y == rowCount && x == columnCount)
                {
                    continue;
                }

                if (y < 0 || x < 0 || y >= RowsAmount || x >= ColumnsAmount)
                {
                    continue;
                }

                if (visited.Contains((y, x)))
                {
                    continue;
                }

                visitQueue.Enqueue((y, x));

                if (Field[y, x].CellContent != CellContent.Mine)
                {
                    Field[y, x].Status = CellStatus.OpenedCell;
                }
            }
        }
    }

    private void OpenAllCells()
    {
        for (var y = 0; y < RowsAmount; y++)
        {
            for (var x = 0; x < ColumnsAmount; x++)
            {
                Field[y, x].Status = CellStatus.OpenedCell;
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
        for (var y = 0; y < RowsAmount; y++)
        {
            for (var x = 0; x < ColumnsAmount; x++)
            {
                Field[y, x] = new Cell
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
        for (var y = 0; y < RowsAmount; y++)
        {
            for (var x = 0; x < ColumnsAmount; x++)
            {
                if (Field[y, x].CellContent == CellContent.Empty)
                {
                    PlaceDigitsInsideField(y, x);
                }
            }
        }
    }

    private void PlaceDigitsInsideField(int i, int j)
    {
        var neighboringMinesAmount = 0;

        for (var rowCount = i - 1; rowCount <= i + 1; rowCount++)
        {
            for (var columnCount = j - 1; columnCount <= j + 1; columnCount++)
            {
                if (rowCount < 0 || columnCount < 0 || rowCount > RowsAmount - 1 || columnCount > ColumnsAmount - 1)
                {
                    continue;
                }

                if (Field[rowCount, columnCount].CellContent == CellContent.Mine)
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

        for (var y = 0; y < RowsAmount; y++)
        {
            for (var x = 0; x < ColumnsAmount; x++)
            {
                if (Field[y, x].CellContent == CellContent.Mine)
                {
                    stringBuilder.Append("*" + " ");
                }
                else
                {
                    stringBuilder.Append((int)Field[y, x].CellContent + " ");
                }
            }

            stringBuilder.AppendLine();
        }

        return stringBuilder.ToString();
    }

    public string FieldView()
    {
        var stringBuilder = new StringBuilder();

        for (var y = 0; y < RowsAmount; y++)
        {
            for (var x = 0; x < ColumnsAmount; x++)
            {
                stringBuilder.Append((int)Field[y, x].Status + " ");
            }

            stringBuilder.AppendLine();
        }

        return stringBuilder.ToString();
    }
}
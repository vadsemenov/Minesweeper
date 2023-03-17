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
                RowsAmount = 8;
                ColumnsAmount = 8;
                MineAmount = 6;
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
        if (cell.CellStatus == CellStatus.OpenedCell)
        {
            return;
        }

        if (cell.CellStatus == CellStatus.Flag)
        {
            cell.CellStatus = CellStatus.ClosedCell;
        }

        cell.CellStatus = CellStatus.Flag;
    }

    public GameStatus TryOpenCell(int rowCount, int columnCount)
    {
        if (rowCount < 0 || columnCount < 0 || rowCount >= Field.GetLength(1)
            || columnCount >= Field.GetLength(0))
        {
            return GameStatus;
        }

        if (Field[rowCount, columnCount].CellStatus == CellStatus.OpenedCell)
        {
            return GameStatus;
        }

        if (Field[rowCount, columnCount].CellStatus == CellStatus.Flag)
        {
            return GameStatus;
        }

        if (Field[rowCount, columnCount].CellContent == CellContent.Mine)
        {
            Field[rowCount, columnCount].CellContent = CellContent.ExplosedMine;

            GameStatus = GameStatus.Lose;

            OpenAllCells();

            return GameStatus;
        }

        OpenCellsInBreadth(rowCount, columnCount);

        if (CheckWinGameStatus())
        {
            GameStatus = GameStatus.Win;
        }

        return GameStatus;
    }

    private bool CheckWinGameStatus()
    {
        for (int i = 0; i < RowsAmount; i++)
        {
            for (int j = 0; j < ColumnsAmount; j++)
            {
                if (Field[i, j].CellStatus != CellStatus.OpenedCell)
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

            if (currentCell.CellContent == CellContent.EmptyCell
                && currentCell.CellStatus != CellStatus.Flag)
            {
                currentCell.CellStatus = CellStatus.OpenedCell;

                AddNeighboringCellsToQueue(currentCoord.rowCount, currentCoord.columnCount, visitQueue, visited);
            }
        }
    }

    private void AddNeighboringCellsToQueue(int rowCount, int columnCount, Queue<(int, int)> visitQueue,
        List<(int, int)> visited)
    {
        if (rowCount - 1 >= 0)
        {
            if (!visited.Contains((rowCount - 1, columnCount)))
            {
                visitQueue.Enqueue((rowCount - 1, columnCount));
            }
        }

        if (rowCount + 1 < RowsAmount)
        {
            if (!visited.Contains((rowCount + 1, columnCount)))
            {
                visitQueue.Enqueue((rowCount + 1, columnCount));
            }
        }

        if (columnCount - 1 >= 0)
        {
            if (!visited.Contains((rowCount, columnCount - 1)))
            {
                visitQueue.Enqueue((rowCount, columnCount - 1));
            }
        }

        if (columnCount + 1 < ColumnsAmount)
        {
            if (!visited.Contains((rowCount, columnCount + 1)))
            {
                visitQueue.Enqueue((rowCount, columnCount + 1));
            }
        }
    }

    private void OpenAllCells()
    {
        for (int i = 0; i < RowsAmount; i++)
        {
            for (int j = 0; j < ColumnsAmount; j++)
            {
                Field[i, j].CellStatus = CellStatus.OpenedCell;
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
        for (int i = 0; i < RowsAmount; i++)
        {
            for (int j = 0; j < ColumnsAmount; j++)
            {
                Field[i, j] = new Cell();
                Field[i, j].CellContent = CellContent.EmptyCell;
                Field[i, j].CellStatus = CellStatus.ClosedCell;
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
        for (int i = 0; i < RowsAmount; i++)
        {
            for (int j = 0; j < ColumnsAmount; j++)
            {
                if (Field[i, j].CellContent == CellContent.EmptyCell)
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

        for (int i = 0; i < RowsAmount; i++)
        {
            for (int j = 0; j < ColumnsAmount; j++)
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

        for (int i = 0; i < RowsAmount; i++)
        {
            for (int j = 0; j < ColumnsAmount; j++)
            {
                stringBuilder.Append((int)Field[i, j].CellStatus + " ");
            }

            stringBuilder.AppendLine();
        }

        return stringBuilder.ToString();
    }
}
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Minesweeper.Logic.Enums;

namespace Minesweeper.Logic;

public class Game
{
    public Cell[,] Field { get; private set; }
    public int RowsAmount { get; private set; }
    public int ColumnsAmount { get; private set; }

    public int MineAmount { get; private set; }

    private GameStatus _gameStatus;

    public GameStatus GameStatus
    {
        get => _gameStatus;

        private set
        {
            if (value is GameStatus.Lose or GameStatus.Win)
            {
                _timer.Stop();
            }

            _gameStatus = value;
        }
    }

    private readonly Random _random = new();

    private readonly Stopwatch _timer = new();

    public double ElapsedTime => (double)_timer.ElapsedMilliseconds / 1000;

    private const int TopRecordsAmount = 5;

    public Game(int rowsAmount, int columnsAmount, int mineAmount)
    {
        Field = new Cell[rowsAmount, columnsAmount];

        RowsAmount = rowsAmount;
        ColumnsAmount = columnsAmount;

        MineAmount = mineAmount;

        GameStatus = GameStatus.Run;

        GenerateNewField();

        _timer.Restart();
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

        _timer.Restart();
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

    private bool CheckCellIsInsideField(int rowCount, int columnCount)
    {
        if (rowCount < 0 || columnCount < 0 || rowCount >= RowsAmount || columnCount >= ColumnsAmount)
        {
            return false;
        }

        return true;
    }

    public GameStatus TryOpenNeighboringCells(int rowCount, int columnCount)
    {
        if (!CheckCellIsInsideField(rowCount, columnCount))
        {
            return GameStatus;
        }

        var currentCell = Field[rowCount, columnCount];

        if (currentCell.Status == CellStatus.NotOpenedCell ||
            currentCell.CellContent == CellContent.Empty && currentCell.Status == CellStatus.OpenedCell ||
            currentCell.Status == CellStatus.Flag)
        {
            return GameStatus;
        }

        var flagedNeighboringCellsAmount = GetFlagedNeighboringCellsAmount(rowCount, columnCount);

        if (flagedNeighboringCellsAmount != 0 && flagedNeighboringCellsAmount == (int)currentCell.CellContent)
        {
            return OpenNieghboringCells(rowCount, columnCount);
        }

        return GameStatus;
    }

    private GameStatus OpenNieghboringCells(int rowCount, int columnCount)
    {
        for (var y = rowCount - 1; y <= rowCount + 1; y++)
        {
            for (var x = columnCount - 1; x <= columnCount + 1; x++)
            {
                if (y == rowCount && x == columnCount)
                {
                    continue;
                }

                if (!CheckCellIsInsideField(y, x))
                {
                    continue;
                }

                var gameStatus = TryOpenCell(y, x);

                if (gameStatus == GameStatus.Lose || gameStatus == GameStatus.Win)
                {
                    return GameStatus;
                }
            }
        }

        return GameStatus;
    }

    private int GetFlagedNeighboringCellsAmount(int rowCount, int columnCount)
    {
        var flagedCellsAmount = 0;

        for (var y = rowCount - 1; y <= rowCount + 1; y++)
        {
            for (var x = columnCount - 1; x <= columnCount + 1; x++)
            {
                if (!CheckCellIsInsideField(y, x))
                {
                    continue;
                }

                if (y == rowCount && x == columnCount)
                {
                    continue;
                }

                if (Field[y, x].Status == CellStatus.Flag)
                {
                    flagedCellsAmount++;
                }
            }
        }

        return flagedCellsAmount;
    }

    public GameStatus TryOpenCell(int rowCount, int columnCount)
    {
        if (!CheckCellIsInsideField(rowCount, columnCount))
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

                if (!CheckCellIsInsideField(y, x))
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
                if (!CheckCellIsInsideField(rowCount, columnCount))
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
                    stringBuilder.Append("* ");
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

    public int GetNewRecordPlace()
    {
        var records = ReadRecordsFromFile();

        var insertIndex = records.TakeWhile(rt => rt.Time <= ElapsedTime).Count();

        if (insertIndex < TopRecordsAmount)
        {
            return insertIndex;
        }

        return -1;
    }

    public bool AddNewRecord(int placeNumber, string name, double elapsedTime)
    {
        var records = ReadRecordsFromFile();

        if (placeNumber is >= 0 and < TopRecordsAmount)
        {
            records.Insert(placeNumber, new RecordTime(name ?? "Unknown", elapsedTime));

            WriteRecordsToFile(records);

            return true;
        }

        return false;
    }

    public List<RecordTime> ReadRecordsFromFile()
    {
        if (!File.Exists("records.txt"))
        {
            return new List<RecordTime>();
        }

        var text = File.ReadAllText("records.txt");

        List<RecordTime> records;

        try
        {
            records = JsonSerializer.Deserialize<List<RecordTime>>(text);
        }
        catch
        {
            return new List<RecordTime>();
        }

        return records?.OrderBy(rt => rt.Time).ToList();
    }

    private void WriteRecordsToFile(List<RecordTime> recordsList)
    {
        var text = JsonSerializer.Serialize(recordsList);

        File.WriteAllText("records.txt", text);
    }
}
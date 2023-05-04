using System.ComponentModel;
using System.Text;
using System.Timers;
using Minesweeper.Logic.Enums;
using Timer = System.Timers.Timer;

namespace Minesweeper.Logic;

public class Game
{
    public Cell[,] Field { get; private set; }

    public int RowsAmount { get; private set; }
    public int ColumnsAmount { get; private set; }

    public int MinesAmount { get; private set; }

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

    public IRecordsService RecordsService { get; private set; }

    private Timer _timer;

    public Action<TimeSpan> TimerTickAction;

    private TimeSpan _time = TimeSpan.Zero;

    public Game(int rowsAmount, int columnsAmount, int minesAmount, IRecordsService recordsService, ISynchronizeInvoke syncObject)
    {
        SetGameSettings(rowsAmount, columnsAmount, minesAmount, recordsService, syncObject);
    }

    public Game(GameDifficulty gameDifficulty, IRecordsService recordsService, ISynchronizeInvoke syncObject)
    {
        switch (gameDifficulty)
        {
            case GameDifficulty.Easy:
                SetGameSettings(9, 9, 10, recordsService, syncObject);
                break;
            case GameDifficulty.Normal:
                SetGameSettings(12, 12, 20, recordsService, syncObject);
                break;
            case GameDifficulty.Hard:
                SetGameSettings(20, 20, 55, recordsService, syncObject);
                break;
        }
    }

    private void SetGameSettings(int rowsAmount, int columnsAmount, int minesAmount, IRecordsService recordsService, ISynchronizeInvoke syncObject)
    {
        RecordsService = recordsService;

        Field = new Cell[rowsAmount, columnsAmount];

        RowsAmount = rowsAmount;
        ColumnsAmount = columnsAmount;

        MinesAmount = minesAmount;


        GameStatus = GameStatus.Run;

        GenerateNewField();

        _timer = new Timer();
        _timer.SynchronizingObject = syncObject;
        _timer.Elapsed += TimerOnElapsed;
        _timer.Interval = 10;
        _timer.Start();
    }

    private void TimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        _time += TimeSpan.FromMilliseconds(10);
        TimerTickAction.Invoke(_time);
    }

    public static void TrySetOrRemoveFlag(Cell cell)
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
        return rowCount >= 0 && columnCount >= 0 && rowCount < RowsAmount && columnCount < ColumnsAmount;
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

        var flaggedNeighboringCellsAmount = GetFlaggedNeighboringCellsAmount(rowCount, columnCount);

        if (flaggedNeighboringCellsAmount != 0 && flaggedNeighboringCellsAmount == (int)currentCell.CellContent)
        {
            return OpenNeighboringCells(rowCount, columnCount);
        }

        return GameStatus;
    }

    private GameStatus OpenNeighboringCells(int rowCount, int columnCount)
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

    private int GetFlaggedNeighboringCellsAmount(int rowCount, int columnCount)
    {
        var flaggedCellsAmount = 0;

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
                    flaggedCellsAmount++;
                }
            }
        }

        return flaggedCellsAmount;
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
        var mineCount = MinesAmount;

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
                    stringBuilder.Append((int)Field[y, x].CellContent);
                    stringBuilder.Append(' ');
                }
            }

            stringBuilder.AppendLine();
        }

        return stringBuilder.ToString();
    }
}
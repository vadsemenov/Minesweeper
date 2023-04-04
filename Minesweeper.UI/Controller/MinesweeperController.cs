using System.Diagnostics;
using Minesweeper.Logic;
using Minesweeper.Logic.Enums;
using System.Text.Json;

namespace Minesweeper.UI.Controller;
public class MinesweeperController
{
    private readonly Game _game;
    public GameDifficulty GameDifficulty { get; private set; }

    public int RowsAmount => _game.RowsAmount;
    public int ColumnsAmount => _game.ColumnsAmount;

    public Cell[,] Field => _game.Field;

    public GameStatus GameStatus => _game.GameStatus;

    public event Action RedrawFieldEvent;

    public double ElapsedTime => _game.ElapsedTime;

    public List<RecordTime> RecordsTimes => _game.ReadRecordsFromFile();

    public int GetNewRecordPlace => _game.GetNewRecordPlace();

    public MinesweeperController(GameDifficulty gameDifficulty, Action redrawFieldEvent)
    {
        RedrawFieldEvent = redrawFieldEvent;

        GameDifficulty = gameDifficulty;

        _game = new Game(gameDifficulty);
    }

    public void TryOpenCell(int rowCount, int columnCount)
    {
        _game.TryOpenCell(rowCount, columnCount);
        RedrawFieldEvent?.Invoke();
    }

    public bool AddNewRecord(int placeNumber, string name, double elapsedTime)
    {
        return _game.AddNewRecord(placeNumber, name, elapsedTime);
    }

    public void TrySetRemoveFlag(Cell cell)
    {
        _game.TrySetOrRemoveFlag(cell);
        RedrawFieldEvent?.Invoke();
    }
}

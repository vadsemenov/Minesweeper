using Minesweeper.Logic;
using Minesweeper.Logic.Enums;

namespace Minesweeper.UI.Controller;
public class MinesweeperController
{
    private readonly Game _game;
    public GameDifficulty GameDifficulty { get; }

    public int RowsAmount => _game.RowsAmount;
    public int ColumnsAmount => _game.ColumnsAmount;

    public Cell[,] Field => _game.Field;

    public GameStatus GameStatus => _game.GameStatus;

    public event Action RedrawFieldEvent;

    public double ElapsedTime => _game.ElapsedTime;

    public List<RecordTime> RecordsTimes => RecordsService.ReadRecordsFromFile().Where(x => x.GameDifficulty == GameDifficulty).ToList();

    public int GetNewRecordPlace => RecordsService.GetNewRecordPlace(GameDifficulty, _game.ElapsedTime);

    private bool _isFirstClick = true;

    public MinesweeperController(GameDifficulty gameDifficulty, Action redrawFieldEvent)
    {
        RedrawFieldEvent = redrawFieldEvent;

        GameDifficulty = gameDifficulty;

        _game = new Game(gameDifficulty);
    }

    public void TryOpenNeighboringCells(int rowCount, int columnCount)
    {
        _game.TryOpenNeighboringCells(rowCount, columnCount);
        RedrawFieldEvent?.Invoke();
    }

    public void TryOpenCell(int rowCount, int columnCount)
    {
        if (_isFirstClick)
        {
            while (Field[rowCount, columnCount].CellContent != CellContent.Empty)
            {
                _game.GenerateNewField();
            }

            _isFirstClick = false;
        }

        _game.TryOpenCell(rowCount, columnCount);
        RedrawFieldEvent?.Invoke();
    }

    public bool AddNewRecord(int placeNumber, string name, double elapsedTime)
    {
        return RecordsService.AddNewRecord(placeNumber, GameDifficulty, name, elapsedTime);
    }

    public void TrySetRemoveFlag(Cell cell)
    {
        _game.TrySetOrRemoveFlag(cell);
        RedrawFieldEvent?.Invoke();
    }
}

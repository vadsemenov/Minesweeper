using System.ComponentModel;
using System.Runtime.CompilerServices;
using Minesweeper.Logic;
using Minesweeper.Logic.Enums;

namespace Minesweeper.UI.Controller;
public class MinesweeperController : INotifyPropertyChanged
{
    private readonly Game _game;

    public GameDifficulty GameDifficulty { get; }

    public int RowsAmount => _game.RowsAmount;
    public int ColumnsAmount => _game.ColumnsAmount;

    public Cell[,] Field => _game.Field;

    public GameStatus GameStatus => _game.GameStatus;
    public event Action RedrawFieldEvent;

    public IRecordsService RecordsService => _game.RecordsService;

    public string ElapsedTime { get; private set; }

    private TimeSpan _time;

    public List<RecordTime> RecordsTimes => RecordsService.GetAllRecordsTimes(GameDifficulty);

    public int GetNewRecordPlace => RecordsService.GetNewRecordPlace(GameDifficulty, _time);

    private bool _isFirstClick = true;

    public MinesweeperController(GameDifficulty gameDifficulty, Action redrawFieldAction, ISynchronizeInvoke synchronizingObject)
    {
        RedrawFieldEvent = redrawFieldAction;

        GameDifficulty = gameDifficulty;

        _game = new Game(gameDifficulty, new RecordsService(), synchronizingObject);

        _game.TimerTickAction += TimerTickAction;
    }

    private void TimerTickAction(TimeSpan time)
    {
        if (GameStatus == GameStatus.Run)
        {
            _time = time;
            ElapsedTime = $"{_time.Minutes:00}:{_time.Seconds:00}:{_time.Milliseconds / 10:00}";
            OnPropertyChanged(nameof(ElapsedTime));
        }
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

    public bool AddNewRecord(int placeNumber, string name)
    {
        return RecordsService.AddNewRecord(placeNumber, GameDifficulty, name, _time);
    }

    public void TrySetRemoveFlag(Cell cell)
    {
        Game.TrySetOrRemoveFlag(cell);
        RedrawFieldEvent?.Invoke();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;

        OnPropertyChanged(propertyName);

        return true;
    }
}
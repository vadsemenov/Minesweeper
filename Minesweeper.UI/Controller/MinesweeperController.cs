using System.ComponentModel;
using System.Runtime.CompilerServices;
using Minesweeper.Logic;
using Minesweeper.Logic.Enums;
using Timer = System.Windows.Forms.Timer;

namespace Minesweeper.UI.Controller;
public class MinesweeperController : INotifyPropertyChanged
{
    private readonly Game _game;

    private readonly Timer _timer = new();
    public GameDifficulty GameDifficulty { get; }

    public int RowsAmount => _game.RowsAmount;
    public int ColumnsAmount => _game.ColumnsAmount;

    public Cell[,] Field => _game.Field;

    public GameStatus GameStatus => _game.GameStatus;
    public event Action RedrawFieldEvent;

    public string ElapsedTime => $"{_game.ElapsedTime.Minutes:00}:{_game.ElapsedTime.Seconds:00}:{_game.ElapsedTime.Milliseconds / 10:00}";

    public List<RecordTime> RecordsTimes => RecordsService.ReadRecordsFromFile().Where(x => x.GameDifficulty == GameDifficulty).ToList();

    public int GetNewRecordPlace => RecordsService.GetNewRecordPlace(GameDifficulty, _game.ElapsedTime);

    private bool _isFirstClick = true;

    public MinesweeperController(GameDifficulty gameDifficulty, Action redrawFieldEvent)
    {
        _timer.Tick += TimerTick;
        _timer.Interval = 10;
        _timer.Start();


        RedrawFieldEvent = redrawFieldEvent;

        GameDifficulty = gameDifficulty;

        _game = new Game(gameDifficulty);
    }

    private void TimerTick(object sender, EventArgs e)
    {
        OnPropertyChanged(nameof(ElapsedTime));
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
        return RecordsService.AddNewRecord(placeNumber, GameDifficulty, name, _game.ElapsedTime);
    }

    public void TrySetRemoveFlag(Cell cell)
    {
        _game.TrySetOrRemoveFlag(cell);
        RedrawFieldEvent?.Invoke();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
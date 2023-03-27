using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Minesweeper.Logic;
using Minesweeper.Logic.Enums;
using System.IO;
using System.Linq;
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

    private readonly Stopwatch _timer = new();

    public double ElapsedTime => (double)_timer.ElapsedMilliseconds / 1000;

    public List<RecordTime> RecordsTimes => ReadRecordsFromFile();

    public MinesweeperController(GameDifficulty gameDifficulty, Action redrawFieldEvent)
    {
        RedrawFieldEvent = redrawFieldEvent;

        GameDifficulty = gameDifficulty;

        _game = new Game(gameDifficulty);

        _timer.Restart();
    }

    public void TryOpenCell(int rowCount, int columnCount)
    {
        _game.TryOpenCell(rowCount, columnCount);
        RedrawFieldEvent?.Invoke();

        CheckWinOrLoseGame();
    }

    private void CheckWinOrLoseGame()
    {
        if (_game.GameStatus == GameStatus.Win)
        {
            _timer.Stop();

            MessageBox.Show($"Win!\r\n Time: {ElapsedTime} seconds", "Win the Game!", MessageBoxButtons.OK, MessageBoxIcon.Information);

            CheckAndAddNewRecord();
        }

        if (_game.GameStatus == GameStatus.Lose)
        {
            _timer.Stop();

            MessageBox.Show("Lose the Game!", "Lose!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void CheckAndAddNewRecord()
    {
        var records = ReadRecordsFromFile();

        var insertIndex = records.OrderBy(rt => rt.Time).TakeWhile(rt => rt.Time <= ElapsedTime).Count();

        if (insertIndex < 5)
        {
            MessageBox.Show($"New Record!\r\n Time: {ElapsedTime} seconds", "New Record!", MessageBoxButtons.OK, MessageBoxIcon.Information);

            var recordsUserName = "Unknown";

            var newRecordsUserName = new RecordsUserNameForm();

            if (newRecordsUserName.ShowDialog() == DialogResult.OK)
            {
                recordsUserName = newRecordsUserName.UserName;
            }

            records.Insert(insertIndex, new RecordTime(recordsUserName, ElapsedTime));

            var highScores = new BestTimesForm(records);

            highScores.ShowDialog();
        }

        WriteRecordsToFile(records);
    }

    private List<RecordTime> ReadRecordsFromFile()
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

        return records;
    }

    private void WriteRecordsToFile(List<RecordTime> recordsList)
    {
        var text = JsonSerializer.Serialize(recordsList);

        File.WriteAllText("records.txt", text);
    }

    public void TrySetRemoveFlag(Cell cell)
    {
        _game.TrySetOrRemoveFlag(cell);
        RedrawFieldEvent?.Invoke();
    }
}

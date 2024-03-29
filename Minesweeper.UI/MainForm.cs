﻿using Minesweeper.Logic.Enums;
using Minesweeper.UI.Controller;

namespace Minesweeper.UI;
public partial class MainForm : Form
{
    private MinesweeperController _controller;

    private ExtendedButton[,] _buttonsField;

    private const int ButtonWidth = 36;
    private const int ButtonHeight = 36;

    private bool _isGameOver;

    public MainForm()
    {
        InitializeComponent();

        InitializeNewField(GameDifficulty.Easy);
    }

    private void BindTimeLabel()
    {
        var bindingSource = new BindingSource();
        bindingSource.DataSource = _controller;

        timeLabel.DataBindings.Clear();

        timeLabel.DataBindings.Add(new Binding("Text", bindingSource, nameof(_controller.ElapsedTime),
            true, DataSourceUpdateMode.OnPropertyChanged));
    }

    private void InitializeNewField(GameDifficulty gameDifficulty)
    {
        _controller = new MinesweeperController(gameDifficulty, RedrawField, this);

        CreateFieldView();

        SetFormSize();

        BindTimeLabel();

        _isGameOver = false;
    }

    private void CreateFieldView()
    {
        tableLayoutPanel.Controls.Clear();

        tableLayoutPanel.RowCount = _controller.RowsAmount;
        tableLayoutPanel.ColumnCount = _controller.ColumnsAmount;

        _buttonsField = new ExtendedButton[_controller.RowsAmount, _controller.ColumnsAmount];

        SuspendLayout();

        for (var y = 0; y < _controller.RowsAmount; y++)
        {
            for (var x = 0; x < _controller.ColumnsAmount; x++)
            {
                var button = new ExtendedButton(y, x);
                button.Name = $"{y},{x}";
                button.Width = ButtonWidth;
                button.Height = ButtonHeight;
                button.Margin = Padding.Empty;
                button.Padding = Padding.Empty;
                button.Image = Properties.Resources.NotOpened;
                button.MouseUp += ClickMouse;

                _buttonsField[y, x] = button;

                tableLayoutPanel.Controls.Add(button, x, y);
            }
        }

        ResumeLayout();
    }

    public void RedrawField()
    {
        for (var y = 0; y < _controller.RowsAmount; y++)
        {
            for (var x = 0; x < _controller.ColumnsAmount; x++)
            {
                var cell = _controller.Field[y, x];

                if (cell.Status == CellStatus.NotOpenedCell)
                {
                    _buttonsField[y, x].Image = Properties.Resources.NotOpened;
                }
                else if (cell.Status == CellStatus.Flag)
                {
                    _buttonsField[y, x].Image = Properties.Resources.Flagged;
                }
                else
                {
                    switch (cell.CellContent)
                    {
                        case CellContent.Mine:
                            _buttonsField[y, x].Image = Properties.Resources.Mine;
                            break;
                        case CellContent.NearOneMine:
                            _buttonsField[y, x].Image = Properties.Resources.Opened1;
                            break;
                        case CellContent.NearTwoMines:
                            _buttonsField[y, x].Image = Properties.Resources.Opened2;
                            break;
                        case CellContent.NearThreeMines:
                            _buttonsField[y, x].Image = Properties.Resources.Opened3;
                            break;
                        case CellContent.NearFourMines:
                            _buttonsField[y, x].Image = Properties.Resources.Opened4;
                            break;
                        case CellContent.NearFiveMines:
                            _buttonsField[y, x].Image = Properties.Resources.Opened5;
                            break;
                        case CellContent.NearSixMines:
                            _buttonsField[y, x].Image = Properties.Resources.Opened6;
                            break;
                        case CellContent.NearSevenMines:
                            _buttonsField[y, x].Image = Properties.Resources.Opened7;
                            break;
                        case CellContent.NearEightMines:
                            _buttonsField[y, x].Image = Properties.Resources.Opened8;
                            break;
                        case CellContent.ExplodedMine:
                            _buttonsField[y, x].Image = Properties.Resources.ExplodedMine;
                            break;
                        case CellContent.Empty:
                            _buttonsField[y, x].Image = Properties.Resources.OpenedEmpty;
                            break;
                    }
                }
            }
        }

        tableLayoutPanel.Focus();
    }

    private void SetFormSize()
    {
        var width = tableLayoutPanel.Bounds.Width + 20;
        var height = tableLayoutPanel.Bounds.Height + 90 + 20;

        Size = new Size(width, height);
    }

    private void ClickMouse(object sender, MouseEventArgs e)
    {
        if (_isGameOver)
        {
            return;
        }

        var button = (ExtendedButton)sender;

        if (e.Button == MouseButtons.Right)
        {
            _controller.TrySetRemoveFlag(_controller.Field[button.RowIndex, button.ColumnIndex]);
        }
        else if (e.Button == MouseButtons.Middle)
        {
            _controller.TryOpenNeighboringCells(button.RowIndex, button.ColumnIndex);
        }
        else
        {
            _controller.TryOpenCell(button.RowIndex, button.ColumnIndex);
        }

        if (_controller.GameStatus is GameStatus.Win or GameStatus.Lose)
        {
            CheckWinOrLoseGame();

            _isGameOver = true;
        }
    }

    private void CheckWinOrLoseGame()
    {
        if (_controller.GameStatus == GameStatus.Win)
        {
            MessageBox.Show($"Win!\r\nTime: {_controller.ElapsedTime} seconds", "Win the Game!", MessageBoxButtons.OK, MessageBoxIcon.Information);

            var recordPlace = _controller.GetNewRecordPlace;

            if (recordPlace is >= 0 and < 5)
            {
                MessageBox.Show($"New Record!\r\nTime: {_controller.ElapsedTime} seconds", "New Record!", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                var recordsUserName = "Unknown";

                var newRecordsUserNameForm = new RecordsUserNameForm();

                if (newRecordsUserNameForm.ShowDialog() == DialogResult.OK)
                {
                    recordsUserName = newRecordsUserNameForm.UserName;
                }

                if (_controller.AddNewRecord(recordPlace, recordsUserName))
                {
                    var highScores = new BestTimesForm(_controller.RecordsTimes);

                    highScores.ShowDialog();
                }
            }
        }

        if (_controller.GameStatus == GameStatus.Lose)
        {
            MessageBox.Show("Lose the Game!", "Lose!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void CreateNewGame()
    {
        var newGameForm = new NewGameForm();

        if (newGameForm.ShowDialog(this) == DialogResult.OK)
        {
            InitializeNewField(newGameForm.GameDifficulty);
        }
    }

    private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
    {
        CreateNewGame();
    }

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
        MessageBox.Show("Developed by Vadim Semenov.\r\nEmail: 5587394@mail.ru\r\nTelegram: @VadSemenov",
            "About game", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void highScoresToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var highScoresForm = new BestTimesForm(_controller.RecordsTimes);

        highScoresForm.ShowDialog();
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Dispose();
    }

    private void newGameButton_Click(object sender, EventArgs e)
    {
        CreateNewGame();
    }
}
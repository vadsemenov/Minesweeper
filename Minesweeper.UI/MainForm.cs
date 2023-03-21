using System;
using System.Drawing;
using System.Windows.Forms;
using Minesweeper.Logic.Enums;
using Minesweeper.UI.Controller;

namespace Minesweeper.UI
{
    public partial class MainForm : Form
    {
        private MinesweeperController _controller;

        private ExtendedButton[,] _buttonsField;

        private const int ButtonWidth = 36;
        private const int ButtonHeight = 36;

        public MainForm()
        {
            InitializeComponent();

            InitializeNewField(GameDifficulty.Hard);
        }

        private void InitializeNewField(GameDifficulty gameDifficulty)
        {
            _controller = new MinesweeperController(gameDifficulty, RedrawFieldEvent);

            CreateFieldView();

            SetFormSize();
        }

        private void CreateFieldView()
        {
            tableLayoutPanel1.Controls.Clear();

            tableLayoutPanel1.RowCount = _controller.RowsAmount;
            tableLayoutPanel1.ColumnCount = _controller.ColumnsAmount;

            _buttonsField = new ExtendedButton[_controller.RowsAmount, _controller.ColumnsAmount];

            for (int i = 0; i < _controller.RowsAmount; i++)
            {
                for (int j = 0; j < _controller.ColumnsAmount; j++)
                {
                    var button = new ExtendedButton(i, j);
                    button.Name = $"{i},{j}";
                    button.Width = ButtonWidth;
                    button.Height = ButtonHeight;
                    button.Margin = Padding.Empty;
                    button.Padding = Padding.Empty;
                    button.Image = Properties.Resources.NotOpened;
                    button.MouseUp += ClickMouse;
                    button.Click += ClickButton;

                    _buttonsField[i, j] = button;

                    tableLayoutPanel1.Controls.Add(button, j, i);
                }
            }
        }


        private void RedrawFieldEvent()
        {
            for (int i = 0; i < _controller.RowsAmount; i++)
            {
                for (int j = 0; j < _controller.ColumnsAmount; j++)
                {
                    var cell = _controller.Field[i, j];

                    if (cell.CellStatus == CellStatus.NotOpenedCell)
                    {
                        _buttonsField[i, j].Image = Properties.Resources.NotOpened;
                    }
                    else if (cell.CellStatus == CellStatus.Flag)
                    {
                        _buttonsField[i, j].Image = Properties.Resources.Flaged;
                    }
                    else
                    {
                        switch (cell.CellContent)
                        {
                            case CellContent.Mine:
                                _buttonsField[i, j].Image = Properties.Resources.Mine;
                                break;
                            case CellContent.NearOneMine:
                                _buttonsField[i, j].Image = Properties.Resources.Opened1;
                                break;
                            case CellContent.NearTwoMine:
                                _buttonsField[i, j].Image = Properties.Resources.Opened2;
                                break;
                            case CellContent.NearThreeMine:
                                _buttonsField[i, j].Image = Properties.Resources.Opened3;
                                break;
                            case CellContent.NearFourMine:
                                _buttonsField[i, j].Image = Properties.Resources.Opened4;
                                break;
                            case CellContent.NearFiveMine:
                                _buttonsField[i, j].Image = Properties.Resources.Opened5;
                                break;
                            case CellContent.NearSixMine:
                                _buttonsField[i, j].Image = Properties.Resources.Opened6;
                                break;
                            case CellContent.NearSevenMine:
                                _buttonsField[i, j].Image = Properties.Resources.Opened7;
                                break;
                            case CellContent.NearEightMine:
                                _buttonsField[i, j].Image = Properties.Resources.Opened8;
                                break;
                            case CellContent.ExplosedMine:
                                _buttonsField[i, j].Image = Properties.Resources.ExplosedMine;
                                break;
                            case CellContent.EmptyCell:
                                _buttonsField[i, j].Image = Properties.Resources.OpenedEmpty;
                                break;
                        }
                    }
                }
            }

            tableLayoutPanel1.Focus();
        }

        private void SetFormSize()
        {

            var width = tableLayoutPanel1.Bounds.Width + 20;
            var height = tableLayoutPanel1.Bounds.Height + 50 + 20;

            Size = new Size(width, height);
        }

        private void ClickMouse(object sender, MouseEventArgs e)
        {
            var button = (ExtendedButton)sender;

            if (e.Button == MouseButtons.Right)
            {
                _controller.TrySetRemoveFlag(_controller.Field[button.RowCount, button.ColumnCount]);
            }
            else
            {
                _controller.TryOpenCell(button.RowCount, button.ColumnCount);

                if (_controller.GameStatus == GameStatus.Win)
                {
                    MessageBox.Show("Выиграл!");
                }
            }
        }

        private void ClickButton(object sender, EventArgs e)
        {
            var button = (ExtendedButton)sender;

            _controller.TryOpenCell(button.RowCount, button.ColumnCount);
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var newGameForm = new NewGameForm();

            if (newGameForm.ShowDialog(this) == DialogResult.OK)
            {
                InitializeNewField(newGameForm.GameDifficulty);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Developed by Vadim Semenov. \r\n Email: 5587394@mail.ru \r\n Telegram: @VadSemenov",
                "About game", MessageBoxButtons.OK);
        }

        private void highScoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var highScoresForm = new HighScoresForm();

            highScoresForm.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }

    class ExtendedButton : Button
    {
        public int RowCount { get; set; }

        public int ColumnCount { get; set; }

        public ExtendedButton(int rowCount, int columnCount)
        {
            RowCount = rowCount;
            ColumnCount = columnCount;
        }
    }
}
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

        private bool _isGameOver;

        public MainForm()
        {
            InitializeComponent();

            InitializeNewField(GameDifficulty.Easy);
        }

        private void InitializeNewField(GameDifficulty gameDifficulty)
        {
            _controller = new MinesweeperController(gameDifficulty, RedrawFieldEvent);

            CreateFieldView();

            SetFormSize();

            _isGameOver = false;
        }

        private void CreateFieldView()
        {
            tableLayoutPanel1.Controls.Clear();

            tableLayoutPanel1.RowCount = _controller.RowsAmount;
            tableLayoutPanel1.ColumnCount = _controller.ColumnsAmount;

            _buttonsField = new ExtendedButton[_controller.RowsAmount, _controller.ColumnsAmount];

            SuspendLayout();

            for (var i = 0; i < _controller.RowsAmount; i++)
            {
                for (var j = 0; j < _controller.ColumnsAmount; j++)
                {
                    var button = new ExtendedButton(i, j);
                    button.Name = $"{i},{j}";
                    button.Width = ButtonWidth;
                    button.Height = ButtonHeight;
                    button.Margin = Padding.Empty;
                    button.Padding = Padding.Empty;
                    button.Image = Properties.Resources.NotOpened;
                    button.MouseUp += ClickMouse;

                    _buttonsField[i, j] = button;

                    tableLayoutPanel1.Controls.Add(button, j, i);
                }
            }

            ResumeLayout();
        }

        private void RedrawFieldEvent()
        {
            for (var i = 0; i < _controller.RowsAmount; i++)
            {
                for (var j = 0; j < _controller.ColumnsAmount; j++)
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
                            case CellContent.ExplodedMine:
                                _buttonsField[i, j].Image = Properties.Resources.ExplodedMine;
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
            if (_isGameOver)
            {
                return;
            }

            var button = (ExtendedButton)sender;

            if (e.Button == MouseButtons.Right)
            {
                _controller.TrySetRemoveFlag(_controller.Field[button.RowCount, button.ColumnCount]);
            }
            else
            {
                _controller.TryOpenCell(button.RowCount, button.ColumnCount);

                if (_controller.GameStatus is GameStatus.Win or GameStatus.Lose)
                {
                    _isGameOver = true;
                }
            }
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
    }
}
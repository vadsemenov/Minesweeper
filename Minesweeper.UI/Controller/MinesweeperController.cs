using System;
using Minesweeper.Logic;
using Minesweeper.Logic.Enums;

namespace Minesweeper.UI.Controller
{
    public class MinesweeperController
    {
        private Game _game;
        public GameDifficulty GameDifficulty { get; private set; }

        public int RowsAmount => _game.RowsAmount;
        public int ColumnsAmount => _game.ColumnsAmount;

        public Cell[,] Field => _game.Field;

        public GameStatus GameStatus => _game.GameStatus;

        public event Action RedrawFieldEvent; 

        public MinesweeperController(GameDifficulty gameDifficulty, Action redrawFieldEvent)
        {
            RedrawFieldEvent = redrawFieldEvent;

            GameDifficulty = gameDifficulty;

            _game = new Game(GameDifficulty);
        }

        public void TryOpenCell(int rowCount, int columnCount)
        {
            _game.TryOpenCell(rowCount, columnCount);
            RedrawFieldEvent?.Invoke();
        }

        public void TrySetRemoveFlag(Cell cell)
        {
            _game.TrySetOrRemoveFlag(cell);
            RedrawFieldEvent?.Invoke();
        }
    }
}
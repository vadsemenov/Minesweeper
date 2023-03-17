using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Logic
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var board = new Game(12, 12, 20);

            board.TryOpenCell(0, 3);
            board.TryOpenCell(11, 11);

            Console.WriteLine(board);
            Console.WriteLine(board.FieldView());

            Console.Read();
        }
    }
}

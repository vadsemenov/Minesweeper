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
            // Блок «легкий» настраивает игровое поле по размерам 8х8 и количеством мин -6.
            // Блок «средний» настраивает игровое поле по размерам 12х12 и количеством мин -20.
            // Блок «сложный» настраивает игровое поле по размерам 20х20 и количеством мин -55.
            var board = new Board(5, 5, 5);

            Console.WriteLine(board);

            Console.Read();
        }
    }
}

using System.Windows.Forms;

namespace Minesweeper.UI
{
    public partial class HighScoresForm : Form
    {

        public HighScoresForm()
        {
            InitializeComponent();
        }
    }
    public record RecordTime
    {
        public string Name { get; set; }

        public double Time { get; set; }
    }
}

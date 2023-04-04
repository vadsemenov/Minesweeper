using System.Globalization;
using Minesweeper.Logic;
using Minesweeper.UI.Controller;

namespace Minesweeper.UI
{
    public partial class BestTimesForm : Form
    {
        public List<RecordTime> RecordsTimes { get; set; }

        public BestTimesForm()
        {
            InitializeComponent();
        }

        public BestTimesForm(List<RecordTime> recordsTimes)
        {
            RecordsTimes = recordsTimes;

            InitializeComponent();

            FillRecords();
        }

        private void FillRecords()
        {
            if (RecordsTimes == null)
            {
                return;
            }

            var recordsCount = RecordsTimes.Take(5).Count();

            for (var i = 0; i < recordsCount; i++)
            {
                ((Label)tableLayoutPanel1.GetControlFromPosition(1, i)).Text = RecordsTimes[i].Name;
                ((Label)tableLayoutPanel1.GetControlFromPosition(2, i)).Text = RecordsTimes[i].Time.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            Dispose();
        }
    }
}
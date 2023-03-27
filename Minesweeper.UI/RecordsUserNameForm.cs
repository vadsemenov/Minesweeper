using System;
using System.Windows.Forms;

namespace Minesweeper.UI
{
    public partial class RecordsUserNameForm : Form
    {

        public string UserName { get; set; }

        public RecordsUserNameForm()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            UserName = userNameTextBox.Text;

            Dispose();
        }
    }
}

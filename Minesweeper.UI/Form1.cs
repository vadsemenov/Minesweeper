using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper.UI
{
    public partial class Form1 : Form
    {
        private int fieldSize = 10;
        private Button[,] fieldsButtons;
        private int buttonSize = 36;
        public Form1()
        {
            InitializeComponent();

            InitFieldButtons();
        }

        private void InitFieldButtons()
        {
            fieldsButtons = new Button[fieldSize, fieldSize];

            for (int i = 0; i < fieldsButtons.GetLength(0); i++)
            {
                for (int j = 0; j < fieldsButtons.GetLength(1); j++)
                {
                    var button = new Button();
                    // button.Click += ClickButton;
                    button.MouseUp += ClickMouse;
                    button.Image = Properties.Resources.NotOpened;
                    button.Location = new Point(i*buttonSize, j*buttonSize);
                    button.Size = new Size(buttonSize, buttonSize);
                    this.Controls.Add(button);
                    fieldsButtons[j,i] = button;
                }
                
            }
        }

        private void ClickMouse(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ((Button) sender).Image = Properties.Resources.Flaged;
            }
            else
            {
                var but = (Button)sender;
                but.Image = Properties.Resources.Mine;
            }

        }

        private void ClickButton(object button, EventArgs e)
        {
            var but = (Button) button;
            but.Image = Properties.Resources.Mine;
        }
    }
}

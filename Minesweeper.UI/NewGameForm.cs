﻿using System;
using System.Windows.Forms;
using Minesweeper.Logic.Enums;

namespace Minesweeper.UI
{
    public partial class NewGameForm : Form
    {
        public GameDifficulty GameDifficulty { get; private set; } = GameDifficulty.Easy;

        public NewGameForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (easyDifficultRadioButton.Checked)
            {
                GameDifficulty = GameDifficulty.Easy;
            }else if (normalDifficultRadioButton.Checked)
            {
                GameDifficulty = GameDifficulty.Normal;
            }
            else if (hardDifficultRadioButton.Checked)
            {
                GameDifficulty = GameDifficulty.Hard;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
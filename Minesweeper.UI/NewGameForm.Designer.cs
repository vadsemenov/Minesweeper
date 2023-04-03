namespace Minesweeper.UI
{
    partial class NewGameForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.hardDifficultRadioButton = new System.Windows.Forms.RadioButton();
            this.normalDifficultRadioButton = new System.Windows.Forms.RadioButton();
            this.easyDifficultRadioButton = new System.Windows.Forms.RadioButton();
            this.difficultySelectionButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.difficultySelectionButton, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(208, 192);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.hardDifficultRadioButton);
            this.groupBox1.Controls.Add(this.normalDifficultRadioButton);
            this.groupBox1.Controls.Add(this.easyDifficultRadioButton);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(202, 147);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Game difficulty";
            // 
            // hardDifficultRadioButton
            // 
            this.hardDifficultRadioButton.AutoSize = true;
            this.hardDifficultRadioButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.hardDifficultRadioButton.Location = new System.Drawing.Point(69, 106);
            this.hardDifficultRadioButton.Name = "hardDifficultRadioButton";
            this.hardDifficultRadioButton.Size = new System.Drawing.Size(62, 24);
            this.hardDifficultRadioButton.TabIndex = 2;
            this.hardDifficultRadioButton.Text = "Hard";
            this.hardDifficultRadioButton.UseVisualStyleBackColor = true;
            // 
            // normalDifficultRadioButton
            // 
            this.normalDifficultRadioButton.AutoSize = true;
            this.normalDifficultRadioButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.normalDifficultRadioButton.Location = new System.Drawing.Point(69, 65);
            this.normalDifficultRadioButton.Name = "normalDifficultRadioButton";
            this.normalDifficultRadioButton.Size = new System.Drawing.Size(77, 24);
            this.normalDifficultRadioButton.TabIndex = 1;
            this.normalDifficultRadioButton.Text = "Normal";
            this.normalDifficultRadioButton.UseVisualStyleBackColor = true;
            // 
            // easyDifficultRadioButton
            // 
            this.easyDifficultRadioButton.AutoSize = true;
            this.easyDifficultRadioButton.Checked = true;
            this.easyDifficultRadioButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.easyDifficultRadioButton.Location = new System.Drawing.Point(69, 26);
            this.easyDifficultRadioButton.Name = "easyDifficultRadioButton";
            this.easyDifficultRadioButton.Size = new System.Drawing.Size(62, 24);
            this.easyDifficultRadioButton.TabIndex = 0;
            this.easyDifficultRadioButton.TabStop = true;
            this.easyDifficultRadioButton.Text = "Easy";
            this.easyDifficultRadioButton.UseVisualStyleBackColor = true;
            // 
            // difficultySelectionButton
            // 
            this.difficultySelectionButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.difficultySelectionButton.Location = new System.Drawing.Point(3, 156);
            this.difficultySelectionButton.Name = "difficultySelectionButton";
            this.difficultySelectionButton.Size = new System.Drawing.Size(202, 33);
            this.difficultySelectionButton.TabIndex = 1;
            this.difficultySelectionButton.Text = "OK";
            this.difficultySelectionButton.UseVisualStyleBackColor = true;
            this.difficultySelectionButton.Click += new System.EventHandler(this.difficultySelectionButton_Click);
            // 
            // NewGameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(208, 192);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewGameForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New game difficulty";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton hardDifficultRadioButton;
        private System.Windows.Forms.RadioButton normalDifficultRadioButton;
        private System.Windows.Forms.RadioButton easyDifficultRadioButton;
        private System.Windows.Forms.Button difficultySelectionButton;
    }
}
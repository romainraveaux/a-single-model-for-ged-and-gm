namespace LP
{
    partial class MainWindow
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.grapheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chargeDuGraphe1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chargeDuGraphe2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.problèmeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.isomorphismeDeSousgrapheExactF1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.isomorphismeDeSousgrapheExactF2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.isomorphismeDeSousgrapheInexactF1aToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.isomorphismeDeSousgrapheInexactF2aToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.isomorphismeDeGrapheInexactF1bToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.isomorphismeDeGrapheInexactF2bToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.solveurToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cPLEXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gLPKToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportDuProblèmeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportDuRésultatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exploreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.comboBoxGraphe = new System.Windows.Forms.ComboBox();
            this.Graphe = new System.Windows.Forms.Label();
            this.buttonResoudre = new System.Windows.Forms.Button();
            this.labelMode = new System.Windows.Forms.Label();
            this.comboBoxMode = new System.Windows.Forms.ComboBox();
            this.comboBoxSolveur = new System.Windows.Forms.ComboBox();
            this.comboBoxProblem = new System.Windows.Forms.ComboBox();
            this.labelSolveur = new System.Windows.Forms.Label();
            this.labelProblem = new System.Windows.Forms.Label();
            this.groupBoxGraphe1 = new System.Windows.Forms.GroupBox();
            this.pictureBoxGraphe1 = new System.Windows.Forms.PictureBox();
            this.groupBoxGraphe2 = new System.Windows.Forms.GroupBox();
            this.pictureBoxGraphe2 = new System.Windows.Forms.PictureBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.richTextBoxAppariement = new System.Windows.Forms.RichTextBox();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBoxGraphe1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGraphe1)).BeginInit();
            this.groupBoxGraphe2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGraphe2)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.grapheToolStripMenuItem,
            this.problèmeToolStripMenuItem,
            this.solveurToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.exploreToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(921, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // grapheToolStripMenuItem
            // 
            this.grapheToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chargeDuGraphe1ToolStripMenuItem,
            this.chargeDuGraphe2ToolStripMenuItem});
            this.grapheToolStripMenuItem.Name = "grapheToolStripMenuItem";
            this.grapheToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.grapheToolStripMenuItem.Text = "Graphe";
            // 
            // chargeDuGraphe1ToolStripMenuItem
            // 
            this.chargeDuGraphe1ToolStripMenuItem.Name = "chargeDuGraphe1ToolStripMenuItem";
            this.chargeDuGraphe1ToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.chargeDuGraphe1ToolStripMenuItem.Text = "Charge G1";
            this.chargeDuGraphe1ToolStripMenuItem.Click += new System.EventHandler(this.chargeDuGraphe1ToolStripMenuItem_Click);
            // 
            // chargeDuGraphe2ToolStripMenuItem
            // 
            this.chargeDuGraphe2ToolStripMenuItem.Name = "chargeDuGraphe2ToolStripMenuItem";
            this.chargeDuGraphe2ToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.chargeDuGraphe2ToolStripMenuItem.Text = "Charge G2";
            this.chargeDuGraphe2ToolStripMenuItem.Click += new System.EventHandler(this.chargeDuGraphe2ToolStripMenuItem_Click);
            // 
            // problèmeToolStripMenuItem
            // 
            this.problèmeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.isomorphismeDeSousgrapheExactF1ToolStripMenuItem,
            this.isomorphismeDeSousgrapheExactF2ToolStripMenuItem,
            this.isomorphismeDeSousgrapheInexactF1aToolStripMenuItem,
            this.isomorphismeDeSousgrapheInexactF2aToolStripMenuItem,
            this.isomorphismeDeGrapheInexactF1bToolStripMenuItem,
            this.isomorphismeDeGrapheInexactF2bToolStripMenuItem});
            this.problèmeToolStripMenuItem.Name = "problèmeToolStripMenuItem";
            this.problèmeToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
            this.problèmeToolStripMenuItem.Text = "Problème";
            // 
            // isomorphismeDeSousgrapheExactF1ToolStripMenuItem
            // 
            this.isomorphismeDeSousgrapheExactF1ToolStripMenuItem.Name = "isomorphismeDeSousgrapheExactF1ToolStripMenuItem";
            this.isomorphismeDeSousgrapheExactF1ToolStripMenuItem.Size = new System.Drawing.Size(299, 22);
            this.isomorphismeDeSousgrapheExactF1ToolStripMenuItem.Text = "Isomorphisme de sous-graphe exact F1";
            this.isomorphismeDeSousgrapheExactF1ToolStripMenuItem.Click += new System.EventHandler(this.isomorphismeDeSousgrapheExactF1ToolStripMenuItem_Click);
            // 
            // isomorphismeDeSousgrapheExactF2ToolStripMenuItem
            // 
            this.isomorphismeDeSousgrapheExactF2ToolStripMenuItem.Name = "isomorphismeDeSousgrapheExactF2ToolStripMenuItem";
            this.isomorphismeDeSousgrapheExactF2ToolStripMenuItem.Size = new System.Drawing.Size(299, 22);
            this.isomorphismeDeSousgrapheExactF2ToolStripMenuItem.Text = "Isomorphisme de sous-graphe exact F2";
            this.isomorphismeDeSousgrapheExactF2ToolStripMenuItem.Click += new System.EventHandler(this.isomorphismeDeSousgrapheExactF2ToolStripMenuItem_Click);
            // 
            // isomorphismeDeSousgrapheInexactF1aToolStripMenuItem
            // 
            this.isomorphismeDeSousgrapheInexactF1aToolStripMenuItem.Name = "isomorphismeDeSousgrapheInexactF1aToolStripMenuItem";
            this.isomorphismeDeSousgrapheInexactF1aToolStripMenuItem.Size = new System.Drawing.Size(299, 22);
            this.isomorphismeDeSousgrapheInexactF1aToolStripMenuItem.Text = "Isomorphisme de sous-graphe inexact F1.a";
            this.isomorphismeDeSousgrapheInexactF1aToolStripMenuItem.Click += new System.EventHandler(this.isomorphismeDeSousgrapheInexactF1aToolStripMenuItem_Click);
            // 
            // isomorphismeDeSousgrapheInexactF2aToolStripMenuItem
            // 
            this.isomorphismeDeSousgrapheInexactF2aToolStripMenuItem.Name = "isomorphismeDeSousgrapheInexactF2aToolStripMenuItem";
            this.isomorphismeDeSousgrapheInexactF2aToolStripMenuItem.Size = new System.Drawing.Size(299, 22);
            this.isomorphismeDeSousgrapheInexactF2aToolStripMenuItem.Text = "Isomorphisme de sous-graphe inexact F2.a";
            this.isomorphismeDeSousgrapheInexactF2aToolStripMenuItem.Click += new System.EventHandler(this.isomorphismeDeSousgrapheInexactF2aToolStripMenuItem_Click);
            // 
            // isomorphismeDeGrapheInexactF1bToolStripMenuItem
            // 
            this.isomorphismeDeGrapheInexactF1bToolStripMenuItem.Name = "isomorphismeDeGrapheInexactF1bToolStripMenuItem";
            this.isomorphismeDeGrapheInexactF1bToolStripMenuItem.Size = new System.Drawing.Size(299, 22);
            this.isomorphismeDeGrapheInexactF1bToolStripMenuItem.Text = "Isomorphisme de graphe inexact F1.b";
            this.isomorphismeDeGrapheInexactF1bToolStripMenuItem.Click += new System.EventHandler(this.isomorphismeDeGrapheInexactF1bToolStripMenuItem_Click);
            // 
            // isomorphismeDeGrapheInexactF2bToolStripMenuItem
            // 
            this.isomorphismeDeGrapheInexactF2bToolStripMenuItem.Name = "isomorphismeDeGrapheInexactF2bToolStripMenuItem";
            this.isomorphismeDeGrapheInexactF2bToolStripMenuItem.Size = new System.Drawing.Size(299, 22);
            this.isomorphismeDeGrapheInexactF2bToolStripMenuItem.Text = "Isomorphisme de graphe inexact F2.b";
            this.isomorphismeDeGrapheInexactF2bToolStripMenuItem.Click += new System.EventHandler(this.isomorphismeDeGrapheInexactF2bToolStripMenuItem_Click);
            // 
            // solveurToolStripMenuItem
            // 
            this.solveurToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cPLEXToolStripMenuItem,
            this.gLPKToolStripMenuItem});
            this.solveurToolStripMenuItem.Name = "solveurToolStripMenuItem";
            this.solveurToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.solveurToolStripMenuItem.Text = "Solveur";
            // 
            // cPLEXToolStripMenuItem
            // 
            this.cPLEXToolStripMenuItem.Name = "cPLEXToolStripMenuItem";
            this.cPLEXToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.cPLEXToolStripMenuItem.Text = "CPLEX";
            this.cPLEXToolStripMenuItem.Click += new System.EventHandler(this.cPLEXToolStripMenuItem_Click);
            // 
            // gLPKToolStripMenuItem
            // 
            this.gLPKToolStripMenuItem.Name = "gLPKToolStripMenuItem";
            this.gLPKToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.gLPKToolStripMenuItem.Text = "GLPK";
            this.gLPKToolStripMenuItem.Click += new System.EventHandler(this.gLPKToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportDuProblèmeToolStripMenuItem,
            this.exportDuRésultatToolStripMenuItem});
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.exportToolStripMenuItem.Text = "Export";
            // 
            // exportDuProblèmeToolStripMenuItem
            // 
            this.exportDuProblèmeToolStripMenuItem.Name = "exportDuProblèmeToolStripMenuItem";
            this.exportDuProblèmeToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.exportDuProblèmeToolStripMenuItem.Text = "Export du problème";
            this.exportDuProblèmeToolStripMenuItem.Click += new System.EventHandler(this.exportDuProblèmeToolStripMenuItem_Click);
            // 
            // exportDuRésultatToolStripMenuItem
            // 
            this.exportDuRésultatToolStripMenuItem.Name = "exportDuRésultatToolStripMenuItem";
            this.exportDuRésultatToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.exportDuRésultatToolStripMenuItem.Text = "Export du résultat";
            this.exportDuRésultatToolStripMenuItem.Click += new System.EventHandler(this.exportDuRésultatToolStripMenuItem_Click);
            // 
            // exploreToolStripMenuItem
            // 
            this.exploreToolStripMenuItem.Name = "exploreToolStripMenuItem";
            this.exploreToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.exploreToolStripMenuItem.Text = "Explore";
            this.exploreToolStripMenuItem.Click += new System.EventHandler(this.exploreToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 315F));
            this.tableLayoutPanel1.Controls.Add(this.comboBoxGraphe, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.Graphe, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonResoudre, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelMode, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxMode, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxSolveur, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxProblem, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelSolveur, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelProblem, 0, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 27);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(380, 151);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // comboBoxGraphe
            // 
            this.comboBoxGraphe.FormattingEnabled = true;
            this.comboBoxGraphe.Items.AddRange(new object[] {
            "Graphe Grec",
            "Graphe Mutagenicity",
            "Graphe LOW"});
            this.comboBoxGraphe.Location = new System.Drawing.Point(68, 3);
            this.comboBoxGraphe.Name = "comboBoxGraphe";
            this.comboBoxGraphe.Size = new System.Drawing.Size(305, 21);
            this.comboBoxGraphe.TabIndex = 1;
            this.comboBoxGraphe.SelectedIndexChanged += new System.EventHandler(this.comboBoxGraphe_SelectedIndexChanged);
            // 
            // Graphe
            // 
            this.Graphe.AutoSize = true;
            this.Graphe.Location = new System.Drawing.Point(3, 0);
            this.Graphe.Name = "Graphe";
            this.Graphe.Size = new System.Drawing.Size(51, 13);
            this.Graphe.TabIndex = 8;
            this.Graphe.Text = "Graphe : ";
            // 
            // buttonResoudre
            // 
            this.buttonResoudre.Location = new System.Drawing.Point(68, 123);
            this.buttonResoudre.Name = "buttonResoudre";
            this.buttonResoudre.Size = new System.Drawing.Size(305, 23);
            this.buttonResoudre.TabIndex = 7;
            this.buttonResoudre.Text = "Résoudre par programmation linéaire";
            this.buttonResoudre.UseVisualStyleBackColor = true;
            this.buttonResoudre.Click += new System.EventHandler(this.buttonResoudre_Click);
            // 
            // labelMode
            // 
            this.labelMode.AutoSize = true;
            this.labelMode.Location = new System.Drawing.Point(3, 90);
            this.labelMode.Name = "labelMode";
            this.labelMode.Size = new System.Drawing.Size(43, 13);
            this.labelMode.TabIndex = 4;
            this.labelMode.Text = "Mode : ";
            // 
            // comboBoxMode
            // 
            this.comboBoxMode.FormattingEnabled = true;
            this.comboBoxMode.Location = new System.Drawing.Point(68, 93);
            this.comboBoxMode.Name = "comboBoxMode";
            this.comboBoxMode.Size = new System.Drawing.Size(305, 21);
            this.comboBoxMode.TabIndex = 5;
            // 
            // comboBoxSolveur
            // 
            this.comboBoxSolveur.FormattingEnabled = true;
            this.comboBoxSolveur.Items.AddRange(new object[] {
            "CPLEX",
            "GLPK"});
            this.comboBoxSolveur.Location = new System.Drawing.Point(68, 63);
            this.comboBoxSolveur.Name = "comboBoxSolveur";
            this.comboBoxSolveur.Size = new System.Drawing.Size(305, 21);
            this.comboBoxSolveur.TabIndex = 3;
            this.comboBoxSolveur.SelectedIndexChanged += new System.EventHandler(this.comboBoxSolveur_SelectedIndexChanged);
            // 
            // comboBoxProblem
            // 
            this.comboBoxProblem.FormattingEnabled = true;
            this.comboBoxProblem.Items.AddRange(new object[] {
            "Isomorphisme de sous-graphe exact F1",
            "Isomorphisme de sous-graphe exact F2",
            "Isomorphisme de sous-graphe inexact F1a",
            "Isomorphisme de sous-graphe inexact F2a",
            "Isomorphisme de graphe inexact F1b",
            "Isomorphisme de graphe inexact F2b",
            "QAPGMGED"});
            this.comboBoxProblem.Location = new System.Drawing.Point(68, 33);
            this.comboBoxProblem.Name = "comboBoxProblem";
            this.comboBoxProblem.Size = new System.Drawing.Size(305, 21);
            this.comboBoxProblem.TabIndex = 1;
            // 
            // labelSolveur
            // 
            this.labelSolveur.AutoSize = true;
            this.labelSolveur.Location = new System.Drawing.Point(3, 60);
            this.labelSolveur.Name = "labelSolveur";
            this.labelSolveur.Size = new System.Drawing.Size(52, 13);
            this.labelSolveur.TabIndex = 2;
            this.labelSolveur.Text = "Solveur : ";
            // 
            // labelProblem
            // 
            this.labelProblem.AutoSize = true;
            this.labelProblem.Location = new System.Drawing.Point(3, 30);
            this.labelProblem.Name = "labelProblem";
            this.labelProblem.Size = new System.Drawing.Size(57, 13);
            this.labelProblem.TabIndex = 0;
            this.labelProblem.Text = "Problème : ";
            // 
            // groupBoxGraphe1
            // 
            this.groupBoxGraphe1.Controls.Add(this.pictureBoxGraphe1);
            this.groupBoxGraphe1.Location = new System.Drawing.Point(13, 184);
            this.groupBoxGraphe1.Name = "groupBoxGraphe1";
            this.groupBoxGraphe1.Size = new System.Drawing.Size(379, 213);
            this.groupBoxGraphe1.TabIndex = 2;
            this.groupBoxGraphe1.TabStop = false;
            this.groupBoxGraphe1.Text = "Graphe1";
            // 
            // pictureBoxGraphe1
            // 
            this.pictureBoxGraphe1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxGraphe1.Location = new System.Drawing.Point(3, 16);
            this.pictureBoxGraphe1.Name = "pictureBoxGraphe1";
            this.pictureBoxGraphe1.Size = new System.Drawing.Size(373, 194);
            this.pictureBoxGraphe1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxGraphe1.TabIndex = 0;
            this.pictureBoxGraphe1.TabStop = false;
            // 
            // groupBoxGraphe2
            // 
            this.groupBoxGraphe2.Controls.Add(this.pictureBoxGraphe2);
            this.groupBoxGraphe2.Location = new System.Drawing.Point(12, 403);
            this.groupBoxGraphe2.Name = "groupBoxGraphe2";
            this.groupBoxGraphe2.Size = new System.Drawing.Size(379, 213);
            this.groupBoxGraphe2.TabIndex = 3;
            this.groupBoxGraphe2.TabStop = false;
            this.groupBoxGraphe2.Text = "Graphe2";
            // 
            // pictureBoxGraphe2
            // 
            this.pictureBoxGraphe2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxGraphe2.Location = new System.Drawing.Point(3, 16);
            this.pictureBoxGraphe2.Name = "pictureBoxGraphe2";
            this.pictureBoxGraphe2.Size = new System.Drawing.Size(373, 194);
            this.pictureBoxGraphe2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxGraphe2.TabIndex = 0;
            this.pictureBoxGraphe2.TabStop = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 619);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(921, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.richTextBoxAppariement);
            this.panel1.Location = new System.Drawing.Point(399, 27);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(510, 586);
            this.panel1.TabIndex = 6;
            // 
            // richTextBoxAppariement
            // 
            this.richTextBoxAppariement.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxAppariement.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxAppariement.Name = "richTextBoxAppariement";
            this.richTextBoxAppariement.Size = new System.Drawing.Size(510, 586);
            this.richTextBoxAppariement.TabIndex = 0;
            this.richTextBoxAppariement.Text = "";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(921, 641);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBoxGraphe2);
            this.Controls.Add(this.groupBoxGraphe1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.Text = "Appariement de deux graphes";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBoxGraphe1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGraphe1)).EndInit();
            this.groupBoxGraphe2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGraphe2)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem grapheToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem problèmeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem solveurToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chargeDuGraphe1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chargeDuGraphe2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem isomorphismeDeSousgrapheExactF1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem isomorphismeDeSousgrapheExactF2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem isomorphismeDeSousgrapheInexactF1aToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem isomorphismeDeSousgrapheInexactF2aToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem isomorphismeDeGrapheInexactF1bToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem isomorphismeDeGrapheInexactF2bToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cPLEXToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gLPKToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportDuProblèmeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportDuRésultatToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelProblem;
        private System.Windows.Forms.ComboBox comboBoxProblem;
        private System.Windows.Forms.Label labelSolveur;
        private System.Windows.Forms.ComboBox comboBoxSolveur;
        private System.Windows.Forms.Label labelMode;
        private System.Windows.Forms.ComboBox comboBoxMode;
        private System.Windows.Forms.Button buttonResoudre;
        private System.Windows.Forms.GroupBox groupBoxGraphe1;
        private System.Windows.Forms.GroupBox groupBoxGraphe2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.PictureBox pictureBoxGraphe1;
        private System.Windows.Forms.PictureBox pictureBoxGraphe2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox richTextBoxAppariement;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem exploreToolStripMenuItem;
        private System.Windows.Forms.Label Graphe;
        private System.Windows.Forms.ComboBox comboBoxGraphe;
    }
}


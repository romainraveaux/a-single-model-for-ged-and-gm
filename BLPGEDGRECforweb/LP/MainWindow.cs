using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Graphs;
using Matching;

namespace LP
{
    /// <summary>
    /// L'interface graphique pour l'appariement de deux graphe
    /// </summary>
    public partial class MainWindow : Form
    {
        IsomorphismLP isoLP;

        internal IsomorphismLP IsoLP
        {
            get { return isoLP; }
            set { isoLP = value; }
        }
        public MainWindow()
        {
            InitializeComponent();
            this.isoLP = new IsomorphismLP();
            init();
        }
        private void init()
        {
            this.comboBoxGraphe.SelectedIndex = 0;
            this.comboBoxProblem.SelectedIndex = 0;
            this.comboBoxSolveur.SelectedIndex = 1;
            this.comboBoxMode.SelectedIndex = 0;
        }
        /// <summary>
        /// Charger le graphe1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chargeDuGraphe1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (this.pictureBoxGraphe1.Image != null)
                pictureBoxGraphe1.Image.Dispose();
            string nomFichier = GraphLibManager.ChoisirDirectory();
            this.isoLP.Graph1= GraphLibManager.LoadGraph(nomFichier, this.comboBoxGraphe.SelectedIndex);
            if (this.isoLP.Graph1 != null)
            {
                GraphLibManager.DisplayGraph(this.isoLP.Graph1, this.pictureBoxGraphe1, "./Graph1.jpg", toolStripStatusLabel1);
                this.isoLP.Graph1.Name = "Graph1";
                if (!this.isoLP.Graph1.IsDirected)
                {
                    this.isoLP.DirectedGraph1 = GraphLibManager.LoadGraph(nomFichier, this.comboBoxGraphe.SelectedIndex);
                    GraphLibManager.transToDirectedGraph(this.isoLP.DirectedGraph1, false);
                    this.isoLP.DirectedGraph1.Name = "Graph1";
                }
                else
                {
                    this.isoLP.DirectedGraph1 = this.isoLP.Graph1;
                }

            }           
        }
        /// <summary>
        /// charger le graphe2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chargeDuGraphe2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.pictureBoxGraphe2.Image != null)
                pictureBoxGraphe2.Image.Dispose();
           // this.isoLP.Graph2 = GraphLibManager.LoadGraph(this.richTextBoxAppariement, this.toolStripStatusLabel1);
            string nomFichier = GraphLibManager.ChoisirDirectory();
            this.isoLP.Graph2 = GraphLibManager.LoadGraph(nomFichier,comboBoxGraphe.SelectedIndex);
            if (this.isoLP.Graph2 != null)
            {
                GraphLibManager.DisplayGraph(this.isoLP.Graph2, this.pictureBoxGraphe2, "./Graph2.jpg", toolStripStatusLabel1);
                this.isoLP.Graph2.Name = "Graph2";
                if (!this.isoLP.Graph2.IsDirected)
                {
                    this.isoLP.DirectedGraph2 = GraphLibManager.LoadGraph(nomFichier,comboBoxGraphe.SelectedIndex);
                    GraphLibManager.transToDirectedGraph(this.isoLP.DirectedGraph2,false);
                    this.isoLP.DirectedGraph2.Name = "Graph2";
                }
                else
                {
                    this.isoLP.DirectedGraph2 = this.isoLP.Graph2;
                }

            }  

        }
        #region choisir la formulation
        private void isomorphismeDeSousgrapheExactF1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.comboBoxProblem.SelectedIndex = 0;
        }

        private void isomorphismeDeSousgrapheExactF2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.comboBoxProblem.SelectedIndex = 1;
        }

        private void isomorphismeDeSousgrapheInexactF1aToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.comboBoxProblem.SelectedIndex = 2;
        }

        private void isomorphismeDeSousgrapheInexactF2aToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.comboBoxProblem.SelectedIndex = 3;
        }

        private void isomorphismeDeGrapheInexactF1bToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.comboBoxProblem.SelectedIndex = 4;
        }

        private void isomorphismeDeGrapheInexactF2bToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.comboBoxProblem.SelectedIndex = 5;
        }
        #endregion
        private void cPLEXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.comboBoxSolveur.SelectedIndex = 0;
        }

        private void gLPKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.comboBoxSolveur.SelectedIndex = 1;
        }

        private void unCoeurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.comboBoxMode.SelectedIndex = 0;
        }

        private void multicoeurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.comboBoxMode.SelectedIndex = 1;
        }

        private void buttonResoudre_Click(object sender, EventArgs e)
        {
            if ((this.isoLP.Graph1 == null) || (this.isoLP.Graph2 == null))
            {
                MessageBox.Show("Chargez les 2 graphes, svp");
                return; 
            }
            else if ((this.isoLP.Graph1.IsDirected) ^ (this.isoLP.Graph2.IsDirected))
            {
                MessageBox.Show("Les graphes ne sont pas valide pour résoudre");
                return;
            }               
            this.isoLP.SolverType = this.comboBoxSolveur.SelectedIndex==0?"CPLEX":"GLPK";  
            this.isoLP.FormulaType="QAPGMGED";//Program.formulas[this.comboBoxProblem.SelectedIndex];
            this.isoLP.initial();
            if (this.comboBoxMode.SelectedIndex == 0)
                this.isoLP.Solver.setThreadNumber(1);
            else if (this.comboBoxMode.SelectedIndex == 1)
                this.isoLP.Solver.setThreadNumber(Program.nbCoeursCplex);//Multi-coeurs
            this.toolStripStatusLabel1.Text = "";

            isoLP.run(); //construire le problème linéaire et le résoudre
            isoLP.printMatchingResult(this.richTextBoxAppariement, true);
            
            isoLP.Solver.SaveProb2File(Program.binPath + "\\data\\problem.txt");
            isoLP.Solver.solutionSave(Program.binPath + "\\data\\solution.txt");  
            isoLP.Solver.closeSolver();       
        }

        private void exportDuProblèmeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (isoLP.Solver== null)
                MessageBox.Show("D'abord, il faut résoudre le problème par le solveur");
            else
            {
                try
                {
                    string outputPath = Program.binPath + "\\data\\problem.txt";
                    System.Diagnostics.Process.Start(outputPath);
                }
                catch (Exception)
                {

                    Console.Write("Erreur lors qu'ouvrir le fichier problem.txt");
                }
            }
       
        }

        private void comboBoxSolveur_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<string> modes=null;
            if (comboBoxSolveur.SelectedIndex == 0)
            {
                modes = new List<string> { "Un coeur", "Multi-coeur" };
                this.comboBoxMode.DataSource = modes;
                this.comboBoxMode.SelectedIndex = 1;
            }
            else
            {
                modes = new List<string> { "Un coeur" };
                this.comboBoxMode.DataSource = modes;
                this.comboBoxMode.SelectedIndex = 0;
 
            }     
        }
        private void exploreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            System.Diagnostics.Process.Start(path);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
           this.Close();
        }

        private void comboBoxGraphe_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBoxGraphe.SelectedIndex == 0)
            {
                if (this.isoLP.Graph1 != null) this.isoLP.Graph1.DynamicCastLabel(new LabelNum(), new LabelNum());
                if (this.isoLP.Graph2 != null) this.isoLP.Graph2.DynamicCastLabel(new LabelNum(), new LabelNum());
                if (this.isoLP.DirectedGraph1 != null) this.isoLP.DirectedGraph1.DynamicCastLabel(new LabelNum(), new LabelNum());
                if (this.isoLP.DirectedGraph2 != null) this.isoLP.DirectedGraph2.DynamicCastLabel(new LabelNum(), new LabelNum());
 
            }
            else if (comboBoxGraphe.SelectedIndex == 1)
            {
                if (this.isoLP.Graph1 != null) this.isoLP.Graph1.DynamicCastLabel(new LabelNodeGrec(), new LabelEdgeGrec());
                if (this.isoLP.Graph2 != null) this.isoLP.Graph2.DynamicCastLabel(new LabelNodeGrec(), new LabelEdgeGrec());
                if (this.isoLP.DirectedGraph1 != null) this.isoLP.DirectedGraph1.DynamicCastLabel(new LabelNodeGrec(), new LabelEdgeGrec());
                if (this.isoLP.DirectedGraph2 != null) this.isoLP.DirectedGraph2.DynamicCastLabel(new LabelNodeGrec(), new LabelEdgeGrec());

            }
            else if (comboBoxGraphe.SelectedIndex == 2)
            {
                if (this.isoLP.Graph1 != null) this.isoLP.Graph1.DynamicCastLabel(new LabelNodeMutagen(), new LabelEdgeMutagen());
                if (this.isoLP.Graph2 != null) this.isoLP.Graph2.DynamicCastLabel(new LabelNodeMutagen(), new LabelEdgeMutagen());
                if (this.isoLP.DirectedGraph1 != null) this.isoLP.DirectedGraph1.DynamicCastLabel(new LabelNodeMutagen(), new LabelEdgeMutagen());
                if (this.isoLP.DirectedGraph2 != null) this.isoLP.DirectedGraph2.DynamicCastLabel(new LabelNodeMutagen(), new LabelEdgeMutagen());

            }
        }

        private void exportDuRésultatToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (isoLP.Solver == null)
                MessageBox.Show("D'abord, il faut résoudre le problème par le solveur");
            else
            {
                try
                {
                    string outputPath = Program.binPath + "\\data\\solution.txt";
                    System.Diagnostics.Process.Start(outputPath);
                }
                catch (Exception)
                {

                    Console.Write("Erreur lors qu'ouvrir le fichier solution.txt");
                }
            }
        }

      

       

      


    }
}

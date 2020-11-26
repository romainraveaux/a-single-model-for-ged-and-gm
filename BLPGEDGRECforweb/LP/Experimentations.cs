using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Graphs;
using System.IO;
using System.Diagnostics;

namespace LP
{
    public partial class Experimentations : Form
    {
        Graph graph1, graph2;
        IsomorphismLP iso;
        String[] allFiles = null;

        int filesNum = 0;
        FileStream fs = null;
        StreamWriter monStream = null;
        List<string[]> classMap;
        string binPath;
        string csvPath;
        int graphType;
        String graphTypeStr;
        int MaxNumberNodes;
        Encoding outputEncoding;

        String dbDirecttory;
        String trainingFile;
        String readTrainingCXLFile;
        bool flagTransform;
        int nodeDebut;
        int nodeFin;
        int interval;
        
        public Experimentations()
        {
            InitializeComponent();
            this.comboBoxGraphe.SelectedIndex = 0;
            binPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            csvPath = binPath + "\\data\\result";
            outputEncoding = new UTF8Encoding(false);

            //ConsoleToFile();
        }

        private void buttonTester_Click(object sender, EventArgs e)
        {
            if (int.TryParse(this.textBoxDebut.Text, out nodeDebut))
            {
                if(nodeDebut<2) 
                { 
                    MessageBox.Show("il faut la valeur de  \"debut\" supérieur ou égale à 5");
                    return;               
                }
                if (int.TryParse(this.textBoxFin.Text, out nodeFin))
                {
                    if (nodeFin < nodeDebut)
                    {
                        MessageBox.Show("la valeur de \"fin\" pas valide ");
                        return;
                    }
                    else if (int.TryParse(this.textBoxinterval.Text, out interval))
                    {
                        if (interval < 0)
                        {
                            MessageBox.Show("la valeur d'interval pas valide ");
                            return;
                        }

                    }
                    else
                    {
                        MessageBox.Show("la valeur d'interval pas valide ");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("la valeur de  \"fin\" pas valide ");
                    return;
                }

            }
            else 
            { 
                    MessageBox.Show("la valeur de  \"debut\" pas valide ");
                    return;               
            }


            //Integer.partextBox1.Text
            
            double.TryParse(this.textBox1.Text, out Program.MAXTIME_SIMPLEX);
           Program.MAXTIME_SECOND= Program.MAXTIME_SIMPLEX / 1000.0;

           double.TryParse(this.textBox2.Text, out Program.MAXMEMORY_MB);
            
            graphType = this.comboBoxGraphe.SelectedIndex;
            if (graphType == Program.noGrec)
            {
                graphTypeStr = "Grec";
                this.MaxNumberNodes = 20;
            }

            if (graphType == Program.noLetter)
            {
                graphTypeStr = "Letter";
                this.MaxNumberNodes = 10;
            }

            if (graphType == Program.noAlkane)
            {
                graphTypeStr = "Alkane";
                this.MaxNumberNodes = 30;
            }
            
            if (graphType == Program.noMuta)
            {
                graphTypeStr = "Mutagen";
                this.MaxNumberNodes = 85;
            }


            if (graphType == Program.noPRO)
            {
                graphTypeStr = "Protein";
                this.MaxNumberNodes = 85;
            }

            if (graphType == Program.noIlpiso)
            {
                graphTypeStr = "ILPISO";
                this.MaxNumberNodes = 50;
            }




            if (graphType == Program.noLOW) graphTypeStr = "LOW";            

            if ( nodeDebut< 1) this.nodeDebut = 1;
            if (nodeFin > this.MaxNumberNodes) this.nodeFin = this.MaxNumberNodes;
            //if (interval == 0) interval = 1;
            if (allFiles == null)
            { 
                MessageBox.Show("Choisissez un dossier de graphes, svp");
            }
            else if (allFiles.Length < 1)
            {
                MessageBox.Show("Le dossier choisit n'est pas valide");
            }
            else
            {
                this.toolStripStatusTestLabel.Text = "test en cours";
                /**
                 * Generer les fichiers cxl regrouper par noeud
                 **/
                for (int noOfNodes =this.nodeDebut; noOfNodes <= this.nodeFin; noOfNodes+=this.interval)
                {
                    if (dbDirecttory != null)
                    {
                        readTrainingCXLFile = this.dbDirecttory + "\\cplex.cxl";
                    
                      
                        //donc pas de découpe en sous base
                        if (interval == 0)
                        {
                            this.trainingFile = this.dbDirecttory + "\\cplex.cxl";
                            interval = 0;
                            nodeFin = 0;
                            noOfNodes = 10000;
                        }
                        else
                        {
                            this.trainingFile = this.dbDirecttory + "\\train" + noOfNodes + ".cxl";
                            //this.trainingFile = this.dbDirecttory + "\\mixed-graphs.cxl";
                            CxlNNodes cxlNNNodes = new CxlNNodes(noOfNodes, this.graphType, this.dbDirecttory, this.trainingFile, this.readTrainingCXLFile);
                        }
                        classMap = GraphLibManager.loadGraphClass(this.trainingFile, this.dbDirecttory);
                        try
                        {
                            if (!this.checkBox7.Checked)
                            {
                                this.toolStripStatusTestLabel.Text = "";
                                fs = new FileStream(csvPath + "\\result" + this.graphTypeStr + noOfNodes + "nodes.csv", System.IO.FileMode.Create, System.IO.FileAccess.Write);
                                //monStream = new StreamWriter(fs, System.Text.UnicodeEncoding.UTF8);
                                monStream = new StreamWriter(fs,outputEncoding);
                                monStream.WriteLine("Method;Param;Graph1 Name;Graph2 Name;Graph1 nb nodes;" +
                                                     "Graph2 nb nodes;Graph1 nb edges;Graph2 nb edges; distance;" +
                                                     "explored nodes;max open size;time;feasible solution found;" +
                                                     "optimal solution found;memory overflow;time overflow;class Graph1;class Graph2;Node Matching");
                                monStream.WriteLine(classMap.Count + ";;;;;;;;;;;;;;;;;;");
                            }
                        }
                        catch (Exception OpenFileException)
                        {
                            MessageBox.Show("Impossible à écrir dans le fichier csv : " + OpenFileException.ToString());
                            return;
                        }
                        finally
                        {
                            if (monStream != null) monStream.Close();
                        }

                      //  continue;
                        try
                        {
                            this.toolStripStatusTestLabel.Text = "";
                            this.toolStripStatusTestLabel.Text = "en cours...";
                            if (this.checkBox7.Checked)
                            {
                                this.toolStripStatusTestLabel.Text = "branch and bound";
                               

                                //Create process
                                System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
                                double time = (Program.MAXTIME_SECOND * 1000);
                                pProcess.StartInfo.FileName = "java";
                               // pProcess.StartInfo.Arguments = "-Xmx1024m -jar " + binPath + "\\ressource\\EditPath.jar " +
                               //      this.dbDirecttory + " " + csvPath + "\\result" + this.graphTypeStr + noOfNodes + "nodes.csv" + " 0 " + this.graphType + " "+time+" 5 " + noOfNodes;
                                pProcess.StartInfo.Arguments = "-Xmx1024m -jar " + binPath + "\\ressource\\grapheditdistance.jar " +
                                          this.dbDirecttory + " " + csvPath + "\\result" + this.graphTypeStr + noOfNodes + "nodes.csv" + " 13 0 60000 2,6,3,100,1,3,5,3";

                                pProcess.StartInfo.UseShellExecute = false;
                                pProcess.StartInfo.RedirectStandardOutput = true;
                                pProcess.StartInfo.RedirectStandardError = true;
                                pProcess.StartInfo.CreateNoWindow = true;

                                //Start the process
                                pProcess.Start();
                                string strOutput = pProcess.StandardOutput.ReadToEnd();
                                string strError = pProcess.StandardError.ReadToEnd();

                                pProcess.WaitForExit();
                                Console.Out.WriteLine(strOutput);
                                Console.Out.WriteLine(strError);
                            }
                            if (this.checkBox1.Checked)
                            {
                                this.toolStripStatusTestLabel.Text = "F1b cplex avec un seul coeur";
                                if (!this.runTestByMethode(graphType, 1, noOfNodes))
                                    return;
                            }
                            if (this.checkBox2.Checked)
                            {
                                this.toolStripStatusTestLabel.Text = "F1b cplex avec plusieurs coeurs";
                                if (!this.runTestByMethode(graphType, 2, noOfNodes))
                                    return;

                            }
                            if (this.checkBox3.Checked)
                            {
                                this.toolStripStatusTestLabel.Text = "F2b cplex avec un seul coeur";
                                if (!this.runTestByMethode(graphType, 3, noOfNodes))
                                    return;

                            }
                            if (this.checkBox4.Checked)
                            {
                                this.toolStripStatusTestLabel.Text = "F2b cplex avec plusieurs coeurs";
                                if (!this.runTestByMethode(graphType, 4, noOfNodes))
                                    return;

                            }
                            if (this.checkBox5.Checked)
                            {
                                this.toolStripStatusTestLabel.Text = "F1b glpk avec un seul coeur";
                                if (!this.runTestByMethode(graphType, 5, noOfNodes))
                                return;

                            }
                            if (this.checkBox6.Checked)
                            {
                                this.toolStripStatusTestLabel.Text = "F2b glpk avec un seul coeur";
                                if (!this.runTestByMethode(graphType, 6, noOfNodes)) 
                                    return;
                            }
                           
                            if (this.checkBox10.Checked)
                            {
                                this.toolStripStatusTestLabel.Text = "BLPjusticehero cplex avec un seul coeur";
                                if (!this.runTestByMethode(graphType, 10, noOfNodes))
                                    return;
                            }

                             if (this.checkBox11.Checked)
                            {
                                this.toolStripStatusTestLabel.Text = "BLPjusticehero Quadratic cplex avec un seul coeur";
                                if (!this.runTestByMethode(graphType, 11, noOfNodes))
                                    return;
                            }

                             if (this.checkBox12.Checked)
                             {
                                 this.toolStripStatusTestLabel.Text = "QAPGMGED cplex avec un seul coeur";
                                 if (!this.runTestByMethode(graphType, 12, noOfNodes))
                                     return;
                             }
                        }
                        catch (Exception exce)
                        {
                            Console.Out.WriteLine("Exception : " + exce.ToString());
                        }
                        
                    }
                    else return;
                } 
                this.toolStripStatusTestLabel.Text = "Fini";
                Console.SetOut(System.IO.TextWriter.Null);
                return;
            }
        }

        private bool runTestByMethode(int graphType, int methode, int noOfNodes)
        {
            string methodeName="",solverType="", formulaType="";
            int mode=0;
            switch (methode)
            {
                case 1:
                    methodeName="F1bCplexUnCoeur";
                    solverType="CPLEX";
                    formulaType="IsoGraphInexactF1b";
                    mode=0;
                    break;
                case 2 :
                    methodeName="F1bCplexPlusieursCoeurs";
                    solverType="CPLEX";
                    formulaType="IsoGraphInexactF1b";
                    mode=1;
                    break;
                case 3 :
                    methodeName="F2bCPLEXUnCoeur";
                    solverType="CPLEX";
                    formulaType="IsoGraphInexactF2b";
                    mode=0;
                    break;
                case 4:
                    methodeName="F2bCplexPlusieursCoeurs";
                    solverType="CPLEX";
                    formulaType="IsoGraphInexactF2b";
                    mode=1;
                    break;
                case 5 :
                    methodeName="F1bGlpkUnCoeur";
                    solverType="GLPK";
                    formulaType="IsoGraphInexactF1b";
                    mode=0;
                    break;
                case 6 :
                    methodeName="F2bGlpkUnCoeur";
                    solverType="GLPK";
                    formulaType="IsoGraphInexactF2b";
                    mode=0;
                    break;
                case 7 :
                    methodeName="BranchAndBound";
                    solverType="GLPK";
                    formulaType="EditPath";
                    mode=0;
                    break;

                case 10:
                    methodeName = "GEDBLPjusticehero";
                    solverType = "CPLEX";
                    formulaType = "BLPjusticehero";
                    mode = 0;
                    break;

                case 11:
                    methodeName = "BLPjusticeheroQuadratic";
                    solverType = "CPLEX";
                    formulaType = "BLPjusticeheroQuadratic";
                    mode = 0;
                    break;

                case 12:
                    methodeName = "QAPGMGED";
                    solverType = "CPLEX";
                    formulaType = "QAPGMGED";
                    mode = 0;
                    break;

                    
               
            }
            
            string strToWrite = "";
            for (int i = 0; i < classMap.Count; i++)
            {
                for (int j = 0; j < classMap.Count; j++)
                {
                        try
                        {
                            this.graph1 = GraphLibManager.LoadGraph((classMap[i][0]), graphType);//LoadGraph(this.richTextGraph, this.toolStripStatusTestLabel);
                            this.graph2 = GraphLibManager.LoadGraph(classMap[j][0], graphType);
                            Console.Out.WriteLine("i:"+i + "---> j : " + j);
                            Console.Out.WriteLine(graph1.Id +"--->"+ graph2.Id);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show("Ne peut pas charger les graphes."+e);
                            return false;
                        }
                        
                        if ((graph1 == null) || (graph2 == null)) j++;//jump the wrong .gxl if exist
                        else if (this.graph1.IsDirected ^ this.graph2.IsDirected) j++;
                        else
                        {
                           /* if (!(this.graph1.IsDirected) && !(this.graph2.IsDirected))
                            {
                                GraphLibManager.transToDirectedGraph(this.graph1,false);
                                GraphLibManager.transToDirectedGraph(this.graph2,false);
                                this.flagTransform = true;
                            }*/
                            this.flagTransform = false;
                            this.iso = new IsomorphismLP();

                            iso.Graph1 = this.graph1;
                            iso.Graph2 = this.graph2;

                            iso.DirectedGraph1 = this.graph1;
                            iso.DirectedGraph2 = this.graph2;

                            this.iso.SolverType = solverType;//this.comboBoxSolveur2.SelectedIndex == 0 ? "CPLEX" : "GLPK";
                            this.iso.FormulaType = formulaType;// Program.formulas[formulaTypeIndice];//Program.formulas[this.comboBoxProblem2.SelectedIndex];

                            this.iso.initial();
                            Program.nbCoeursCplex = int.Parse(textBoxnbthread.Text);
                            this.iso.Solver.setThreadNumber(Program.nbCoeursCplex);
                            /*if (mode == 0)
                                this.iso.Solver.setThreadNumber(1);
                            else if (mode == 1)
                                this.iso.Solver.setThreadNumber(Program.nbCoeursCplex);*/

                            iso.run();
                            try
                            {
                                fs = new FileStream(csvPath + "\\result" + this.graphTypeStr+noOfNodes + "nodes.csv", System.IO.FileMode.Append, System.IO.FileAccess.Write);
                                monStream = new StreamWriter(fs, this.outputEncoding);
                                strToWrite = methodeName + ";rien;" + classMap[i][0] + ";" + classMap[j][0] + ";";
                                // strToWrite = methodeName + ";rien;" + graph1.Id + ";" + graph2.Id + ";";
                                strToWrite += iso.Graph1.ListNodes.Count + ";" + iso.Graph2.ListNodes.Count + ";";
                                if(this.flagTransform)
                                    strToWrite += iso.Graph1.ListEdges.Count/2 + ";" + iso.Graph2.ListEdges.Count/2 + ";";
                                else
                                strToWrite += iso.Graph1.ListEdges.Count + ";" + iso.Graph2.ListEdges.Count + ";";
                                this.flagTransform = false;
                                //strToWrite += (iso.MatchingResult.Distance.ToString()).Replace(',','.') + ";";
                                strToWrite += iso.MatchingResult.Distance + ";";
                                strToWrite += iso.MatchingResult.NbNodes + ";";
                                strToWrite += "-1;";
                                strToWrite += iso.MatchingResult.TimeUse + ";";
                                if(iso.MatchingResult.Feasible)
                                    strToWrite += "true;";
                                else strToWrite += "false;";
                                if(iso.MatchingResult.Optimal)
                                    strToWrite += "true;";
                                else strToWrite += "false;";
                                if(iso.MatchingResult.MemoryOverFlow)
                                    strToWrite +=  "true;";
                                else strToWrite += "false;";
                                if(iso.MatchingResult.TimeOverFlow)
                                strToWrite += "true;";
                                else strToWrite += "false;";
                                strToWrite += classMap[i][1] + ";";
                                strToWrite += classMap[j][1] + ";";
                                strToWrite += MakeMatchingToString(iso.MatchingResult.NodeMatchingDictionary) ;
                                monStream.WriteLine(strToWrite);

                            }
                            catch (Exception)
                            {

                                throw;
                            }
                            finally 
                            {
                                if (monStream != null) monStream.Close();
                                iso.Solver.closeSolver();
                            }  
                    }
                }
            }
            return true;
        }

        private string MakeMatchingToString(Dictionary<string, string> dictionary)
        {
            string s="";
            foreach (KeyValuePair<string, string> entry in dictionary)
            {
                // do something with entry.Value or entry.Key
                s+=entry.Key+"->"+entry.Value+"=???/";
            }
            return s;
        }


        /// <summary>
        ///  load a series of files .gxl to the table : files
        ///  chose a directory by oneself
        ///  perhaps not be used
        /// </summary>
        public int loadFilesByChoseDirectory()
        {
            try
            {
                FolderBrowserDialog monDossier = new FolderBrowserDialog();
                // monDossier.SelectedPath = binPath + "\\data";
                if (monDossier.ShowDialog() == DialogResult.OK)
                {
                    this.dbDirecttory = monDossier.SelectedPath;
                    if (File.Exists(monDossier.SelectedPath + "\\cplex.cxl"))
                    {
                        allFiles = Directory.GetFiles(@dbDirecttory, "*.gxl", SearchOption.TopDirectoryOnly);
                        return allFiles.Length;
                    }
                    else MessageBox.Show("le dossier n'est pas valide parce qu'il n'y a pas le fichier cplex.cxl");
                }
                return 0;
            }
            catch (Exception e)
            {

                Console.Write(e.ToString());
                return 0;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            
        }

        private void leFichierCsvToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //string csvPath = Directory.GetCurrentDirectory() + "\\TestResultMultiMethod.csv";//System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location+);
                System.Diagnostics.Process.Start(this.csvPath);
            }
            catch (Exception )
            {

                Console.Write("Erreur lors qu'ouvrir le fichier resultatTest.csv");
            }
            
        }

        private void checkBoxCocherTout_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBoxCocherTout.Checked)
            {
                this.checkBox1.Checked = true;
                this.checkBox2.Checked = true;
                this.checkBox3.Checked = true;
                this.checkBox4.Checked = true;
                this.checkBox5.Checked = true;
                this.checkBox6.Checked = true;
                this.checkBox7.Checked = true;
                this.checkBox8.Checked = true;
                this.checkBox9.Checked = true;
                 
            }
            else
            {
                this.checkBox1.Checked = false;
                this.checkBox2.Checked = false;
                this.checkBox3.Checked = false;
                this.checkBox4.Checked = false;
                this.checkBox5.Checked = false;
                this.checkBox6.Checked = false;
                this.checkBox7.Checked = false;
                this.checkBox8.Checked = false;
                this.checkBox9.Checked = false;
            }  
        }
        public  void ConsoleToFile()
        {
            string filePath =System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) +"\\ConsoleOut.xml";
              
            FileStream filestream = new FileStream(filePath, FileMode.Create);
            var streamwriter = new StreamWriter(filestream);
            streamwriter.AutoFlush = true;
            Console.SetOut(streamwriter);
            Console.SetError(streamwriter);
        }

        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string outputPath = binPath + "\\ConsoleOut.xml";//System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location+);
                System.Diagnostics.Process.Start(outputPath);
            }
            catch (Exception)
            {

                Console.Write("Erreur lors qu'ouvrir le fichier ConsoleOut.xml");
            }
        }
        private int countMethod()
        {
            int i = 0;
            if (this.checkBox1.Checked == true) i++;
            if (this.checkBox2.Checked == true) i++;
            if (this.checkBox3.Checked == true) i++;
            if (this.checkBox4.Checked == true) i++;
            if (this.checkBox5.Checked == true) i++;
            if (this.checkBox6.Checked == true) i++;
            if (this.checkBox7.Checked == true) i++;
            return i;
        }


        private void buttonBase_Click(object sender, EventArgs e)
        {
            filesNum = loadFilesByChoseDirectory();
            if (filesNum > 1)
                this.toolStripStatusTestLabel.Text = filesNum + " graphes selectés";

            else this.toolStripStatusTestLabel.Text = "Le dossier choisit n'est pas valide";
        }

        private void comboBoxGraphe_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBoxGraphe.SelectedIndex == Program.noGrec)
            {
                this.textBoxDebut.Text = "5";
                this.textBoxFin.Text = "20";
                this.textBoxinterval.Text = "5";
 
            }
            if (this.comboBoxGraphe.SelectedIndex == Program.noMuta)
            {
                this.textBoxDebut.Text = "10";
                this.textBoxFin.Text = "70";
                this.textBoxinterval.Text = "10";
            }

            if (this.comboBoxGraphe.SelectedIndex == Program.noLetter)
            {
                this.textBoxDebut.Text = "2";
                this.textBoxFin.Text = "10";
                this.textBoxinterval.Text = "1";
            }

            if (this.comboBoxGraphe.SelectedIndex == Program.noPRO)
            {
                this.textBoxDebut.Text = "10";
                this.textBoxFin.Text = "70";
                this.textBoxinterval.Text = "10";
            }

            if (this.comboBoxGraphe.SelectedIndex == Program.noIlpiso)
            {
                this.textBoxDebut.Text = "10";
                this.textBoxFin.Text = "50";
                this.textBoxinterval.Text = "5";
            }

            if (this.comboBoxGraphe.SelectedIndex == Program.noAlkane)
            {
                this.textBoxDebut.Text = "2";
                this.textBoxFin.Text = "10";
                this.textBoxinterval.Text = "0";
            }

            
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
           /* if (this.checkBox1.Checked || this.checkBox2.Checked || this.checkBox3.Checked || this.checkBox4.Checked || this.checkBox5.Checked || this.checkBox6.Checked)
            {
                SolverCPLEX.relaxationcontinue = true;
            }
            else this.toolStripStatusTestLabel.Text = "erreur de selection";*/

            if(SolverCPLEX.relaxationcontinue==true) {
                SolverCPLEX.relaxationcontinue=false;
            }

            if (SolverCPLEX.relaxationcontinue == false)
            {
                SolverCPLEX.relaxationcontinue = true;
            }
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
           /* if (this.checkBox1.Checked || this.checkBox2.Checked || this.checkBox3.Checked || this.checkBox4.Checked || this.checkBox5.Checked || this.checkBox6.Checked)
            {
                SolverCPLEX.UBrootnode = true;
            }
            else this.toolStripStatusTestLabel.Text = "erreur de selection";*/

            if (SolverCPLEX.UBrootnode == true)
            {
                SolverCPLEX.UBrootnode = false;
            }

            if (SolverCPLEX.UBrootnode == false)
            {
                SolverCPLEX.UBrootnode = true;
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBoxinterval_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(textBoxinterval.Text, out this.interval);
        }

       

      

    }
}

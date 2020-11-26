using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Graphs;
using Matching;
using System.Threading;
using System.Diagnostics;

namespace LP
{
    public partial class TestUneMethodeForm : Form
    {
        Graph graph1, graph2;
        IsomorphismLP iso;
        String[] files = null;
        int filesNum=0;
        bool writeToCSV = true;
        FileStream fs=null;
        StreamWriter monStream = null;
        string binPath;
        string csvPath;
        string dbdirectory;
        bool toRunEditPath;
        /**/
        public TestUneMethodeForm()
        {
            InitializeComponent();
              
        }

        private void Test_Load(object sender, EventArgs e)
        {
            this.comboBoxGraphe.SelectedIndex = 0;
            this.comboBoxProblem2.SelectedIndex = 0;
            this.comboBoxSolveur2.SelectedIndex = 0;
            this.comboBoxMode2.SelectedIndex =1;
            binPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            csvPath = binPath + "\\TestResult1.csv";
            this.dbdirectory = null;
            InitializeOutput();       
          
        }
      
       
        private void testerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buttonTester.PerformClick(); 
        }
     
        private void InitializeOutput()
        {
            RichBoxStreamWriter _writer = new RichBoxStreamWriter(this.richTextBoxResultat2);
            Console.SetOut(_writer);
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
                string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                //monDossier.SelectedPath = path + "\\gxl_files";
                if (monDossier.ShowDialog() == DialogResult.OK)
                {
                    
                    this.dbdirectory = monDossier.SelectedPath;
                    files = Directory.GetFiles(@dbdirectory, "*.gxl", SearchOption.TopDirectoryOnly);
                    return files.Length;
                }
                return 0;

            }
            catch (Exception e)
            {

                Console.Write(e.ToString());
                return 0;
            }

        }

       
        public void setDirectoryEmpty(System.IO.DirectoryInfo directory)
        {
            foreach (System.IO.FileInfo file in directory.GetFiles()) file.Delete();
            foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
        }

        private void buttonTester_Click(object sender, EventArgs e)
        {
            String methodName = this.getMethodName(this.comboBoxProblem2.SelectedIndex, this.comboBoxSolveur2.SelectedIndex, this.comboBoxMode2.SelectedIndex);
            if (methodName == null) this.writeToCSV = false;
            else this.writeToCSV = true;

            this.toolStripStatusTestLabel.Text = "";
            this.richTextBoxResultat2.Clear(); //matching results
            this.richTextGraph.Clear(); //output
            if (files == null)
            { 
                MessageBox.Show("Choisissez un dossier de graphes, svp");
            }
            else if (files.Length < 1)
            {
                MessageBox.Show("Le dossier choisit n'est pas valide");
            }
            else
            {
               
                if(writeToCSV==true)
                {
                    try
                    {
                        bool csvExist = false;
                        if(File.Exists(this.csvPath))
                        {
                            using (FileStream tmpFileStream = File.OpenRead(csvPath))
                            {
                                byte[] b = new byte[100];
                                UTF8Encoding temp = new UTF8Encoding(true);
                                if(tmpFileStream.Read(b,0,b.Length) > 0)
                                {
                                    String a = temp.GetString(b);
                                    tmpFileStream.Close();
                                    Console.WriteLine("read test : "+a);
                                    if(temp.GetString(b).StartsWith("Method;"))
                                    {
                                        Console.WriteLine("resultTest.csv exist. Mode = append");
                                        csvExist = true;
                                    }
                                    else
                                    {
                                        Console.WriteLine("le contenu du fichier resultTest.csv ne semble pas bon. Le supprimer");
                                        File.Delete(csvPath);
                                        csvExist= false;
                                    }
                                }
                            }
                        } 
                        fs = new FileStream(csvPath, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                        monStream = new StreamWriter(fs, System.Text.UnicodeEncoding.UTF8);
                        if(csvExist==false)
                        {
                            monStream.WriteLine("Method;Param;Graph1 Name;Graph2 Name;Graph1 nb nodes;Graph2 nb nodes;Graph1 nb edges;Graph2 nb edges; distance;explored nodes;max open size;time;feasible solution found;optimal solution found;memory overflow;time overflow;class graph1;class graph2");
                            monStream.WriteLine(files.Length + ";;;;;;;;;;;;;;;;;");//"------------------------------------------------------------;;;;;;;;;;;;;;;");
                        }
                    }
                    catch(Exception OpenFileException)
                    {
                        MessageBox.Show("Impossible à écrir dans le fichier resultatTest.csv : " + OpenFileException.ToString());
                        return;
                    }
                }

                this.toolStripStatusTestLabel.Text = "";
                this.progressBar1.Visible = true;
                this.progressBar1.Maximum = (files.Length) * (files.Length - 1);
                progressBar1.Minimum = 1;
                progressBar1.Value = 1;
                progressBar1.Step = 1;
                int graphType = this.comboBoxGraphe.SelectedIndex;
                String strToWrite = null;
                if (this.comboBoxProblem2.SelectedIndex == 6)
                {
                    
                    //Create process
                    System.Diagnostics.Process pProcess = new System.Diagnostics.Process();

                    pProcess.StartInfo.FileName = "java";
                    pProcess.StartInfo.Arguments = "-Xmx600m -jar " + binPath + "\\ressource\\grapheditdistance.jar " +
                         this.dbdirectory + " " + csvPath + " 0 " + graphType + " 300000 5 ";

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
                else
                {
                    try
                    {

                        for (int i = 0; i < files.Length; i++)
                        {
                            for (int j = 0; j < files.Length; j++)
                            {
                                if (i != j)
                                {
                                    progressBar1.PerformStep();
                                    this.graph1 = GraphLibManager.LoadGraph(files[i], graphType);//LoadGraph(this.richTextGraph, this.toolStripStatusTestLabel);
                                    this.graph2 = GraphLibManager.LoadGraph(files[j], graphType);
                                    if ((graph1 == null) || (graph2 == null)) j++;//jump the wrong .gxl if exist
                                    else if (this.graph1.IsDirected ^ this.graph2.IsDirected) j++;
                                    else
                                    {
                                        if (!(this.graph1.IsDirected) && !(this.graph2.IsDirected))
                                        {
                                            GraphLibManager.transToDirectedGraph(this.graph1, false);
                                            GraphLibManager.transToDirectedGraph(this.graph2, false);
                                        }

                                        this.iso = new IsomorphismLP();

                                        iso.Graph1 = this.graph1;
                                        iso.Graph2 = this.graph2;

                                        iso.DirectedGraph1 = this.graph1;
                                        iso.DirectedGraph2 = this.graph2;

                                        this.iso.SolverType = this.comboBoxSolveur2.SelectedIndex == 0 ? "CPLEX" : "GLPK";
                                        this.iso.FormulaType = Program.formulas[this.comboBoxProblem2.SelectedIndex];

                                        this.iso.initial();
                                        if (this.comboBoxMode2.SelectedIndex == 0)
                                            this.iso.Solver.setThreadNumber(1);
                                        else if (this.comboBoxMode2.SelectedIndex == 1)
                                            this.iso.Solver.setThreadNumber(Program.nbCoeursCplex);//Multi-coeurs

                                       
                                        iso.run();
                                        iso.printMatchingResult(this.richTextBoxResultat2, false); 
                                        if (writeToCSV == true)
                                        {
                                            strToWrite = methodName + ";rien;" + iso.Graph1.Id + ";" + iso.Graph2.Id + ";";
                                            strToWrite += iso.Graph1.ListNodes.Count + ";" + iso.Graph2.ListNodes.Count + ";";
                                            if (iso.Graph1.IsDirected)
                                                strToWrite += iso.Graph1.ListEdges.Count + ";" + iso.Graph2.ListEdges.Count + ";";
                                            else strToWrite += iso.Graph1.ListEdges.Count / 2 + ";" + iso.Graph2.ListEdges.Count / 2 + ";";
                                            strToWrite += iso.MatchingResult.Distance + ";";
                                            strToWrite += iso.MatchingResult.NbNodes + ";";
                                            strToWrite += "-1";
                                            strToWrite += iso.MatchingResult.TimeUse + ";";
                                            strToWrite += iso.MatchingResult.Feasible + ";";
                                            strToWrite += iso.MatchingResult.Optimal + ";";
                                            strToWrite += iso.MatchingResult.MemoryOverFlow + ";";
                                            strToWrite += iso.MatchingResult.TimeOverFlow + ";";
                                            monStream.WriteLine(strToWrite);
                                        }
                                        richTextGraph.AppendText("\n" + this.graph1.Id + " ---> " + this.graph2.Id + "\n");
                                        if (iso.MatchingResult.Optimal == true) richTextGraph.AppendText("solution optimale\n");
                                        else if (iso.MatchingResult.Feasible == true) richTextGraph.AppendText("solution réalisable\n");
                                        else richTextGraph.AppendText("ne peut pas trouver une solution réalisable\n");
                                        richTextGraph.AppendText("distance : " + iso.MatchingResult.Distance + "\n");
                                        richTextGraph.AppendText("temps d'exécution : " + iso.MatchingResult.TimeUse + "\n");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception exce)
                    {
                        Console.Out.WriteLine("Exception : " + exce.ToString());
                    }
                    finally
                    {
                        if (monStream != null) monStream.Close();
                    }

                }
                
                this.progressBar1.Visible = false;
                this.toolStripStatusTestLabel.Text = "Fini";
            }
        }
                
        /*        
                            } 
                        }
                    }
                }
                
                if (this.writeToCSV)
                {
                    FileStream fs=null;
                    StreamWriter monStream = null;
                    
                    try
                    {
                        fs = new FileStream(Directory.GetCurrentDirectory() + "\\resultatTest.csv", System.IO.FileMode.Append, System.IO.FileAccess.Write);
                        monStream = new StreamWriter(fs, System.Text.UnicodeEncoding.UTF8);
                        for (int i = 0; i < 12; i++)
                        {
                            monStream.WriteLine(strLignes[i]);
                        }
                        monStream.WriteLine();
                        
                    }
                    catch (Exception exc)
                    {
                        Console.Out.WriteLine("Exception occured when writing to csv :" +exc.ToString());
                    }
                    finally
                    {
                        
                        
                    }
                    
                    
                }
               
               
            }            
        }*/
     
        private void comboBoxSolveur2_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<string> modes = null;
            if (comboBoxSolveur2.SelectedIndex == 0)
            {
                modes = new List<string> { "Un coeur", "Multi-coeur" };
                this.comboBoxMode2.DataSource = modes;
                this.comboBoxMode2.SelectedIndex = 1;
            }
            else
            {
                modes = new List<string> { "Un coeur" };
                this.comboBoxMode2.DataSource = modes;
                this.comboBoxMode2.SelectedIndex = 0;

            }
        }

        private String getMethodName(int formulaId, int solverId, int modeId)
        {
            switch (solverId)
            {
                case 1: //GLPK
                    switch (formulaId)
                    {
                        case 4:
                            return "F1.B avec GLPK un seul cœur"; //5°) F1.B avec GLPK un seul cœur
                        case 5:
                            return "F2.B avec GLPK un seul cœur"; //6°) F2.B avec GLPK un seul cœur
                        default:
                            return null;
                    }
                case 0: //CPLEX
                    switch (formulaId)
                    {
                        case 4:
                            if (modeId == 0) return "F1.B avec CPLEX un seul coeur"; //1°) "F1.B avec CPLEX un seul coeur"
                            else return "F1.B avec CPLEX plusieurs coeurs";  //2°) "F1.B avec CPLEX plusieurs coeurs"
                        case 5:
                            if (modeId == 0) return "F2.B avec CPLEX un seul cœur"; //3°) "F2.B avec CPLEX un seul cœur"
                            else return "F2.B avec CPLEX plusieurs cœurs";  //4°) "F2.B avec CPLEX plusieurs cœurs"
                        default:
                            return null;
                    }
                default:
                    return "Branch and Bound";
            }
        }

       
        private void explorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            System.Diagnostics.Process.Start(path);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void exportDuRésultatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string outputPath = csvPath;//System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location+);
                System.Diagnostics.Process.Start(outputPath);
            }
            catch (Exception)
            {
                MessageBox.Show("aucun fichier scv");
                
            }
        }

        private void buttonBase_Click(object sender, EventArgs e)
        {
            filesNum = loadFilesByChoseDirectory();
            if (filesNum > 1)
                this.toolStripStatusTestLabel.Text = filesNum + " graphes selectés";
            else this.toolStripStatusTestLabel.Text = "Le dossier choisit n'est pas valide";
        }


       
    }
}

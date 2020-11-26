using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Graphs;
using Matching;

namespace LP
{
    public static class Program
    {
        public static string[] formulas = new string[7] { "IsoSousGraphExactF1", "IsoSousGraphExactF2", "IsoSousGraphInexactF1a", "IsoSousGraphInexactF2a", "IsoGraphInexactF1b", "IsoGraphInexactF2b", "EditPath" };
        // public static int MAXTIME = 300000;//the max time for brand and cut each (GLPK)
        public static double MAXTIME_SIMPLEX = 28000;//300000;//10000;//60000;//300000; //the max time for simplex (GLPK)
        public static double MAXTIME_SECOND = 28;//10;//300;
        public static double MAXMEMORY_MB = 8024;//the maxi memory in MB
        public static readonly int noGrec = 0;
        public static readonly int noMuta = 1;
        public static readonly int noLetter = 2;
        public static readonly int noPRO = 3;
        public static readonly int noIlpiso = 4;
        public static readonly int noLOW = 6;
        public static readonly int noAlkane = 5;

        public static int nbCoeursCplex = 24;
        public static readonly string binPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        /// <summary>
        /// The main entry point for the application.
        /// You need to provide the file name of graph g1
        /// You need to provide the file name of graph g2
        /// </summary>
        /* [STAThread]*/
        static void Main(string[] args)
        {

            string g1file = (args[0]);
            string g2file = (args[1]);


            onecomparison(g1file, g2file);          


        }


        private static void manycomparison(string[] args)
        {


            //CxlNNodes cxlNNNodes = new CxlNNodes(noOfNodes, this.graphType, this.dbDirecttory, this.trainingFile, this.readTrainingCXLFile);
            string dbDirecttory = "D:\\recherche\\data\\GREC\\GREC\\data";
            List<string[]> classMap = GraphLibManager.loadGraphClass("D:\\recherche\\data\\GREC\\GREC\\data\\cplex.cxl", dbDirecttory);


            System.IO.StreamWriter monStream = null;
            for (int i = 0; i < classMap.Count; i++)
            {
                for (int j = 0; j < classMap.Count; j++)
                {
                    try
                    {


                        Graph g1 = new Graph();
                        Graph g2 = new Graph();
                        string g1file = classMap[i][0];
                        string g2file = classMap[j][0];
                        string res = onecomparisonwithres(g1file, g2file);

                        System.IO.FileStream fs = new System.IO.FileStream("res.csv", System.IO.FileMode.Append, System.IO.FileAccess.Write);
                        monStream = new System.IO.StreamWriter(fs);
                        monStream.WriteLine(res);

                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    finally
                    {
                        if (monStream != null) monStream.Close();

                    }
                }
            }


        }


        private static string onecomparisonwithres(string g1file, string g2file)
        {

            string result = "" + g1file + ";" + g2file;
            Graph g1 = new Graph();
            g1.loadGXL(g1file);


            Graph g2 = new Graph();
            g2.loadGXL(g2file);


            g1.DynamicCastLabel(new LabelNodeGrecConstest(), new LabelEdgeGrecContest());
            g2.DynamicCastLabel(new LabelNodeGrecConstest(), new LabelEdgeGrecContest());

            IsomorphismLP iso = new IsomorphismLP();

            iso.Graph1 = g1;
            iso.Graph2 = g2;

            iso.DirectedGraph1 = g1;
            iso.DirectedGraph2 = g2;

            iso.SolverType = "CPLEX"; //this.comboBoxSolveur2.SelectedIndex == 0 ? "CPLEX" : "GLPK";
            iso.FormulaType = "QAPGMGED";//"IsoGraphInexactF2b";//"BLPjusticehero";//"IsoGraphInexactF1b";// Program.formulas[formulaTypeIndice];//Program.formulas[this.comboBoxProblem2.SelectedIndex];

            TimeSpan start = new TimeSpan(DateTime.Now.Ticks);

            iso.initial();
            //Program.nbCoeursCplex = int.Parse(textBoxnbthread.Text);
            iso.Solver.setThreadNumber(1);
            //SolverCPLEX.UBrootnode = true;
            iso.run();
            TimeSpan end = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan usingtime = end.Subtract(start).Duration();
            double timeUse = usingtime.TotalSeconds;
            int optsol = 2;

            if (iso.MatchingResult.Optimal) optsol = 1;
            if (iso.MatchingResult.Optimal == false) optsol = 0;

            Console.Write(optsol + ";");
            result += optsol + ";";
            Console.Write(timeUse.ToString("N8") + ";");
            result += timeUse.ToString("N8") + ";";
            Dictionary<string, string> NM = iso.MatchingResult.NodeMatchingDictionary;
            foreach (Node n in g1.ListNodes)
            {
                if (NM.ContainsKey("" + n.Id) == true)
                {
                    Console.Write(NM["" + n.Id] + " ");
                    result += NM["" + n.Id] + " ";
                }
                else
                {
                    Console.Write("-1 ");
                    result += "-1 ";
                }
            }
            Console.Write(";" + iso.MatchingResult.Distance);
            Console.WriteLine();
            result += ";" + iso.MatchingResult.Distance;
            return result;

        }

        private static void onecomparison(string g1file, string g2file)
        {


            
            Graph g1 = new Graph();
            g1.loadGXL(g1file);


            Graph g2 = new Graph();
            g2.loadGXL(g2file);


            g1.DynamicCastLabel(new LabelNodeGrecConstest(), new LabelEdgeGrecContest());
            g2.DynamicCastLabel(new LabelNodeGrecConstest(), new LabelEdgeGrecContest());

            IsomorphismLP iso = new IsomorphismLP();

            iso.Graph1 = g1;
            iso.Graph2 = g2;

            iso.DirectedGraph1 = g1;
            iso.DirectedGraph2 = g2;

            iso.SolverType = "CPLEX"; //this.comboBoxSolveur2.SelectedIndex == 0 ? "CPLEX" : "GLPK";
            iso.FormulaType = "QAPGMGED";//"IsoGraphInexactF2b";//"BLPjusticehero";//"IsoGraphInexactF1b";// Program.formulas[formulaTypeIndice];//Program.formulas[this.comboBoxProblem2.SelectedIndex];
            TimeSpan start = new TimeSpan(DateTime.Now.Ticks);
            iso.initial();
            //Program.nbCoeursCplex = int.Parse(textBoxnbthread.Text);
            iso.Solver.setThreadNumber(1);
            //SolverCPLEX.UBrootnode = true;
            iso.run();
            TimeSpan end = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan usingtime = end.Subtract(start).Duration();
            double timeUse = usingtime.TotalSeconds;
            int optsol = 2;

            if (iso.MatchingResult.Optimal) optsol = 1;
            if (iso.MatchingResult.Optimal == false) optsol = 0;

            Console.Write(optsol + ";");
            Console.Write(timeUse.ToString("N8") + ";");
            Dictionary<string, string> NM = iso.MatchingResult.NodeMatchingDictionary;
            foreach (Node n in g1.ListNodes)
            {
                if (NM.ContainsKey("" + n.Id) == true)
                {
                    Console.Write(NM["" + n.Id] + " ");
                }
                else
                {
                    Console.Write("-1 ");
                }
            }
            Console.Write(";" + iso.MatchingResult.Distance);
            Console.WriteLine();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            
        }
    }
}



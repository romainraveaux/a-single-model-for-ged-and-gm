using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphs;
using System.Windows.Forms;
using Matching;
using System.Runtime.InteropServices;
using ILOG.Concert;
using ILOG.CPLEX;
using System.IO;


namespace LP
{
    public class SolverCPLEX : ISolver
    {
        //Pour obtenir le treeNode pour l'instant.
        /*  internal class MyCallback : Cplex.MIPCallback
          {
              public override void Main()
              {
                  int tempTreeNode = this.GetNnodes() - SolverCPLEX.nnode + this.GetNremainingNodes();
                  SolverCPLEX.nnode = this.GetNnodes();
                  if (tempTreeNode > SolverCPLEX.treeNode)
                      SolverCPLEX.treeNode = tempTreeNode;
              }
          }*/


        internal class MyCallback : ILOG.CPLEX.Cplex.BranchCallback
        {

            public override void Main()
            {
                if (this.GetNnodes() == 0)
                {
                    //System.Console.WriteLine("UB=" + this.GetBestObjValue());
                    //System.Console.WriteLine("LB=" + this.GetObjValue());
                    SolverCPLEX.flagsolveur = true;
                    SolverCPLEX.bestsol=this.GetObjValue();
                }
               
            }
        }

        // CPLEX problem;
        Cplex cplex;
        ILPMatrix ilpMatrix;
        double[] objCoef;
        int threadNumber = -1;
        public static int nnode = 0;
        public static int treeNode = 0;
        double timeUse = -1;

        public SolverCPLEX()
        {
        }

        public void setProb(Problem prob)
        {
            if (prob is ProblemCPLEX)
            {
                this.cplex = ((ProblemCPLEX)prob).cplex;
                this.ilpMatrix = ((ProblemCPLEX)prob).ilpMatrix;
                this.objCoef = ((ProblemCPLEX)prob).objCoef;
            }
            else
            {
                this.cplex = ((ProblemCPLEXUndirected)prob).cplex;
                this.ilpMatrix = ((ProblemCPLEXUndirected)prob).ilpMatrix;
                this.objCoef = ((ProblemCPLEXUndirected)prob).objCoef;
            }
           
        }
        public bool solveProb()
        {
            try
            {
                // Solve initial problem with the auto algorithm                
                cplex.SetParam(Cplex.IntParam.RootAlg, Cplex.Algorithm.Auto);
                cplex.SetParam(Cplex.DoubleParam.TiLim, Program.MAXTIME_SECOND);               
                cplex.SetParam(Cplex.IntParam.ClockType, 2);
                cplex.SetParam(Cplex.DoubleParam.TreLim, Program.MAXMEMORY_MB);
                //cplex.SetOut(null);
               
                //cplex.SetParam(Cplex.IntParam.MIPDisplay, 5);//display all details
                cplex.SetParam(Cplex.IntParam.ParallelMode, Cplex.ParallelMode.Deterministic);
                cplex.SetParam(Cplex.IntParam.Threads, this.threadNumber);
                cplex.SetOut(null);// ; setOut(env.getNullStream());
                if (relaxationcontinue == true)
                {


                    MyCallback c = new MyCallback();
                    cplex.Use((ILOG.CPLEX.Cplex.ControlCallback)c);
                    cplex.SetParam(Cplex.Param.MIP.Limits.Nodes, (long)1);

                }


                if (UBrootnode == true)
                {
                                      
                    cplex.SetParam(Cplex.Param.MIP.Limits.Nodes, (long)0);

                }


                TimeSpan start = new TimeSpan(DateTime.Now.Ticks);

                flagsolveur = false;
                bool resSolve = cplex.Solve();
                TimeSpan end = new TimeSpan(DateTime.Now.Ticks);
                TimeSpan usingtime = end.Subtract(start).Duration();
                this.timeUse = usingtime.TotalMilliseconds;

                if (resSolve) //A Boolean value reporting whether a  feasible solution has been found.
                {
                    //Console.Out.WriteLine(cplex.Nnodes);
                    return true;
                }
            }
            catch (ILOG.Concert.Exception e)
            {
                System.Console.WriteLine("Concert exception '" + e + "' caught");//A Modifier
            }
            return false;
        }


        /// <summary>
        ///  get the matchingResult
        /// </summary>
        /// <param name="MatchingNodes"></param>
        /// <param name="ResultText"></param>
        public bool getMatchingResult(MatchingResult matchingResult)
        {
            matchingResult.cplx = this.cplex;
           
            matchingResult.Feasible = true;
            matchingResult.TimeUse = this.timeUse;
            //System.Console.WriteLine("Solution status = " + cplex.GetCplexStatus());
            //System.Console.WriteLine("Solution value  = " + cplex.ObjValue);
            //string[] te = cplex.GetCplexStatus().ToString().Split(new string[] { "Best Bound" }, StringSplitOptions.None);//[1].Split(new string[] { "        " }, StringSplitOptions.None)[6];
          
            int nbCols = this.ilpMatrix.NumVars.Count();
            //this.SaveProb2File("./prob.txt");
            //this.solutionSave("./sol.txt");


            Cplex.CplexStatus status = cplex.GetCplexStatus();
            matchingResult.NbNodes = cplex.Nnodes;
            matchingResult.MaxTreeNodes = treeNode;
            if (status.Equals(Cplex.CplexStatus.MemLimFeas) || status.Equals(Cplex.CplexStatus.MemLimInfeas))
            {
                matchingResult.MemoryOverFlow = true;
                //Console.Out.WriteLine("Feasible");
            }
            if (status.Equals(Cplex.CplexStatus.AbortTimeLim))
            {
                matchingResult.TimeOverFlow = true;
            }

            Cplex.Status status2 = cplex.GetStatus();
            if (status2.Equals(Cplex.Status.Optimal))
            {
                matchingResult.Optimal = true;
                //Console.Out.WriteLine("optimal");
            }
            if (status2.Equals(Cplex.Status.Feasible))
            {
                matchingResult.Feasible = true;
                //Console.Out.WriteLine("Feasible");
            }
            if (status2.Equals(Cplex.Status.Infeasible))
            {
                matchingResult.Feasible = false;
               // Console.Out.WriteLine("no Feasible solution"); //jamais ici
            }
          
            double ress;



            if (relaxationcontinue == true)
            {
                matchingResult.Optimal = false;
                matchingResult.Feasible = false;
            }

            if (flagsolveur == false)
            {
                bestsol = cplex.ObjValue;
            }
           
            double[] sol = cplex.GetValues(ilpMatrix);
           
            string valName;
            for (int i = 0; i < nbCols; i++)
            {
                ress = sol[i];
                valName = ilpMatrix.NumVars[i].Name;
                 // System.Console.WriteLine("LB  = " + this.ilpMatrix.NumVars[i].LB);

                if ((ress - 1 < 0.0001) && (ress - 1 > -0.0001))
                {
                    if (valName.StartsWith("x_"))
                    {
                        KeyValuePair<string, string> pair = seperateValName(valName);
                        matchingResult.NodeMatchingDictionary.Add(pair.Key, pair.Value);


                    }
                    else if (valName.StartsWith("y_"))
                    {
                        KeyValuePair<string, string> pair = seperateValName(valName);
                        if (!pair.Key.StartsWith("copy_"))
                        {
                            if (pair.Value.StartsWith("copy_"))
                            {
                                matchingResult.EdgeMatchingDictionary.Add(pair.Key, pair.Value.Substring(5));
                            }
                            else
                            {
                                matchingResult.EdgeMatchingDictionary.Add(pair.Key, pair.Value);
                            }
                        }
                    }
                    else if (valName.StartsWith("u_"))
                    {
                        KeyValuePair<string, string> pair = seperateValName(valName);
                        matchingResult.NodeMatchingDictionary.Add(pair.Key, pair.Value);

                    }
                    else if (valName.StartsWith("e_"))
                    {
                        KeyValuePair<string, string> pair = seperateValName(valName);
                        if (!pair.Key.StartsWith("copy_"))
                            matchingResult.EdgeMatchingDictionary.Add(pair.Key, pair.Value);
                    }

                    else if (valName.StartsWith("v_"))
                    {
                        KeyValuePair<string, string> pair = seperateValName(valName);
                        matchingResult.NodeMatchingDictionary.Add(pair.Key, pair.Value);

                    }
                    else if (valName.StartsWith("f_"))
                    {
                        KeyValuePair<string, string> pair = seperateValName(valName);
                        if (!pair.Key.StartsWith("Del_copy_"))
                            matchingResult.EdgeMatchingDictionary.Add(pair.Key, pair.Value);
                    }
                }

            }
            if (bestsol < 0.000001 && bestsol > -0.000001) bestsol = 0;
            matchingResult.Distance = bestsol;
            return true;
        }

        /// <summary>
        ///  small tool function. Analysing the name of a variable
        ///  for exemple: valName="x_node1,node3", the function return a table of 2 elements: "node1", "node3"
        /// </summary>
        /// <param name="valName"></param>
        private KeyValuePair<string, string> seperateValName(string valName)
        {
            string[] result;
            KeyValuePair<string, string> pair;
            valName = valName.Substring(2);
            result = valName.Split(',');
            if (Problem.exchange == false) pair = new KeyValuePair<string, string>(result[0], result[1]);
            else pair = new KeyValuePair<string, string>(result[1], result[0]);
            return pair;
        }

        /// <summary>
        ///  save the problem in CPLEX format into a text file.
        /// </summary>
        /// <param name="fname"></param>
        public int SaveProb2File(string fname)
        {

            FileStream fs = new FileStream(fname, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            //monStream = new StreamWriter(fs, System.Text.UnicodeEncoding.UTF8);
            StreamWriter monStream = new StreamWriter(fs, System.Text.UnicodeEncoding.UTF8);
            monStream.WriteLine("Cplex Problem");
            for (int i = 0; i < ilpMatrix.Ncols; i++)
                monStream.WriteLine(ilpMatrix.NumVars[i].LB + "<=" + ilpMatrix.NumVars[i].Name + "<=" + ilpMatrix.NumVars[i].UB);
            for (int i = 0; i < ilpMatrix.Ranges.Count(); i++)
                monStream.WriteLine(ilpMatrix.Ranges[i]);
            monStream.Close();
            return 1;
        }

        /// <summary>
        ///  save the solve result into a text file.
        /// </summary>
        /// <param name="fname"></param>
        public int solutionSave(string fname)
        {
            double[] sol = cplex.GetValues(ilpMatrix);
            FileStream fs = new FileStream(fname, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            //monStream = new StreamWriter(fs, System.Text.UnicodeEncoding.UTF8);
            StreamWriter monStream = new StreamWriter(fs, System.Text.UnicodeEncoding.UTF8);
            monStream.WriteLine("Cplex Solution");
            for (int i = 0; i < ilpMatrix.Ncols; i++)
                monStream.WriteLine(ilpMatrix.NumVars[i].Name + ":" + sol[i]);
            monStream.Close();

            return 1;
        }
        public static int GetIndexByName(INumVar[] x, String nameVar)
        {
            for (int ind = 0; ind < x.Length; ind++)
            {
                if (x[ind].Name.Equals(nameVar))
                    return ind;
            }
            return -1;
        }

        /// <summary>
        ///  close the Cplex
        /// </summary>

        public void setThreadNumber(int i)
        {
            if (i == 1)
                this.threadNumber = 1;
            else this.threadNumber = i;
        }
        public void closeSolver()
        {
            cplex.End();
        }

        static public double bestsol;// { get; set; }
      
        public static bool flagsolveur=false;
        public static bool relaxationcontinue = false;
        public static bool UBrootnode = false;
    }

    
}

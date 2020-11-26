using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using Graphs;
using Matching;

namespace LP
{
    /// <summary>
    /// cette classe represente un appariement. 
    /// Elle emballe trois objets ayant chacun un rôle différent : « Problèm », « Solveur », et « Résultat ».
    /// </summary>
    public class IsomorphismLP
    {
        #region Attributes
        Graph graph1;
        Graph graph2;
        Graph directedGraph1;
        Graph directedGraph2;
        Problem prob;
        ISolver solver;
        MatchingResult matchingResult;

        string solverType;
        string formulaType;

        #endregion 

        #region Getters and setters
       
        public Graph DirectedGraph1
        {
            get { return directedGraph1; }
            set { directedGraph1 = value; }
        }
        public Graph DirectedGraph2
        {
            get { return directedGraph2; }
            set { directedGraph2 = value; }
        }
        public Graph Graph1
        {
            get { return graph1; }
            set { graph1 = value; }
        }
        
        public Graph Graph2
        {
            get { return graph2; }
            set { graph2 = value; }
        }
        
        public string FormulaType
        {
            get { return formulaType; }
            set { formulaType = value; }
        }

        public string SolverType
        {
            get { return solverType; }
            set { solverType = value; }
        }
        public Problem Prob
        {
            get { return prob; }
            set { prob = value; }
        }
        public ISolver Solver
        {
            get { return solver; }
            set { solver = value; }
        }
        public MatchingResult MatchingResult
        {
            get { return matchingResult; }
            set { matchingResult = value; }
        }
        #endregion

        public IsomorphismLP()
        {
            graph1 = null;
            graph2 = null;
            directedGraph1 = null;
            directedGraph2 = null;
           
        }
        public IsomorphismLP(Graph g1,Graph g2)
        {
            graph1 = g1;
            graph2 = g2;
            directedGraph1 = g1;
            directedGraph2 = g2;
        }
        public void initial()
        {
           try 
	        {	 
                solver = (ISolver)(Activator.CreateInstance(Type.GetType("LP.Solver" + this.solverType)));
                if (this.solverType == "GLPK")
                {
                    prob = new ProblemGLPK(this.directedGraph1, this.directedGraph2);
                }
                else
                {
                    if (this.graph1.IsDirected == true)
                    {
                        prob = new ProblemCPLEX(this.directedGraph1, this.directedGraph2);
                    }
                    else
                    {
                        prob = new ProblemCPLEXUndirected(this.directedGraph1, this.directedGraph2);
                    }
                   // prob = new ProblemCPLEX(this.directedGraph1, this.directedGraph2);
                    
                }
                matchingResult = new MatchingResult();
	        }
	        catch (Exception e)
	        {
		        Console.Write( e.ToString());
	        }            
           
        }
        /// <summary>
        /// affichier les résultats d'appariement dans le RichTextBox
        /// </summary>
        /// <param name="ResultText"></param>
        /// <param name="clearOrNo">indiquer si on veut déblayer le richtextbox ou pas</param>
        public void printMatchingResult(RichTextBox ResultText, bool clearOrNo)
        {
            if(clearOrNo)ResultText.Clear();
            ResultText.AppendText("\n" + this.graph1.Id + " ---> " + this.graph2.Id + "\n");
            ResultText.AppendText("Temps d'exécution : " + this.MatchingResult.TimeUse.ToString() + "\n");
            if (this.matchingResult == null) ResultText.AppendText("///No Solution///");
            else
            {
                ResultText.AppendText("Distance : " + this.matchingResult.Distance.ToString());
                if (this.matchingResult.NodeMatchingDictionary.Count == 0)
                    ResultText.AppendText("\n///No Node Mathing///\n");
                else
                {
                    ResultText.AppendText("\n///Node Mathing///\n");
                    foreach (KeyValuePair<string, string> kvp in this.matchingResult.NodeMatchingDictionary)
                    {
                        //ResultText.AppendText("\n("+this.graph1.Name+") "+ kvp.Key.Key+"  -->  ("+this.graph2.Name + ") "+kvp.Key.Value+" = "+  kvp.Value.ToString()+"\n");
                        ResultText.AppendText("" + kvp.Key + "  -->  " + kvp.Value + " \n");
                    }
                }
                if (this.matchingResult.EdgeMatchingDictionary.Count == 0)
                    ResultText.AppendText("\n///No Edge Mathing///\n");
                else
                {
                    ResultText.AppendText("///Edge Mathing///\n");
                    foreach (KeyValuePair<string, string> kvp in this.matchingResult.EdgeMatchingDictionary)
                    {
                        ResultText.AppendText("" + kvp.Key + "  -->  " + kvp.Value + " \n");
                    }
                }
            }            
        }
        /// <summary>
        /// Construir le problème et après le résoudre
        /// </summary>
        public void run()
        {
            //Construir le problème linéaire
            this.Prob.constructProb(this.FormulaType);
            this.solve();
           
        }

       public bool solve()
       {
           solver.setProb(prob);
           if (solver.solveProb())
           {
               if (solver.getMatchingResult(this.matchingResult))
                   return true;
           }
           else matchingResult.Feasible = false;
           return false;
       }

       /// <summary>
       /// Cette méthode est la méthode "editPath" de matching.dll. Son corretement est à voir.
       /// pour l'instant, on utilise pas ça. 
       /// on utilse la méthode branch and bound de code java que Monsieur Raveaux fournis.
       /// </summary>
       /// <param name="graph1"></param>
       /// <param name="graph2"></param>
       public void runEditGraph(Graph graph1, Graph graph2)
       {

           if (graph1 != null && graph2 != null)
           {
               GraphEditDistance graphEditDistance = new GraphEditDistance();
               EditPath Result;
               TimeSpan start = new TimeSpan(DateTime.Now.Ticks); 
               Result = graphEditDistance.EditDistance(graph1, graph2);
               TimeSpan end = new TimeSpan(DateTime.Now.Ticks);
               TimeSpan usingtime = end.Subtract(start).Duration();
               matchingResult.TimeUse= usingtime.TotalMilliseconds;
               matchingResult = new MatchingResult();
               matchingResult.NodeMatchingDictionary = new Dictionary<string, string>();
               matchingResult.EdgeMatchingDictionary = new Dictionary<string, string>();
               getNodeOperations(Result);
               getEdgeOperations(Result);
               matchingResult.Distance = Result.Cost;
               matchingResult.Optimal = true;
               matchingResult.Feasible = true;
           }

       }
       /// <summary>
       /// méthode d'outil. 
       /// </summary>
       /// <param name="ep"></param>
       public void getNodeOperations(EditPath ep)
       {
            
            foreach (Node[] myOpe in ep.Operations)
            {
                if (myOpe[0] != null && myOpe[1] != null)
                    this.matchingResult.NodeMatchingDictionary.Add(myOpe[0].Id,myOpe[1].Id);
                else if (myOpe[0] == null)
                    this.matchingResult.NodeMatchingDictionary.Add("Del_" + myOpe[1].Id, myOpe[1].Id);
               
                else
                    this.matchingResult.NodeMatchingDictionary.Add(myOpe[0].Id, "Ins_" + myOpe[0].Id);
            }
            
        }

       /// <summary>
        /// méthode d'outil. Return a string of the edgeOperation List 
        /// </summary>
        /// <returns></returns>
       public void getEdgeOperations(EditPath ep)
       {
           foreach (Edge[] myOpe in ep.EdgeOperations)
           {
               if (myOpe[0] != null && myOpe[1] != null)
                   this.matchingResult.EdgeMatchingDictionary.Add(myOpe[0].Id, myOpe[1].Id);
               else if (myOpe[0] == null)
                   this.matchingResult.EdgeMatchingDictionary.Add("Del_" + myOpe[1].Id, myOpe[1].Id);
                   
               else
                   this.matchingResult.EdgeMatchingDictionary.Add(myOpe[0].Id, "Ins_" + myOpe[0].Id);
           }
       }
       /// <summary>
       /// Calculate the distance between two edges
       /// </summary>
       /// <param name="_Edge1"></param>
       /// <param name="_Edge2"></param>
       /// <returns>The distance between this two edges</returns>
       private double distanceEdge(Edge _Edge1, Edge _Edge2)
       {
           return _Edge1.Label.dissimilarity(_Edge2.Label);
       }
       /// <summary>
       /// Calculate the distance between two nodes
       /// </summary>
       /// <param name="_Node1"></param>
       /// <param name="_Node2"></param>
       /// <returns>The distance between this two nodes</returns>
       private double distanceNode(Node _Node1, Node _Node2)
        {
            return _Node1.Label.dissimilarity(_Node2.Label);
        }
       /// <summary>
        /// on crée un nouveau thread pour évaluer la mémoire utilisé par la programme.
        /// mais pour l'instant on l'utilise pas. Parce que le solveur a son propre moyen de obtenir son cosommation de mémoire
        /// </summary>
       /*
        public void memoryUseByProc()
        {
            try
            {
                while (true)
                {
                    if (threadFinish == true) break;
                    if (Process.GetCurrentProcess() != null)
                    {
                        long memLong = Process.GetCurrentProcess().WorkingSet64;
                        if (memLong > Program.MAXMEMORY_MB*1024*1024)
                            this.matchingResult.MemoryOutFlow = true;
                        if ( this.matchingResult.MemoryUse < memLong)
                            this.matchingResult.MemoryUse = memLong;
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The following exception was raised: ");
                Console.WriteLine(e.Message);
            }
        }*/

    }
}

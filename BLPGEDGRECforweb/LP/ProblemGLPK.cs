using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using Graphs;
using Matching;

namespace LP
{
    public unsafe class ProblemGLPK : Problem
    {
        
        public double* lp;
        bool copyEdge;
        protected struct ConstraintMatrix
        {
            public int[] ia;
            public int[] ja;
            public double[] ar;
        }
        public ProblemGLPK(Graph g1,Graph g2)
        {
            this.graph1 = g1;
            this.graph2 = g2;
            lp = SolverGLPK.glp_create_prob();
            SolverGLPK.glp_set_prob_name(lp, this.graph1.Id + "-->" + this.graph2.Id);
            Console.Out.WriteLine(this.graph1.Id + "-->" + this.graph2.Id);
            exchange = false;
            if (this.graph1.IsDirected)
                this.copyEdge = false;
            else copyEdge = true;
        }

        public override void IsoSousGraphExactF1()
        {
            //rapport LeBodic P7
            Graph smallGraph, bigGraph;
            if ((this.graph1.ListNodes.Count() <= this.graph2.ListNodes.Count()) && (this.graph1.ListEdges.Count() <= this.graph2.ListEdges.Count()))
            {
                smallGraph = this.graph1;
                bigGraph = this.graph2;
                exchange = false;
            }
            else if ((this.graph1.ListNodes.Count() >= this.graph2.ListNodes.Count()) && (this.graph1.ListEdges.Count() >= this.graph2.ListEdges.Count()))
            {
                smallGraph = this.graph2;
                bigGraph = this.graph1;
                exchange = true;
            }
            else throw new System.Exception("can't be solved by F1");

            SolverGLPK.glp_set_prob_name(lp, smallGraph.Name + " --> " + bigGraph.Name + " : F1");
            
            //for storing the value of the constraint matrix
            List<int> iaList = new List<int>();
            List<int> jaList = new List<int>();
            List<double> arList = new List<double>();
            iaList.Add(0);
            jaList.Add(0);
            arList.Add(0);

            int rownum = 0;
            int colnum = 0;

            int numNodes1 = smallGraph.ListNodes.Count();
            int numNodes2 = bigGraph.ListNodes.Count();
            int numEdges1 = smallGraph.ListEdges.Count();
            int numEdges2 = bigGraph.ListEdges.Count();

            //construct the objet funcion and the contraints.
            //the objet funcion
            for (int i = 1; i <= numNodes1; i++)
            {
                for (int k = 1; k <= numNodes2; k++)
                {
                    SolverGLPK.glp_add_cols(lp, 1);
                    ++colnum;
                    SolverGLPK.glp_set_col_kind(lp, colnum, SolverGLPK.GLP_BV);
                    SolverGLPK.glp_set_col_name(lp, colnum, ("x_" + smallGraph.ListNodes[i - 1].Id + "," + bigGraph.ListNodes[k - 1].Id));
                    SolverGLPK.glp_set_obj_coef(lp, colnum, smallGraph.ListNodes[i - 1].Label.dissimilarity(bigGraph.ListNodes[k - 1].Label));

                }
            }

            for (int ij = 1; ij <= numEdges1; ij++)
                for (int kl = 1; kl <= numEdges2; kl++)
                {
                    SolverGLPK.glp_add_cols(lp, 1);
                    SolverGLPK.glp_set_col_name(lp, ++colnum, ("y_" + smallGraph.ListEdges[ij - 1].Id + "," + bigGraph.ListEdges[kl - 1].Id));
                    SolverGLPK.glp_set_col_kind(lp, colnum, SolverGLPK.GLP_BV);
                    double costEdge = smallGraph.ListEdges[ij - 1].Label.dissimilarity(bigGraph.ListEdges[kl - 1].Label);
                    if (this.copyEdge) SolverGLPK.glp_set_obj_coef(lp, colnum, costEdge / 2);
                    else SolverGLPK.glp_set_obj_coef(lp, colnum, costEdge);

                }
            
            SolverGLPK.glp_create_index(lp);


            // contraint: equation 2
            for (int i = 1; i <= numNodes1; i++)
            {
                SolverGLPK.glp_add_rows(lp, 1);
                SolverGLPK.glp_set_row_name(lp, ++rownum, "equation2_" + rownum);
                SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_FX, 1, 1);
                for (int k = 1; k <= numNodes2; k++)
                {
                    iaList.Add(rownum);
                    jaList.Add((i - 1) * numNodes2 + k);
                    arList.Add(1);
                }
                             
            }


            // equation 3
            for (int ij = 1; ij <= numEdges1; ij++)
            {
                SolverGLPK.glp_add_rows(lp, 1);
                SolverGLPK.glp_set_row_name(lp, ++rownum, "equation3_" + rownum);
                SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_FX, 1, 1);
                for (int kl = 1; kl <= numEdges2; kl++)
                {

                    iaList.Add(rownum);
                    jaList.Add(numNodes1 * numNodes2 + (ij - 1) * numEdges2 + kl);
                    arList.Add(1);
                }
               

            }

            //equation 4 
            for (int k = 1; k <= numNodes2; k++)
            {
                SolverGLPK.glp_add_rows(lp, 1);
                SolverGLPK.glp_set_row_name(lp, ++rownum, "equation4_" + rownum);
                SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_DB, 0, 1);
                for (int i = 1; i <= numNodes1; i++)
                {
                    iaList.Add(rownum);
                    jaList.Add(numNodes2 * (i - 1) + k);
                    arList.Add(1);
                }
            }

            //equation 5 6 7

            for (int ij = 1; ij <= numEdges1; ij++)
                for (int kl = 1; kl <= numEdges2; kl++)
                {
                    string source_i = smallGraph.ListEdges[ij - 1].NodeSource.Id;
                    string source_k = bigGraph.ListEdges[kl - 1].NodeSource.Id;
                    string target_i = smallGraph.ListEdges[ij - 1].NodeTarget.Id;
                    string target_k = bigGraph.ListEdges[kl - 1].NodeTarget.Id;

                    string nameVar = "x_" + source_i + "," + source_k;
                    int colInd = SolverGLPK.glp_find_col(lp, nameVar);
                    if (colInd == 0)
                        throw new InvalidProgramException();

                    string nameVar2 = "x_" + target_i + "," + target_k;
                    int colInd2 = SolverGLPK.glp_find_col(lp, nameVar2);
                    if (colInd2 == 0)
                        throw new InvalidProgramException();

                    SolverGLPK.glp_add_rows(lp, 1);
                    SolverGLPK.glp_set_row_bnds(lp, ++rownum, SolverGLPK.GLP_DB, 0, 1);
                    SolverGLPK.glp_set_row_name(lp, rownum, "equation22_" + rownum);
                    iaList.Add(rownum);
                    jaList.Add(colInd);
                    arList.Add(1);
                    iaList.Add(rownum);
                    jaList.Add(numNodes1 * numNodes2 + (ij - 1) * numEdges2 + kl);
                    arList.Add(-1);

                    SolverGLPK.glp_add_rows(lp, 1);
                    SolverGLPK.glp_set_row_bnds(lp, ++rownum, SolverGLPK.GLP_DB, 0, 1);
                    SolverGLPK.glp_set_row_name(lp, rownum, "equation23_" + rownum);
                    iaList.Add(rownum);
                    jaList.Add(colInd2);
                    arList.Add(1);
                    iaList.Add(rownum);
                    jaList.Add(numNodes1 * numNodes2 + (ij - 1) * numEdges2 + kl);
                    arList.Add(-1);

                    SolverGLPK.glp_add_rows(lp, 1);
                    SolverGLPK.glp_set_row_bnds(lp, ++rownum, SolverGLPK.GLP_UP, -1, 1);
                    SolverGLPK.glp_set_row_name(lp, rownum, "equation24_" + rownum);
                    iaList.Add(rownum);
                    jaList.Add(colInd);
                    arList.Add(1);
                    iaList.Add(rownum);
                    jaList.Add(colInd2);
                    arList.Add(1);
                    iaList.Add(rownum);
                    jaList.Add(numNodes1 * numNodes2 + (ij - 1) * numEdges2 + kl);
                    arList.Add(-1);
                }

            //declare the contraints matrix
            ConstraintMatrix CM = new ConstraintMatrix();
            int numEle = arList.Count() - 1;
            CM.ia = iaList.ToArray();
            CM.ja = jaList.ToArray();
            CM.ar = arList.ToArray();
            SolverGLPK.glp_load_matrix(lp, numEle, CM.ia, CM.ja, CM.ar);
          
        }
        public override void IsoSousGraphExactF2() 
        {
            Graph smallGraph, bigGraph;
            if ((this.graph1.ListNodes.Count() <= this.graph2.ListNodes.Count()) && (this.graph1.ListEdges.Count() <= this.graph2.ListEdges.Count()))
            {
                smallGraph = this.graph1;
                bigGraph = this.graph2;
                exchange = false;
            }
            else if ((this.graph1.ListNodes.Count() >= this.graph2.ListNodes.Count()) && (this.graph1.ListEdges.Count() >= this.graph2.ListEdges.Count()))
            {
                smallGraph = this.graph2;
                bigGraph = this.graph1;
                exchange = true;
            }
            else throw new System.Exception("can't be solved by F1");
            
            SolverGLPK.glp_set_prob_name(lp, smallGraph.Name + " --> " + bigGraph.Name + " : F1");

            //for storing the value of the constraint matrix
            List<int> iaList = new List<int>();
            List<int> jaList = new List<int>();
            List<double> arList = new List<double>();
            iaList.Add(0);
            jaList.Add(0);
            arList.Add(0);

            int rownum = 0;
            int colnum = 0;

            int numNodes1 = smallGraph.ListNodes.Count();
            int numNodes2 = bigGraph.ListNodes.Count();
            int numEdges1 = smallGraph.ListEdges.Count();
            int numEdges2 = bigGraph.ListEdges.Count();

            //construct the objet funcion and the contraints.
            //the objet funcion
            for (int i = 1; i <= numNodes1; i++)
            {
                for (int k = 1; k <= numNodes2; k++)
                {
                    SolverGLPK.glp_add_cols(lp, 1);
                    ++colnum;
                    SolverGLPK.glp_set_col_kind(lp, colnum, SolverGLPK.GLP_BV);
                    SolverGLPK.glp_set_col_name(lp, colnum, ("x_" + smallGraph.ListNodes[i - 1].Id + "," + bigGraph.ListNodes[k - 1].Id));
                    SolverGLPK.glp_set_obj_coef(lp, colnum, smallGraph.ListNodes[i - 1].Label.dissimilarity(bigGraph.ListNodes[k - 1].Label));

                }
            }

            for (int ij = 1; ij <= numEdges1; ij++)
                for (int kl = 1; kl <= numEdges2; kl++)
                {
                    SolverGLPK.glp_add_cols(lp, 1);
                    SolverGLPK.glp_set_col_name(lp, ++colnum, ("y_" + smallGraph.ListEdges[ij - 1].Id + "," + bigGraph.ListEdges[kl - 1].Id));
                    SolverGLPK.glp_set_col_kind(lp, colnum, SolverGLPK.GLP_BV);

                    double costEdge = smallGraph.ListEdges[ij - 1].Label.dissimilarity(bigGraph.ListEdges[kl - 1].Label);
                    if (this.copyEdge) SolverGLPK.glp_set_obj_coef(lp, colnum, costEdge / 2);
                    else SolverGLPK.glp_set_obj_coef(lp, colnum, costEdge);
                    

                }

            SolverGLPK.glp_create_index(lp);


            // contraint: equation 3 ( Every vertex of smallGraph must be matched to a unique vertex of bigGraph)
            for (int i = 1; i <= numNodes1; i++)
            {
                SolverGLPK.glp_add_rows(lp, 1);
                SolverGLPK.glp_set_row_name(lp, ++rownum, "equation3_" + rownum);
                SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_FX, 1, 1);
                for (int k = 1; k <= numNodes2; k++)
                {
                    iaList.Add(rownum);
                    jaList.Add((i - 1) * numNodes2 + k);
                    arList.Add(1);
                }


            }


            // equation 4 ( Every edge of smallGraph must be matched to a unique edge of bigGraph)
            for (int ij = 1; ij <= numEdges1; ij++)
            {
                SolverGLPK.glp_add_rows(lp, 1);
                SolverGLPK.glp_set_row_name(lp, ++rownum, "equation4_" + rownum);
                SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_FX, 1, 1);
                for (int kl = 1; kl <= numEdges2; kl++)
                {

                    iaList.Add(rownum);
                    jaList.Add(numNodes1 * numNodes2 + (ij - 1) * numEdges2 + kl);
                    arList.Add(1);
                }
            }

            //equation 5 (Every vertex of bigGraph must be matched to at most an edge of smallGraph )
            for (int k = 1; k <= numNodes2; k++)
            {
                SolverGLPK.glp_add_rows(lp, 1);
                SolverGLPK.glp_set_row_name(lp, ++rownum, "equation5_" + rownum);
                SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_DB, 0, 1);
                for (int i = 1; i <= numNodes1; i++)
                {
                    iaList.Add(rownum);
                    jaList.Add(numNodes2 * (i - 1) + k);
                    arList.Add(1);
                }
            }

            //equation 6 (If two vertices are matched together, 
            //an edge originating one of these two vertices must be matched with an edge originating the other vertex)
            for (int k = 1; k <= numNodes2; k++)
                for (int ij = 1; ij <= numEdges1; ij++)
                {
                    SolverGLPK.glp_add_rows(lp, 1);
                    rownum++;
                    SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_FX, 0, 0);
                    SolverGLPK.glp_set_row_name(lp, rownum, "equation6_" + rownum);


                    string source = smallGraph.ListEdges[ij - 1].NodeSource.Id;
                    string nameVar = "x_" + source + "," + bigGraph.ListNodes[k - 1].Id;
                    int colInd = SolverGLPK.glp_find_col(lp, nameVar);
                    if (colInd == 0)
                        throw new InvalidProgramException();

                    iaList.Add(rownum);
                    jaList.Add(colInd);
                    arList.Add(1);

                    string name1 = smallGraph.ListEdges[ij - 1].Id;
                    foreach (Edge e in bigGraph.ListNodes[k - 1].ListEdgesOut)
                    {
                        string name2 = e.Id;
                        nameVar = "y_" + name1 + "," + name2;
                        colInd = SolverGLPK.glp_find_col(lp, nameVar);
                        if (colInd == 0)
                            throw new InvalidProgramException();

                        iaList.Add(rownum);
                        jaList.Add(colInd);
                        arList.Add(-1);
                    }
                }

            //equation 7 (If two vertices are matched together, 
            //an edge targeting one of these two vertices must be matched with an edge targeting the other vertex)
            for (int l = 1; l <= numNodes2; l++)
                for (int ij = 1; ij <= numEdges1; ij++)
                {
                    SolverGLPK.glp_add_rows(lp, 1);
                    SolverGLPK.glp_set_row_name(lp, ++rownum, "equation7_" + rownum);
                    SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_FX, 0, 0);

                    string target = smallGraph.ListEdges[ij - 1].NodeTarget.Id;
                    string nameVar = "x_" + target + "," + bigGraph.ListNodes[l - 1].Id;
                    int colInd = SolverGLPK.glp_find_col(lp, nameVar);
                    if (colInd == 0)
                        throw new InvalidProgramException();

                    iaList.Add(rownum);
                    jaList.Add(colInd);
                    arList.Add(1);

                    string name1 = smallGraph.ListEdges[ij - 1].Id;
                    foreach (Edge e in bigGraph.ListNodes[l - 1].ListEdgesIn)
                    {
                        string name2 = e.Id;
                        nameVar = "y_" + name1 + "," + name2;
                        colInd = SolverGLPK.glp_find_col(lp, nameVar);
                        if (colInd == 0)
                            throw new InvalidProgramException();

                        iaList.Add(rownum);
                        jaList.Add(colInd);
                        arList.Add(-1);
                    }

                }
            //declare the contraints matrix
            ConstraintMatrix CM = new ConstraintMatrix();
            int numEle = arList.Count() - 1;
            CM.ia = iaList.ToArray();
            CM.ja = jaList.ToArray();
            CM.ar = arList.ToArray();
            SolverGLPK.glp_load_matrix(lp, numEle, CM.ia, CM.ja, CM.ar);
        }
        public override void IsoSousGraphInexactF1a()
        {
            exchange = false;
            Graphs.Label nodeepslabel;
            //for storing the value of the constraint matrix
            List<int> iaList = new List<int>();
            List<int> jaList = new List<int>();
            List<double> arList = new List<double>();
            iaList.Add(0);
            jaList.Add(0);
            arList.Add(0);

            int rownum = 0;
            int colnum = 0;

            int numNodes1 = graph1.ListNodes.Count();
            int numNodes2 = graph2.ListNodes.Count();
            int numEdges1 = graph1.ListEdges.Count();
            int numEdges2 = graph2.ListEdges.Count();

            //construct the objet funcion and the contraints.
            //the objet funcion
            for (int i = 1; i <= numNodes1; i++)
            {
                for (int k = 1; k <= numNodes2; k++)
                {
                    SolverGLPK.glp_add_cols(lp, 1);
                    ++colnum;
                    SolverGLPK.glp_set_col_kind(lp, colnum, SolverGLPK.GLP_BV);
                    SolverGLPK.glp_set_col_name(lp, colnum, ("x_" + graph1.ListNodes[i - 1].Id + "," + graph2.ListNodes[k - 1].Id));
                    SolverGLPK.glp_set_obj_coef(lp, colnum, graph1.ListNodes[i - 1].Label.dissimilarity(graph2.ListNodes[k - 1].Label));

                }
            }

            for (int ij = 1; ij <= numEdges1; ij++)
                for (int kl = 1; kl <= numEdges2; kl++)
                {
                    SolverGLPK.glp_add_cols(lp, 1);
                    SolverGLPK.glp_set_col_name(lp, ++colnum, ("y_" + graph1.ListEdges[ij - 1].Id + "," + graph2.ListEdges[kl - 1].Id));
                    SolverGLPK.glp_set_col_kind(lp, colnum, SolverGLPK.GLP_BV);

                    double costEdge = graph1.ListEdges[ij - 1].Label.dissimilarity(graph2.ListEdges[kl - 1].Label);
                    if (this.copyEdge) SolverGLPK.glp_set_obj_coef(lp, colnum, costEdge / 2);
                    else SolverGLPK.glp_set_obj_coef(lp, colnum, costEdge);

                }
            for (int i = 1; i <= numNodes1; i++)
            {
                SolverGLPK.glp_add_cols(lp, 1);
                ++colnum;
                SolverGLPK.glp_set_col_kind(lp, colnum, SolverGLPK.GLP_BV);
                SolverGLPK.glp_set_col_name(lp, colnum, ("u_" + graph1.ListNodes[i - 1].Id + ",Ins_" + graph1.ListNodes[i - 1].Id));

                Type typeLabel = graph1.ListNodes[i - 1].Label.GetType();
                object obj = Activator.CreateInstance(typeLabel);
                nodeepslabel = (Graphs.Label)obj;
                nodeepslabel.Id = ConstantsAC.EPS_ID;

                SolverGLPK.glp_set_obj_coef(lp, colnum, (graph1.ListNodes[i - 1].Label).dissimilarity(nodeepslabel));
            }

            for (int ij = 1; ij <= numEdges1; ij++)
            {
                SolverGLPK.glp_add_cols(lp, 1);
                SolverGLPK.glp_set_col_name(lp, ++colnum, ("e_" + graph1.ListEdges[ij - 1].Id + ",Ins_" + graph1.ListEdges[ij - 1].Id));
                SolverGLPK.glp_set_col_kind(lp, colnum, SolverGLPK.GLP_BV);

                Type typeLabel = graph1.ListEdges[ij - 1].Label.GetType();
                object obj = Activator.CreateInstance(typeLabel);
                nodeepslabel = (Graphs.Label)obj;
                nodeepslabel.Id = ConstantsAC.EPS_ID;

                double costEdge = (graph1.ListEdges[ij - 1].Label).dissimilarity(nodeepslabel);
                if (this.copyEdge) SolverGLPK.glp_set_obj_coef(lp, colnum, costEdge / 2);
                else SolverGLPK.glp_set_obj_coef(lp, colnum, costEdge);
            }
            SolverGLPK.glp_create_index(lp);


            // contraint: equation 19
            for (int i = 1; i <= numNodes1; i++)
            {
                SolverGLPK.glp_add_rows(lp, 1);
                SolverGLPK.glp_set_row_name(lp, ++rownum, "equation19_" + rownum);
                SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_FX, 1, 1);
                for (int k = 1; k <= numNodes2; k++)
                {
                    iaList.Add(rownum);
                    jaList.Add((i - 1) * numNodes2 + k);
                    arList.Add(1);
                }

                iaList.Add(rownum);
                jaList.Add(numNodes1 * numNodes2 + numEdges1 * numEdges2 + i);
                arList.Add(1);
            }


            // equation 20 
            for (int ij = 1; ij <= numEdges1; ij++)
            {
                SolverGLPK.glp_add_rows(lp, 1);
                SolverGLPK.glp_set_row_name(lp, ++rownum, "equation20_" + rownum);
                SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_FX, 1, 1);
                for (int kl = 1; kl <= numEdges2; kl++)
                {

                    iaList.Add(rownum);
                    jaList.Add(numNodes1 * numNodes2 + (ij - 1) * numEdges2 + kl);
                    arList.Add(1);
                }
                iaList.Add(rownum);
                jaList.Add(numNodes1 * numNodes2 + numEdges1 * numEdges2 + numNodes1 + ij);
                arList.Add(1);

            }

            //equation 21 
            for (int k = 1; k <= numNodes2; k++)
            {
                SolverGLPK.glp_add_rows(lp, 1);
                SolverGLPK.glp_set_row_name(lp, ++rownum, "equation21_" + rownum);
                SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_DB, 0, 1);
                for (int i = 1; i <= numNodes1; i++)
                {
                    iaList.Add(rownum);
                    jaList.Add(numNodes2 * (i - 1) + k);
                    arList.Add(1);
                }
            }

            //equation 22 23 24

            for (int ij = 1; ij <= numEdges1; ij++)
                for (int kl = 1; kl <= numEdges2; kl++)
                {
                    string source_i = graph1.ListEdges[ij - 1].NodeSource.Id;
                    string source_k = graph2.ListEdges[kl - 1].NodeSource.Id;
                    string target_i = graph1.ListEdges[ij - 1].NodeTarget.Id;
                    string target_k = graph2.ListEdges[kl - 1].NodeTarget.Id;

                    string nameVar = "x_" + source_i + "," + source_k;
                    int colInd = SolverGLPK.glp_find_col(lp, nameVar);
                    if (colInd == 0)
                        throw new InvalidProgramException();

                    string nameVar2 = "x_" + target_i + "," + target_k;
                    int colInd2 = SolverGLPK.glp_find_col(lp, nameVar2);
                    if (colInd2 == 0)
                        throw new InvalidProgramException();

                    SolverGLPK.glp_add_rows(lp, 1);
                    SolverGLPK.glp_set_row_bnds(lp, ++rownum, SolverGLPK.GLP_DB, 0, 1);
                    SolverGLPK.glp_set_row_name(lp, rownum, "equation22_" + rownum);
                    iaList.Add(rownum);
                    jaList.Add(colInd);
                    arList.Add(1);
                    iaList.Add(rownum);
                    jaList.Add(numNodes1 * numNodes2 + (ij - 1) * numEdges2 + kl);
                    arList.Add(-1);

                    SolverGLPK.glp_add_rows(lp, 1);
                    SolverGLPK.glp_set_row_bnds(lp, ++rownum, SolverGLPK.GLP_DB, 0, 1);
                    SolverGLPK.glp_set_row_name(lp, rownum, "equation23_" + rownum);
                    iaList.Add(rownum);
                    jaList.Add(colInd2);
                    arList.Add(1);
                    iaList.Add(rownum);
                    jaList.Add(numNodes1 * numNodes2 + (ij - 1) * numEdges2 + kl);
                    arList.Add(-1);

                    SolverGLPK.glp_add_rows(lp, 1);
                    SolverGLPK.glp_set_row_bnds(lp, ++rownum, SolverGLPK.GLP_UP, -1, 1);
                    SolverGLPK.glp_set_row_name(lp, rownum, "equation24_" + rownum);
                    iaList.Add(rownum);
                    jaList.Add(colInd);
                    arList.Add(1);
                    iaList.Add(rownum);
                    jaList.Add(colInd2);
                    arList.Add(1);
                    iaList.Add(rownum);
                    jaList.Add(numNodes1 * numNodes2 + (ij - 1) * numEdges2 + kl);
                    arList.Add(-1);


                }

            //declare the contraints matrix
            ConstraintMatrix CM = new ConstraintMatrix();
            int numEle = arList.Count() - 1;
            CM.ia = iaList.ToArray();
            CM.ja = jaList.ToArray();
            CM.ar = arList.ToArray();
            SolverGLPK.glp_load_matrix(lp, numEle, CM.ia, CM.ja, CM.ar);
            
        }
        public override void IsoSousGraphInexactF2a()
        {
            //rapport master de monsieur LeBodic P16
            exchange = false;
            Graphs.Label nodeepslabel;
            //for storing the value of the constraint matrix
            List<int> iaList = new List<int>();
            List<int> jaList = new List<int>();
            List<double> arList = new List<double>();
            iaList.Add(0);
            jaList.Add(0);
            arList.Add(0);

            int rownum = 0;
            int colnum = 0;

            int numNodes1 = graph1.ListNodes.Count();
            int numNodes2 = graph2.ListNodes.Count();
            int numEdges1 = graph1.ListEdges.Count();
            int numEdges2 = graph2.ListEdges.Count();

            //construct the objet funcion and the contraints.
            //the objet funcion
            for (int i = 1; i <= numNodes1; i++)
            {
                for (int k = 1; k <= numNodes2; k++)
                {
                    SolverGLPK.glp_add_cols(lp, 1);
                    ++colnum;
                    SolverGLPK.glp_set_col_kind(lp, colnum, SolverGLPK.GLP_BV);
                    SolverGLPK.glp_set_col_name(lp, colnum, ("x_" + graph1.ListNodes[i - 1].Id + "," + graph2.ListNodes[k - 1].Id));
                    SolverGLPK.glp_set_obj_coef(lp, colnum, graph1.ListNodes[i - 1].Label.dissimilarity(graph2.ListNodes[k - 1].Label));

                }
            }

            for (int ij = 1; ij <= numEdges1; ij++)
                for (int kl = 1; kl <= numEdges2; kl++)
                {
                    SolverGLPK.glp_add_cols(lp, 1);
                    SolverGLPK.glp_set_col_name(lp, ++colnum, ("y_" + graph1.ListEdges[ij - 1].Id + "," + graph2.ListEdges[kl - 1].Id));
                    SolverGLPK.glp_set_col_kind(lp, colnum, SolverGLPK.GLP_BV);

                    double costEdge = graph1.ListEdges[ij - 1].Label.dissimilarity(graph2.ListEdges[kl - 1].Label);
                    if (this.copyEdge) SolverGLPK.glp_set_obj_coef(lp, colnum, costEdge / 2);
                    else SolverGLPK.glp_set_obj_coef(lp, colnum, costEdge);

                }
            for (int i = 1; i <= numNodes1; i++)
            {
                SolverGLPK.glp_add_cols(lp, 1);
                ++colnum;
                SolverGLPK.glp_set_col_kind(lp, colnum, SolverGLPK.GLP_BV);
                SolverGLPK.glp_set_col_name(lp, colnum, ("u_" + graph1.ListNodes[i - 1].Id + ",Ins_" + graph1.ListNodes[i - 1].Id));

                Type typeLabel = graph1.ListNodes[i - 1].Label.GetType();
                object obj = Activator.CreateInstance(typeLabel);
                nodeepslabel = (Graphs.Label)obj;
                nodeepslabel.Id = ConstantsAC.EPS_ID;

                SolverGLPK.glp_set_obj_coef(lp, colnum, (graph1.ListNodes[i - 1].Label).dissimilarity(nodeepslabel));
            }

            for (int ij = 1; ij <= numEdges1; ij++)
            {
                SolverGLPK.glp_add_cols(lp, 1);
                SolverGLPK.glp_set_col_name(lp, ++colnum, ("e_" + graph1.ListEdges[ij - 1].Id + ",Ins_" + graph1.ListEdges[ij - 1].Id));
                SolverGLPK.glp_set_col_kind(lp, colnum, SolverGLPK.GLP_BV);

                Type typeLabel = graph1.ListEdges[ij - 1].Label.GetType();
                object obj = Activator.CreateInstance(typeLabel);
                nodeepslabel = (Graphs.Label)obj;
                nodeepslabel.Id = ConstantsAC.EPS_ID;

                double costEdge = (graph1.ListEdges[ij - 1].Label).dissimilarity(nodeepslabel);
                if (this.copyEdge) SolverGLPK.glp_set_obj_coef(lp, colnum, costEdge / 2);
                else SolverGLPK.glp_set_obj_coef(lp, colnum, costEdge);
            }
            SolverGLPK.glp_create_index(lp);


            // contraint: equation 34
            for (int i = 1; i <= numNodes1; i++)
            {
                SolverGLPK.glp_add_rows(lp, 1);
                SolverGLPK.glp_set_row_name(lp, ++rownum, "equation34_" + rownum);
                SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_FX, 1, 1);
                for (int k = 1; k <= numNodes2; k++)
                {
                    iaList.Add(rownum);
                    jaList.Add((i - 1) * numNodes2 + k);
                    arList.Add(1);
                }
                //u_i
                iaList.Add(rownum);
                jaList.Add(numNodes1 * numNodes2 + numEdges1 * numEdges2 + i);
                arList.Add(1);
            }


            // equation 35 
            for (int ij = 1; ij <= numEdges1; ij++)
            {
                SolverGLPK.glp_add_rows(lp, 1);
                SolverGLPK.glp_set_row_name(lp, ++rownum, "equation35_" + rownum);
                SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_FX, 1, 1);
                for (int kl = 1; kl <= numEdges2; kl++)
                {

                    iaList.Add(rownum);
                    jaList.Add(numNodes1 * numNodes2 + (ij - 1) * numEdges2 + kl);
                    arList.Add(1);
                }
                iaList.Add(rownum);
                jaList.Add(numNodes1 * numNodes2 + numEdges1 * numEdges2 + numNodes1 + ij);
                arList.Add(1);

            }

            //equation 36
            for (int k = 1; k <= numNodes2; k++)
            {
                SolverGLPK.glp_add_rows(lp, 1);
                SolverGLPK.glp_set_row_name(lp, ++rownum, "equation36_" + rownum);
                SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_DB, 0, 1);
                for (int i = 1; i <= numNodes1; i++)
                {
                    iaList.Add(rownum);
                    jaList.Add(numNodes2 * (i - 1) + k);
                    arList.Add(1);
                }
            }

            //38  If two vertices are matched together, 
            //an edge originating one of these two vertices must be matched with an edge originating the other vertex)
            for (int k = 1; k <= numNodes2; k++)
                for (int ij = 1; ij <= numEdges1; ij++)
                {
                    SolverGLPK.glp_add_rows(lp, 1);
                    rownum++;
                    SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_DB, 0, 1);
                    SolverGLPK.glp_set_row_name(lp, rownum, "equation38_" + rownum);


                    string source = graph1.ListEdges[ij - 1].NodeSource.Id;
                    string nameVar = "x_" + source + "," + graph2.ListNodes[k - 1].Id;
                    int colInd = SolverGLPK.glp_find_col(lp, nameVar);
                    if (colInd == 0)
                        throw new InvalidProgramException();

                    iaList.Add(rownum);
                    jaList.Add(colInd);
                    arList.Add(1);

                    string name1 = graph1.ListEdges[ij - 1].Id;
                    foreach (Edge e in graph2.ListNodes[k - 1].ListEdgesOut)
                    {
                        string name2 = e.Id;
                        nameVar = "y_" + name1 + "," + name2;
                        colInd = SolverGLPK.glp_find_col(lp, nameVar);
                        if (colInd == 0)
                            throw new InvalidProgramException();

                        iaList.Add(rownum);
                        jaList.Add(colInd);
                        arList.Add(-1);
                    }
                }
/*
            //37  
            for (int k = 1; k <= numNodes2; k++)
                for (int ij = 1; ij <= numEdges1; ij++)
                {
                    SolverGLPK.glp_add_rows(lp, 1);
                    rownum++;
                    SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_DB, -1, 0);
                    SolverGLPK.glp_set_row_name(lp, rownum, "equation37_" + rownum);


                    string source = graph1.ListEdges[ij - 1].NodeSource.Id;
                    string nameVar = "x_" + source + "," + graph2.ListNodes[k - 1].Id;
                    int colInd = SolverGLPK.glp_find_col(lp, nameVar);
                    if (colInd == 0)
                        throw new InvalidProgramException();

                    iaList.Add(rownum);
                    jaList.Add(colInd);
                    arList.Add(1);

                    nameVar = "u_" + source + ",Ins_" + source;
                    colInd = SolverGLPK.glp_find_col(lp, nameVar);
                    if (colInd == 0)
                        throw new InvalidProgramException();
                    iaList.Add(rownum);
                    jaList.Add(colInd);
                    arList.Add(1);

                    string name1 = graph1.ListEdges[ij - 1].Id;
                    foreach (Edge e in graph2.ListNodes[k - 1].ListEdgesOut)
                    {
                        string name2 = e.Id;
                        nameVar = "y_" + name1 + "," + name2;
                        colInd = SolverGLPK.glp_find_col(lp, nameVar);
                        if (colInd == 0)
                            throw new InvalidProgramException();

                        iaList.Add(rownum);
                        jaList.Add(colInd);
                        arList.Add(-1);
                    }

                    iaList.Add(rownum);
                    jaList.Add(numNodes1 * numNodes2 + numEdges1 * numEdges2 + numNodes1 + ij);
                    arList.Add(-1);
                }*/

            //equation 40 (If two vertices are matched together, 
            //an edge targeting one of these two vertices must be matched with an edge targeting the other vertex)
            for (int l = 1; l <= numNodes2; l++)
                for (int ij = 1; ij <= numEdges1; ij++)
                {
                    SolverGLPK.glp_add_rows(lp, 1);
                    SolverGLPK.glp_set_row_name(lp, ++rownum, "equation40_" + rownum);
                    SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_DB, 0, 1);

                    string target = graph1.ListEdges[ij - 1].NodeTarget.Id;
                    string nameVar = "x_" + target + "," + graph2.ListNodes[l - 1].Id;
                    int colInd = SolverGLPK.glp_find_col(lp, nameVar);
                    if (colInd == 0)
                        throw new InvalidProgramException();

                    iaList.Add(rownum);
                    jaList.Add(colInd);
                    arList.Add(1);

                    string name1 = graph1.ListEdges[ij - 1].Id;
                    foreach (Edge e in graph2.ListNodes[l - 1].ListEdgesIn)
                    {
                        string name2 = e.Id;
                        nameVar = "y_" + name1 + "," + name2;
                        colInd = SolverGLPK.glp_find_col(lp, nameVar);
                        if (colInd == 0)
                            throw new InvalidProgramException();

                        iaList.Add(rownum);
                        jaList.Add(colInd);
                        arList.Add(-1);
                    }

                }

         /*   //equation 39
            for (int l = 1; l <= numNodes2; l++)
                for (int ij = 1; ij <= numEdges1; ij++)
                {
                    SolverGLPK.glp_add_rows(lp, 1);
                    SolverGLPK.glp_set_row_name(lp, ++rownum, "equation39_" + rownum);
                    SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_DB, -1, 0);

                    string target = graph1.ListEdges[ij - 1].NodeTarget.Id;
                    string nameVar = "x_" + target + "," + graph2.ListNodes[l - 1].Id;
                    int colInd = SolverGLPK.glp_find_col(lp, nameVar);
                    if (colInd == 0)
                        throw new InvalidProgramException();

                    iaList.Add(rownum);
                    jaList.Add(colInd);
                    arList.Add(1);

                    nameVar = "u_" + target + ",Ins_" + target;
                    colInd = SolverGLPK.glp_find_col(lp, nameVar);
                    if (colInd == 0)
                        throw new InvalidProgramException();
                    iaList.Add(rownum);
                    jaList.Add(colInd);
                    arList.Add(1);

                    string name1 = graph1.ListEdges[ij - 1].Id;
                    foreach (Edge e in graph2.ListNodes[l - 1].ListEdgesIn)
                    {
                        string name2 = e.Id;
                        nameVar = "y_" + name1 + "," + name2;
                        colInd = SolverGLPK.glp_find_col(lp, nameVar);
                        if (colInd == 0)
                            throw new InvalidProgramException();

                        iaList.Add(rownum);
                        jaList.Add(colInd);
                        arList.Add(-1);
                    }

                    iaList.Add(rownum);
                    jaList.Add(numNodes1 * numNodes2 + numEdges1 * numEdges2 + numNodes1 + ij);
                    arList.Add(-1);

                }*/

            //declare the contraints matrix
            ConstraintMatrix CM = new ConstraintMatrix();
            int numEle = arList.Count() - 1;
            CM.ia = iaList.ToArray();
            CM.ja = jaList.ToArray();
            CM.ar = arList.ToArray();
            SolverGLPK.glp_load_matrix(lp, numEle, CM.ia, CM.ja, CM.ar);
        }
        public override void IsoGraphInexactF1b()
        {
            exchange = false;
            Graphs.Label nodeepslabel;
            //for storing the value of the constraint matrix
            List<int> iaList = new List<int>();
            List<int> jaList = new List<int>();
            List<double> arList = new List<double>();
            iaList.Add(0);
            jaList.Add(0);
            arList.Add(0);

            int rownum = 0;
            int colnum = 0;

            int numNodes1 = graph1.ListNodes.Count();
            int numNodes2 = graph2.ListNodes.Count();
            int numEdges1 = graph1.ListEdges.Count();
            int numEdges2 = graph2.ListEdges.Count();

            //construct the objet funcion and the contraints.
            //the objet funcion
            for (int i = 1; i <= numNodes1; i++)
            {
                for (int k = 1; k <= numNodes2; k++)
                {
                    SolverGLPK.glp_add_cols(lp, 1);
                    ++colnum;
                    SolverGLPK.glp_set_col_kind(lp, colnum, SolverGLPK.GLP_BV);
                    SolverGLPK.glp_set_col_name(lp, colnum, ("x_" + graph1.ListNodes[i - 1].Id + "," + graph2.ListNodes[k - 1].Id));
                    SolverGLPK.glp_set_obj_coef(lp, colnum, graph1.ListNodes[i - 1].Label.dissimilarity(graph2.ListNodes[k - 1].Label));

                }
            }

            for (int ij = 1; ij <= numEdges1; ij++)
                for (int kl = 1; kl <= numEdges2; kl++)
                {
                    SolverGLPK.glp_add_cols(lp, 1);
                    SolverGLPK.glp_set_col_name(lp, ++colnum, ("y_" + graph1.ListEdges[ij - 1].Id + "," + graph2.ListEdges[kl - 1].Id));
                    SolverGLPK.glp_set_col_kind(lp, colnum, SolverGLPK.GLP_BV);

                    double costEdge = graph1.ListEdges[ij - 1].Label.dissimilarity(graph2.ListEdges[kl - 1].Label);
                    if (this.copyEdge) SolverGLPK.glp_set_obj_coef(lp, colnum, costEdge / 2);
                    else SolverGLPK.glp_set_obj_coef(lp, colnum, costEdge);

                }
            for (int i = 1; i <= numNodes1; i++)
            {
                SolverGLPK.glp_add_cols(lp, 1);
                ++colnum;
                SolverGLPK.glp_set_col_kind(lp, colnum, SolverGLPK.GLP_BV);
                SolverGLPK.glp_set_col_name(lp, colnum, ("u_" + graph1.ListNodes[i - 1].Id + ",Ins_" + graph1.ListNodes[i - 1].Id));

                Type typeLabel = graph1.ListNodes[i - 1].Label.GetType();
                object obj = Activator.CreateInstance(typeLabel);
                nodeepslabel = (Graphs.Label)obj;
                nodeepslabel.Id = ConstantsAC.EPS_ID;

                SolverGLPK.glp_set_obj_coef(lp, colnum, (graph1.ListNodes[i - 1].Label).dissimilarity(nodeepslabel));
            }

            for (int ij = 1; ij <= numEdges1; ij++)
            {
                SolverGLPK.glp_add_cols(lp, 1);
                SolverGLPK.glp_set_col_name(lp, ++colnum, ("e_" + graph1.ListEdges[ij - 1].Id + ",Ins_" + graph1.ListEdges[ij - 1].Id));
                SolverGLPK.glp_set_col_kind(lp, colnum, SolverGLPK.GLP_BV);

                Type typeLabel = graph1.ListEdges[ij - 1].Label.GetType();
                object obj = Activator.CreateInstance(typeLabel);
                nodeepslabel = (Graphs.Label)obj;
                nodeepslabel.Id = ConstantsAC.EPS_ID;

                double costEdge = (graph1.ListEdges[ij - 1].Label).dissimilarity(nodeepslabel);
                if (this.copyEdge) SolverGLPK.glp_set_obj_coef(lp, colnum, costEdge / 2);
                else SolverGLPK.glp_set_obj_coef(lp, colnum, costEdge);
            }

            for (int k = 1; k <= numNodes2; k++)
            {
                SolverGLPK.glp_add_cols(lp, 1);
                ++colnum;
                SolverGLPK.glp_set_col_kind(lp, colnum, SolverGLPK.GLP_BV);
                SolverGLPK.glp_set_col_name(lp, colnum, ("v_Del_" + graph2.ListNodes[k - 1].Id + "," + graph2.ListNodes[k - 1].Id));

                Type typeLabel = graph2.ListNodes[k - 1].Label.GetType();
                object obj = Activator.CreateInstance(typeLabel);
                nodeepslabel = (Graphs.Label)obj;
                nodeepslabel.Id = ConstantsAC.EPS_ID;

                SolverGLPK.glp_set_obj_coef(lp, colnum, (graph2.ListNodes[k - 1].Label).dissimilarity(nodeepslabel));
            }

            for (int kl = 1; kl <= numEdges2; kl++)
            {
                SolverGLPK.glp_add_cols(lp, 1);
                SolverGLPK.glp_set_col_name(lp, ++colnum, ("f_Del_" + graph2.ListEdges[kl - 1].Id + "," + graph2.ListEdges[kl - 1].Id));
                SolverGLPK.glp_set_col_kind(lp, colnum, SolverGLPK.GLP_BV);

                Type typeLabel = graph2.ListEdges[kl - 1].Label.GetType();
                object obj = Activator.CreateInstance(typeLabel);
                nodeepslabel = (Graphs.Label)obj;
                nodeepslabel.Id = ConstantsAC.EPS_ID;

                double costEdge = (graph2.ListEdges[kl - 1].Label).dissimilarity(nodeepslabel);
                if (this.copyEdge) SolverGLPK.glp_set_obj_coef(lp, colnum, costEdge / 2);
                else SolverGLPK.glp_set_obj_coef(lp, colnum, costEdge);
            }
            SolverGLPK.glp_create_index(lp);


            // contraint: equation [Fb.1]-2
            for (int i = 1; i <= numNodes1; i++)
            {
                SolverGLPK.glp_add_rows(lp, 1);
                SolverGLPK.glp_set_row_name(lp, ++rownum, "equation2_" + rownum);
                SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_FX, 1, 1);
                for (int k = 1; k <= numNodes2; k++)
                {
                    iaList.Add(rownum);
                    jaList.Add((i - 1) * numNodes2 + k);
                    arList.Add(1);
                }

                iaList.Add(rownum);
                jaList.Add(numNodes1 * numNodes2 + numEdges1 * numEdges2 + i);
                arList.Add(1);
            }


            // equation 3 
            for (int ij = 1; ij <= numEdges1; ij++)
            {
                SolverGLPK.glp_add_rows(lp, 1);
                SolverGLPK.glp_set_row_name(lp, ++rownum, "equation3_" + rownum);
                SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_FX, 1, 1);
                for (int kl = 1; kl <= numEdges2; kl++)
                {

                    iaList.Add(rownum);
                    jaList.Add(numNodes1 * numNodes2 + (ij - 1) * numEdges2 + kl);
                    arList.Add(1);
                }
                iaList.Add(rownum);
                jaList.Add(numNodes1 * numNodes2 + numEdges1 * numEdges2 + numNodes1 + ij);
                arList.Add(1);
            }

            // contraint: equation [Fb.1]-4
            for (int k = 1; k <= numNodes2; k++)
            {
                SolverGLPK.glp_add_rows(lp, 1);
                SolverGLPK.glp_set_row_name(lp, ++rownum, "equation4_" + rownum);
                SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_FX, 1, 1);
                for (int i = 1; i <= numNodes1; i++)
                {
                    iaList.Add(rownum);
                    jaList.Add((i - 1) * numNodes2 + k);
                    arList.Add(1);
                }

                iaList.Add(rownum);
                jaList.Add(numNodes1 * numNodes2 + numEdges1 * numEdges2 + numNodes1 + numEdges1 + k);
                arList.Add(1);
            }


            // equation 5
            for (int kl = 1; kl <= numEdges2; kl++)
            {
                SolverGLPK.glp_add_rows(lp, 1);
                SolverGLPK.glp_set_row_name(lp, ++rownum, "equation5_" + rownum);
                SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_FX, 1, 1);
                for (int ij = 1; ij <= numEdges1; ij++)
                {

                    iaList.Add(rownum);
                    jaList.Add(numNodes1 * numNodes2 + (ij - 1) * numEdges2 + kl);
                    arList.Add(1);
                }
                iaList.Add(rownum);
                jaList.Add(numNodes1 * numNodes2 + numEdges1 * numEdges2 + numNodes1 + numEdges1 + numNodes2+kl);
                arList.Add(1);

            }


            //equation 6 7 8

            for (int ij = 1; ij <= numEdges1; ij++)
                for (int kl = 1; kl <= numEdges2; kl++)
                {
                    string source_i = graph1.ListEdges[ij - 1].NodeSource.Id;
                    string source_k = graph2.ListEdges[kl - 1].NodeSource.Id;
                    string target_i = graph1.ListEdges[ij - 1].NodeTarget.Id;
                    string target_k = graph2.ListEdges[kl - 1].NodeTarget.Id;

                    string nameVar = "x_" + source_i + "," + source_k;
                    int colInd = SolverGLPK.glp_find_col(lp, nameVar);
                    if (colInd == 0)
                        throw new InvalidProgramException();

                    string nameVar2 = "x_" + target_i + "," + target_k;
                    int colInd2 = SolverGLPK.glp_find_col(lp, nameVar2);
                    if (colInd2 == 0)
                        throw new InvalidProgramException();

                    SolverGLPK.glp_add_rows(lp, 1);
                    SolverGLPK.glp_set_row_bnds(lp, ++rownum, SolverGLPK.GLP_DB, 0, 1);
                    SolverGLPK.glp_set_row_name(lp, rownum, "equation6_" + rownum);
                    iaList.Add(rownum);
                    jaList.Add(colInd);
                    arList.Add(1);
                    iaList.Add(rownum);
                    jaList.Add(numNodes1 * numNodes2 + (ij - 1) * numEdges2 + kl);
                    arList.Add(-1);

                    SolverGLPK.glp_add_rows(lp, 1);
                    SolverGLPK.glp_set_row_bnds(lp, ++rownum, SolverGLPK.GLP_DB, 0, 1);
                    SolverGLPK.glp_set_row_name(lp, rownum, "equation7_" + rownum);
                    iaList.Add(rownum);
                    jaList.Add(colInd2);
                    arList.Add(1);
                    iaList.Add(rownum);
                    jaList.Add(numNodes1 * numNodes2 + (ij - 1) * numEdges2 + kl);
                    arList.Add(-1);

                    SolverGLPK.glp_add_rows(lp, 1);
                    SolverGLPK.glp_set_row_bnds(lp, ++rownum, SolverGLPK.GLP_UP, -1, 1);
                    SolverGLPK.glp_set_row_name(lp, rownum, "equation8_" + rownum);
                    iaList.Add(rownum);
                    jaList.Add(colInd);
                    arList.Add(1);
                    iaList.Add(rownum);
                    jaList.Add(colInd2);
                    arList.Add(1);
                    iaList.Add(rownum);
                    jaList.Add(numNodes1 * numNodes2 + (ij - 1) * numEdges2 + kl);
                    arList.Add(-1);


                }

            //declare the contraints matrix
            ConstraintMatrix CM = new ConstraintMatrix();
            int numEle = arList.Count() - 1;
            CM.ia = iaList.ToArray();
            CM.ja = jaList.ToArray();
            CM.ar = arList.ToArray();
            SolverGLPK.glp_load_matrix(lp, numEle, CM.ia, CM.ja, CM.ar); 
            
        }
        public override void IsoGraphInexactF2b()
        {
            exchange = false;
            Graphs.Label nodeepslabel;
            //for storing the value of the constraint matrix
            List<int> iaList = new List<int>();
            List<int> jaList = new List<int>();
            List<double> arList = new List<double>();
            iaList.Add(0);
            jaList.Add(0);
            arList.Add(0);

            int rownum = 0;
            int colnum = 0;

            int numNodes1 = graph1.ListNodes.Count();
            int numNodes2 = graph2.ListNodes.Count();
            int numEdges1 = graph1.ListEdges.Count();
            int numEdges2 = graph2.ListEdges.Count();

            //construct the objet funcion and the contraints.
            //the objet funcion
            for (int i = 1; i <= numNodes1; i++)
            {
                for (int k = 1; k <= numNodes2; k++)
                {
                    SolverGLPK.glp_add_cols(lp, 1);
                    ++colnum;
                    SolverGLPK.glp_set_col_kind(lp, colnum, SolverGLPK.GLP_BV);
                    SolverGLPK.glp_set_col_name(lp, colnum, ("x_" + graph1.ListNodes[i - 1].Id + "," + graph2.ListNodes[k - 1].Id));
                    SolverGLPK.glp_set_obj_coef(lp, colnum, graph1.ListNodes[i - 1].Label.dissimilarity(graph2.ListNodes[k - 1].Label));
                }
            }

            for (int ij = 1; ij <= numEdges1; ij++)
                for (int kl = 1; kl <= numEdges2; kl++)
                {
                    SolverGLPK.glp_add_cols(lp, 1);
                    SolverGLPK.glp_set_col_name(lp, ++colnum, ("y_" + graph1.ListEdges[ij - 1].Id + "," + graph2.ListEdges[kl - 1].Id));
                    SolverGLPK.glp_set_col_kind(lp, colnum, SolverGLPK.GLP_BV);

                    double costEdge =  graph1.ListEdges[ij - 1].Label.dissimilarity(graph2.ListEdges[kl - 1].Label);
                    if (this.copyEdge) SolverGLPK.glp_set_obj_coef(lp, colnum, costEdge / 2);
                    else SolverGLPK.glp_set_obj_coef(lp, colnum, costEdge);

                }
            for (int i = 1; i <= numNodes1; i++)
            {
                SolverGLPK.glp_add_cols(lp, 1);
                ++colnum;
                SolverGLPK.glp_set_col_kind(lp, colnum, SolverGLPK.GLP_BV);
                SolverGLPK.glp_set_col_name(lp, colnum, ("u_" + graph1.ListNodes[i - 1].Id + ",Ins_" + graph1.ListNodes[i - 1].Id));

                Type typeLabel = graph1.ListNodes[i - 1].Label.GetType();
                object obj = Activator.CreateInstance(typeLabel);
                nodeepslabel = (Graphs.Label)obj;
                nodeepslabel.Id = ConstantsAC.EPS_ID;

                SolverGLPK.glp_set_obj_coef(lp, colnum, (graph1.ListNodes[i - 1].Label).dissimilarity(nodeepslabel));
            }

            for (int ij = 1; ij <= numEdges1; ij++)
            {
                SolverGLPK.glp_add_cols(lp, 1);
                SolverGLPK.glp_set_col_name(lp, ++colnum, ("e_" + graph1.ListEdges[ij - 1].Id + ",Ins_" + graph1.ListEdges[ij - 1].Id));
                SolverGLPK.glp_set_col_kind(lp, colnum, SolverGLPK.GLP_BV);

                Type typeLabel = graph1.ListEdges[ij - 1].Label.GetType();
                object obj = Activator.CreateInstance(typeLabel);
                nodeepslabel = (Graphs.Label)obj;
                nodeepslabel.Id = ConstantsAC.EPS_ID;

                double costEdge = (graph1.ListEdges[ij - 1].Label).dissimilarity(nodeepslabel);
                if (this.copyEdge) SolverGLPK.glp_set_obj_coef(lp, colnum, costEdge / 2);
                else SolverGLPK.glp_set_obj_coef(lp, colnum, costEdge);
            }

            for (int k = 1; k <= numNodes2; k++)
            {
                SolverGLPK.glp_add_cols(lp, 1);
                ++colnum;
                SolverGLPK.glp_set_col_kind(lp, colnum, SolverGLPK.GLP_BV);
                SolverGLPK.glp_set_col_name(lp, colnum, ("v_Del_" + graph2.ListNodes[k - 1].Id + "," + graph2.ListNodes[k - 1].Id));

                Type typeLabel = graph2.ListNodes[k - 1].Label.GetType();
                object obj = Activator.CreateInstance(typeLabel);
                nodeepslabel = (Graphs.Label)obj;
                nodeepslabel.Id = ConstantsAC.EPS_ID;

                SolverGLPK.glp_set_obj_coef(lp, colnum, (graph2.ListNodes[k - 1].Label).dissimilarity(nodeepslabel));
            }

            for (int kl = 1; kl <= numEdges2; kl++)
            {
                SolverGLPK.glp_add_cols(lp, 1);
                SolverGLPK.glp_set_col_name(lp, ++colnum, ("f_Del_" + graph2.ListEdges[kl - 1].Id + "," + graph2.ListEdges[kl - 1].Id));
                SolverGLPK.glp_set_col_kind(lp, colnum, SolverGLPK.GLP_BV);

                Type typeLabel = graph2.ListEdges[kl - 1].Label.GetType();
                object obj = Activator.CreateInstance(typeLabel);
                nodeepslabel = (Graphs.Label)obj;
                nodeepslabel.Id = ConstantsAC.EPS_ID;
                double costEdge = (graph2.ListEdges[kl - 1].Label).dissimilarity(nodeepslabel);
                if (this.copyEdge) SolverGLPK.glp_set_obj_coef(lp, colnum, costEdge / 2);
                else SolverGLPK.glp_set_obj_coef(lp, colnum, costEdge);
            }
            SolverGLPK.glp_create_index(lp);


            // contraint: equation [Fb.1]-2
            for (int i = 1; i <= numNodes1; i++)
            {
                SolverGLPK.glp_add_rows(lp, 1);
                SolverGLPK.glp_set_row_name(lp, ++rownum, "equation2_" + rownum);
                SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_FX, 1, 1);
                for (int k = 1; k <= numNodes2; k++)
                {
                    iaList.Add(rownum);
                    jaList.Add((i - 1) * numNodes2 + k);
                    arList.Add(1);
                }

                iaList.Add(rownum);
                jaList.Add(numNodes1 * numNodes2 + numEdges1 * numEdges2 + i);
                arList.Add(1);
            }

            // equation 3 
            for (int ij = 1; ij <= numEdges1; ij++)
            {
                SolverGLPK.glp_add_rows(lp, 1);
                SolverGLPK.glp_set_row_name(lp, ++rownum, "equation3_" + rownum);
                SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_FX, 1, 1);
                for (int kl = 1; kl <= numEdges2; kl++)
                {

                    iaList.Add(rownum);
                    jaList.Add(numNodes1 * numNodes2 + (ij - 1) * numEdges2 + kl);
                    arList.Add(1);
                }
                iaList.Add(rownum);
                jaList.Add(numNodes1 * numNodes2 + numEdges1 * numEdges2 + numNodes1 + ij);
                arList.Add(1);
            }
            // contraint: equation [Fb.1]-4
            for (int k = 1; k <= numNodes2; k++)
            {
                SolverGLPK.glp_add_rows(lp, 1);
                SolverGLPK.glp_set_row_name(lp, ++rownum, "equation4_" + rownum);
                SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_FX, 1, 1);
                for (int i = 1; i <= numNodes1; i++)
                {
                    iaList.Add(rownum);
                    jaList.Add((i - 1) * numNodes2 + k);
                    arList.Add(1);
                }

                iaList.Add(rownum);
                jaList.Add(numNodes1 * numNodes2 + numEdges1 * numEdges2 + numNodes1 + numEdges1 + k);
                arList.Add(1);
            }


            // equation 5
            for (int kl = 1; kl <= numEdges2; kl++)
            {
                SolverGLPK.glp_add_rows(lp, 1);
                SolverGLPK.glp_set_row_name(lp, ++rownum, "equation5_" + rownum);
                SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_FX, 1, 1);
                for (int ij = 1; ij <= numEdges1; ij++)
                {

                    iaList.Add(rownum);
                    jaList.Add(numNodes1 * numNodes2 + (ij - 1) * numEdges2 + kl);
                    arList.Add(1);
                }
                iaList.Add(rownum);
                jaList.Add(numNodes1 * numNodes2 + numEdges1 * numEdges2 + numNodes1 + numEdges1 + numNodes2+kl);
                arList.Add(1);
            }

            //6 If two vertices are matched together, 
            //an edge originating one of these two vertices must be matched with an edge originating the other vertex)
            for (int k = 1; k <= numNodes2; k++)
                for (int ij = 1; ij <= numEdges1; ij++)
                {
                    SolverGLPK.glp_add_rows(lp, 1);
                    rownum++;
                    SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_DB, 0, 1);
                    SolverGLPK.glp_set_row_name(lp, rownum, "equation38_" + rownum);


                    string source = graph1.ListEdges[ij - 1].NodeSource.Id;
                    string nameVar = "x_" + source + "," + graph2.ListNodes[k - 1].Id;
                    int colInd = SolverGLPK.glp_find_col(lp, nameVar);
                    if (colInd == 0)
                        throw new InvalidProgramException();

                    iaList.Add(rownum);
                    jaList.Add(colInd);
                    arList.Add(1);

                    string name1 = graph1.ListEdges[ij - 1].Id;
                    foreach (Edge e in graph2.ListNodes[k - 1].ListEdgesOut)
                    {
                        string name2 = e.Id;
                        nameVar = "y_" + name1 + "," + name2;
                        colInd = SolverGLPK.glp_find_col(lp, nameVar);
                        if (colInd == 0)
                            throw new InvalidProgramException();

                        iaList.Add(rownum);
                        jaList.Add(colInd);
                        arList.Add(-1);
                    }
                }

            //7
        /*    for (int k = 1; k <= numNodes2; k++)
                for (int ij = 1; ij <= numEdges1; ij++)
                {
                    SolverGLPK.glp_add_rows(lp, 1);
                    rownum++;
                    SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_DB, -1, 0);
                    SolverGLPK.glp_set_row_name(lp, rownum, "equation37_" + rownum);


                    string source = graph1.ListEdges[ij - 1].NodeSource.Id;
                    string nameVar = "x_" + source + "," + graph2.ListNodes[k - 1].Id;
                    int colInd = SolverGLPK.glp_find_col(lp, nameVar);
                    if (colInd == 0)
                        throw new InvalidProgramException();

                    iaList.Add(rownum);
                    jaList.Add(colInd);
                    arList.Add(1);

                    nameVar = "u_" + source + ",Ins_" + source;
                    colInd = SolverGLPK.glp_find_col(lp, nameVar);
                    if (colInd == 0)
                        throw new InvalidProgramException();
                    iaList.Add(rownum);
                    jaList.Add(colInd);
                    arList.Add(1);

                    string name1 = graph1.ListEdges[ij - 1].Id;
                    foreach (Edge e in graph2.ListNodes[k - 1].ListEdgesOut)
                    {
                        string name2 = e.Id;
                        nameVar = "y_" + name1 + "," + name2;
                        colInd = SolverGLPK.glp_find_col(lp, nameVar);
                        if (colInd == 0)
                            throw new InvalidProgramException();

                        iaList.Add(rownum);
                        jaList.Add(colInd);
                        arList.Add(-1);
                    }

                    iaList.Add(rownum);
                    jaList.Add(numNodes1 * numNodes2 + numEdges1 * numEdges2 + numNodes1 + ij);
                    arList.Add(-1);
                }*/
            //equation 8 (If two vertices are matched together, 
            //an edge targeting one of these two vertices must be matched with an edge targeting the other vertex)
            for (int l = 1; l <= numNodes2; l++)
                for (int ij = 1; ij <= numEdges1; ij++)
                {
                    SolverGLPK.glp_add_rows(lp, 1);
                    SolverGLPK.glp_set_row_name(lp, ++rownum, "equation40_" + rownum);
                    SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_DB, 0, 1);

                    string target = graph1.ListEdges[ij - 1].NodeTarget.Id;
                    string nameVar = "x_" + target + "," + graph2.ListNodes[l - 1].Id;
                    int colInd = SolverGLPK.glp_find_col(lp, nameVar);
                    if (colInd == 0)
                        throw new InvalidProgramException();

                    iaList.Add(rownum);
                    jaList.Add(colInd);
                    arList.Add(1);

                    string name1 = graph1.ListEdges[ij - 1].Id;
                    foreach (Edge e in graph2.ListNodes[l - 1].ListEdgesIn)
                    {
                        string name2 = e.Id;
                        nameVar = "y_" + name1 + "," + name2;
                        colInd = SolverGLPK.glp_find_col(lp, nameVar);
                        if (colInd == 0)
                            throw new InvalidProgramException();

                        iaList.Add(rownum);
                        jaList.Add(colInd);
                        arList.Add(-1);
                    }

                }

          /*  //equation 9
            for (int l = 1; l <= numNodes2; l++)
                for (int ij = 1; ij <= numEdges1; ij++)
                {
                    SolverGLPK.glp_add_rows(lp, 1);
                    SolverGLPK.glp_set_row_name(lp, ++rownum, "equation39_" + rownum);
                    SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_DB, -1, 0);

                    string target = graph1.ListEdges[ij - 1].NodeTarget.Id;
                    string nameVar = "x_" + target + "," + graph2.ListNodes[l - 1].Id;
                    int colInd = SolverGLPK.glp_find_col(lp, nameVar);
                    if (colInd == 0)
                        throw new InvalidProgramException();

                    iaList.Add(rownum);
                    jaList.Add(colInd);
                    arList.Add(1);

                    nameVar = "u_" + target + ",Ins_" + target;
                    colInd = SolverGLPK.glp_find_col(lp, nameVar);
                    if (colInd == 0)
                        throw new InvalidProgramException();
                    iaList.Add(rownum);
                    jaList.Add(colInd);
                    arList.Add(1);

                    string name1 = graph1.ListEdges[ij - 1].Id;
                    foreach (Edge e in graph2.ListNodes[l - 1].ListEdgesIn)
                    {
                        string name2 = e.Id;
                        nameVar = "y_" + name1 + "," + name2;
                        colInd = SolverGLPK.glp_find_col(lp, nameVar);
                        if (colInd == 0)
                            throw new InvalidProgramException();

                        iaList.Add(rownum);
                        jaList.Add(colInd);
                        arList.Add(-1);
                    }

                    iaList.Add(rownum);
                    jaList.Add(numNodes1 * numNodes2 + numEdges1 * numEdges2 + numNodes1 + ij);
                    arList.Add(-1);

                }*/

            //10 If two vertices are matched together, 
            //an edge originating one of these two vertices must be matched with an edge originating the other vertex)
            for (int i = 1; i <= numNodes1; i++)
                for (int kl = 1; kl<= numEdges2; kl++)
                {
                    SolverGLPK.glp_add_rows(lp, 1);
                    rownum++;
                    SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_DB, 0, 1);
                    SolverGLPK.glp_set_row_name(lp, rownum, "equation10_" + rownum);


                    string source = graph2.ListEdges[kl - 1].NodeSource.Id;
                    string nameVar = "x_" + graph1.ListNodes[i - 1].Id+ ","+ source;
                    int colInd = SolverGLPK.glp_find_col(lp, nameVar);
                    if (colInd == 0)
                        throw new InvalidProgramException();

                    iaList.Add(rownum);
                    jaList.Add(colInd);
                    arList.Add(1);

                    string name1 = graph2.ListEdges[kl - 1].Id;
                    foreach (Edge e in graph1.ListNodes[i - 1].ListEdgesOut)
                    {
                        string name2 = e.Id;
                        nameVar = "y_" + name2 + "," + name1;
                        colInd = SolverGLPK.glp_find_col(lp, nameVar);
                        if (colInd == 0)
                            throw new InvalidProgramException();

                        iaList.Add(rownum);
                        jaList.Add(colInd);
                        arList.Add(-1);
                    }
                }

        /*    //11
            for (int i = 1; i <= numNodes1; i++)
                for (int kl = 1; kl <= numEdges2; kl++)
                {
                    SolverGLPK.glp_add_rows(lp, 1);
                    rownum++;
                    SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_DB, -1, 0);
                    SolverGLPK.glp_set_row_name(lp, rownum, "equation11_" + rownum);


                    string source = graph2.ListEdges[kl - 1].NodeSource.Id;
                    string nameVar = "x_" + graph1.ListNodes[i - 1].Id + "," + source;
                    int colInd = SolverGLPK.glp_find_col(lp, nameVar);
                    if (colInd == 0)
                        throw new InvalidProgramException();

                    iaList.Add(rownum);
                    jaList.Add(colInd);
                    arList.Add(1);

                    nameVar = "v_Del_" + source + "," + source;
                    colInd = SolverGLPK.glp_find_col(lp, nameVar);
                    if (colInd == 0)
                        throw new InvalidProgramException();
                    iaList.Add(rownum);
                    jaList.Add(colInd);
                    arList.Add(1);

                    string name1 = graph2.ListEdges[kl - 1].Id;
                    foreach (Edge e in graph1.ListNodes[i - 1].ListEdgesOut)
                    {
                        string name2 = e.Id;
                        nameVar = "y_" + name2 + "," + name1;
                        colInd = SolverGLPK.glp_find_col(lp, nameVar);
                        if (colInd == 0)
                            throw new InvalidProgramException();

                        iaList.Add(rownum);
                        jaList.Add(colInd);
                        arList.Add(-1);
                    }

                    iaList.Add(rownum);
                    jaList.Add(numNodes1 * numNodes2 + numEdges1 * numEdges2 + numNodes1 + numEdges1 + numNodes2 + kl);
                    arList.Add(-1);
                }*/

            //equation 12 (If two vertices are matched together, 
            //an edge targeting one of these two vertices must be matched with an edge targeting the other vertex)
            for (int j = 1; j <= numNodes1; j++)
                for (int kl = 1; kl <= numEdges2; kl++)
                {
                    SolverGLPK.glp_add_rows(lp, 1);
                    SolverGLPK.glp_set_row_name(lp, ++rownum, "equation12_" + rownum);
                    SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_DB, 0, 1);

                    string target = graph2.ListEdges[kl - 1].NodeTarget.Id;
                    string nameVar = "x_" + graph1.ListNodes[j - 1].Id + "," + target;
                    int colInd = SolverGLPK.glp_find_col(lp, nameVar);
                    if (colInd == 0)
                        throw new InvalidProgramException();

                    iaList.Add(rownum);
                    jaList.Add(colInd);
                    arList.Add(1);

                    string name1 = graph2.ListEdges[kl - 1].Id;
                    foreach (Edge e in graph1.ListNodes[j- 1].ListEdgesIn)
                    {
                        string name2 = e.Id;
                        nameVar = "y_" + name2 + "," + name1;
                        colInd = SolverGLPK.glp_find_col(lp, nameVar);
                        if (colInd == 0)
                            throw new InvalidProgramException();

                        iaList.Add(rownum);
                        jaList.Add(colInd);
                        arList.Add(-1);
                    }

                }

            //equation 13
       /*     for (int j = 1; j <= numNodes1; j++)
                for (int kl = 1; kl <= numEdges2; kl++)
                {
                    SolverGLPK.glp_add_rows(lp, 1);
                    SolverGLPK.glp_set_row_name(lp, ++rownum, "equation9_" + rownum);
                    SolverGLPK.glp_set_row_bnds(lp, rownum, SolverGLPK.GLP_DB, -1, 0);

                    string target = graph2.ListEdges[kl - 1].NodeTarget.Id;
                    string nameVar = "x_" + graph1.ListNodes[j - 1].Id + "," + target;
                    int colInd = SolverGLPK.glp_find_col(lp, nameVar);
                    if (colInd == 0)
                        throw new InvalidProgramException();

                    iaList.Add(rownum);
                    jaList.Add(colInd);
                    arList.Add(1);

                    nameVar = "v_Del_" + target + "," + target;
                    colInd = SolverGLPK.glp_find_col(lp, nameVar);
                    if (colInd == 0)
                        throw new InvalidProgramException();
                    iaList.Add(rownum);
                    jaList.Add(colInd);
                    arList.Add(1);

                    string name1 = graph2.ListEdges[kl - 1].Id;
                    foreach (Edge e in graph1.ListNodes[j - 1].ListEdgesIn)
                    {
                        string name2 = e.Id;
                        nameVar = "y_" + name2 + "," + name1;
                        colInd = SolverGLPK.glp_find_col(lp, nameVar);
                        if (colInd == 0)
                            throw new InvalidProgramException();

                        iaList.Add(rownum);
                        jaList.Add(colInd);
                        arList.Add(-1);
                    }

                    iaList.Add(rownum);
                    jaList.Add(numNodes1 * numNodes2 + numEdges1 * numEdges2 + numNodes1 + numEdges1 + numNodes2 + kl);
                    arList.Add(-1);

                }

           
            */
            //declare the contraints matrix
            ConstraintMatrix CM = new ConstraintMatrix();
            int numEle = arList.Count() - 1;
            CM.ia = iaList.ToArray();
            CM.ja = jaList.ToArray();
            CM.ar = arList.ToArray();
            SolverGLPK.glp_load_matrix(lp, numEle, CM.ia, CM.ja, CM.ar);
        }

        public override void BLPjusticehero()
        {
        }

        public override void BLPjusticeheroQuadratic()
        {
        }

        public override void QAPGMGED()
        { 
        }

    }
}

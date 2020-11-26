using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ILOG.Concert;
using ILOG.CPLEX;
using Graphs;
using Matching;


namespace LP
{
    public class ProblemCPLEX : Problem
    {
        public int nbRows = 0;
        public int nbCols = 0;
        public int nbNzeros = 0;
        int nbNode1;
        int nbNode2;
        int nbEdge1;
        int nbEdge2;
        bool copyEdge;

        public Cplex cplex;
        public ILPMatrix ilpMatrix;
        public double[] objCoef;

        public ProblemCPLEX(Graph g1, Graph g2)
        {
            this.graph1 = g1;
            this.graph2 = g2;
            exchange = false;
            nbNode1 = g1.ListNodes.Count;
            nbNode2 = g2.ListNodes.Count;
            nbEdge1 = g1.ListEdges.Count;
            nbEdge2 = g2.ListEdges.Count;
            if (!g1.IsDirected) copyEdge = true;
            else copyEdge = false;
        }

        public override void BLPjusticeheroQuadratic()
        {
        }

        public override void IsoSousGraphExactF1()
        {
           
        }
        public override void IsoSousGraphExactF2()
        {
           

        }
        public override void IsoSousGraphInexactF1a()
        {
           
        }

        public override void BLPjusticehero()
        {

        }
        public override void IsoSousGraphInexactF2a()
        {
            


        }
        public override void IsoGraphInexactF1b()
        {
            this.nbRows = nbNode1 + nbEdge1 + nbNode2 + nbEdge2 + 3 * nbEdge1 * nbEdge2; //ns + ms+ng+mg+3msmg
            this.nbCols = nbNode1 * nbNode2 + nbEdge1 * nbEdge2 + nbNode1 + nbEdge1 + nbNode2 + nbEdge2;//nsng+msmg+ns+ms+ng+mg
            Graphs.Label nodeepslabel;

            #region objectFunction
            List<double> objList = new List<double>();
            List<string> colNameList = new List<string>();
            List<char> typeList = new List<char>();
            List<Double> ubList = new List<Double>();
            //the objet funcion
            for (int i = 1; i <= nbNode1; i++)
            {
                for (int k = 1; k <= nbNode2; k++)
                {
                    objList.Add(graph1.ListNodes[i - 1].Label.dissimilarity(graph2.ListNodes[k - 1].Label));
                    colNameList.Add("x_" + graph1.ListNodes[i - 1].Id + "," + graph2.ListNodes[k - 1].Id);
                }
            }

            for (int ij = 1; ij <= nbEdge1; ij++)
                for (int kl = 1; kl <= nbEdge2; kl++)
                {
                    double costEdge = graph1.ListEdges[ij - 1].Label.dissimilarity(graph2.ListEdges[kl - 1].Label);
                    if (copyEdge)
                        objList.Add(costEdge / 2);
                    else objList.Add(costEdge);
                    colNameList.Add("y_" + graph1.ListEdges[ij - 1].Id + "," + graph2.ListEdges[kl - 1].Id);
                }
            for (int i = 1; i <= nbNode1; i++)
            {
                Type typeLabel = graph1.ListNodes[i - 1].Label.GetType();
                object obj = Activator.CreateInstance(typeLabel);
                nodeepslabel = (Graphs.Label)obj;
                nodeepslabel.Id = ConstantsAC.EPS_ID;

                objList.Add((graph1.ListNodes[i - 1].Label).dissimilarity(nodeepslabel));
                colNameList.Add("u_" + graph1.ListNodes[i - 1].Id + ",Ins_" + graph1.ListNodes[i - 1].Id);
            }

            for (int ij = 1; ij <= nbEdge1; ij++)
            {
                Type typeLabel = graph1.ListEdges[ij - 1].Label.GetType();
                object obj = Activator.CreateInstance(typeLabel);
                nodeepslabel = (Graphs.Label)obj;
                nodeepslabel.Id = ConstantsAC.EPS_ID;
                double costEdge = (graph1.ListEdges[ij - 1].Label).dissimilarity(nodeepslabel);
                if (copyEdge)
                    objList.Add(costEdge / 2);
                else objList.Add(costEdge);
                colNameList.Add("e_" + graph1.ListEdges[ij - 1].Id + ",Ins_" + graph1.ListEdges[ij - 1].Id);
            }

            for (int k = 1; k <= nbNode2; k++)
            {
                Type typeLabel = graph2.ListNodes[k - 1].Label.GetType();
                object obj = Activator.CreateInstance(typeLabel);
                nodeepslabel = (Graphs.Label)obj;
                nodeepslabel.Id = ConstantsAC.EPS_ID;
                objList.Add((graph2.ListNodes[k - 1].Label).dissimilarity(nodeepslabel));
                colNameList.Add("v_Del_" + graph2.ListNodes[k - 1].Id + "," + graph2.ListNodes[k - 1].Id);
            }

            for (int kl = 1; kl <= nbEdge2; kl++)
            {
                Type typeLabel = graph2.ListEdges[kl - 1].Label.GetType();
                object obj = Activator.CreateInstance(typeLabel);
                nodeepslabel = (Graphs.Label)obj;
                nodeepslabel.Id = ConstantsAC.EPS_ID;
                double costEdge = (graph2.ListEdges[kl - 1].Label).dissimilarity(nodeepslabel);
                if (copyEdge)
                    objList.Add(costEdge / 2);
                else objList.Add(costEdge);
                colNameList.Add("f_Del_" + graph2.ListEdges[kl - 1].Id + "," + graph2.ListEdges[kl - 1].Id);
            }
            #endregion

            try
            {
                cplex = new Cplex();
                ilpMatrix = cplex.AddLPMatrix();

                // add empty corresponding to new variables columns to ilpMatrix
                INumVar[] x = cplex.NumVarArray(cplex.ColumnArray(ilpMatrix, nbCols), 0, 1, NumVarType.Bool, colNameList.ToArray());

                List<Double> lbMatrixList = new List<Double>();
                List<Double> ubMatrixList = new List<Double>();
                Int32[][] indiceH = new Int32[nbRows][];
                Double[][] valeurH = new Double[nbRows][];
                List<Int32> jaList;//les indice des valeurs
                List<Double> arList;//les valeurs non zeros dans la ligne

                int rownum = 0;
                #region construir constraintes
                for (int i = 0; i < nbNode1; i++)
                {
                    jaList = new List<int>();
                    arList = new List<Double>();
                    lbMatrixList.Add(1.0);
                    ubMatrixList.Add(1.0);

                    for (int k = 0; k < nbNode2; k++)
                    {

                        jaList.Add(i * nbNode2 + k);
                        arList.Add(1);
                    }
                    jaList.Add(nbNode1 * nbNode2 + nbEdge1 * nbEdge2 + i);
                    arList.Add(1);
                    indiceH[rownum] = jaList.ToArray();
                    valeurH[rownum] = arList.ToArray();
                    rownum++;
                }


                // equation 3 
                for (int ij = 0; ij < nbEdge1; ij++)
                {
                    jaList = new List<int>();
                    arList = new List<Double>();
                    lbMatrixList.Add(1.0);
                    ubMatrixList.Add(1.0);
                    for (int kl = 0; kl < nbEdge2; kl++)
                    {
                        jaList.Add(nbNode1 * nbNode2 + ij * nbEdge2 + kl);
                        arList.Add(1);
                    }
                    jaList.Add(nbNode1 * nbNode2 + nbEdge1 * nbEdge2 + nbNode1 + ij);
                    arList.Add(1);

                    indiceH[rownum] = jaList.ToArray();
                    valeurH[rownum] = arList.ToArray();
                    rownum++;
                }

                // contraint: equation [Fb.1]-4
                for (int k = 0; k < nbNode2; k++)
                {
                    jaList = new List<int>();
                    arList = new List<Double>();
                    lbMatrixList.Add(1.0);
                    ubMatrixList.Add(1.0);
                    for (int i = 0; i < nbNode1; i++)
                    {
                        jaList.Add(i * nbNode2 + k);
                        arList.Add(1);
                    }
                    jaList.Add(nbNode1 * nbNode2 + nbEdge1 * nbEdge2 + nbNode1 + nbEdge1 + k);
                    arList.Add(1);

                    indiceH[rownum] = jaList.ToArray();
                    valeurH[rownum] = arList.ToArray();
                    rownum++;
                }


                // equation 5
                for (int kl = 0; kl < nbEdge2; kl++)
                {
                    jaList = new List<int>();//les indice des valeurs
                    arList = new List<Double>();//les valeurs non zeros dans la ligne
                    lbMatrixList.Add(1.0);
                    ubMatrixList.Add(1.0);

                    for (int ij = 0; ij < nbEdge1; ij++)
                    {
                        jaList.Add(nbNode1 * nbNode2 + ij * nbEdge2 + kl);
                        arList.Add(1);
                    }
                    jaList.Add(nbNode1 * nbNode2 + nbEdge1 * nbEdge2 + nbNode1 + nbEdge1 + nbNode2 + kl);
                    arList.Add(1);

                    indiceH[rownum] = jaList.ToArray();
                    valeurH[rownum] = arList.ToArray();
                    rownum++;

                }


                //equation 6 7 8
                for (int ij = 0; ij < nbEdge1; ij++)
                    for (int kl = 0; kl < nbEdge2; kl++)
                    {
                        string source_i = graph1.ListEdges[ij].NodeSource.Id;
                        string source_k = graph2.ListEdges[kl].NodeSource.Id;
                        string target_i = graph1.ListEdges[ij].NodeTarget.Id;
                        string target_k = graph2.ListEdges[kl].NodeTarget.Id;

                        string nameVar = "x_" + source_i + "," + source_k;
                        int colInd = SolverCPLEX.GetIndexByName(x, nameVar);
                        if (colInd == -1)
                            throw new InvalidProgramException();

                        string nameVar2 = "x_" + target_i + "," + target_k;
                        int colInd2 = SolverCPLEX.GetIndexByName(x, nameVar2);
                        if (colInd2 == -1)
                            throw new InvalidProgramException();

                        jaList = new List<int>();
                        arList = new List<Double>();
                        lbMatrixList.Add(0.0);
                        ubMatrixList.Add(1.0);
                        jaList.Add(colInd);
                        arList.Add(1);
                        jaList.Add(nbNode1 * nbNode2 + ij * nbEdge2 + kl);
                        arList.Add(-1);
                        indiceH[rownum] = jaList.ToArray();
                        valeurH[rownum] = arList.ToArray();
                        rownum++;

                        ////////////////////////////////
                        jaList = new List<int>();
                        arList = new List<Double>();
                        lbMatrixList.Add(0.0);
                        ubMatrixList.Add(1.0);


                        jaList.Add(colInd2);
                        arList.Add(1);
                        jaList.Add(nbNode1 * nbNode2 + ij * nbEdge2 + kl);
                        arList.Add(-1);

                        indiceH[rownum] = jaList.ToArray();
                        valeurH[rownum] = arList.ToArray();
                        rownum++;

                        ////////////////////////////////////////
                       /* jaList = new List<int>();
                        arList = new List<Double>();
                        lbMatrixList.Add(-1.0);
                        ubMatrixList.Add(1.0);

                        jaList.Add(colInd);
                        arList.Add(1);
                        jaList.Add(colInd2);
                        arList.Add(1);
                        jaList.Add(nbNode1 * nbNode2 + ij * nbEdge2 + kl);
                        arList.Add(-1);

                        indiceH[rownum] = jaList.ToArray();
                        valeurH[rownum] = arList.ToArray();
                        rownum++;*/
                    }
                #endregion
                double[] lb = lbMatrixList.ToArray();
                double[] ub = ubMatrixList.ToArray();

                Int32 res = ilpMatrix.AddRows(lb, ub, indiceH, valeurH);

                // add the objective function
                objCoef = objList.ToArray();
                cplex.AddMinimize(cplex.ScalProd(x, objCoef));
            }
            catch (ILOG.Concert.Exception e)
            {
                System.Console.WriteLine("Concert exception '" + e + "' caught");
            }
        }
        public override void IsoGraphInexactF2b()
        {
            this.nbRows = nbNode1 + nbNode2 + 2*nbNode2 * nbEdge1; //ns +ms+ng+mg+4ngms +4nsmg
            this.nbCols = nbNode1 * nbNode2 + nbEdge1 * nbEdge2 + nbNode1 + nbEdge1 + nbNode2 + nbEdge2;//nsng+msmg+ns+ms+ng+mg

            Graphs.Label nodeepslabel;

            #region objectFunction
            List<double> objList = new List<double>();
            List<string> colNameList = new List<string>();
            List<char> typeList = new List<char>();
            List<Double> ubList = new List<Double>();
            //the objet funcion
            for (int i = 1; i <= nbNode1; i++)
            {
                for (int k = 1; k <= nbNode2; k++)
                {
                    objList.Add(graph1.ListNodes[i - 1].Label.dissimilarity(graph2.ListNodes[k - 1].Label));
                    colNameList.Add("x_" + graph1.ListNodes[i - 1].Id + "," + graph2.ListNodes[k - 1].Id);
                }
            }

            for (int ij = 1; ij <= nbEdge1; ij++)
                for (int kl = 1; kl <= nbEdge2; kl++)
                {
                    double costEdge = graph1.ListEdges[ij - 1].Label.dissimilarity(graph2.ListEdges[kl - 1].Label);
                    if (copyEdge)
                        objList.Add(costEdge / 2);
                    else objList.Add(costEdge);
                    colNameList.Add("y_" + graph1.ListEdges[ij - 1].Id + "," + graph2.ListEdges[kl - 1].Id);
                }
            for (int i = 1; i <= nbNode1; i++)
            {
                Type typeLabel = graph1.ListNodes[i - 1].Label.GetType();
                object obj = Activator.CreateInstance(typeLabel);
                nodeepslabel = (Graphs.Label)obj;
                nodeepslabel.Id = ConstantsAC.EPS_ID;

                objList.Add((graph1.ListNodes[i - 1].Label).dissimilarity(nodeepslabel));
                colNameList.Add("u_" + graph1.ListNodes[i - 1].Id + ",Ins_" + graph1.ListNodes[i - 1].Id);
            }

            for (int ij = 1; ij <= nbEdge1; ij++)
            {
                Type typeLabel = graph1.ListEdges[ij - 1].Label.GetType();
                object obj = Activator.CreateInstance(typeLabel);
                nodeepslabel = (Graphs.Label)obj;
                nodeepslabel.Id = ConstantsAC.EPS_ID;
                double costEdge = (graph1.ListEdges[ij - 1].Label).dissimilarity(nodeepslabel);
                if (copyEdge)
                    objList.Add(costEdge / 2);
                else objList.Add(costEdge);
                colNameList.Add("e_" + graph1.ListEdges[ij - 1].Id + ",Ins_" + graph1.ListEdges[ij - 1].Id);
            }

            for (int k = 1; k <= nbNode2; k++)
            {
                Type typeLabel = graph2.ListNodes[k - 1].Label.GetType();
                object obj = Activator.CreateInstance(typeLabel);
                nodeepslabel = (Graphs.Label)obj;
                nodeepslabel.Id = ConstantsAC.EPS_ID;
                objList.Add((graph2.ListNodes[k - 1].Label).dissimilarity(nodeepslabel));
                colNameList.Add("v_Del_" + graph2.ListNodes[k - 1].Id + "," + graph2.ListNodes[k - 1].Id);
            }

            for (int kl = 1; kl <= nbEdge2; kl++)
            {
                Type typeLabel = graph2.ListEdges[kl - 1].Label.GetType();
                object obj = Activator.CreateInstance(typeLabel);
                nodeepslabel = (Graphs.Label)obj;
                nodeepslabel.Id = ConstantsAC.EPS_ID;
                double costEdge = (graph2.ListEdges[kl - 1].Label).dissimilarity(nodeepslabel);
                if (copyEdge)
                    objList.Add(costEdge / 2);
                else objList.Add(costEdge);
                colNameList.Add("f_Del_" + graph2.ListEdges[kl - 1].Id + "," + graph2.ListEdges[kl - 1].Id);
            }
            #endregion

            try
            {
                cplex = new Cplex();
                ilpMatrix = cplex.AddLPMatrix();

                // add empty corresponding to new variables columns to ilpMatrix
                INumVar[] x = cplex.NumVarArray(cplex.ColumnArray(ilpMatrix, nbCols), 0, 1, NumVarType.Bool, colNameList.ToArray());
                //INumVar[] x = cplex.NumVarArray(cplex.ColumnArray(ilpMatrix, nbCols), 0, 1, NumVarType.Int, colNameList.ToArray());

                List<Double> lbMatrixList = new List<Double>();
                List<Double> ubMatrixList = new List<Double>();
                Int32[][] indiceH = new Int32[nbRows][];
                Double[][] valeurH = new Double[nbRows][];
                List<Int32> jaList;//les indice des valeurs
                List<Double> arList;//les valeurs non zeros dans la ligne

                int rownum = 0;
                #region construir constraintes
                for (int i = 0; i < nbNode1; i++)
                {
                    jaList = new List<int>();
                    arList = new List<Double>();
                    lbMatrixList.Add(1.0);
                    ubMatrixList.Add(1.0);

                    for (int k = 0; k < nbNode2; k++)
                    {

                        jaList.Add(i * nbNode2 + k);
                        arList.Add(1);
                    }
                    jaList.Add(nbNode1 * nbNode2 + nbEdge1 * nbEdge2 + i);
                    arList.Add(1);
                    indiceH[rownum] = jaList.ToArray();
                    valeurH[rownum] = arList.ToArray();
                    rownum++;
                }


                

                // contraint: equation [Fb.1]-4
                for (int k = 0; k < nbNode2; k++)
                {
                    jaList = new List<int>();
                    arList = new List<Double>();
                    lbMatrixList.Add(1.0);
                    ubMatrixList.Add(1.0);
                    for (int i = 0; i < nbNode1; i++)
                    {
                        jaList.Add(i * nbNode2 + k);
                        arList.Add(1);
                    }
                    jaList.Add(nbNode1 * nbNode2 + nbEdge1 * nbEdge2 + nbNode1 + nbEdge1 + k);
                    arList.Add(1);

                    indiceH[rownum] = jaList.ToArray();
                    valeurH[rownum] = arList.ToArray();
                    rownum++;
                }


                


                //6 If two vertices are matched together, 
                //an edge originating one of these two vertices must be matched with an edge originating the other vertex)
                for (int k = 0; k < nbNode2; k++)
                    for (int ij = 0; ij < nbEdge1; ij++)
                    {
                        jaList = new List<int>();//les indice des valeurs
                        arList = new List<Double>();//les valeurs non zeros dans la ligne
                        lbMatrixList.Add(0.0);
                        ubMatrixList.Add(1.0);


                        string source = graph1.ListEdges[ij].NodeSource.Id;
                        string nameVar = "x_" + source + "," + graph2.ListNodes[k].Id;
                        int colInd = SolverCPLEX.GetIndexByName(x, nameVar);
                        if (colInd == -1)
                            throw new InvalidProgramException();

                        jaList.Add(colInd);
                        arList.Add(1);


                        string name1 = graph1.ListEdges[ij].Id;
                        foreach (Edge e in graph2.ListNodes[k].ListEdgesOut)
                        {
                            string name2 = e.Id;
                            nameVar = "y_" + name1 + "," + name2;
                            colInd = SolverCPLEX.GetIndexByName(x, nameVar);
                            if (colInd == -1)
                                throw new InvalidProgramException();
                            jaList.Add(colInd);
                            arList.Add(-1);
                        }

                        indiceH[rownum] = jaList.ToArray();
                        valeurH[rownum] = arList.ToArray();
                        rownum++;
                    }

           
                //equation 8 (If two vertices are matched together, 
                //an edge targeting one of these two vertices must be matched with an edge targeting the other vertex)
                for (int l = 0; l < nbNode2; l++)
                    for (int ij = 0; ij < nbEdge1; ij++)
                    {
                        jaList = new List<int>();//les indice des valeurs
                        arList = new List<Double>();//les valeurs non zeros dans la ligne
                        lbMatrixList.Add(0.0);
                        ubMatrixList.Add(1.0);

                        string target = graph1.ListEdges[ij].NodeTarget.Id;
                        string nameVar = "x_" + target + "," + graph2.ListNodes[l].Id;
                        int colInd = SolverCPLEX.GetIndexByName(x, nameVar);
                        if (colInd == -1)
                            throw new InvalidProgramException();
                        jaList.Add(colInd);
                        arList.Add(1);

                        string name1 = graph1.ListEdges[ij].Id;
                        foreach (Edge e in graph2.ListNodes[l].ListEdgesIn)
                        {
                            string name2 = e.Id;
                            nameVar = "y_" + name1 + "," + name2;
                            colInd = SolverCPLEX.GetIndexByName(x, nameVar);
                            if (colInd == -1)
                                throw new InvalidProgramException();
                            jaList.Add(colInd);
                            arList.Add(-1);
                        }
                        indiceH[rownum] = jaList.ToArray();
                        valeurH[rownum] = arList.ToArray();
                        rownum++;

                    }

              

              

             
                #endregion
                Int32 res = ilpMatrix.AddRows(lbMatrixList.ToArray(), ubMatrixList.ToArray(), indiceH, valeurH);

                // add the objective function
                objCoef = objList.ToArray();
                cplex.AddMinimize(cplex.ScalProd(x, objCoef));
            }
            catch (ILOG.Concert.Exception e)
            {
                System.Console.WriteLine("Concert exception '" + e + "' caught");
            }


        }


        //A quadratic interger program for graph matching,
        //When tuned with the right similarity function it can represent the GED problem.
        // See Pattern Reocgnition Paper paper On the unification of graph matching and graph edit distance paper.
        //The directed version has not been tested yet
        public override void QAPGMGED()
        {
            //some important variables
            int nbNode1 = graph1.ListNodes.Count;
            int nbNode2 = graph2.ListNodes.Count;
            int nbvar = nbNode1 * nbNode2; // number of variables
            int nbcontraintes = nbNode1 + nbNode2; // number of constraints
            Graphs.Label nodeepslabel;

            //Adjacency matrices
            MatrixLibrary.Matrix adj1 = graph1.calculateAdjacencyMatrix();
            MatrixLibrary.Matrix adj2 = graph2.calculateAdjacencyMatrix();
            //Similarity matrix
            Double[,] S = new Double[nbvar, nbvar];

            //Creation of the Similarity matrix
            //4 for loops integrated
            int countrow = -1;
            for (int i = 0; i < nbNode1; i++)
            {
                Node nodei = graph1.ListNodes[i];
                for (int k = 0; k < nbNode2; k++)
                {
                    Node nodek = graph2.ListNodes[k];
                    countrow = countrow + 1;
                    int countcol = -1;
                    for (int j = 0; j < nbNode1; j++)
                    {
                        Node nodej = graph1.ListNodes[j];
                        for (int l = 0; l < nbNode2; l++)
                        {
                            Node nodel = graph2.ListNodes[l];
                            countcol = countcol + 1;
                            if (i == j && k == l) // Similarity between nodes
                            {

                                Type typeLabel = nodei.Label.GetType();
                                object obj = Activator.CreateInstance(typeLabel);
                                nodeepslabel = (Graphs.Label)obj;
                                nodeepslabel.Id = ConstantsAC.EPS_ID;

                                double costsub = nodei.Label.dissimilarity(nodek.Label);
                                double costdel = nodei.Label.dissimilarity(nodeepslabel);
                                double costins = nodeepslabel.dissimilarity(nodek.Label);
                                double cost = costsub - costdel - costins;
                                cost *= -1;

                                S[countrow, countcol] = cost;


                            }
                            else // Similarity between edges
                            {
                                int connect1 = (int)adj1[i, j];
                                int connect2 = (int)adj2[k, l];
                                if (connect1 == 1 && connect2 == 1) // Two edges are connected
                                {
                                    Edge ij = findedge(nodei, nodej);
                                    Edge kl = findedge(nodek, nodel);


                                    Type typeLabel = ij.Label.GetType();
                                    object obj = Activator.CreateInstance(typeLabel);
                                    nodeepslabel = (Graphs.Label)obj;
                                    nodeepslabel.Id = ConstantsAC.EPS_ID;

                                    double costsub = ij.Label.dissimilarity(kl.Label);
                                    double costdel = ij.Label.dissimilarity(nodeepslabel);
                                    double costins = nodeepslabel.dissimilarity(kl.Label);
                                    double cost = costsub - costdel - costins;
                                    cost *= -1;
                                    //cost *= 0.5;
                                    S[countrow, countcol] = cost;


                                }


                            }

                        }
                    }
                }
            }


            //We compute the constant that represents the
            //deletion and insertion of the nodes and edges

            //deletions of the nodes of g1
            double constante = 0;
            for (int i = 1; i <= nbNode1; i++)
            {
                Type typeLabel = graph1.ListNodes[i - 1].Label.GetType();
                object obj = Activator.CreateInstance(typeLabel);
                nodeepslabel = (Graphs.Label)obj;
                nodeepslabel.Id = ConstantsAC.EPS_ID;
                constante += (graph1.ListNodes[i - 1].Label).dissimilarity(nodeepslabel);

            }

            //deletions of the edges of g1
            for (int ij = 1; ij <= nbEdge1; ij++)
            {
                Type typeLabel = graph1.ListEdges[ij - 1].Label.GetType();
                object obj = Activator.CreateInstance(typeLabel);
                nodeepslabel = (Graphs.Label)obj;
                nodeepslabel.Id = ConstantsAC.EPS_ID;
                double costEdge = (graph1.ListEdges[ij - 1].Label).dissimilarity(nodeepslabel);
                constante += costEdge;
            }

            //insertions of the nodes of g2
            for (int k = 1; k <= nbNode2; k++)
            {
                Type typeLabel = graph2.ListNodes[k - 1].Label.GetType();
                object obj = Activator.CreateInstance(typeLabel);
                nodeepslabel = (Graphs.Label)obj;
                nodeepslabel.Id = ConstantsAC.EPS_ID;
                constante += (graph2.ListNodes[k - 1].Label).dissimilarity(nodeepslabel);

            }

            //insertions of the edges of g2
            for (int kl = 1; kl <= nbEdge2; kl++)
            {
                Type typeLabel = graph2.ListEdges[kl - 1].Label.GetType();
                object obj = Activator.CreateInstance(typeLabel);
                nodeepslabel = (Graphs.Label)obj;
                nodeepslabel.Id = ConstantsAC.EPS_ID;
                double costEdge = (graph2.ListEdges[kl - 1].Label).dissimilarity(nodeepslabel);
                constante += costEdge;


            }

            // the number of variables is the number of columns 
            int nbCols = nbvar;
            // the number of constraints is the number of rows 
            int nbRows = nbcontraintes;
            List<string> colNameList = new List<string>();


            //We create the name of the vrariables for all couples of nodes in g1 and g2 
            for (int i = 0; i < nbNode1; i++)
            {
                for (int k = 0; k < nbNode2; k++)
                {
                    colNameList.Add("x_" + graph1.ListNodes[i].Id + "," + graph2.ListNodes[k].Id + "");
                }
            }
            try
            {
                cplex = new Cplex(); // creation du modèle CPLEX

                this.ilpMatrix = cplex.AddLPMatrix();  // matrice du pb (initialisée à 0 colonnes et 0 lignes)

                ColumnArray colonnes = cplex.ColumnArray(ilpMatrix, nbCols); // définition des variables-colonnes du pb

                // ajout des variables-colonnes dans la matrice du pb 
                // y is a vector
                INumVar[] y = cplex.NumVarArray(colonnes, 0, 1, NumVarType.Bool, colNameList.ToArray());


                #region création fonction objectif

                // CALCUL DES COEFFICIENTS 
                // We create the ojective function
                INumExpr[] coeffs_expr = new INumExpr[nbvar * nbvar]; // vecteur des coeffs des coûts des arrêtes

                int count = 0;
                for (int ik = 0; ik < nbvar; ik++)
                {
                    for (int jl = 0; jl < nbvar; jl++)
                    {
                        coeffs_expr[count] = cplex.Prod(y[ik], y[jl]);
                        coeffs_expr[count] = cplex.Prod(S[ik, jl], coeffs_expr[count]);
                        count = count + 1;
                    }
                }
                INumExpr ff = cplex.Sum(coeffs_expr);
                INumExpr gg = cplex.Sum(ff, cplex.Constant(-constante));
                cplex.AddMaximize(gg);
                #endregion


                #region définition des contraintes du pb

                // We create the constraints

                // The sum of x variables by rows

                for (int i = 0; i < nbNode1; i++)
                {
                    INumVar[] sommeLignes = new INumVar[nbNode2];
                    for (int k = 0; k < nbNode2; k++)
                    {
                        string nameVarik = "x_" + i + "," + k;
                        int indexik = SolverCPLEX.GetIndexByName(y, nameVarik);
                        sommeLignes[k] = y[indexik];

                    }
                    cplex.AddLe(cplex.Sum(sommeLignes), 1);

                }

                // The sum of x variables by columns
                for (int k = 0; k < nbNode2; k++)
                {
                    INumVar[] sommeLignes = new INumVar[nbNode1];
                    for (int i = 0; i < nbNode1; i++)
                    {
                        string nameVarik = "x_" + i + "," + k;
                        int indexik = SolverCPLEX.GetIndexByName(y, nameVarik);
                        sommeLignes[i] = y[indexik];
                    }
                    cplex.AddLe(cplex.Sum(sommeLignes), 1);

                }




                #endregion

            }
            catch (ILOG.Concert.Exception e)
            {
                System.Console.WriteLine("Concert exception `" + e + "` caught");
            }


        }

        private Edge findedge(Node nodei, Node nodej)
        {
            Edge e = null;
            foreach (Edge etmp in nodei.ListEdgesOut)
            {
                if (etmp.NodeTarget.Id == nodej.Id)
                {
                    e = etmp;
                    return e;
                }
            }

           
            throw new NotImplementedException("Could not find the edge !!!! not normal");
        }
    }
}

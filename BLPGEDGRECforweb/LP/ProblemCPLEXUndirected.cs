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
    public class ProblemCPLEXUndirected : Problem
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

        public ProblemCPLEXUndirected(Graph g1, Graph g2)
        {
            this.graph1 = g1;
            this.graph2 = g2;
            exchange = false;
            nbNode1 = g1.ListNodes.Count;
            nbNode2 = g2.ListNodes.Count;
            nbEdge1 = g1.ListEdges.Count;
            nbEdge2 = g2.ListEdges.Count;
            
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
        public override void IsoSousGraphInexactF2a()
        {
            



        }

        //Formlation F1 for the GED problem. Very expressive form.
        // https://hal.archives-ouvertes.fr/hal-01619313

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

                //18.B
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
                //18.d
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
                //
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
                        string target_j = graph1.ListEdges[ij].NodeTarget.Id;
                        string target_l = graph2.ListEdges[kl].NodeTarget.Id;

                        //string eij ="e_" + graph1.ListEdges[ij - 1].Id + ",Ins_" + graph1.ListEdges[ij - 1].Id;
                        //string fkl = "f_Del_" + graph2.ListEdges[kl - 1].Id + "," + graph2.ListEdges[kl - 1].Id;

                        string nameVar = "x_" + source_i + "," + source_k;
                        int colIndxik = SolverCPLEX.GetIndexByName(x, nameVar);
                        if (colIndxik == -1)
                            throw new InvalidProgramException();

                        string nameVar2 = "x_" + target_j + "," + target_l;
                        int colInd2xjl = SolverCPLEX.GetIndexByName(x, nameVar2);
                        if (colInd2xjl == -1)
                            throw new InvalidProgramException();

                        string nameVar3 = "x_" + source_i + "," + target_l;
                        int colIndxil = SolverCPLEX.GetIndexByName(x, nameVar3);
                        if (colIndxil == -1)
                            throw new InvalidProgramException();

                        string nameVar4 = "x_" + target_j + "," + source_k;
                        int colIndxjk = SolverCPLEX.GetIndexByName(x, nameVar4);
                        if (colIndxjk == -1)
                            throw new InvalidProgramException();



                        ////////////////////////////////
                        jaList = new List<int>();
                        arList = new List<Double>();
                        lbMatrixList.Add(0.0);
                        ubMatrixList.Add(2.0);
                        jaList.Add(colIndxik);
                        arList.Add(1);
                        jaList.Add(colIndxil);
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
                        ubMatrixList.Add(2.0);
                        jaList.Add(colInd2xjl);
                        arList.Add(1);
                        jaList.Add(colIndxjk);
                        arList.Add(1);
                        jaList.Add(nbNode1 * nbNode2 + ij * nbEdge2 + kl);
                        arList.Add(-1);

                        indiceH[rownum] = jaList.ToArray();
                        valeurH[rownum] = arList.ToArray();
                        rownum++;

                        
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



        //F2 version with constant and slack variables
        ////Formlation F2 for the GED problem. Very efficient.
        // https://hal.archives-ouvertes.fr/hal-01619313

        public override void IsoGraphInexactF2b()
        {
            int mincst = Math.Min((nbNode2 * nbEdge1), (nbNode1 * nbEdge2));

            this.nbRows = nbNode1 + nbNode2 + 1 * mincst; //ns +ms+ng+mg+4ngms +4nsmg /contrainte
            this.nbCols = nbNode1 * nbNode2 + nbEdge1 * nbEdge2;//nsng+msmg+ns+ms+ng+mg //variable

            Graphs.Label nodeepslabel;

            #region objectFunction
            List<double> objList = new List<double>();
            List<string> colNameList = new List<string>();
            List<char> typeList = new List<char>();
            List<Double> ubList = new List<Double>();
            //the objetive funcion

            //variable x
            for (int i = 1; i <= nbNode1; i++)
            {
                for (int k = 1; k <= nbNode2; k++)
                {
                    Type typeLabel = graph1.ListNodes[i - 1].Label.GetType();
                    object obj = Activator.CreateInstance(typeLabel);
                    nodeepslabel = (Graphs.Label)obj;
                    nodeepslabel.Id = ConstantsAC.EPS_ID;

                    double costsub = graph1.ListNodes[i - 1].Label.dissimilarity(graph2.ListNodes[k - 1].Label);
                    double costdel = graph1.ListNodes[i - 1].Label.dissimilarity(nodeepslabel);
                    double costins = nodeepslabel.dissimilarity(graph2.ListNodes[k - 1].Label);
                    double cost = costsub - costdel - costins;
                    objList.Add(cost);
                    colNameList.Add("x_" + graph1.ListNodes[i - 1].Id + "," + graph2.ListNodes[k - 1].Id);
                }
            }

            //variable y
            for (int ij = 1; ij <= nbEdge1; ij++)
                for (int kl = 1; kl <= nbEdge2; kl++)
                {
                    Type typeLabel = graph1.ListEdges[ij - 1].Label.GetType();
                    object obj = Activator.CreateInstance(typeLabel);
                    nodeepslabel = (Graphs.Label)obj;
                    nodeepslabel.Id = ConstantsAC.EPS_ID;

                    double costsub = graph1.ListEdges[ij - 1].Label.dissimilarity(graph2.ListEdges[kl - 1].Label);
                    double costdel = graph1.ListEdges[ij - 1].Label.dissimilarity(nodeepslabel);
                    double costins = nodeepslabel.dissimilarity(graph2.ListEdges[kl - 1].Label);
                    double cost = costsub - costdel - costins;
                    objList.Add(cost);

                    colNameList.Add("y_" + graph1.ListEdges[ij - 1].Id + "," + graph2.ListEdges[kl - 1].Id);
                }
            double constante = 0;
            for (int i = 1; i <= nbNode1; i++)
            {
                Type typeLabel = graph1.ListNodes[i - 1].Label.GetType();
                object obj = Activator.CreateInstance(typeLabel);
                nodeepslabel = (Graphs.Label)obj;
                nodeepslabel.Id = ConstantsAC.EPS_ID;
                constante += (graph1.ListNodes[i - 1].Label).dissimilarity(nodeepslabel);

            }

            for (int ij = 1; ij <= nbEdge1; ij++)
            {
                Type typeLabel = graph1.ListEdges[ij - 1].Label.GetType();
                object obj = Activator.CreateInstance(typeLabel);
                nodeepslabel = (Graphs.Label)obj;
                nodeepslabel.Id = ConstantsAC.EPS_ID;
                double costEdge = (graph1.ListEdges[ij - 1].Label).dissimilarity(nodeepslabel);
                constante += costEdge;
            }

            for (int k = 1; k <= nbNode2; k++)
            {
                Type typeLabel = graph2.ListNodes[k - 1].Label.GetType();
                object obj = Activator.CreateInstance(typeLabel);
                nodeepslabel = (Graphs.Label)obj;
                nodeepslabel.Id = ConstantsAC.EPS_ID;
                constante += (graph2.ListNodes[k - 1].Label).dissimilarity(nodeepslabel);
                // colNameList.Add("v_Del_" + graph2.ListNodes[k - 1].Id + "," + graph2.ListNodes[k - 1].Id);
            }

            for (int kl = 1; kl <= nbEdge2; kl++)
            {
                Type typeLabel = graph2.ListEdges[kl - 1].Label.GetType();
                object obj = Activator.CreateInstance(typeLabel);
                nodeepslabel = (Graphs.Label)obj;
                nodeepslabel.Id = ConstantsAC.EPS_ID;
                double costEdge = (graph2.ListEdges[kl - 1].Label).dissimilarity(nodeepslabel);
                constante += costEdge;

                //colNameList.Add("f_Del_" + graph2.ListEdges[kl - 1].Id + "," + graph2.ListEdges[kl - 1].Id);
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
                #region construire constraintes
                //18.B
                for (int i = 0; i < nbNode1; i++)
                {
                    jaList = new List<int>();
                    arList = new List<Double>();
                    lbMatrixList.Add(0.0);
                    ubMatrixList.Add(1.0);

                    for (int k = 0; k < nbNode2; k++)
                    {

                        jaList.Add(i * nbNode2 + k);
                        arList.Add(1);
                    }
                    indiceH[rownum] = jaList.ToArray();
                    valeurH[rownum] = arList.ToArray();
                    rownum++;
                }


                // contraint: equation [Fb.1]-4
                //18.c
                for (int k = 0; k < nbNode2; k++)
                {
                    jaList = new List<int>();
                    arList = new List<Double>();
                    lbMatrixList.Add(0.0);
                    ubMatrixList.Add(1.0);
                    for (int i = 0; i < nbNode1; i++)
                    {
                        jaList.Add(i * nbNode2 + k);
                        arList.Add(1);
                    }

                    indiceH[rownum] = jaList.ToArray();
                    valeurH[rownum] = arList.ToArray();
                    rownum++;
                }




                if (nbNode2 * nbEdge1 < nbNode1 * nbEdge2)
                {


                    //6 If two vertices are matched together, 
                    //an edge originating one of these two vertices must be matched with an edge originating the other vertex)
                    //18.F
                    for (int k = 0; k < nbNode2; k++)
                    {
                        for (int ij = 0; ij < nbEdge1; ij++)
                        {
                            jaList = new List<int>();//les indice des valeurs
                            arList = new List<Double>();//les valeurs non zeros dans la ligne
                            lbMatrixList.Add(0.0);
                            ubMatrixList.Add(1.0);


                            string sourcei = graph1.ListEdges[ij].NodeSource.Id;
                            string nameVarik = "x_" + sourcei + "," + graph2.ListNodes[k].Id;
                            int colIndxik = SolverCPLEX.GetIndexByName(x, nameVarik);
                            if (colIndxik == -1)
                                throw new InvalidProgramException();

                            jaList.Add(colIndxik);
                            arList.Add(1);

                            string sourcej = graph1.ListEdges[ij].NodeTarget.Id;
                            string nameVarxjk = "x_" + sourcej + "," + graph2.ListNodes[k].Id;
                            int colIndxjk = SolverCPLEX.GetIndexByName(x, nameVarxjk);
                            if (colIndxjk == -1)
                                throw new InvalidProgramException();

                            jaList.Add(colIndxjk);
                            arList.Add(1);

                            string name1 = graph1.ListEdges[ij].Id;
                            foreach (Edge e in graph2.ListNodes[k].ListEdgesOut)
                            {
                                string name2 = e.Id;
                                string nameVar = "y_" + name1 + "," + name2;
                                int colInd = SolverCPLEX.GetIndexByName(x, nameVar);
                                if (colInd == -1)
                                    throw new InvalidProgramException();
                                jaList.Add(colInd);
                                arList.Add(-1);
                            }
                           


                           

                            indiceH[rownum] = jaList.ToArray();
                            valeurH[rownum] = arList.ToArray();
                            rownum++;
                        }
                    }


                }
                else
                {
                    //Todo
                    for (int i = 0; i < nbNode1; i++)
                    {
                        for (int kl = 0; kl < nbEdge2; kl++)
                        {
                            jaList = new List<int>();//les indice des valeurs
                            arList = new List<Double>();//les valeurs non zeros dans la ligne
                            lbMatrixList.Add(0.0);
                            ubMatrixList.Add(1.0);


                            string sourcei = graph2.ListEdges[kl].NodeSource.Id;
                            string nameVarik = "x_" + graph1.ListNodes[i].Id + "," + sourcei;
                            int colIndxik = SolverCPLEX.GetIndexByName(x, nameVarik);
                            if (colIndxik == -1)
                                throw new InvalidProgramException();

                            jaList.Add(colIndxik);
                            arList.Add(1);

                            string sourcej = graph2.ListEdges[kl].NodeTarget.Id;
                            string nameVarxjk = "x_" + graph1.ListNodes[i].Id + "," + sourcej;
                            int colIndxjk = SolverCPLEX.GetIndexByName(x, nameVarxjk);
                            if (colIndxjk == -1)
                                throw new InvalidProgramException();

                            jaList.Add(colIndxjk);
                            arList.Add(1);

                            string name1 = graph2.ListEdges[kl].Id;
                            foreach (Edge e in graph1.ListNodes[i].ListEdgesOut)
                            {
                                string name2 = e.Id;
                                string nameVar = "y_" + name2 + "," + name1;
                                int colInd = SolverCPLEX.GetIndexByName(x, nameVar);
                                if (colInd == -1)
                                    throw new InvalidProgramException();
                                jaList.Add(colInd);
                                arList.Add(-1);
                            }

                            indiceH[rownum] = jaList.ToArray();
                            valeurH[rownum] = arList.ToArray();
                            rownum++;
                        }
                    }

                }

                #endregion
                Int32 res = ilpMatrix.AddRows(lbMatrixList.ToArray(), ubMatrixList.ToArray(), indiceH, valeurH);

                // add the objective function
                objCoef = objList.ToArray();
                cplex.AddMinimize(cplex.Sum(cplex.Constant(constante), cplex.ScalProd(x, objCoef)));

            }
            catch (ILOG.Concert.Exception e)
            {
                System.Console.WriteLine("Concert exception '" + e + "' caught");
            }

        }

        // GED formulation from the paper 
        // https://ieeexplore.ieee.org/document/1642656
        public override void BLPjusticehero()
        {
            int nbNode1 = graph1.ListNodes.Count;
            int nbNode2 = graph2.ListNodes.Count;
            int N = nbNode1 + nbNode2; // nombre de noeuds du graphe d'édition
            // #region 
            //création matrices de placement standard A0 et A1
            Graph gridg1 = new Graph();
            Graph gridg2 = new Graph();
            Type typeNodeLabel = graph1.ListNodes[0].Label.GetType();
            object objNode = Activator.CreateInstance(typeNodeLabel);
            Graphs.Label nodeepslabel = (Graphs.Label)objNode;
            nodeepslabel.Id = ConstantsAC.EPS_ID;

            Type typeEdgeLabel = null;
            object objEdge = null;
            Graphs.Label edgeepslabel = null;
            if (graph1.ListEdges.Count > 0)
            {
                typeEdgeLabel = graph1.ListEdges[0].Label.GetType();
                objEdge = Activator.CreateInstance(typeEdgeLabel);
                edgeepslabel = (Graphs.Label)objEdge;
                edgeepslabel.Id = ConstantsAC.EPS_ID;
            }

            if (graph2.ListEdges.Count > 0)
            {
                typeEdgeLabel = graph2.ListEdges[0].Label.GetType();
                objEdge = Activator.CreateInstance(typeEdgeLabel);
                edgeepslabel = (Graphs.Label)objEdge;
                edgeepslabel.Id = ConstantsAC.EPS_ID;
            }

            if (graph1.ListEdges.Count == 0 && graph2.ListEdges.Count == 0)
            {
                Console.Out.WriteLine("error no edges random edge label alkane");
                edgeepslabel = (Graphs.Label)new LabelEdgeAlkane();
                edgeepslabel.Id = ConstantsAC.EPS_ID;
            }

            //else
            //{

            //edgeepslabel = (Graphs.Label)new LabelEdgeAlkane();
            //edgeepslabel.Id = ConstantsAC.EPS_ID;
            //}

            for (int i = 0; i < nbNode1; i++)
            {
                gridg1.addNode(graph1.ListNodes[i]);
            }

            for (int i = 0; i < graph1.ListEdges.Count; i++)
            {
                gridg1.addEdge(graph1.ListEdges[i]);
            }

            // ajout des noeuds "virtuels" au graphe g1, pour que g1 corresponde au placement standard dans le graphe d'édition (A0)
            for (int i = nbNode1; i < N; i++)
            {

                Node n = new Node((i + 2).ToString());
                gridg1.addNode(n);
                n.Label = nodeepslabel;
                n.Label.Id = ConstantsAC.EPS_ID;

                // LabelEdgeLetter
                //n.Label = new Graphs.GenericLabel();
                //n.Label.Id = n.Id;


            }


            MatrixLibrary.Matrix A0 = gridg1.calculateAdjacencyMatrix(); // création matrice d'adajcence de g1

            // idem pour g2 (A1)
            for (int i = 0; i < nbNode2; i++)
            {
                gridg2.addNode(graph2.ListNodes[i]);
            }

            for (int i = 0; i < graph2.ListEdges.Count; i++)
            {
                gridg2.addEdge(graph2.ListEdges[i]);
            }

            for (int i = nbNode2; i < N; i++)
            {
                Node n = new Node((i + 2).ToString());
                gridg2.addNode(n);
                n.Label = nodeepslabel;//new LabelNodeMutagen();
                n.Label.Id = ConstantsAC.EPS_ID;
                //n.Label = new Graphs.GenericLabel();
                //n.Label.Id = n.Id;
            }
            MatrixLibrary.Matrix A1 = gridg2.calculateAdjacencyMatrix(); // création matrice d'adajcence de g2

            // les graphes vont être étiquetés avec un label LabelNodeMolecule (pour les noeuds) et LabelEdgeMolecule (pour les arrêtes)
            // cela va nous permettre d'utiliser notre propre méthode "dissimilarity" pour les noeuds et arrêtes
            // gridg1.DynamicCastLabel(graph1.ListNodes[0].Label, graph1.ListEdges[0].Label);
            //  gridg2.DynamicCastLabel(graph2.ListNodes[0].Label, graph2.ListEdges[0].Label);


            // nb variables du pb : matrices P(N*N), S(N*N) et T(N*N)
            int nbCols = 3 * (N * N);
            // nb contraintes du pb
            int nbRows = N * N + N + N;


            List<double> objList = new List<double>();
            List<string> colNameList = new List<string>();

            // coût des opérations d'édition des sommets (coeff de P dans la formule de la distance d'édition)
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    double costNode = gridg1.ListNodes[i].Label.dissimilarity(gridg2.ListNodes[j].Label);
                    objList.Add(costNode);
                    //colNameList.Add("P[" + gridg1.ListNodes[i].Id + "][" + gridg2.ListNodes[j].Id + "]");
                    colNameList.Add("x_" + gridg1.ListNodes[i].Id + "," + gridg2.ListNodes[j].Id + "");
                }
            }


            // coût des opérations d'édition des arrêtes (coeffs de S et T)
            //Edge ee = new Edge((0).ToString());
            //ee.Label = new LabelEdgeMutagen();
            //ee.Label.Id = ConstantsAC.EPS_ID;
            // coeff de S
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    // dans notre cas, l'opération est soit 0->1 ou 1->0 (insertion ou suppression d'une arrête)
                    // double costNode = ee.Label.dissimilarity(ee.Label) / 2.0;
                    double costNode = edgeepslabel.dissimilarity(edgeepslabel) / 2.0;


                    //double costNode = 0.5 / 2.0;
                    objList.Add(costNode);
                    colNameList.Add("S[" + gridg1.ListNodes[i].Id + "][" + gridg2.ListNodes[j].Id + "]");
                }
            }

            // coeff de T
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    // dans notre cas, l'opération est soit 0->1 ou 1->0 (insertion ou suppression d'une arrête)
                    //double costNode = ee.Label.dissimilarity(ee.Label) / 2.0;
                    double costNode = edgeepslabel.dissimilarity(edgeepslabel) / 2.0;
                    //  double costNode = 0.5 / 2.0;
                    objList.Add(costNode);
                    colNameList.Add("T[" + gridg1.ListNodes[i].Id + "][" + gridg2.ListNodes[j].Id + "]");
                }
            }

            try
            {
                // Cplex cplex = new Cplex(); // creation du modèle CPLEX

                //ILPMatrix lp_matrix = cplex.AddLPMatrix();  // matrice du pb (initialisée à 0 colonnes et 0 lignes)

                cplex = new Cplex();
                ilpMatrix = cplex.AddLPMatrix();



                ColumnArray colonnes = cplex.ColumnArray(ilpMatrix, nbCols); // définition des variables-colonnes du pb

                // ajout des variables-colonnes dans la matrice du pb (avec définition des bornes: P[i][j] = 0 ou 1, idem avec S et T)
                INumVar[] x = cplex.NumVarArray(colonnes, 0, 1, NumVarType.Bool, colNameList.ToArray());

                INumVar[,] P = new INumVar[N, N];
                INumVar[,] S = new INumVar[N, N];
                INumVar[,] T = new INumVar[N, N];

                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        P[i, j] = x[(N * i) + j];
                    }
                }


                // indice de S et T dans x (utilisés lors de la définition des contraintes)
                int indicedebutS = N * N; // indice du 1er élément de S dans x
                int indicedebutT = 2 * (N * N); // indice du 1er élément de T dans x

                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        S[i, j] = x[indicedebutS + (N * i) + j];
                    }
                }

                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        T[i, j] = x[indicedebutT + (N * i) + j];
                    }
                }

                objCoef = objList.ToArray();
                // ajout de la fonction objectif du pb
                cplex.AddMinimize(cplex.ScalProd(x, objCoef));


                // contrainte n°1
                for (int i = 0; i < N; i++)
                {

                    int[] coeffsA0 = new int[N]; // vecteur des coeffs ligne i de A0
                    INumVar[] coeffsP_A1 = new INumVar[N]; // vecteur des coeffs ligne i de P
                    for (int c = 0; c < N; c++)
                    {
                        coeffsA0[c] = (int)A0[i, c];
                        coeffsP_A1[c] = P[i, c];
                    }

                    for (int j = 0; j < N; j++)
                    {
                        INumVar[] coeffsP = new INumVar[N]; // vecteur des coeffs colonne j de P
                        int[] coeffsA1 = new int[N]; // vecteur des coeffs colonne j de A1
                        for (int c = 0; c < N; c++)
                        {
                            coeffsP[c] = P[c, j];
                            coeffsA1[c] = (int)A1[c, j];
                        }
                        cplex.AddEq(cplex.Sum(
                            cplex.Diff(
                                cplex.ScalProd(coeffsP, coeffsA0), cplex.ScalProd(coeffsP_A1, coeffsA1)),
                            cplex.Diff(
                                S[i, j], T[i, j]))
                            , 0);
                        /* (ANCIEN PRODUIT TERME A TERME)
                                cplex.AddEq(cplex.Sum(
                                    cplex.Diff(
                                        cplex.Prod(A0[i,j],P[i,j]),cplex.Prod(P[i,j],A1[i,j])),
                                    cplex.Diff(
                                        S[i, j], T[i, j]))
                                    , 0); */
                    }
                }

                // contrainte n°2 (somme des lignes dans P = 1 et somme des colonnes dans P = 1)
                for (int i = 0; i < N; i++)
                {
                    INumVar[] sommeLignes = new INumVar[N];
                    INumVar[] sommeColonnes = new INumVar[N];
                    for (int j = 0; j < N; j++)
                    {
                        sommeLignes[j] = x[N * i + j];
                        sommeColonnes[j] = x[N * j + i];
                    }
                    cplex.AddEq(cplex.Sum(sommeLignes), 1);
                    cplex.AddEq(cplex.Sum(sommeColonnes), 1);
                }

            }
            catch (ILOG.Concert.Exception e)
            {
                System.Console.WriteLine("Concert exception `" + e + "` caught");
            }


        }







        public override void BLPjusticeheroQuadratic()
        {
            
        }

        //A quadratic interger program for graph matching,
        //When tuned with the right similarity function it can represent the GED problem.
        // See Pattern Reocgnition Paper paper On the unification of graph matching and graph edit distance paper.
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
                                    cost *= 0.5;
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

            foreach (Edge etmp in nodej.ListEdgesOut)
            {
                if (etmp.NodeTarget.Id == nodei.Id)
                {
                    e = etmp;
                    return e;
                }
            }
            throw new NotImplementedException("Could not find the edge !!!! not normal");
        }


    }


}

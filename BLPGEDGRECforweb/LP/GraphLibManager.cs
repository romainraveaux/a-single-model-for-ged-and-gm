using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Xml;
using Matching;
using Graphs;

namespace LP
{
    /// <summary>
    /// Cette classe est pour accéder à la librairie graphsLib et pour manipuler les graphes
    /// </summary>
    public class GraphLibManager
    {
        /// <summary>
        ///  Choisir le directory of the graphe
        /// </summary>
        public static string ChoisirDirectory()
        {

            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            OpenFileDialog myOpen = new OpenFileDialog();
            //myOpen.InitialDirectory = path + "\\gxl_files";
            myOpen.RestoreDirectory = true;
            myOpen.Filter = "GXL Files (*.gxl)|*.gxl";
            myOpen.ShowDialog();

            string nomFichier = myOpen.FileName;
            return nomFichier;


        }
        /// <summary>
        ///  Load a graph from the file .gxl, doesn't print out the graph
        ///  <param name="nomFichier">the full name of the fiche gxl</param>
        ///  <param name="grapheType">the type of the graph to be loaded.
        ///  3-Graph LOW, 0-Graph Grec, 1-Graph Mutagenity
        ///  the others numbers - Graph Generic
        ///  </param>
        /// </summary>
        public static Graph LoadGraph(String nomFichier, int grapheType)
        {
            if (nomFichier != "")
            {
                Graph myGraph = new Graph();
                myGraph.loadGXL(nomFichier);
                if (grapheType == Program.noLOW)
                    myGraph.DynamicCastLabel(new LabelNum(), new LabelNum());
                else if (grapheType == Program.noGrec)
                    myGraph.DynamicCastLabel(new LabelNodeGrec(), new LabelEdgeGrec());
                else if (grapheType == Program.noLetter)
                    myGraph.DynamicCastLabel(new LabelNodeLetter(), new LabelEdgeLetter());
                else if (grapheType == Program.noMuta)
                    myGraph.DynamicCastLabel(new LabelNodeMutagen(), new LabelEdgeMutagen());
                else if (grapheType == Program.noPRO)
                    myGraph.DynamicCastLabel(new LabelNodeProtein(), new LabelEdgeProtein());
                else if (grapheType == Program.noIlpiso)
                    myGraph.DynamicCastLabel(new LabelNodeIlpiso(), new LabelEdgeIlpiso());
                else if (grapheType == Program.noAlkane)
                    myGraph.DynamicCastLabel(new LabelNodeAlkane(), new LabelEdgeAlkane());
                else
                    myGraph.DynamicCastLabel(new GenericLabel(), new GenericLabel());

                //Le nom du fichier est inséré dans Graph
                char[] monSplit = new char[] { '\\' };
                String[] tabNom = nomFichier.Split(monSplit);
                myGraph.Id = tabNom[tabNom.Length - 1];
                return myGraph;
            }
            else return null;
        }
        /// <summary>
        ///  print the graph to the textbox (from: GraphLibTest)
        /// </summary>
        /// <param name="myGraph"></param>
        /// <param name="isGraph"></param>
        /// <param name="textBox"></param>
        public static void PrintGraph(Graph myGraph, bool isGraph, RichTextBox textBox)
        {
            if (isGraph)
            {
                textBox.Clear();
            }
            textBox.AppendText("id : " + myGraph.Id + "\n");
            textBox.AppendText("name : " + myGraph.Name + "\n");
            textBox.AppendText("////// \n");
            textBox.AppendText("Nodes list : \n");
            foreach (Node n in myGraph.ListNodes)
            {
                textBox.AppendText(n.Id + " : ");
                if (n.Label == null) textBox.AppendText("Null label \n");
                else textBox.AppendText(n.Label.toString() + "\n");
            }
            textBox.AppendText("////// \n");
            textBox.AppendText("Edges list : \n");
            foreach (Edge ed in myGraph.ListEdges)
            {
                if (ed.Id == null) textBox.AppendText("Null Id from " + ed.NodeSource.Id + " to " + ed.NodeTarget.Id + " : ");
                else textBox.AppendText(ed.Id + " from " + ed.NodeSource.Id + " to " + ed.NodeTarget.Id + " : ");

                if (ed.Label == null) textBox.AppendText("Null edge etiquette \n");
                else textBox.AppendText(ed.Label.toString() + "\n");
            }
            textBox.AppendText("\n");
        }

        /// <summary>
        /// draw the graph to the pictureBox ( from : GraphLibTest)
        /// </summary>
        /// <param name="myGraph"></param>
        /// <param name="picture"></param>
        /// <param name="jpgfile"></param>
        ///  <param name="status"></param>
        public static void DisplayGraph(Graph myGraph, PictureBox picture, String jpgfile, ToolStripStatusLabel status)
        {
            if (myGraph != null)
            {
                try
                {
                    generateImage(myGraph, jpgfile);
                    picture.Image = Bitmap.FromFile(jpgfile);
                    status.Text = "Graph loaded with success";
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            else Console.WriteLine("Warning : Null Graph !!");
        }

        /// <summary>
        /// generate a image (jpg file) for the graph
        /// </summary>
        /// <param name="myGraph"></param>
        /// <param name="jpgfile"></param>
        public static void generateImage(Graph myGraph, string jpgfile)
        {
            if (myGraph != null)
            {
                GraphDisplay GD = new GraphDisplay();
                string ressource = Path.GetDirectoryName(Application.ExecutablePath) + "/ressource";

                try
                {
                    GD.ImageDisplay(myGraph, ressource, jpgfile);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }


        /// <summary>
        /// generate a html  file for the graphs
        /// </summary>
        /// <param name="glpk_controller"></param>
        /// <param name="FileNameHTML"></param>
        /// <param name="NextFileNameHTML"></param>
        //public static void generateHTML(Glpk_Controller glpk_controller, string FileNameHTML, string NextFileNameHTML)
        //{
        //    string nameG1 = FileNameHTML + "_graphs1.jpg";
        //    string nameG2 = FileNameHTML + "_graphs2.jpg";
        //    string nameIso = FileNameHTML + "_graphIso.jpg";
        //    generateImage(glpk_controller.Graph1, nameG1);
        //    generateImage(glpk_controller.Graph2, nameG2);
        //    generateImage(glpk_controller.IsoGraph, nameIso);
        //    string ressource = Path.GetDirectoryName(Application.ExecutablePath) + "/ressource";
        //    ExportHTML(glpk_controller, ressource, FileNameHTML, NextFileNameHTML);
        //}


        ///// <summary>
        ///// generate a html  file for the graphs (from the lib Graph)
        ///// copy and make some changes 
        ///// </summary>
        ///// <param name="glpk_controller"></param>
        ///// <param name="PathRessource"></param>
        ///// <param name="FileNameHTML"></param>
        ///// <param name="NextFileNameHTML"></param>
        //public static void ExportHTML(Glpk_Controller glpk_controller, string PathRessource, string FileNameHTML, string NextFileNameHTML)
        //{
        //    string nameG1 = FileNameHTML + "_graphs1.jpg";
        //    string nameG2 = FileNameHTML + "_graphs2.jpg";
        //    string nameIso = FileNameHTML + "_graphIso.jpg";
        //    TextWriter tw;
        //    tw = new StreamWriter(FileNameHTML + ".html");
        //    tw.WriteLine("<html>");
        //    tw.WriteLine("<head>");
        //    tw.WriteLine("<title>Test GLPK</title>");
        //    tw.WriteLine("</head>");
        //    tw.WriteLine("<body>");
        //    tw.WriteLine("<div style = \"clear: right; float: right; text-align: right;\">");
        //    tw.WriteLine("<right><p><font face=\"Comic Sans MS\" color=\"#0000dd\" size=\"2\">Solve Graph <font color=\"#000000\">" + glpk_controller.Graph1.Id + "</font> with Graph <font color=\"#000000\">" + glpk_controller.Graph2.Id + "</font></font></p>");
        //    if (glpk_controller.OptimalValue == -1)
        //        tw.WriteLine("<p><font face=\"Comic Sans MS\" color=\"#0000dd\" size=\"2\">No solution" + "</font></p>");
        //    else
        //        tw.WriteLine("<p><font face=\"Comic Sans MS\" color=\"#0000dd\" size=\"2\">Optimal value: " + glpk_controller.OptimalValue + "</font></p>");
        //    tw.WriteLine("<p><font face=\"Comic Sans MS\" color=\"#0000dd\" size=\"2\">Execute time: " + glpk_controller.ExecuteTime + "</font></p>");
        //    tw.WriteLine("<a href=\"" + NextFileNameHTML + ".html\"><font face=\"Comic Sans MS\" color=\"#000000\" size=\"5\"> NEXT</font></a></right>");
        //    tw.WriteLine("</div>");

        //    tw.WriteLine("<table border cellpadding=\"10\">");
        //    tw.WriteLine("<tr>");
        //    tw.WriteLine("<td><font size=\"2\">" + glpk_controller.Graph1.Name + " (" + glpk_controller.Graph1.Id + ")  <font/></td>");
        //    //tw.WriteLine("<td><font size=\"2\">" + glpk_controller.Graph2.Name + " (" + glpk_controller.Graph2.Id + ")  <font/></td>");
        //    tw.WriteLine("</tr>");
        //    tw.WriteLine("<tr>");
        //    tw.WriteLine("<td bordercolor=\"blue\"><img src=\"" + System.IO.Path.GetFullPath(nameG1) + "\" alt=\" graph1 \" style=\"max-height:300px;\" /> </td>");
        //    //tw.WriteLine("<td bordercolor=\"blue\"><img src=\"" + System.IO.Path.GetFullPath(nameG2) + "\" alt=\" graph2 \" style=\"max-width:400px;\" /> </td>");
        //    tw.WriteLine("</tr>");
        //    tw.WriteLine("</table>");

        //    tw.WriteLine("<table border cellpadding=\"10\">");
        //    tw.WriteLine("<tr>");
        //    tw.WriteLine("<td><font size=\"2\">" + glpk_controller.Graph2.Name + " (" + glpk_controller.Graph2.Id + ")  <font/></td>");
        //    tw.WriteLine("</tr>");
        //    tw.WriteLine("<tr>");
        //    tw.WriteLine("<td bordercolor=\"blue\"><img src=\"" + System.IO.Path.GetFullPath(nameG2) + "\" alt=\" graph2 \" style=\"max-height:300px;\" /> </td>");
        //    tw.WriteLine("</tr>");
        //    tw.WriteLine("</table>");

        //    tw.WriteLine("<table border cellpadding=\"10\">");
        //    tw.WriteLine("<tr>");
        //    tw.WriteLine("<td><font size=\"2\">Graph Iso<font/></td>");
        //    tw.WriteLine("</tr>");
        //    tw.WriteLine("<tr>");
        //    tw.WriteLine("<td bordercolor=\"blue\"><img src=\"" + System.IO.Path.GetFullPath(nameIso) + "\"  alt= \"can't solve the two graphs\" \" /></td>");
        //    tw.WriteLine("</tr>");
        //    tw.WriteLine("</table>");

        //    tw.WriteLine("</body>");
        //    tw.WriteLine("</html>");

        //    tw.Close();


        //}

        /// <summary>
        ///Display Touch Image (a copy from Lib Graph)
        /// </summary>
        public static void DisplayTouch(Graph myGraph, String jpgfile, ToolStripStatusLabel status)
        {
            if (myGraph != null)
            {
                GraphDisplay GD = new GraphDisplay();
                string ressource = Path.GetDirectoryName(Application.ExecutablePath) + "/ressource";
                try
                {
                    GD.TouchDisplay(myGraph, ressource);
                    status.Text = "Show touch graph";
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            else Console.WriteLine("Warning : Null Graph !!");
        }

        /// <summary>
        /// combain the two graph (a copy from GraphLibTest)
        /// </summary>
        /// <param name="g1"></param>
        /// <param name="g2"></param>
        /// <param name="IsoNode"></param>
        /// <returns> the graph combined or null</returns>
        public static Graph CreateIsoGraph(Graph g1, Graph g2, List<String[]> IsoNode)
        {
            if (IsoNode.Count != 0)
            {
                Graph ResultGraph = new Graph("IsoGraph", "IsoGraph", false);
                ResultGraph.IsDirected = g1.IsDirected;
                Node currentNode;
                Edge currentEdge;
                //Ajout des éléments du graphe 1
                foreach (Node n in g1.ListNodes)
                {
                    currentNode = new Node(g1.Name + "_" + n.Id);
                    ResultGraph.addNode(currentNode);
                }
                foreach (Edge e in g1.ListEdges)
                {
                    currentEdge = new Edge(g1.Name + "_" + e.Id);
                    currentEdge.NodeSource = ResultGraph.findNode(g1.Name + "_" + e.NodeSource.Id);
                    currentEdge.NodeTarget = ResultGraph.findNode(g1.Name + "_" + e.NodeTarget.Id);
                    ResultGraph.addEdge(currentEdge);
                }
                //Ajout des éléments du graphe 2
                foreach (Node n in g2.ListNodes)
                {
                    currentNode = new Node(g2.Name + "_" + n.Id);
                    //currentNode.Label = n.Label;
                    ResultGraph.addNode(currentNode);
                }
                foreach (Edge e in g2.ListEdges)
                {
                    //currentEdge = new Edge(e.Id, ResultGraph.findNode("2-" + e.NodeSource.Id), ResultGraph.findNode("2-" + e.NodeTarget.Id), e.Label);
                    currentEdge = new Edge(g2.Name + "_" + e.Id);
                    currentEdge.NodeSource = ResultGraph.findNode(g2.Name + "_" + e.NodeSource.Id);
                    currentEdge.NodeTarget = ResultGraph.findNode(g2.Name + "_" + e.NodeTarget.Id);
                    ResultGraph.addEdge(currentEdge);
                }
                //Ajout des arcs d'isomorphismes
                int cpt = 0;
                foreach (String[] s in IsoNode)
                {
                    currentEdge = new Edge("Iso" + cpt);
                    currentEdge.NodeSource = ResultGraph.findNode(g1.Name + "_" + s[0]);
                    currentEdge.NodeTarget = ResultGraph.findNode(g2.Name + "_" + s[1]);
                    ResultGraph.addEdge(currentEdge);
                    cpt++;
                }
                return ResultGraph;

            }
            else return null;
        }

        /// <summary>
        ///  transform a undirected graph to a directed graph
        /// </summary>
        /// <param name="g">the graph to be transformed</param>
        /// <param name="Directed">if we change the mode of graph or not</param>
        public static void transToDirectedGraph(Graph g,bool changeMode)
        {
            List<Edge> oldListEdges = new List<Edge>();
            oldListEdges.AddRange(g.ListEdges);
            if (g.IsDirected == false)
            {
                g.IsDirected = true;
                foreach (Node n in g.ListNodes)
                {
                    n.ListEdgesOut.Clear();
                    n.ListEdgesIn.Clear();
                }

                foreach (Edge e in oldListEdges)
                {
                    e.NodeSource.ListEdgesOut.Add(e);
                    e.NodeTarget.ListEdgesIn.Add(e);

                    Edge edge = new Edge("copy_" + e.Id, e.NodeTarget, e.NodeSource, null);
                    edge.AttributesGXL.AddRange(e.AttributesGXL);
                    Type typeEdge = e.Label.GetType();
                    object obj = Activator.CreateInstance(typeEdge);
                    edge.Label = (Graphs.Label)obj;
                    edge.Label.Id = edge.Id;
                    if (edge.AttributesGXL.Count != 0) edge.Label.fromAttributes(edge.AttributesGXL);                    
                    g.addEdge(edge);

                }
            }
            g.IsDirected=changeMode;
        }

        /// <summary>
        /// Charge les classes de graphe depuis un fichier CXL dont l'URL est donnée en paramètre. 
        /// </summary>
        /// <param name="url">Adresse complète du fichier CXL à charger</param>
        public static List<string[]> loadGraphClass(string url, string dbDirectory)
        {
            //Dictionary<String, String> classMap = new Dictionary<string, string>();
            List<string[]> classMap = new List<string[]>();
            String idGraph = null, classGraph = null;
            XmlDocument objXmlDoc = new XmlDocument();

            //Chargement du fichier train.cxl
            try
            {
                objXmlDoc.Load(url);
            }
            catch (Exception e)
            {
                Console.WriteLine("Fichier train.cxl introuvable");
                Console.WriteLine(e.ToString());
            }
            // Récupération du noeud racine qui est la balise <GraphCollection>
            XmlNode objRootNode = objXmlDoc.DocumentElement;

            // Récupération du premier noeud enfant 
            XmlNode objChildNode = objRootNode.FirstChild;



            // Parcourt l'ensemble des noeuds XML
            while (objChildNode != null)
            {
                objChildNode = objChildNode.FirstChild;
                   
                while (objChildNode != null)
                {
                    if (objChildNode.Name == "print")
                    {
                        foreach (XmlNode item in objChildNode.Attributes)
                        {
                            if (item.Name == "file")
                                idGraph = dbDirectory+"\\"+item.InnerText;
                            else if (item.Name == "class")
                                classGraph = item.InnerText;
                        }
                        classMap.Add(new string[2]{idGraph, classGraph});
                    }
                    objChildNode = objChildNode.NextSibling;
                }
            }
            return classMap;
        }
    }
      
  
}
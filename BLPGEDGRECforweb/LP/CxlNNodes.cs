using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphs;
using System.IO;
using System.Diagnostics;
using System.Xml;

namespace LP
{
    /// <summary>
    /// Cette classe est pour découper les graphes dans train.cxl par le nombre de noeuds
    /// ainsi pour obtenir la classe de chaque graphe
    /// </summary>
    class CxlNNodes
    {
        static public int GREC = 1;
        static public int Mutagen = 2;

        int noOfNodes;
        int NoOfDataBase;
        String dbDirecttory;
        String trainingFile;
        String readTrainingCXLFile;

        FileStream fs;
        StreamWriter monStream;
        
        //	private String readTestCXLFile;

        public CxlNNodes(int numberOfNodes, int typeOfDataBase, string _dbDirectory, string _trainingFile,string _readTrainingCXLFile)
        {
            this.noOfNodes = numberOfNodes;
            this.NoOfDataBase = typeOfDataBase;
            this.dbDirecttory = _dbDirectory;
            this.trainingFile = _trainingFile;
            this.readTrainingCXLFile = _readTrainingCXLFile;
            init();

        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void init()
        {
            Console.Out.WriteLine(" DataBaseNumber ====" + this.NoOfDataBase + " and NumberOfNodes = " + this.noOfNodes);
            if (this.NoOfDataBase == Program.noGrec)
            {
                //-------------------------------------------------------------------------
                Console.Out.WriteLine("   Welcome to GREC....  ");
                //this.dbDirecttory = Directory.GetCurrentDirectory() + "\\data\\testGrec";

               // this.readTrainingCXLFile = Directory.GetCurrentDirectory() + "\\data\\testGrec\\train.cxl";
               // this.trainingFile = this.dbDirecttory + "\\train" + noOfNodes + ".cxl";

                //-------------------------------------------------------------------------
            }


            else if (this.NoOfDataBase == Program.noLetter)
            {
                //-------------------------------------------------------------------------
                Console.Out.WriteLine("   Welcome to Letter....  ");
                //this.dbDirecttory = Directory.GetCurrentDirectory() + "\\data\\testGrec";

                // this.readTrainingCXLFile = Directory.GetCurrentDirectory() + "\\data\\testGrec\\train.cxl";
                // this.trainingFile = this.dbDirecttory + "\\train" + noOfNodes + ".cxl";

                //-------------------------------------------------------------------------
            }
            else if (this.NoOfDataBase == Program.noMuta)
            {
                Console.Out.WriteLine("   Welcome to Mutagen ....  ");

                //this.dbDirecttory = Directory.GetCurrentDirectory() + "\\data\\testMuta";

               // this.readTrainingCXLFile = Directory.GetCurrentDirectory() + "\\data\\testMuta\\train.cxl";
               // this.trainingFile = this.dbDirecttory + "\\train" + noOfNodes + ".cxl";

            }

            else if (this.NoOfDataBase == Program.noPRO)
            {
                Console.Out.WriteLine("   Welcome to Protein ....  ");
            }
            else if (this.NoOfDataBase == Program.noIlpiso)
            {
                Console.Out.WriteLine("   Welcome to IPLPISO ....  ");
            }
            else{
                Console.Out.WriteLine("Error: no database exist......");
                return;
            }

            FilterGraphs(this.dbDirecttory, this.NoOfDataBase);

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        private void WriteToTxt(String testGraphFileName, String trainingGraphFileName, String testGraphClass, String trainingGraphClass)
        {
            // TODO Auto-generated method stub
           // int count = 0;

            monStream.Write(this.NoOfDataBase + "\t");
            monStream.Write(testGraphFileName + "\t");
            monStream.Write(trainingGraphFileName + "\t");
            monStream.Write(testGraphClass + "\t");
            monStream.Write(trainingGraphClass + "\t");
            monStream.WriteLine();

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /*
        private static FilenameFilter gxlFileFilter = new FilenameFilter() {
            public boolean accept(File dir, String name) {
                return name.endsWith(".gxl");
            }
        };
        */
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        bool Filter(Graph g1)
        {


            if (g1.ListNodes.Count != this.noOfNodes)
            {
                return false;
            }

            return true;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void FilterGraphs(String dbDirecttory2, int noOfDataBase)
        {
            // TODO Auto-generated method stub
            if (!Directory.Exists(dbDirecttory2))
            {
                Console.Out.WriteLine("Le répertoire n'existe pas");
                return;
            }

            String trainingGraphFileName = "";
            String trainingGraphClass = "";
            XmlDocument objXmlDoc = new XmlDocument();
            try
            {
                objXmlDoc.Load(this.readTrainingCXLFile);
            }
            catch (Exception e)
            {
                Console.WriteLine("Fichier train.cxl introuvable");
                Console.WriteLine(e.ToString());
            }
            // Récupération du noeud racine qui est la balise <GraphCollection>
            XmlNode objRootNode = objXmlDoc.DocumentElement;

            // Récupération du premier noeud enfant 
            XmlNode objChildNode=null;
            objChildNode = objRootNode.FirstChild;
           
            fs = new FileStream(this.trainingFile, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            Encoding outputEnc = new UTF8Encoding(false);
            monStream = new StreamWriter(fs, outputEnc);
            startTags(noOfDataBase);

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
                                trainingGraphFileName = item.InnerText;
                            else if (item.Name == "class")
                                trainingGraphClass = item.InnerText;
                        }
                        IOGraph ioGraph = new IOGraph();
                        Graph g2 = new Graph();
                        ioGraph.loadGXL(g2, this.dbDirecttory + "/" + trainingGraphFileName);
                        if (this.Filter(g2))
                        {
                            //WriteToTxt(testGraphFileName,trainingGraphFileName,testGraphClass,trainingGraphClass);
                            monStream.WriteLine("<print file=\"" + trainingGraphFileName + "\" class=\"" + trainingGraphClass + "\"/>");
                        }
                    }
                    objChildNode = objChildNode.NextSibling;
                }
            }
            endTags(noOfDataBase);
            if (monStream != null) monStream.Close();

            //#######################################################################################################"

        }


        private void endTags(int noOfDataBase)
        {
            // TODO Auto-generated method stub
            if (noOfDataBase == Program.noGrec)
            {
                monStream.WriteLine("</grec>");
                monStream.WriteLine("</GraphCollection>");
            }
            else if (noOfDataBase == Program.noMuta)
            {
                monStream.WriteLine("</mutagenicity>");
                monStream.WriteLine("</GraphCollection>");

            }

            else if (noOfDataBase == Program.noLetter)
            {
                monStream.WriteLine("</letter>");
                monStream.WriteLine("</GraphCollection>");

            }
            else if (noOfDataBase == Program.noPRO)
            {
                monStream.WriteLine("</protein>");
                monStream.WriteLine("</GraphCollection>");

            }

            else if (noOfDataBase == Program.noIlpiso)
            {
                monStream.WriteLine("</ILPISO>");
                monStream.WriteLine("</GraphCollection>");

            }
            else if (noOfDataBase == Program.noLOW)
            {
                monStream.WriteLine("</fingerprints>");
                monStream.WriteLine("</GraphCollection>");
            }
            else
            {

            }
        }


        private void startTags(int noOfDataBase)
        {
            // TODO Auto-generated method stub
            if (noOfDataBase == Program.noGrec)
            {
                monStream.WriteLine("<?xml version=\"1.0\"?>");
                monStream.WriteLine("<GraphCollection>");
                monStream.WriteLine("<grec>");

            }
            else if (noOfDataBase == Program.noMuta)
            {
                monStream.WriteLine("<?xml version=\"1.0\"?>");
                monStream.WriteLine("<GraphCollection>");
                monStream.WriteLine("<mutagenicity count=\"2337\">");

            }

            else if (noOfDataBase == Program.noLetter)
            {
                monStream.WriteLine("<?xml version=\"1.0\"?>");
                monStream.WriteLine("<GraphCollection>");
                monStream.WriteLine("<letter>");

            }
            else if (noOfDataBase == Program.noIlpiso)
            {
                monStream.WriteLine("<?xml version=\"1.0\"?>");
                monStream.WriteLine("<GraphCollection>");
                monStream.WriteLine("<ILPISO count=\"12\">");

            }
            else if (noOfDataBase == Program.noPRO)
            {
                monStream.WriteLine("<?xml version=\"1.0\"?>");
                monStream.WriteLine("<GraphCollection>");
                monStream.WriteLine("<protein>");

            }
            else if (noOfDataBase == Program.noLOW)
            {
                monStream.WriteLine("<?xml version=\"1.0\"?>");
                monStream.WriteLine("<GraphCollection xmlns:ns=\"http://www.iam.unibe.ch/%7Emneuhaus/FAN/1.0\">");
                monStream.WriteLine("<fingerprints base=\"/scratch/mneuhaus/progs/letter-database/automatic/0.5\" classmodel=\"henry5\" count=\"750\">");

            }
            else
            {

            }

        }
    }
}

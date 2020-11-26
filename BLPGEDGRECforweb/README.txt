This code corresponds to the code uses during the ICPR 2016 contest on graph distance (https://gdc2016.greyc.fr/).
The method called F2 was published in  [1]


[1] Julien Lerouge, Zeina Abu-Aisheh, Romain Raveaux, Pierre Héroux, Sébastien Adam:
New binary linear programming formulation to compute the graph edit distance. Pattern Recognition 72: 254-265 (2017)

Please cite : 
@article{DBLP:journals/pr/LerougeARHA17,
  author    = {Julien Lerouge and
               Zeina Abu{-}Aisheh and
               Romain Raveaux and
               Pierre H{\'{e}}roux and
               S{\'{e}}bastien Adam},
  title     = {New binary linear programming formulation to compute the graph edit
               distance},
  journal   = {Pattern Recognition},
  volume    = {72},
  pages     = {254--265},
  year      = {2017},
  url       = {https://doi.org/10.1016/j.patcog.2017.07.029},
  doi       = {10.1016/j.patcog.2017.07.029},
  timestamp = {Fri, 25 Aug 2017 17:16:17 +0200},
  biburl    = {https://dblp.org/rec/bib/journals/pr/LerougeARHA17},
  bibsource = {dblp computer science bibliography, https://dblp.org}
}

The main of the program is in the file Progam.cs.

According to the contest context, three databases were used CMU,GREC and Symbolic. For each data set, specific cost functions were used.

Progam.cs is coded to work on the GREc data set. See lines :
	    g1.DynamicCastLabel(new LabelNodeGrecConstest(), new LabelEdgeGrecContest());
            g2.DynamicCastLabel(new LabelNodeGrecConstest(), new LabelEdgeGrecContest());
To work another dataset just change these two lines :
For molecules or symbolic graphs use : 
	    g1.DynamicCastLabel(new LabelNodeSymbolicGEDContest(nodesub, nodedel), new LabelEdgeSymbolicGEDContest(edgesub, edgedel));
            g2.DynamicCastLabel(new LabelNodeSymbolicGEDContest(nodesub, nodedel), new LabelEdgeSymbolicGEDContest(edgesub, edgedel));


The number of threads can be tuned. 
 //Number of thred here it is 4
iso.Solver.setThreadNumber(4);

The model F2 is defined in ProblemCPLEXUndirected.cs for undirected graphs and ProblemCPLEX.cs for directed graphs.
The name of the function that describes F2 is IsoGraphInexactF2b.


In addition a binary file is provided for GREC graphs: D:\recherche\pfe yanjing\LP\BLPGEDGRECforweb\LP\bin\x64\Release\LP.exe
The input and output of this binary file is compliant with input and output of the contest.
 
 Example of the command line: 
LP image1_5.gxl image3_10.gxl

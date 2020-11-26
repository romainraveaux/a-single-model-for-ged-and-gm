using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using Graphs;
using Matching;
using System.Reflection;

namespace LP
{
    /// <summary>
    /// L'interface de solveur qui encapsule les méthodes pour résoudre un problème linéaire 
    /// </summary>
    public interface ISolver
    {
        void setProb(Problem prob); 
        bool solveProb();
        int SaveProb2File(string nomFichier);
        int solutionSave(string fname);
        bool getMatchingResult(MatchingResult mr);
        void closeSolver();
        void setThreadNumber(int i);
        
    }
}

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using Graphs;
using Matching;

namespace LP
{
    public class MatchingResult //: IGraphMatching
    {
        double timeUse;
        double memoryUse;
        bool memoryOverFlow;
        bool timeOverFlow;
        double distance;
        bool optimal;
        bool feasible;
        int nbNodes;
        int maxTreeNodes;
        Dictionary<string, string> nodeMatchingDictionary;        
        Dictionary<string, string> edgeMatchingDictionary;
        public ILOG.CPLEX.Cplex cplx;

        #region Getters and setters
        public double Distance
        {
            get { return distance; }
            set { distance = value; }
        }
        public Dictionary<string, string> NodeMatchingDictionary
        {
            get { return nodeMatchingDictionary; }
            set { nodeMatchingDictionary = value; }
        }
        public Dictionary<string, string> EdgeMatchingDictionary
        {
            get { return edgeMatchingDictionary; }
            set { edgeMatchingDictionary = value; }
        }
        public int NbNodes
        {
            get { return nbNodes; }
            set { nbNodes = value; }
        }
        public int MaxTreeNodes
        {
            get { return maxTreeNodes; }
            set { maxTreeNodes = value; }
        }
        public double MemoryUse
        {
            get { return memoryUse; }
            set { memoryUse = value; }
        }
        public double TimeUse
        {
            get { return timeUse; }
            set { timeUse = value; }
        }
        public bool TimeOverFlow
        {
            get { return timeOverFlow; }
            set { timeOverFlow = value; }
        }
        public bool MemoryOverFlow
        {
            get { return memoryOverFlow; }
            set { memoryOverFlow = value; }
        }
        public bool Optimal
        {
            get { return optimal; }
            set { optimal = value; }
        }
        public bool Feasible
        {
            get { return feasible; }
            set { feasible = value; }
        }
        #endregion

        public MatchingResult()
        {
            //this.numNodeSupp = 0;
            this.distance = -1;
            NodeMatchingDictionary = new Dictionary<string, string>();
            EdgeMatchingDictionary = new Dictionary<string, string>();
            this.feasible = false;
            this.optimal = false;
            this.timeOverFlow = false;
            this.memoryOverFlow = false;
            this.timeUse = -1;
            this.memoryUse = -1;
        }
       public bool matchingEqual(MatchingResult res)
       {
           if ((res == null) || (this == null)) return false;
         /*  foreach (KeyValuePair<string, string> kvp in this.nodeMatchingDictionary)
           {
                string value=null;
                res.nodeMatchingDictionary.TryGetValue(kvp.Key, out value);
                if (value != null)
                {
                    if (value != kvp.Value) return false;
                    
                }
           }
           foreach (KeyValuePair<string, string> kvp in this.edgeMatchingDictionary)
           {
               string value = null;
               res.edgeMatchingDictionary.TryGetValue(kvp.Key, out value);
               if (value != null)
               {
                   if (value != kvp.Value) return false;

               }
           }*/
           if (Math.Abs(res.distance - this.distance) < 0.0001)
               return true;
           else return false;
       }

       public bool matchingEqual(MatchingResult res, RichTextBox ResultText)
       {
           if ((res == null) || (this == null)) return false;
           foreach (KeyValuePair<string, string> kvp in this.nodeMatchingDictionary)
           {
               string value = null;
               res.nodeMatchingDictionary.TryGetValue(kvp.Key, out value);
               if (value != null)
               {
                   if (value != kvp.Value) return false;
                   else
                   {
                       ResultText.AppendText("\n" + kvp.Key + "--->" + kvp.Value + "   OK");
                   }
               }
           }
           foreach (KeyValuePair<string, string> kvp in this.edgeMatchingDictionary)
           {
               string value = null;
               res.edgeMatchingDictionary.TryGetValue(kvp.Key, out value);
               if (value != null)
               {
                   if (value != kvp.Value) return false;
                   else
                   {
                       ResultText.AppendText("\n" + kvp.Key + "--->" + kvp.Value + "   OK");
                   }
               }
           }
           return true;
       }
    }
}

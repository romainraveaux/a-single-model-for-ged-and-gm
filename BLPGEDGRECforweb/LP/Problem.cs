using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphs;
using Matching;
using System.Reflection;



namespace LP
{
    public abstract class Problem
    {

        protected Graph graph1=null;
        protected Graph graph2 =null;
        public  static bool exchange=false;
        

        // double* lp;

        //construct the variables and the constraints
        public bool constructProb(string formulaType)
        {
            Type thisType = this.GetType();
            if (formulaType != null)
            {

                try
                {
                    MethodInfo theMethod = thisType.GetMethod(formulaType);
                    theMethod.Invoke(this, null);
                    return true;
                    //IsoSousGraphInexactF2a();
                   // IsoGraphInexactF1b();
                  
                    
                }
                catch (Exception e)
                {                    
                    Console.WriteLine("LP.Problem.constructProb : "+ e.Message);
                    return false; 
                }
                
            }
            else return false;
        }

        public abstract void IsoSousGraphExactF1();
        public abstract void IsoSousGraphExactF2();
        public abstract void IsoSousGraphInexactF1a();
        public abstract void IsoSousGraphInexactF2a();
        public abstract void IsoGraphInexactF1b();
        public abstract void IsoGraphInexactF2b();
        public abstract void BLPjusticehero();
        public abstract void BLPjusticeheroQuadratic();
        public abstract void QAPGMGED();

    }
}


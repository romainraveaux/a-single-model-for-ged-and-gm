using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphs;
using Matching;

namespace LP
{
    class LabelEdgeAlkane: Label
    {
        /*////////Attributs////////////////*/

        private double edgeCosts;  
	    private double alpha;
      

        /*////////Constructeur/////////////*/
        public LabelEdgeAlkane()
        {
            this.edgeCosts = 3;
            this.alpha=0.5;
        }
        public LabelEdgeAlkane(double _edgecosts, double _alpha)
        {
            this.edgeCosts=_edgecosts;
            this.alpha=_alpha;
        }
        public LabelEdgeAlkane(string _chemSym, double _edgecosts, double _alpha)
        {
            this.edgeCosts = _edgecosts;
            this.alpha=_alpha;
        }
        
        public override string toString()
        {
            string str = this.Id +", chem : C" ;
           /* foreach (double d in stringMatrix)
            {
                str += d.ToString()+" ";
                
            }*/
            return str;
        }


        public override double dissimilarity(Label label)
        {
            if (label is LabelEdgeAlkane)
            {
                if(this.Id.Equals(ConstantsAC.EPS_ID))//delection
                {
                    return this.alpha * this.edgeCosts;
                }
                if(label.Id.Equals(ConstantsAC.EPS_ID))//insertion
                {

                    return this.alpha * this.edgeCosts;
                }
                    //substitution
                    return this.alpha *0;
               
             }
            return Double.MaxValue;

       }
         
        public override void fromAttributes(List<AttributeGXL> Attributes)
        {
          
        }

        public override List<AttributeGXL> toAttributes()
        {
            List<AttributeGXL> attributes = new List<AttributeGXL>();
           

            return attributes;

        }

      

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphs;
using Matching;

namespace LP
{
    class LabelNodeAlkane: Label
    {
        /*////////Attributs////////////////*/
           
	    private double nodeCosts;  
	    private double alpha;
      

        /*////////Constructeur/////////////*/
        public LabelNodeAlkane()
        {
            this.nodeCosts = 3;
            this.alpha=0.5;
        }
        public LabelNodeAlkane(double _nodecosts, double _alpha)
        {
            this.nodeCosts=_nodecosts;
            this.alpha=_alpha;
        }
        public LabelNodeAlkane(string _chemSym, double _nodecosts, double _alpha)
        {
            this.nodeCosts=_nodecosts;
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
            if (label is LabelNodeAlkane)
            {
            
                if (this.Id.Equals(ConstantsAC.EPS_ID) && label.Id.Equals(ConstantsAC.EPS_ID))
                {
                    return this.alpha * 0;
                }

                if(this.Id.Equals(ConstantsAC.EPS_ID))//delection
                {                     
                    return this.alpha  * this.nodeCosts;
                }
                if(label.Id.Equals(ConstantsAC.EPS_ID))//insertion
                {
                    
                    return this.alpha * this.nodeCosts;
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

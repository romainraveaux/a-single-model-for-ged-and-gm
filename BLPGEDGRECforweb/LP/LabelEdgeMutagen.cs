using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphs;
using Matching;

namespace LP
{
    class LabelEdgeMutagen: Label
    {
        /*////////Attributs////////////////*/
        public int valence;
	    private double EdgeCosts;  
	    private double alpha;

        /*////////Constructeur/////////////*/
        public LabelEdgeMutagen()
        {
            this.valence = 0;
            this.EdgeCosts = 1.1;
            this.alpha=0.25;
        }
        public LabelEdgeMutagen(double _Edgecosts, double _alpha)
        {
            this.valence = 0;
            this.EdgeCosts=_Edgecosts;
            this.alpha=_alpha;
        }        
        public override string toString()
        {
            string str = this.Id+ " : valence = " + valence + "; ";            
            return str;
        }


        public override double dissimilarity(Label label)
        {
            if (label is LabelEdgeMutagen)
            {                
                if(this.Id.Equals(ConstantsAC.EPS_ID))//delection
                {
                    return (1 - this.alpha) * this.EdgeCosts;
                }
                if(label.Id.Equals(ConstantsAC.EPS_ID))//insertion
                {
                    return (1 - this.alpha) * this.EdgeCosts;
                }
                return 0;
              }
            return Double.MaxValue;

       }
         
        public override void fromAttributes(List<AttributeGXL> Attributes)
        {
            foreach (AttributeGXL att in Attributes)
            {
                if (att.Name == "valence") valence = Int32.Parse(att.Value);
            }
        }

        public override List<AttributeGXL> toAttributes()
        {
            List<AttributeGXL> attributes = new List<AttributeGXL>();
            AttributeGXL attValence = new AttributeGXL("int", "valence", valence.ToString());
            attributes.Add(attValence);
            return attributes;

        }

    }
}

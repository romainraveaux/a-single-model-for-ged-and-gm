using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphs;
using Matching;
using System.Globalization;

namespace LP
{
    class LabelEdgeIlpiso : Label
    {
        /*////////Attributs////////////////*/
        public double val;
      
      
        //the constant costs	    
	    private double edgeCosts;  
	    private double alpha;

        /*////////Constructeur/////////////*/
        public LabelEdgeIlpiso()
        {
            val = double.MinValue;
           
           
            this.edgeCosts=200*(2/3.0);
            this.alpha=0.5;
        }
        public LabelEdgeIlpiso(double _edgecosts, double _alpha)
        {
            val = double.MinValue;
            this.edgeCosts = _edgecosts;
            this.alpha=_alpha;
        }
        public LabelEdgeIlpiso(double _val, double _edgecosts, double _alpha)
        {
            val = _val;

            this.edgeCosts = _edgecosts;
            this.alpha=_alpha;
        }
        
        public override string toString()
        {
            return this.Id + " : " + val;
            //return this.Id + " : x = " + x + ";y = " + y + "; type =" + type;
        }


        public override double dissimilarity(Label label)
        {
            if (label is LabelEdgeIlpiso)
            {
                if(this.Id.Equals(ConstantsAC.EPS_ID))//delection
                {
                    return (1-this.alpha) * this.edgeCosts;
                }
                if(label.Id.Equals(ConstantsAC.EPS_ID))//insertion
                {
                    return (1 - this.alpha) * this.edgeCosts;
                }
                return (1 - this.alpha) * Math.Abs(val - ((LabelEdgeIlpiso)label).val);
              }
            return -99999;

       }
         
        public override void fromAttributes(List<AttributeGXL> Attributes)
        {
            foreach (AttributeGXL att in Attributes)
            {
                if (att.Name == "label") val = double.Parse(att.Value);
                
            }
        }

        public override List<AttributeGXL> toAttributes()
        {
            List<AttributeGXL> attributes = new List<AttributeGXL>();
            AttributeGXL attX = new AttributeGXL("double", "label", val.ToString());
           

            attributes.Add(attX);

            return attributes;

        }

    }
}

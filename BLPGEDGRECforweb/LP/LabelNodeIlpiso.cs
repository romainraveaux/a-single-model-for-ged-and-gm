using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphs;
using Matching;
using System.Globalization;

namespace LP
{
    class LabelNodeIlpiso : Label
    {
        /*////////Attributs////////////////*/
        public double val;
      
      
        //the constant costs	    
	    private double nodeCosts;  
	    private double alpha;

        /*////////Constructeur/////////////*/
        public LabelNodeIlpiso()
        {
            val = double.MinValue;
           
           
            this.nodeCosts=200*(2/3.0);
            this.alpha=0.5;
        }
        public LabelNodeIlpiso(double _nodecosts, double _alpha)
        {
            val = double.MinValue;
            this.nodeCosts=_nodecosts;
            this.alpha=_alpha;
        }
        public LabelNodeIlpiso(double _val, double _nodecosts, double _alpha)
        {
            val = _val;
       
            this.nodeCosts=_nodecosts;
            this.alpha=_alpha;
        }
        
        public override string toString()
        {
            return this.Id + " : " + val;
            //return this.Id + " : x = " + x + ";y = " + y + "; type =" + type;
        }


        public override double dissimilarity(Label label)
        {
            if (label is LabelNodeIlpiso)
            {
                if(this.Id.Equals(ConstantsAC.EPS_ID))//delection
                {
                    return this.alpha * this.nodeCosts;
                }
                if(label.Id.Equals(ConstantsAC.EPS_ID))//insertion
                {
                    return this.alpha * this.nodeCosts;
                }
                return this.alpha * Math.Abs(val - ((LabelNodeIlpiso)label).val);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphs;
using Matching;
using System.Globalization;

namespace LP
{
    class LabelNodeGrec : Label
    {
        /*////////Attributs////////////////*/
        public double x;
        public double y;
        public string type;
        //the constant costs	    
	    private double nodeCosts;  
	    private double alpha;

        /*////////Constructeur/////////////*/
        public LabelNodeGrec()
        {
            x = double.MinValue;
            y = double.MinValue;
            type = "";
            this.nodeCosts=90;
            this.alpha=0.5;
        }
        public LabelNodeGrec(double _nodecosts, double _alpha)
        {
            x = double.MinValue;
            y = double.MinValue;
            type = "";
            this.nodeCosts=_nodecosts;
            this.alpha=_alpha;
        }
        public LabelNodeGrec(double _x, double _y, string _type,double _nodecosts, double _alpha)
        {
            x = _x;
            y = _y;
            type = _type;
            this.nodeCosts=_nodecosts;
            this.alpha=_alpha;
        }
        
        public override string toString()
        {
            return this.Id + " : "+ x + ";" + y ;
            //return this.Id + " : x = " + x + ";y = " + y + "; type =" + type;
        }


        public override double dissimilarity(Label label)
        {
            if (label is LabelNodeGrec)
            {
                if(this.Id.Equals(ConstantsAC.EPS_ID))//delection
                {
                    return this.alpha * this.nodeCosts;
                }
                if(label.Id.Equals(ConstantsAC.EPS_ID))//insertion
                {
                    return this.alpha * this.nodeCosts;
                }
                if (this.type == ((LabelNodeGrec)label).type)
                {
                    double distance = Math.Sqrt(Math.Pow((((LabelNodeGrec)label).x - x), 2)+ Math.Pow((((LabelNodeGrec)label).y - y), 2));
				    /*NumberFormatInfo decFormat = new NumberFormatInfo();
                    decFormat.NumberDecimalSeparator = ".";				    
				    String distanceString = distance.ToString(decFormat);
				    distance = Double.Parse(distanceString);*/
				    return this.alpha * distance;
                }
                else 
                {
                    return this.alpha * (2*this.nodeCosts);
                }
              }
                return this.alpha * this.nodeCosts;

       }
         
        public override void fromAttributes(List<AttributeGXL> Attributes)
        {
            foreach (AttributeGXL att in Attributes)
            {
                if (att.Name == "x") x = double.Parse(att.Value);
                else if (att.Name == "y") y = double.Parse(att.Value);
                else if (att.Name == "type") type = att.Value;
            }
        }

        public override List<AttributeGXL> toAttributes()
        {
            List<AttributeGXL> attributes = new List<AttributeGXL>();
            AttributeGXL attX = new AttributeGXL("double", "x", x.ToString());
            AttributeGXL attY = new AttributeGXL("double", "y", y.ToString());
            AttributeGXL attType = new AttributeGXL("string", "type", type);

            attributes.Add(attX);
            attributes.Add(attY);
            attributes.Add(attType);

            return attributes;

        }

    }
}

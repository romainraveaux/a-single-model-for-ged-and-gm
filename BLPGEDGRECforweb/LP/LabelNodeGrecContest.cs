using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphs;
using Matching;
using System.Globalization;

namespace LP
{
    class LabelNodeGrecConstest : Label
    {
        /*////////Attributs////////////////*/
        public double x;
        public double y;
        public string type;
        //the constant costs	    
	  

        /*////////Constructeur/////////////*/
        public LabelNodeGrecConstest()
        {
            x = double.MinValue;
            y = double.MinValue;
            type = "";
          
        }
      
        public override string toString()
        {
            return this.Id + " : "+ x + ";" + y ;
            //return this.Id + " : x = " + x + ";y = " + y + "; type =" + type;
        }


        public override double dissimilarity(Label label)
        {
            if (label is LabelNodeGrecConstest)
            {
                if(this.Id.Equals(ConstantsAC.EPS_ID))//delection
                {
                    return 45;
                }
                if(label.Id.Equals(ConstantsAC.EPS_ID))//insertion
                {
                    return 45;
                }
                if (this.type == ((LabelNodeGrecConstest)label).type)
                {
                    double distance = Math.Sqrt(Math.Pow((((LabelNodeGrecConstest)label).x - x), 2) + Math.Pow((((LabelNodeGrecConstest)label).y - y), 2));
				    /*NumberFormatInfo decFormat = new NumberFormatInfo();
                    decFormat.NumberDecimalSeparator = ".";				    
				    String distanceString = distance.ToString(decFormat);
				    distance = Double.Parse(distanceString);*/
				    return 0.5 * distance;
                }
                else 
                {
                    return 90;
                }
              }
            return double.MaxValue;

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

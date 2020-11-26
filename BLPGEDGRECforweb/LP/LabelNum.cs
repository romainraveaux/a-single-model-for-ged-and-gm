using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphs;
using Matching;

namespace LP
{
    public class LabelNum : GenericLabel
    {

        public override string toString ()
		{
			string s = Id + ";";
			foreach (AttributeGXL att in this.AttributesGXL) {
				s += att.Name + "=" + att.Value.ToString() + ";";
			}
			return s;
		}

	/*public override double dissimilarity (Label label)
		{
            double distance = double.MaxValue;            
            if (label is NumLabel)
            {
                if (label.Id == ConstantsAC.EPS_ID)
                {
                    for (int i = 0; i < this.AttributesGXL.Count(); i++)
                    {
                        double value1 = Convert.ToDouble(this.AttributesGXL[i].Value.Replace('.', ','));
                        distance += Math.Pow(value1, 2);
                    }
                    if (distance != 0) return Math.Sqrt(distance);
                    else return 1;
                }

                if (this.AttributesGXL.Count() != ((NumLabel)label).AttributesGXL.Count()) return double.MaxValue;
                else
                {
                    distance = 0;
                    for (int i = 0; i < this.AttributesGXL.Count(); i++)
                    {                        
                        if (this.AttributesGXL[i].Type != ((NumLabel)label).AttributesGXL[i].Type) return double.MaxValue;
                        else
                        {
                            double value1 = Convert.ToDouble(this.AttributesGXL[i].Value.Replace('.', ','));
                            double value2 = Convert.ToDouble(((NumLabel)label).AttributesGXL[i].Value.Replace('.', ','));
                            double diff = value1 - value2;
                            distance += Math.Pow(diff, 2);
                        }
                    }
                    return Math.Sqrt(distance);
                }
			  }
            return distance;
		}*/

        public override double dissimilarity (Label label)

        {
            double distance = 0;
            
            if (label.Id == ConstantsAC.EPS_ID)
            {

                for (int i = 0; i < this.AttributesGXL.Count(); i++)

                {

                    double value1 = Convert.ToDouble(this.AttributesGXL[i].Value);

                    distance += Math.Pow(value1, 2);

                }

                if (distance != 0) return Math.Sqrt(distance);

                else return 1;

            }

            distance = double.MaxValue;
            if (label is LabelNum)
            {       

                if (this.AttributesGXL.Count() != ((LabelNum)label).AttributesGXL.Count()) return double.MaxValue;

                else

                {

                    distance = 0;

                    for (int i = 0; i < this.AttributesGXL.Count(); i++)

                    {                        

                        if (this.AttributesGXL[i].Type != ((LabelNum)label).AttributesGXL[i].Type) return double.MaxValue;

                        else

                        {

                            double value1 = Convert.ToDouble(this.AttributesGXL[i].Value);

                            double value2 = Convert.ToDouble(((LabelNum)label).AttributesGXL[i].Value);

                            double diff = value1 - value2;

                            distance += Math.Pow(diff, 2);

                        }

                    }

                    return Math.Sqrt(distance);

                }

           }

            return distance;

     }

    }
}

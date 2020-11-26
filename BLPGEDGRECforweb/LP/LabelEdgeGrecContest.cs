using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphs;
using Matching;

namespace LP
{
    class LabelEdgeGrecContest : Label
    {
        public int frequency;
        public List<string> type;
        public List<string> angle;

        //the constant costs	    
        private double edgeCosts;
        private double alpha;
        private MunkresRec munkresRec;

        public LabelEdgeGrecContest()
        {
            frequency=0;
            this.alpha = 0.5;
            this.edgeCosts = 15;
            type = new List<string>();
            angle = new List<string>();
            this.munkresRec = new MunkresRec();
        
        }
       
        public override string toString()
        {
            string edgeString = this.Id + ": ";
           /* for (int i = 0; (i < type.Count)&&(i < type.Count)&&(i<frequency); i++)
            {
                edgeString += " type" + i + " : " + type[i] + " angle" + i + " : " + angle[i];
            }*/
            return edgeString;
        }

        public override double dissimilarity(Label label)
        {
            if (label is LabelEdgeGrecContest)
            {
                int startFreq = this.frequency;
                int endFreq = ((LabelEdgeGrecContest)label).frequency;
                if (this.Id.Equals(ConstantsAC.EPS_ID)) 
                {				   
				    return 7.5*endFreq;
			    }
			    if (label.Id.Equals(ConstantsAC.EPS_ID)) 
                {
                    return 7.5*startFreq;
			    }
                if (startFreq == 1 && endFreq == 1)
                {
                    String startType = this.type[0];
                    String endType = ((LabelEdgeGrecContest)label).type[0];
                    if (startType.Equals(endType))
                    {
                        return 0;
                    }
                    else
                    {
                        return 15;
                    }

                }

                if (startFreq == 2 && endFreq == 2)
                {
                    return 0;
                }

                return 7.5;
		    }
            return Double.MaxValue;
        }

        public override void fromAttributes(List<AttributeGXL> Attributes)
        {
            foreach (AttributeGXL att in Attributes)
            {
                if (att.Name == "frequency") this.frequency = Int32.Parse(att.Value);
                else if(att.Name.StartsWith("type")) this.type.Add(att.Value);
                else if(att.Name.StartsWith("angle")) this.angle.Add(att.Value);
            }
        }

        public override List<AttributeGXL> toAttributes()
        {
            List<AttributeGXL> attributes = new List<AttributeGXL>();
            if ((frequency == type.Count) && (frequency == angle.Count))
            {
                AttributeGXL attFrequency = new AttributeGXL("int", "frequency", frequency.ToString());
                attributes.Add(attFrequency);
                for (int i = 0; i < frequency; i++)
                {
                    AttributeGXL attType = new AttributeGXL("string", "type"+i, type[i]);
                    AttributeGXL attAngle= new AttributeGXL("string", "angle"+i, angle[i]);
                    attributes.Add(attType);
                    attributes.Add(attAngle);
                }
            }
            return attributes;
        }

        private void printMatrix(double[,] matrix)
        {
		    foreach (int i in matrix)
            {
		        Console.Out.Write(i +" ");
		    }
	  }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphs;
using Matching;

namespace LP
{
    class LabelEdgeProtein : Label
    {
        public int frequency;
        public List<string> type;
        public List<string> angle;

        //the constant costs	    
        private double edgeCosts;
        private double alpha;
        private MunkresRec munkresRec;

        public LabelEdgeProtein()
        {
             frequency=0;
            this.alpha = 0.75;
            this.edgeCosts = 1;
            type = new List<string>();
            angle = new List<string>();
            this.munkresRec = new MunkresRec();

        }
        public LabelEdgeProtein(int freq)
        {
            this.alpha = 0.75;
            this.edgeCosts = 1;
            frequency = freq;
            type = new List<string>();
            angle = new List<string>();
            this.munkresRec = new MunkresRec();
        }

        public LabelEdgeProtein(int freq, double _alpha, double _edgeCosts)
        {
            frequency = freq;
            this.alpha = _alpha;
            this.edgeCosts = _edgeCosts;
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
            if (label is LabelEdgeProtein)
            {
                int startFreq = this.frequency;
                int endFreq = ((LabelEdgeProtein)label).frequency;
                if (this.Id.Equals(ConstantsAC.EPS_ID))
                {
                    return (1 - this.alpha) * endFreq * this.edgeCosts;
                }
                if (label.Id.Equals(ConstantsAC.EPS_ID))
                {
                    return (1 - this.alpha) * startFreq * this.edgeCosts;
                }
                int n = startFreq + endFreq;
                double[,] matrix = new double[n, n];
                for (int i = 0; i < startFreq; i++)
                {
                    String startType = this.type[i];
                    for (int j = 0; j < endFreq; j++)
                    {
                        String endType = ((LabelEdgeProtein)label).type[j];
                        if (startType.Equals(endType))
                        {
                            matrix[i, j] = 0;
                        }
                        else
                        {
                            matrix[i, j] = 2 * this.edgeCosts;
                        }
                    }
                }
                for (int i = this.frequency; i < startFreq + endFreq; i++)
                {
                    for (int j = 0; j < endFreq; j++)
                    {
                        if (i - this.frequency == j)
                        {
                            matrix[i, j] = this.edgeCosts;
                        }
                        else
                        {
                            matrix[i, j] = Double.MaxValue;
                        }
                    }
                }
                for (int i = 0; i < this.frequency; i++)
                {
                    for (int j = endFreq; j < startFreq + endFreq; j++)
                    {
                        if (j - endFreq == i)
                        {
                            matrix[i, j] = this.edgeCosts;
                        }
                        else
                        {
                            matrix[i, j] = Double.MaxValue;
                        }
                    }
                }
                return (1 - this.alpha) * this.munkresRec.getCosts(matrix);			//?
                // return Math.Abs(Double.Parse(this.angle) - ((LabelEdgeGrec)label).angle) / 2 * Math.PI;
            }
            return Double.MaxValue;
        }

        public override void fromAttributes(List<AttributeGXL> Attributes)
        {
            foreach (AttributeGXL att in Attributes)
            {
                if (att.Name == "frequency") this.frequency = Int32.Parse(att.Value);
                else if (att.Name.StartsWith("type")) this.type.Add(att.Value);
                else if (att.Name.StartsWith("angle")) this.angle.Add(att.Value);
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
                    AttributeGXL attType = new AttributeGXL("string", "type" + i, type[i]);
                    AttributeGXL attAngle = new AttributeGXL("string", "angle" + i, angle[i]);
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
                Console.Out.Write(i + " ");
            }
        }
    }
}

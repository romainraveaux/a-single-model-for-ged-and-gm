using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphs;
using Matching;

namespace LP
{
    class LabelNodeMutagen: Label
    {
        /*////////Attributs////////////////*/
        public string chemSym;
           
	    private double nodeCosts;  
	    private double alpha;
        private double[,] stringMatrix;

        /*////////Constructeur/////////////*/
        public LabelNodeMutagen()
        {
            chemSym = null;
            stringMatrix=null;
            this.nodeCosts = 11;
            this.alpha=0.25;
        }
        public LabelNodeMutagen(double _nodecosts, double _alpha)
        {
            chemSym = null;
            stringMatrix=null;
            this.nodeCosts=_nodecosts;
            this.alpha=_alpha;
        }
        public LabelNodeMutagen(string _chemSym, double _nodecosts, double _alpha)
        {
            chemSym = _chemSym;
            stringMatrix=null;
            this.nodeCosts=_nodecosts;
            this.alpha=_alpha;
        }
        
        public override string toString()
        {
            string str = this.Id +", chem : "+ chemSym ;
           /* foreach (double d in stringMatrix)
            {
                str += d.ToString()+" ";
                
            }*/
            return str;
        }


        public override double dissimilarity(Label label)
        {
            if (label is LabelNodeMutagen)
            {
                string chemSym1= this.chemSym;
			    string chemSym2= ((LabelNodeMutagen)label).chemSym;

                if (this.Id.Equals(ConstantsAC.EPS_ID) && label.Id.Equals(ConstantsAC.EPS_ID))//delection
                {
                    return this.alpha * 0;
                }

                if(this.Id.Equals(ConstantsAC.EPS_ID))//delection
                {                     
                    /*double l2 = chemSym2.Length;
                    return this.alpha * l2 * this.nodeCosts;*/
                    
                    return this.alpha * 2 * this.nodeCosts;
                }
                if(label.Id.Equals(ConstantsAC.EPS_ID))//insertion
                {
                    /*double l1 = chemSym1.Length;
                    return this.alpha * l1 * this.nodeCosts;*/
                    return this.alpha * 2 * this.nodeCosts;
                }
                if (chemSym1.Equals(chemSym2) == true)
                {
                    return 0;
                }
                else
                {
                    return this.alpha * 2 * this.nodeCosts;
                }
                //return this.alpha * this.getStringDistance(chemSym1, chemSym2);
              }
            return Double.MaxValue;

       }
         
        public override void fromAttributes(List<AttributeGXL> Attributes)
        {
            foreach (AttributeGXL att in Attributes)
            {
                if (att.Name == "chem") chemSym = att.Value;
            }
        }

        public override List<AttributeGXL> toAttributes()
        {
            List<AttributeGXL> attributes = new List<AttributeGXL>();
            AttributeGXL attChem= new AttributeGXL("string", "chem", chemSym.ToString());
            attributes.Add(attChem);

            return attributes;

        }

        private double getStringDistance(String s1, String s2) {
		int n = s1.Length;
		int m = s2.Length;
		if (m > n) {
			String s = s1;
			s1 = s2;
			s2 = s;
			n = s1.Length;
			m = s2.Length;
		}
		s2 += s2;
		m *= 2;
		this.stringMatrix = new double[n + 1,m + 1];
		this.stringMatrix[0,0] = 0;
		for (int i = 1; i <= n; i++) {
			this.stringMatrix[i,0] = this.stringMatrix[i - 1,0]+ this.nodeCosts;
		}
		for (int j = 1; j <= m; j++) {
			this.stringMatrix[0,j] = this.stringMatrix[0,j - 1];
		}

		for (int i = 1; i <= n; i++) {
			for (int j = 1; j <= m; j++) {
				double subst = 0;
				if (s1[i - 1] == s2[j - 1]) {
					subst = 0;
				} else {
					subst = this.nodeCosts;
				}
				double m1 = this.stringMatrix[i - 1,j - 1] + subst;
				double m2 = this.stringMatrix[i - 1,j] + this.nodeCosts;
				double m3 = this.stringMatrix[i,j - 1] + this.nodeCosts;
				this.stringMatrix[i,j] = Math.Min(m1, Math.Min(m2, m3));
			}
		}
		double min = Double.MaxValue;
		for (int j = 0; j <= m; j++) {
			double current = this.stringMatrix[n,j];
			if (current < min) {
				min = current;
			}
		}
		return min;
	}

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphs;
using Matching;

namespace LP
{
    class LabelNodeSymbolicGEDContest: Label
    {
        /*////////Attributs////////////////*/
        public string chemSym;
        public static double sub;
        public static double del;   
	   
        /*////////Constructeur/////////////*/

        public LabelNodeSymbolicGEDContest()
        {
            
            chemSym = null;
        }

        
      
        public LabelNodeSymbolicGEDContest(double sub1,double del1)
        {
            sub = sub1;
            del = del1;
            chemSym = null;
        }

        
        public override string toString()
        {
            string str = this.Id +", chem : "+ chemSym ;
          
            return str;
        }


        public override double dissimilarity(Label label)
        {
            if (label is LabelNodeSymbolicGEDContest)
            {
                string chemSym1= this.chemSym;
                string chemSym2 = ((LabelNodeSymbolicGEDContest)label).chemSym;

               
                if(this.Id.Equals(ConstantsAC.EPS_ID))//delection
                {

                    return del;
                }
                if(label.Id.Equals(ConstantsAC.EPS_ID))//insertion
                {

                    return del;
                }
                if (chemSym1.Equals(chemSym2) == true)
                {
                    return 0;
                }
                else
                {
                    return sub;
                }
               
              }
            return Double.MaxValue;

       }
         
        public override void fromAttributes(List<AttributeGXL> Attributes)
        {
            foreach (AttributeGXL att in Attributes)
            {
                //if (att.Name == "chem") chemSym = att.Value;
                chemSym = att.Value;
            }
        }

        public override List<AttributeGXL> toAttributes()
        {
            List<AttributeGXL> attributes = new List<AttributeGXL>();
            AttributeGXL attChem= new AttributeGXL("string", "chem", chemSym.ToString());
            attributes.Add(attChem);

            return attributes;

        }

       

    }
}

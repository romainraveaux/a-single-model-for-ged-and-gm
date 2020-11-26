using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LP
{
    class MatrixElement
    {

        /**
         * the value of this element
         */
        private double value;

        /**
         * is this element marked
         */
        private bool starred, primed;

        /**
         * an element knows his col and row
         */
        private int myRow, myCol;

        /**
         * Getters and Setters
         */
        public int getMyCol()
        {
            return myCol;
        }
        public void setMyCol(int myCol)
        {
            this.myCol = myCol;
        }
        public int getMyRow()
        {
            return myRow;
        }
        public void setMyRow(int myRow)
        {
            this.myRow = myRow;
        }
        public MatrixElement(double value)
        {
            this.value = value;
        }
        public bool isPrimed()
        {
            return primed;
        }
        public void setPrimed(bool primed)
        {
            this.primed = primed;
        }
        public bool isStarred()
        {
            return starred;
        }
        public void setStarred(bool starred)
        {
            this.starred = starred;
        }
        public double getValue()
        {
            return value;
        }
        public void setValue(double value)
        {
            this.value = value;
        }

    }
}

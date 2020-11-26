using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using Graphs;
using Matching;
using System.Runtime.InteropServices;
using System.Transactions;
using System.Threading;


namespace LP
{
    public unsafe class SolverGLPK : ISolver
    {
        #region Attributes
        /*******************************************************************************************/
        /****************************************Attributes******************************************/
        /*******************************************************************************************/
        long memUse; 
        int simplexReturn;//the value return after solved by simplex
        int mipReturn;//the value return after solved by mip        
        bool simplexOK;
        bool mipOK;
        bool simplexTimeOverflow;
        bool mipTimeOverflow;
        bool mipMemoryOverflow;
        double timeUse=-1;
        int SimplextimeUse = 0;
        /*n_cnt is the current number of all (active and inactive) nodes;
         *t_cnt is the total number of nodes including those which have been already removed from the
         *tree. This count is increased whenever a new node appears in the tree and never decreased.
         */
        int n_cnt=0, t_cnt=0, n_cnt_max=0;
        public struct glp_tree { double _opaque_tree; }


        public const string glpkLibrary = "glpk_4_49";
        public static readonly int GLP_BV = 3; /*binary variable*/

        public static readonly int GLP_LO = 2;  /* lower bound */
        public static readonly int GLP_UP = 3; /* upper bound */
        public static readonly int GLP_DB = 4; /* double-bounded */
        public static readonly int GLP_FX = 5; /* fixed */

        public static readonly int GLP_UNDEF = 1;  /* solution is undefined */
        public static readonly int GLP_FEAS = 2;   /* solution is feasible */
        public static readonly int GLP_INFEAS = 3; /* solution is infeasible */
        public static readonly int GLP_NOFEAS = 4; /* no feasible solution exists */
        public static readonly int GLP_OPT = 5;     /* solution is optimal */
        public static readonly int GLP_UNBND = 6;   /* solution is unbounded */

        public static readonly int GLP_ON = 1;
        public static readonly int GLP_OFF = 0;
        public static readonly int GLP_KKT_PE=1;
        public static readonly int GLP_KKT_PB =2;
        public static readonly int GLP_KKT_DE  =3;
        public static readonly int GLP_KKT_DB  = 4;
        public static readonly int GLP_KKT_CS = 5;


        /* return codes: */
        public const int GLP_EBADB = 0x01;/* invalid basis */
        public const int GLP_ESING = 0x02;  /* singular matrix */
        public const int GLP_ECOND = 0x03; /* ill-conditioned matrix */
        public const int GLP_EBOUND = 0x04;  /* invalid bounds */
        public const int GLP_EFAIL = 0x05;/* solver failed */
        public const int GLP_EOBJLL = 0x06; /* objective lower limit reached */
        public const int GLP_EOBJUL = 0x07;/* objective upper limit reached */
        public const int GLP_EITLIM = 0x08;/* iteration limit exceeded */
        public const int  GLP_ETMLIM = 0x09; /* time limit exceeded */
        public const int GLP_ENOPFS = 0x0A; /* no primal feasible solution */
        public const int GLP_ENODFS = 0x0B; /* no dual feasible solution */
        public const int GLP_EROOT = 0x0C; /* root LP optimum not provided */
        public const int GLP_ESTOP = 0x0D; /* search terminated by application */
        public const int GLP_EMIPGAP = 0x0E; /* relative mip gap tolerance reached */
        public const int GLP_ENOFEAS = 0x0F; /* no primal/dual feasible solution */
        public const int GLP_ENOCVG = 0x10; /* no convergence */
        public const int GLP_EINSTAB = 0x11; /* numerical instability */
        public const int GLP_EDATA = 0x12; /* invalid data */
        public const int GLP_ERANGE = 0x13; /* result out of range */
        public const int GLP_SF_GM = 0x01;
        public const int GLP_SF_EQ  = 0x10;
        public const int GLP_SF_2N  = 0x20;
        public const int GLP_SF_SKIP =  0x40;
        public const int GLP_SF_AUTO =  0x80;
                    
        #endregion

        #region DLLImport
        /*******************************************************************************************/
        /****************************************DLLImport******************************************/
        /*******************************************************************************************/

        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern double* glp_create_prob();
       // [DllImport(glpkLibrary, CharSet=CharSet.Auto,CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        //public static extern int glp_init_smcp([In, MarshalAs(UnmanagedType.LPStruct)]glp_smcp parm);
       // public static extern int glp_init_smcp(void* parm);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void glp_set_prob_name(double* P, string name);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void glp_add_rows(double* lp, int rows);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void glp_add_cols(double* lp, int cols);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void glp_set_col_bnds(double* lp, int col, int bound_type, double lower_bound, double upper_bound);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void glp_set_col_kind(double* lp, int col, int kind);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void glp_set_obj_dir(double* lp, int dir);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void glp_load_matrix(double* lp, int elements, int[] ia, int[] ja, double[] ar);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void glp_set_col_name(double* P, int j, string name);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void glp_set_row_name(double* P, int i, string name);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void glp_set_row_bnds(double* P, int i, int type, double lb, double ub);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void glp_set_obj_coef(double* P, int j, double coef);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void glp_create_index(double* P);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern int glp_find_col(double* P, string name);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        static extern int glp_simplex(double* P,  glp_smcp parm);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        static extern double glp_get_obj_val(double* P);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        static extern double glp_get_obj_coef(double* P, int j);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern double glp_get_col_prim(double* P, int j);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true, CharSet = CharSet.Ansi)]
        //[return: MarshalAs(UnmanagedType.LPStr)]
        public static extern IntPtr glp_get_col_name(double* P, int j);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern int glp_get_num_cols(double* P);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void glp_delete_prob(double* lp);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern int glp_write_prob(double* P, int flags, string fname);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern int glp_print_mip(double* P, string fname);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern int glp_get_col_kind(double* P, int j);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern int glp_get_status(double* P);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern int glp_get_num_int(double* P);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern int glp_get_num_bin(double* P);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern int glp_intopt(double* P, glp_iocp parm);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern double glp_mip_obj_val(double* P);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern double glp_mip_row_val(double* P, int i);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern double glp_mip_col_val(double* P, int j);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern int glp_mip_status(double* P);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern int glp_write_lp(double* P, void* parm, string fname);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void glp_init_iocp(void* parm);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void glp_mem_usage(int* count, int* cpeak, ref long total, int* tpeak);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void glp_mem_limit(int limit);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern int glp_get_prim_stat(double* P);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void glp_error_hook(DelegateFunc func, double* info);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern int glp_free_env();
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void glp_ios_tree_size(glp_tree* T, int* a_cnt, ref int n_cnt, ref int t_cnt);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void glp_ios_terminate(glp_tree* T);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern int glp_term_out(int flag);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void glp_scale_prob(double* P, int flags);

        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void glp_check_kkt(double *P, int sol, int cond, ref double  ae_max, ref int ae_ind,
         ref double re_max, ref int re_ind);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void glp_unscale_prob(double* P);
        [DllImport(glpkLibrary, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern int glp_interior(double *P, double *parm);


        #endregion
        public delegate void DelegateFunc(IntPtr info);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void CallbackDelegate(glp_tree* tree, void* info);
         void callBack( glp_tree* tree, void *info) 
         {
             long memory=0;
             double memoryMB=0;
             try
             {
                glp_mem_usage(null, null, ref memory, null);
                memoryMB = (double)memory / 1048576.0;
                glp_ios_tree_size(tree, null, ref n_cnt, ref t_cnt);
                if (n_cnt_max < n_cnt) n_cnt_max = n_cnt;
                 if (memoryMB > Program.MAXMEMORY_MB)
                 {
                     glp_ios_terminate(tree);
                     Console.Out.WriteLine("Memory Limit exceed");
                     mipMemoryOverflow = true;
                 }
             }
             catch (Exception e)
             {
                 Console.Out.WriteLine(e.ToString());
             }
         }


        #region glp init fonctions

        void glp_init_smcp(glp_smcp parm)
        {
            parm.msg_lev = glp_smcp.GLP_MSG_ALL;
            parm.meth = glp_smcp.GLP_PRIMAL;
            parm.pricing = glp_smcp.GLP_PT_PSE;
            parm.r_test = glp_smcp.GLP_RT_HAR;
            parm.tol_bnd = 1e-7;
            parm.tol_dj = 1e-7;
            parm.tol_piv = 1e-10;
            parm.obj_ll = Double.MinValue;
            parm.obj_ul = Double.MaxValue;
            parm.it_lim = Int32.MaxValue;
            parm.tm_lim = Int32.MaxValue;
            parm.out_frq = 500;
            parm.out_dly = 0;
            parm.presolve = glp_smcp.GLP_OFF;
            return;
        }

        /***********************************************************************
            *  NAME
            *
            *  glp_init_iocp - initialize integer optimizer control parameters
            *
            *  SYNOPSIS
            *
            *  void glp_init_iocp(glp_iocp *parm);
            *
            *  DESCRIPTION
            *
            *  The routine glp_init_iocp initializes control parameters, which are
            *  used by the integer optimizer, with default values.
            *
            *  Default values of the control parameters are stored in a glp_iocp
            *  structure, which the parameter parm points to. */

        void glp_init_iocp(glp_iocp parm)
        {
              parm.msg_lev = glp_smcp.GLP_MSG_ALL;
              parm.br_tech = glp_iocp.GLP_BR_DTH;
              parm.bt_tech = glp_iocp.GLP_BT_BLB;
              parm.tol_int = 1e-5;
              parm.tol_obj = 1e-7;
              parm.tm_lim = Int32.MaxValue;
              parm.out_frq = 5000;
              parm.out_dly = 10000;
              parm.cb_func = null;
              parm.cb_info = null;
              parm.cb_size = 0;
              parm.pp_tech = glp_iocp.GLP_PP_ALL;
              parm.mip_gap = 0.0;
              parm.mir_cuts = glp_smcp.GLP_OFF;
              parm.gmi_cuts = glp_smcp.GLP_OFF;
              parm.cov_cuts = glp_smcp.GLP_OFF;
              parm.clq_cuts = glp_smcp.GLP_OFF;
              parm.presolve = glp_smcp.GLP_OFF;
              parm.binarize = glp_smcp.GLP_OFF;
              parm.fp_heur = glp_smcp.GLP_OFF;
              parm.alien = glp_smcp.GLP_OFF;
              return;
        }

        #endregion

        double* lp;
        public SolverGLPK()
        { 
        }
        public void setProb(Problem prob)
        {
            lp = ((ProblemGLPK)prob).lp;           
        }
       

        public bool solveProb()
        {
            int num_int = glp_get_num_int(lp);
            int num_bin = glp_get_num_bin(lp);
            Console.WriteLine("num_int : " + num_int.ToString());
            Console.WriteLine("num_bin : " + num_bin.ToString());

            //glp_term_out(GLP_OFF);
            /*
             * 
             * ae_max | largest absolute error;
                ae_ind | number of row (KKT.PE), column (KKT.DE), or variable (KKT.PB, KKT.DB) with
                the largest absolute error;
                re_max | largest relative error;
                re_ind | number of row (KKT.PE), column (KKT.DE), or variable (KKT.PB, KKT.DB) with
                the largest relative error
             * 
             */
       
            glp_smcp parm = new glp_smcp();
            glp_init_smcp(parm);
            parm.meth = glp_smcp.GLP_DUALP;
            //parm.presolve = GLP_ON;
            parm.tm_lim = (int)Program.MAXTIME_SIMPLEX;//time limit of simplex
            parm.it_lim = 300000;
            //glp_unscale_prob(lp);

            glp_scale_prob(lp, GLP_SF_AUTO);
           
           // glp_mem_limit(1);//Program.MAXMEMORY_MB);
            TimeSpan start = new TimeSpan(DateTime.Now.Ticks);
            simplexReturn = glp_simplex(lp, parm);//glp_interior(lp, null);
            TimeSpan SimplexEnd = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan usingtimeSimplex = SimplexEnd.Subtract(start).Duration();
            SimplextimeUse = (int)usingtimeSimplex.TotalMilliseconds;

            #region comment : simplex return valeur
            /*
             *0 The problem instance has been successfully solved. (This code does not necessarily 
             *   mean that the solver has found optimal solution. It only means that the solution process was successful.)
             *GLP_EBOUND Unable to start the search, because some double-bounded variables have incorrect 
             *   bounds or some integer variables have non-integer (fractional) bounds. 
             *GLP_EROOT Unable to start the search, because optimal basis for initial LP relaxation is not 
             *   provided. (This code may appear only if the presolver is disabled.) 
             *GLP_ENOPFS Unable to start the search, because LP relaxation of the MIP problem instance has no primal feasible solution. 
             *   (This code may appear only if the presolver is enabled.)
             *GLP_ENODFS Unable to start the search, because LP relaxation of the MIP problem instance has 
             *   no dual feasible solution. In other word, this code means that if the LP relaxation 
             *   has at least one primal feasible solution, its optimal solution is unbounded, so if the MIP problem 
             *   has at least one integer feasible solution, its (integer) optimal solution is also unbounded.
             *   (This code may appear only if the presolver is enabled.) 
             *GLP_EFAIL The search was prematurely terminated due to the solver failure. 
             *GLP_EMIPGAP The search was prematurely terminated, because the relative mip gap tolerance has been reached.
             *GLP_ETMLIM The search was prematurely terminated, because the time limit has been exceeded.
             *GLP_ESTOP The search was prematurely terminated by application. (This code may appear only if the advanced solver interface is used.) 
             * 
             * */
            /*** statusSimplex : 
             * GLP_UNDEF | primal solution is undefined; 
             * GLP_FEAS | primal solution is feasible; 
             * GLP_INFEAS | primal solution is infeasible;
             * GLP_NOFEAS | no primal feasible solution exists.
             * 
             */ 

            #endregion

            int statusSimplex = glp_get_prim_stat(lp);
            switch(simplexReturn)
            {
                case 0:
                    if(statusSimplex==GLP_FEAS)
                    {
                        simplexOK = true;
                        //Console.Out.WriteLine("simplex : solution réalisable");
                        try
                        {
                            mipSolve();
                        }
                        catch (Exception)
                        {
                            Console.Out.WriteLine("memoire dépassée de MIP");
                            //Transaction.Current.Rollback();
                        }

                    }
                    else
                    {
                        simplexOK=false;
                        //Console.Out.WriteLine("Simplex ne peut pas obtenir une solution réalisable");
                    } 
                    break;

                case GLP_ETMLIM :
                    this.simplexTimeOverflow = true;
                    //Console.Out.WriteLine("simplex : temps limit dépassée");
                    if (statusSimplex == GLP_FEAS)
                    {
                        simplexOK = true;
                        //Console.Out.WriteLine("simplex : solution réalisable");
                        try
                        {
                            mipSolve();
                        }
                        catch (Exception)
                        {
                            Console.Out.WriteLine("bloc CATCH : memoire dépassée de MIP");
                            //Transaction.Current.Rollback();
                        }
                    }
                    else 
                    {
                        simplexOK = false;
                        glp_mem_usage(null, null, ref memUse, null);
                        //Console.Out.WriteLine("simplex : pas de solution réalisable");
                    }
                    break;
                default:
                    {
                        simplexOK = false;
                        glp_mem_usage(null, null, ref memUse, null);
                        //Console.Out.WriteLine("simplex failure");
                        break;
                    }

            }
            TimeSpan end = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan usingtime = end.Subtract(start).Duration();
            //executeTime = usingtime.ToString();
            this.timeUse = usingtime.TotalMilliseconds;
            return true;
        }
  

        public void mipSolve() 
        {
            try
            {
                mipReturn = -1;
               /* double ae_max = -1, re_max=-1;
               int ae_ind = 0, re_ind = 0;
                glp_check_kkt(lp, 2, SolverGLPK.GLP_KKT_DE, ref ae_max, ref ae_ind, ref re_max, ref re_ind);*/
                glp_iocp iocpParm = new glp_iocp();                    
                glp_init_iocp(iocpParm);
                CallbackDelegate cd = new CallbackDelegate(callBack);    
                IntPtr ptr = Marshal.GetFunctionPointerForDelegate(cd);
                iocpParm.cb_func = ptr.ToPointer();
                if ((Program.MAXTIME_SIMPLEX - SimplextimeUse) <= 0)
                    iocpParm.tm_lim = 1;
                else iocpParm.tm_lim = (int)Program.MAXTIME_SIMPLEX - SimplextimeUse;
                  iocpParm.gmi_cuts = GLP_ON;
                  iocpParm.mir_cuts = GLP_ON;
                 // iocpParm.tol_int = 0.001;
                  //iocpParm.tol_obj = 0.0001;
                  //iocpParm.pp_tech = glp_iocp.GLP_PP_ALL;
                  iocpParm.presolve = GLP_OFF;
                //iocpParm.mip_gap = 0.01;
                
               /* 
                iocpParm.binarize = GLP_ON;
                iocpParm.presolve = GLP_ON;*/
                //glp_unscale_prob(lp);

                #region comment : mip return valeur
                    /*
                     * glp_intopt : solve MIP problem with the branch-and-cut method
                     * 
                     * 0 The MIP problem instance has been successfully solved. (This code does not necessarily mean that the solver has found optimal solution. 
                     * It only means that the solution process was successful.) 
                     * GLP_EBOUND Unable to start the search, because some double-bounded variables have incorrect bounds or some integer variables have non-integer (fractional) bounds.
                     * GLP_EROOT Unable to start the search, because optimal basis for initial LP relaxation is not provided. (This code may appear only if the presolver is disabled.)
                     * GLP_ENOPFS Unable to start the search, because LP relaxation of the MIP problem instance has no primal feasible solution. (This code may appear only if the presolver is enabled.)
                     * GLP_ENODFS Unable to start the search, because LP relaxation of the MIP problem instance has no dual feasible solution. In other word, this code means that if the LP relaxation has 
                     * at least one primal feasible solution, its optimal solution is unbounded, so if the MIP problem has at least one integer feasible solution, its (integer) optimal solution 
                     * is also unbounded. (This code may appear only if the presolver is enabled.)
                     * GLP_EFAIL The search was prematurely terminated due to the solver failure. 
                     * GLP_EMIPGAP The search was prematurely terminated, because the relative mip gap tolerance has been reached.
                     * GLP_ETMLIM The search was prematurely terminated, because the time limit has been exceeded. 
                     * GLP_ESTOP The search was prematurely terminated by application. (This code may appear only if the advanced solver interface is used.)
                     *
                     */

                    /**mipStatus : 
                     *GLP_UNDEF | MIP solution is undefiened;
                     *GLP_OPT | MIP solution is integer optimal;
                     *GLP_FEAS | MIP solution is integer feasible, however, its optimality (or non-optimality) has 
                     *not been proven, perhaps due to premature termination of the search; 
                     *GLP_NOFEAS | problem has no integer feasible solution (proven by the solver). 
                     * 
                     */
                    #endregion

                mipReturn = glp_intopt(lp, iocpParm);//return 0 if the MIP problem instance has been successfully solved
                int mipStatus = glp_mip_status(lp);
                glp_mem_usage(null, null, ref memUse, null);

                switch (mipReturn)
                {
                    case 0:
                        if (mipStatus == GLP_FEAS)
                        {
                            mipOK = true;
                            //Console.Out.WriteLine("mip : solution réalisable");
                        }
                        else if (mipStatus == GLP_OPT)
                        {
                            mipOK = true;
                            //Console.Out.WriteLine("mip : optimale");
                        }
                        else
                        {
                            mipOK = false;
                            //Console.Out.WriteLine("mip ne peut pas obtenir une solution en nombre entiers réalisable");
                        }
                        break;
                    case GLP_ETMLIM:

                       // Console.Out.WriteLine("mip : temps limit dépassée");
                        mipTimeOverflow = true;
                        if (mipStatus == GLP_FEAS)
                        {
                            mipOK = true;
                            //Console.Out.WriteLine("mip : solution réalisable trouvé");
                        }
                        else if (mipStatus == GLP_OPT)
                        {
                            mipOK = true;
                            //Console.Out.WriteLine("mip : optimale");
                        }
                        else
                        {
                            mipOK = false;
                           // Console.Out.WriteLine("mip : pas de solution réalisable");
                        }
                        break;

                     case GLP_ESTOP:
                        //Console.Out.WriteLine("mip : memory limit dépassée");
                        if (mipStatus == GLP_FEAS)
                        {
                            mipOK = true;
                            //Console.Out.WriteLine("mip : solution réalisable trouvé");
                        }
                        else if (mipStatus == GLP_OPT)
                        {
                            mipOK = true;
                            //Console.Out.WriteLine("mip : optimale");
                        }
                        else
                        {
                            mipOK = false;
                            //Console.Out.WriteLine("mip : pas de solution réalisable");
                        }
                        break;
                        default:
                        {
                            mipOK = false;
                            glp_mem_usage(null, null, ref memUse, null);
                            //Console.Out.WriteLine("mip failure");
                            break;
                        }
                    }
                }
         
            catch (Exception e)
            {
                
                throw e;
            }
            
        }

        /// <summary>
        ///  print the result to the richText of the mainWindow
        ///  And return le matchingResult
        /// </summary>
        /// <param name="MatchingNodes"></param>
        /// <param name="ResultText"></param>
        public bool getMatchingResult(MatchingResult matchingResult)  
        {
            matchingResult.TimeUse = this.timeUse;
            if (simplexOK) Console.Out.WriteLine("Simplex Ok");
            else Console.Out.WriteLine("Simplex failed");
            if (mipOK) Console.Out.WriteLine("Mip ok");
            else Console.Out.WriteLine("Mip failed");

            if (simplexTimeOverflow == true || mipTimeOverflow == true)
            {
                matchingResult.TimeOverFlow = true;
                Console.Out.WriteLine("time over");
            }

            if (mipMemoryOverflow == true)
            {
                matchingResult.MemoryOverFlow = true;
                Console.Out.WriteLine("memory outflow");
            }
                
            if (this.memUse != 0)
            {
                double MemUseMB = (double)this.memUse / 1048576.0;
                matchingResult.MemoryUse = MemUseMB;
                Console.Out.WriteLine("memory : " + MemUseMB + "Mb");
            }
            matchingResult.MaxTreeNodes = this.n_cnt_max;
            matchingResult.NbNodes = this.t_cnt;
            int status1 = glp_mip_status(lp);
            if (status1 == GLP_OPT || status1 == GLP_FEAS)
            {
                matchingResult.Feasible = true;
                if (status1 == GLP_OPT) matchingResult.Optimal = true;
            }
            if ((simplexOK == true) && (mipOK == true))
                return getMatchingResultMip(matchingResult);
            else
            {
                return false;//getMatchingResultSimplex(matchingResult);
            }
        }


        public bool getMatchingResultMip(MatchingResult matchingResult)
        {
            double distance;
            double ress;
            double minimum = -1;
            minimum = glp_mip_obj_val(lp);
            string valName;
            //double distance = -1.0;
            for (int i = 1; i <= (glp_get_num_cols(lp)); i++)
            {
                ress = glp_mip_col_val(lp, i);
                valName = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(glp_get_col_name(lp, i));

                if ((ress-1<0.0001)&&(ress-1>-0.0001))
                {
                    if (valName.StartsWith("x_"))
                    {
                        KeyValuePair<string, string> pair = seperateValName(valName);
                        distance = glp_get_obj_coef(lp, i);
                        matchingResult.NodeMatchingDictionary.Add(pair.Key, pair.Value);
                       // Console.Out.WriteLine(pair.Key + "->" + pair.Value + ": distance: " + distance.ToString() + "\n\n");
                         distance = -1.0;
                    }
                    else if (valName.StartsWith("y_"))
                    {
                        KeyValuePair<string, string> pair = seperateValName(valName);
                        distance = glp_get_obj_coef(lp, i);
                        //Console.Out.WriteLine(pair.Key + "->" + pair.Value + ": distance: " + distance.ToString() + "\n\n");
                        if (!pair.Key.StartsWith("copy_"))
                        {
                            if (pair.Value.StartsWith("copy_"))
                            {
                                matchingResult.EdgeMatchingDictionary.Add(pair.Key, pair.Value.Substring(5));
                            }
                            else
                            {
                                matchingResult.EdgeMatchingDictionary.Add(pair.Key, pair.Value);
                            }
                        }
                    }
                    else if (valName.StartsWith("u_"))
                    {
                        KeyValuePair<string, string> pair = seperateValName(valName); //new KeyValuePair<string, string>(valName.Substring(2), "NUL"); 
                        distance = glp_get_obj_coef(lp, i);
                        matchingResult.NodeMatchingDictionary.Add(pair.Key, pair.Value);
                       // Console.Out.WriteLine(pair.Key + "->" + pair.Value + ": distance: " + distance.ToString() + "\n\n");

                    }
                    else if (valName.StartsWith("e_"))
                    {
                        KeyValuePair<string, string> pair = seperateValName(valName); //new KeyValuePair<string, string>(valName.Substring(2), "NUL"); 
                        distance = glp_get_obj_coef(lp, i);
                        //Console.Out.WriteLine(pair.Key + "->" + pair.Value + ": distance: " + distance.ToString() + "\n\n");
                        if (!pair.Key.StartsWith("copy_"))
                            matchingResult.EdgeMatchingDictionary.Add(pair.Key, pair.Value);
                       
                    }

                    else if (valName.StartsWith("v_"))
                    {
                        KeyValuePair<string, string> pair = seperateValName(valName); //new KeyValuePair<string, string>(valName.Substring(2), "NUL"); 
                        distance = glp_get_obj_coef(lp, i);
                        //Console.Out.WriteLine(pair.Key+"->"+pair.Value+ ": distance: " + distance.ToString() + "\n\n");
                        matchingResult.NodeMatchingDictionary.Add(pair.Key, pair.Value);
                        //ResultText.AppendText("Nodes: " + this.g1.Name + "#" + res[0] + " is lonely..\n");

                    }
                    else if (valName.StartsWith("f_"))
                    {
                        KeyValuePair<string, string> pair = seperateValName(valName); //new KeyValuePair<string, string>(valName.Substring(2), "NUL"); 
                        distance = glp_get_obj_coef(lp, i);
                        //Console.Out.WriteLine(pair.Key + "->" + pair.Value + ": distance: " + distance.ToString() + "\n\n");
                        if (!pair.Key.StartsWith("Del_copy_"))
                            matchingResult.EdgeMatchingDictionary.Add(pair.Key, pair.Value);                        
                    }
                }

            }
            if (minimum < 0.000001 && minimum > -0.000001) minimum = 0;
            matchingResult.Distance = minimum;
            return true;
        }
        
        /// <summary>
        ///  small tool function. Analysing the name of a variable
        ///  for exemple: valName="x_node1,node3", the function return a table of 2 elements: "node1", "node3"
        /// </summary>
        /// <param name="valName"></param>
        private KeyValuePair<string, string>  seperateValName(string valName)
        {
            string[] result;     
            KeyValuePair<string, string> pair;
            valName = valName.Substring(2);
            result = valName.Split(',');
            if(Problem.exchange==false) pair = new KeyValuePair<string, string>(result[0], result[1]);
            else pair = new KeyValuePair<string, string>(result[1], result[0]);
            return pair;
        }
       
        /// <summary>
        ///  save the problem in CPLEX format into a text file.
        /// </summary>
        /// <param name="fname"></param>
        public int SaveProb2File(string fname)
        {
            int res=-1;
            if (lp == null) Console.WriteLine("lp = null");
            try
            {                
                res = glp_write_lp(this.lp, null, fname);

            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.ToString());
            }
            return res;
        }

        /// <summary>
        ///  save the solve result into a text file.
        /// </summary>
        /// <param name="fname"></param>
        public int solutionSave(string fname)
        {
            int res = -1;
            try
            {
                res = glp_print_mip(this.lp, fname);

            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.ToString());
            }
            return res;
        }

        public void setThreadNumber(int i)
        {
            return;
        }
        /// <summary>
        ///  close the GLPK
        /// </summary>
        public void closeSolver()
        {
            glp_free_env();
        }
    }

    #region structure dll
    [StructLayout(LayoutKind.Sequential)]
    public unsafe class glp_smcp
    {     /* simplex method control parameters */
        public int msg_lev;            /* message level: */
        public static readonly int GLP_MSG_OFF = 0;  /* no output */
        public static readonly int GLP_MSG_ERR = 1;  /* warning and error messages only */
        public static readonly int GLP_MSG_ON = 2;  /* normal output */
        public static readonly int GLP_MSG_ALL = 3;  /* full output */
        public static readonly int GLP_MSG_DBG = 4;   /* debug output */

        public int meth;               /* simplex method option: */
        public static readonly int GLP_PRIMAL = 1; /* use primal simplex */

        public static readonly int GLP_DUALP = 2;   /* use dual; if it fails, use primal */
        public static readonly int GLP_DUAL = 3;  /* use dual simplex */

        public int pricing;            /* pricing technique: */
        public static readonly int GLP_PT_STD = 0x11;  /* standard (Dantzig rule) */
        public static readonly int GLP_PT_PSE = 0x22;   /* projected steepest edge */

        public int r_test;             /* ratio test technique: */
        public static readonly int GLP_RT_STD = 0x11;   /* standard (textbook) */
        public static readonly int GLP_RT_HAR = 0x22;   /* two-pass Harris' ratio test */

        public double tol_bnd;         /* spx.tol_bnd */
        public double tol_dj;          /* spx.tol_dj */
        public double tol_piv;         /* spx.tol_piv */
        public double obj_ll;          /* spx.obj_ll */
        public double obj_ul;          /* spx.obj_ul */
        public int it_lim;             /* spx.it_lim */
        public int tm_lim;             /* spx.tm_lim (milliseconds) */
        public int out_frq;            /* spx.out_frq */
        public int out_dly;            /* spx.out_dly (milliseconds) */
        public int presolve;           /* enable/disable using LP presolver */
        public double[] foo_bar = new double[36];     /* (reserved) */

        public static readonly int GLP_OFF = 0;  /* disable something */
        public static readonly int GLP_ON = 1;  /* disable something */
    }


    [StructLayout(LayoutKind.Sequential)]
    public unsafe class glp_iocp
    {     /* integer optimizer control parameters */
        public int msg_lev;            /* message level (see glp_smcp) */
        public int br_tech;            /* branching technique: */
        public static readonly int GLP_BR_FFV = 1; /* first fractional variable */
        public static readonly int GLP_BR_LFV = 2; /* last fractional variable */
        public static readonly int GLP_BR_MFV = 3; /* most fractional variable */
        public static readonly int GLP_BR_DTH = 4; /* heuristic by Driebeck and Tomlin */
        public static readonly int GLP_BR_PCH = 5; /* hybrid pseudocost heuristic */
        public int bt_tech;            /* backtracking technique: */
        public static readonly int GLP_BT_DFS = 1; /* depth first search */
        public static readonly int GLP_BT_BFS = 2; /* breadth first search */
        public static readonly int GLP_BT_BLB = 3;/* best local bound */
        public static readonly int GLP_BT_BPH = 4;  /* best projection heuristic */
        public double tol_int;         /* mip.tol_int */
        public double tol_obj;         /* mip.tol_obj */
        public int tm_lim;             /* mip.tm_lim (milliseconds) */
        public int out_frq;            /* mip.out_frq (milliseconds) */
        public int out_dly;            /* mip.out_dly (milliseconds) */
        //void (*cb_func)(void * T, void *info);
        public void* cb_func;        
        //public SolverGLPK.DelegateCallBack cb_func =null;
        /* mip.cb_func */
        public void* cb_info;          /* mip.cb_info */
        public int cb_size;            /* mip.cb_size */
        public int pp_tech;            /* preprocessing technique: */
        public static readonly int GLP_PP_NONE = 0;  /* disable preprocessing */
        public static readonly int GLP_PP_ROOT = 1;  /* preprocessing only on root level */
        public static readonly int GLP_PP_ALL = 2;/* preprocessing on all levels */
        public double mip_gap;         /* relative MIP gap tolerance */
        public int mir_cuts;           /* MIR cuts       (GLP_ON/GLP_OFF) */
        public int gmi_cuts;           /* Gomory's cuts  (GLP_ON/GLP_OFF) */
        public int cov_cuts;           /* cover cuts     (GLP_ON/GLP_OFF) */
        public int clq_cuts;           /* clique cuts    (GLP_ON/GLP_OFF) */
        public int presolve;           /* enable/disable using MIP presolver */
        public int binarize;           /* try to binarize integer variables */
        public int fp_heur;            /* feasibility pump heuristic */
        public int alien;              /* use alien solver */
        public double[] foo_bar = new double[29];     /* (reserved) */
    }

   
        #endregion

}

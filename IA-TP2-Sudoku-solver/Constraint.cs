using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace IA_TP2_Sudoku_solver
{
    interface Constraint
    {
        public bool check(int[,] state);
        public int[,][] update(int v, int[,] state, int[,][] domain);
        public int constraints(int[,] state);
        public List<Tuple<int, int>> getNeighbour(int[,] state);
        public bool isConsistent(int v, int[,] state);
        public int[,][] remove(int val, int[,] state, int[,][] domain, int[] hist);
        public int[,][] totalUpdate(int v, int[,] state, int[,][] domain);
    }
}

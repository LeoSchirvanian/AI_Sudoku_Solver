using System;
using System.Collections.Generic;

namespace IA_TP2_Sudoku_solver
{
    // Interface of the constraints of a sudoku
    interface Constraint
    {
        public bool check(int[,] state);
        public int[,][] update(int v, int[,] state, int[,][] domain);
        public int constraints(int[,] state);
        public List<Tuple<int, int>> getNeighbour(int[,] state);
        public bool isConsistent(int v, int[,] state);
    }
}

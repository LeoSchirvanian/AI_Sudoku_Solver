using System;
using System.Collections.Generic;

namespace IA_TP2_Sudoku_solver
{
    // Represents the assignment for backtracking search algorithm
    class Assignment
    {
        // Attributes
        public List<Tuple<int, int, int>> assgn = new List<Tuple<int, int, int>>();

        // Constructor
        public Assignment(int[,] state)
        {
            // Fill the init assignment list, it's the initial state and we can't go back from there.
            for (int i = 0; i < state.GetLength(0); i++)
            {
                for (int j = 0; j < state.GetLength(1); j++)
                {
                    
                    if(state[i,j] != 0)
                    {
                        var t = Tuple.Create(i, j, state[i, j]);
                        assgn.Add(t);
                    }
                }
            }
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------- METHODS --------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Is the assignment complete ?
        public bool isComplete(int square_size)
        {
            // Check the size
            int compt = assgn.Count;
            if(compt == square_size * square_size)
            {
                // Check if each variable are assigned
                List<Tuple<int,int>> l = new List<Tuple<int, int>>();
                for (int i = 0; i < compt; i++)
                {
                    // Get the variables
                    var t = Tuple.Create(assgn[i].Item1, assgn[i].Item2);

                    // If this variable is already in the list l then return false
                    if(l.Contains(t))
                    {
                        return false;
                    }
                    //If not add it to l
                    else
                    {
                        l.Add(t);
                    } 
                }
                return true;
            }

            return false;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

    }
}

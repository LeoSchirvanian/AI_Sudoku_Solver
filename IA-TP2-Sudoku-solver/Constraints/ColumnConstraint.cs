using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace IA_TP2_Sudoku_solver.Constraints
{
    class ColumnConstraint : Constraint
    {
        // Attributes
        int[] target;  // Cell coordinate
        int column;    // Cell column number

        // Constructor
        public ColumnConstraint(int[] targetcoo, int column)
        {
            this.target = targetcoo;
            this.column = column;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------- METHODS --------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Check if there is the same value in column
        public bool check(int[,] state)
        {
            for (int i = 0; i < state.GetLength(0); i++)
            {
                if ( (state[i, column] == state[target[0], target[1]]) && ( i != target[0] || column != target[1] ) )
                {
                    return false;
                }
            }

            return true;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Update the column domain
        public int[,][] update(int v, int[,] state, int[,][] domain)
        {
            if (check(state))
            {
                // Remove value for the whole column
                for (int i = 0; i < domain.GetLength(1); i++)
                {
                    // Remove the value v from the domain of each row
                    int numToRemove = v;
                    if (domain[i, column].Contains(numToRemove))
                    {
                        domain[i, column] = domain[i, column].Where(val => val != numToRemove).ToArray();
                    }
                }

                // Update the assigned variable
                domain[target[0], target[1]] = new int[] { v };
            }

            return domain;

        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        public int constraints(int[,] state)
        {
            int constr = 0;
            for (int i = 0; i < state.GetLength(0); i++)
            {
                if (state[i, column] == 0)
                {
                    constr += 1;
                }
            }

            return constr;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Return all the list of column neighbours
        public List<Tuple<int, int>> getNeighbour(int[,] state)
        {
            List<Tuple<int, int>> l = new List<Tuple<int, int>>();

            for (int i = 0; i < state.GetLength(0); i++)
            {
                if ( i != target[0] || column != target[1] )
                {
                    l.Add(Tuple.Create(i, column));
                }
            }

            return l;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Check if value would be consistent in column
        public bool isConsistent(int v, int[,] state)
        {
            int[,] copyState = (int[,])state.Clone();
            copyState[target[0], target[1]] = v;

            return check(copyState);
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        public int[,][] remove(int val, int[,] state, int[,][] domain, int[] hist)
        {
            for (int i = 0; i < state.GetLength(0); i++)
            {
                domain[i, column].Append(val);
            }
            domain[target[0], target[1]] = hist;
            domain = totalUpdate(val, state, domain);
            return domain;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Update the row domain
        public int[,][] totalUpdate(int v, int[,] state, int[,][] domain)
        {
            for (int i = 0; i < state.GetLength(0); i++)
            {
                for (int j = 0; j < state.GetLength(1); j++)
                {
                    if (state[i, j] != 0)
                    {
                        domain = update(v, state, domain);
                    }
                }
            }

            return domain;

        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

    }
}

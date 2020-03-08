using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace IA_TP2_Sudoku_solver.Constraints
{
    class LineConstraint : Constraint
    {

        // Attributes
        int[] target; // Cell coordinate
        int row;      // Cell row number

        // Constructor
        public LineConstraint(int[] targetcoo, int row)
        {
            this.target = targetcoo;
            this.row = row;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------- METHODS --------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Check if there is the same value in row
        public bool check(int[,] state)
        {
            for(int i = 0; i < state.GetLength(0); i++)
            {
                if ( (state[row, i] == state[target[0],target[1]]) && ( row != target[0] || i != target[1] ) )
                {
                    return false;
                }
            }

            return true;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Update the row domain
        public int[,][] update(int v, int[,] state, int[,][] domain)
        {
            if (check(state))
            {
                // Remove value for the whole row
                for (int i = 0; i < domain.GetLength(1); i++)
                {
                    // Remove the value v from the domain of each row
                    int numToRemove = v;
                    if (domain[row, i].Contains(numToRemove))
                    {
                        domain[row, i] = domain[row, i].Where(val => val != numToRemove).ToArray();
                    }
                }

                // Update the assigned variable
                domain[target[0], target[1]] = new int[] { v };
            }

            return domain;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Return the number of constraints the cell have
        public int constraints(int[,] state)
        {
            int constr = 0;
            for (int i = 0; i < state.GetLength(1); i++)
            {
                if(state[row,i] == 0)
                {
                    constr += 1;
                }
            }

            return constr;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Return all the list of line neighbours
        public List<Tuple<int,int>> getNeighbour(int[,] state)
        {
            List<Tuple<int, int>> l = new List<Tuple<int, int>>();

            for (int i = 0; i < state.GetLength(0); i++)
            {
                if ( row != target[0] || i != target[1] )
                {
                    l.Add(Tuple.Create(row, i));
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
                domain[row, i].Append(val);
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
                    if(state[i,j] != 0)
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

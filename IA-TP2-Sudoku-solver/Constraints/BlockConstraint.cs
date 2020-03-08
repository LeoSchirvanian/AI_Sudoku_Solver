using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace IA_TP2_Sudoku_solver.Constraints
{
    class BlockConstraint : Constraint
    {
        // Attributes
        int[] target;     // Cell coordinate
        int[] blockstart; // First cell of the block coordinate
        int[] blockend;   // Last cell of the block coordinate

        // Constructor
        public BlockConstraint(int[] targetcoo, int[] blockstart, int length)
        {
            this.target = targetcoo;
            this.blockstart = blockstart;
            this.blockend = new[] { blockstart[0] + length - 1, blockstart[1] + length - 1 };
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------- METHODS --------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Check if there is the same value in block
        public bool check(int[,] state)
        {
            for (int i = blockstart[0]; i <= blockend[0]; i++)
            {
                for (int j = blockstart[1]; j <= blockend[1]; j++)
                {
                    if ( (state[i,j] == state[target[0],target[1]]) && ( i != target[0] || j != target[1]))
                    {
                        return false;
                    }
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
                // Remove value for the whole block
                for (int i = blockstart[0]; i <= blockend[0]; i++)
                {
                    for (int j = blockstart[1]; j <= blockend[1]; j++)
                    {
                        // Remove the value v from the domain of each row
                        int numToRemove = v;
                        if (domain[i, j].Contains(numToRemove))
                        {
                            domain[i, j] = domain[i, j].Where(val => val != numToRemove).ToArray();
                        }
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
            for (int i = blockstart[0]; i <= blockend[0]; i++)
            {
                for (int j = blockstart[1]; j <= blockend[1]; j++)
                {
                    if (state[i, j] == 0)
                    {
                        constr += 1;
                    }
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

            for (int i = blockstart[0]; i <= blockend[0]; i++)
            {
                for (int j = blockstart[1]; j <= blockend[1]; j++)
                {
                    if ( i != target[0] || j != target[1] )
                    {
                        l.Add(Tuple.Create(i, j));
                    }
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
            for (int i = blockstart[0]; i <= blockend[0]; i++)
            {
                for (int j = blockstart[1]; j <= blockend[1]; j++)
                {
                    domain[i, j].Append(val);
                }
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

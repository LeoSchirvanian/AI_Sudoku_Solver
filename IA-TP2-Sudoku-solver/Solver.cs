using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;


namespace IA_TP2_Sudoku_solver
{
    class Solver
    {
        // Attributes
        private Sudoku sudoku;
        private int[,] initialstate;

        // Constructor
        public Solver(int[,] sudoku_state)
        {
            sudoku = new Sudoku(sudoku_state);
            initialstate = (int[,])sudoku_state.Clone();
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------- METHODS --------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        public void solve()
        {
            Stopwatch sw = Stopwatch.StartNew();

            Console.WriteLine("Before :");
            printGridState(initialstate);

            if (backtracking_search(sudoku))
            {
                Console.WriteLine("\nWe solve the sudoku !");

                Console.WriteLine("\nAfter :");
                printGridState(sudoku.state);
            }
            else
            {
                Console.WriteLine("\nImpossible to solve !");
            }

            sw.Stop();
            Console.WriteLine("\nTime taken: {0}s", sw.Elapsed.TotalMilliseconds / 1000);
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Backtracking search
        public bool backtracking_search(Sudoku sudoku)
        {
            return (recursive_backtracking(sudoku)); ;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Recursive backtracking search
        public bool recursive_backtracking(Sudoku sudoku)
        {
            // If the assignment is complete
            if (sudoku.assgn.isComplete(sudoku.state.GetLength(0)) == true)
            {
                return true;
            }

            // Forward checking AC3
            AC3(sudoku);

            // Select unassigned variables
            // Choose variable with MRV and degree heuristic
            Tuple<int, int> var = minimumRemainingvalues(sudoku.state, sudoku.domain, sudoku.constraints);

            int size = sudoku.state.GetLength(0);
            int cellNumber = size * size;

            int[] values = leastConstrainingValue(var.Item1, var.Item2, sudoku);

            //foreach (int value in sudoku.domain[var.Item1, var.Item2])
            foreach (int value in values)
            {
                //int val = leastConstrainingValue(var.Item1, var.Item2, sudoku.state, sudoku.domain, sudoku.constraints);

                int line_index = var.Item2 + var.Item1 * size;
                int column_index = line_index + cellNumber;
                int block_index = column_index + cellNumber;


                if (sudoku.constraints[line_index].isConsistent(value, sudoku.state) & sudoku.constraints[column_index].isConsistent(value, sudoku.state) & sudoku.constraints[block_index].isConsistent(value, sudoku.state))
                {
                    sudoku.assgn.assgn.Add(Tuple.Create(var.Item1, var.Item2, value));


                    // Update
                    sudoku.state[var.Item1, var.Item2] = value;
                    sudoku.domain = sudoku.constraints[line_index].update(value, sudoku.state, sudoku.domain);
                    sudoku.domain = sudoku.constraints[column_index].update(value, sudoku.state, sudoku.domain);
                    sudoku.domain = sudoku.constraints[block_index].update(value, sudoku.state, sudoku.domain);

                    // Historic
                    int[] hist = sudoku.domain[var.Item1, var.Item2];

                    bool result = recursive_backtracking(sudoku);

                    if (result)
                    {
                        return result;
                    }

                    // Remove from assignment
                    Tuple<int, int, int> t = sudoku.assgn.assgn[sudoku.assgn.assgn.Count - 1];   
                    sudoku.state[t.Item1, t.Item2] = 0;
                    sudoku.assgn.assgn.RemoveAt(sudoku.assgn.assgn.Count - 1);

                    //sudoku.domain[t.Item1, t.Item2] = hist;
                    sudoku.domain = sudoku.createDomain();
                    sudoku.domain = sudoku.initDomain(sudoku.state, sudoku.domain);

                    
                    
                    /*
                    sudoku.domain = sudoku.constraints[line_index].remove(value, sudoku.state, sudoku.domain, hist);
                    sudoku.domain = sudoku.constraints[column_index].remove(value, sudoku.state, sudoku.domain, hist);
                    sudoku.domain = sudoku.constraints[block_index].remove(value, sudoku.state, sudoku.domain, hist);
                    */

                    // Remove the update

                }
            }

            return false;
            // Select the value which constraints the less the csp, avoid impossible csp
            

            // For each value allowed
            // Is consistent ?

            // If consistent add to assgn

            // Call recursive_backtracking with the new assignment
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Minimum remainingvalues (MRV)
        // Return the variable with the fewest values possible in the csp
        public Tuple<int,int> minimumRemainingvalues(int[,] state, int[,][] domain, List<Constraint> constraints)
        {
            List<Tuple<int, int>> l = new List<Tuple<int, int>>();
            int lowestValues = state.GetLength(0) + 1;
            int x = 0;
            int y = 0;

            for (int i = 0; i < state.GetLength(0); i++)
            {
                for (int j = 0; j < state.GetLength(1); j++)
                {
                    // Get unassigned variable
                    if (state[i,j] == 0)
                    {
                        int len = 0;
                        len = domain[i, j].Length;

                        if (len < lowestValues)
                        {
                            l.Clear();
                            lowestValues = len;
                            x = i;
                            y = j;
                            l.Add(Tuple.Create(i, j));
                        }
                        else
                        {
                            // If conflict, call degree heuristic
                            if(len == lowestValues)
                            {
                                l.Add(Tuple.Create(i, j));
                            }
                        }
                    }
                }
            }
            // If conflict, call degree heuristic
            if (l.Count > 1)
            {
                return degreeHeuristic(l, state, domain, constraints);
            }
            else
            {
                var t = Tuple.Create(x, y);
                return t;
            }
            
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Degree heuristic
        // Return the variable with the most impactful constraints on the csp if conflict on MRV
        public Tuple<int, int> degreeHeuristic(List<Tuple<int, int>> l, int[,] state, int[,][] domain, List<Constraint> constraints)
        {
            int maxConstraints = 0;
            int x = 0;
            int y = 0;

            int size = state.GetLength(0);
            int cellNumber = size * size;

            int line_index = 0;
            int column_index = 0;
            int block_index = 0;

            foreach (Tuple<int, int> coo in l)
            {
                int i = coo.Item1;
                int j = coo.Item2;
                
                // Get the number of unassigned variable which are affected by this cell
                int compt = 0;

                line_index = j + i * size;
                column_index = line_index + cellNumber;
                block_index = column_index + cellNumber;

                compt += constraints[line_index].constraints(state);
                compt += constraints[column_index].constraints(state);
                compt += constraints[block_index].constraints(state);

                if (compt > maxConstraints)
                {
                    maxConstraints = compt;
                    x = i;
                    y = j;
                }
            }
            
            var t = Tuple.Create(x, y);
            return t;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Least constraining value
        // Return the value which constraints the less the csp, avoid impossible csp
        public int[] leastConstrainingValue(int x, int y, Sudoku sudoku)
        {
            List<Tuple<int, int>> l = new List<Tuple<int, int>>();

            int size = sudoku.state.GetLength(0);
            int cellNumber = size * size;
            int line_index = y + x * size;
            int column_index = line_index + cellNumber;
            int block_index = column_index + cellNumber;

            foreach (int value in sudoku.domain[x, y])
            {
                int compt = 0;

                sudoku.state[x, y] = value;

                sudoku.domainCopy[x, y] = new int[] { value };
                sudoku.domainCopy = sudoku.constraints[line_index].update(value, sudoku.state, sudoku.domainCopy);
                sudoku.domainCopy = sudoku.constraints[column_index].update(value, sudoku.state, sudoku.domainCopy);
                sudoku.domainCopy = sudoku.constraints[block_index].update(value, sudoku.state, sudoku.domainCopy);

                compt = getNumberValues(sudoku.domainCopy);

                l.Add(Tuple.Create(compt, value));

                // Reset
                sudoku.state[x, y] = 0;
                sudoku.syncDomaintoCopy();
            }

            // Sort compt
            l.Sort(Comparer<Tuple<int, int>>.Default);

            // Get the value by desc compt
            int[] val = new int[l.Count];
            for (int i = l.Count-1; i >= 0; i--)
            {
                val[l.Count - 1 - i] = l[i].Item2;
            }

            return val;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Return the total number of allowed values for each variables (usefull for degree heuristic method)
        public int getNumberValues(int[,][] domain)
        {
            int compt = 0;
            for (int i = 0; i < domain.GetLength(0); i++)
            {
                for (int j = 0; j < domain.GetLength(1); j++)
                {
                    int count = 0;
                    count = domain[i, j].Length;
                    compt += count;
                }
            }

            return compt;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        public void AC3(Sudoku sudoku)
        {
            Queue<Tuple<Tuple<int, int>, Tuple<int, int>>> queue = new Queue<Tuple<Tuple<int, int>, Tuple<int, int>>>();
            // TODO : Need to generate the queue

            // Get all the node unassigned
            List<Tuple<int, int>> node = new List<Tuple<int, int>>();
            for (int i = 0; i < sudoku.state.GetLength(0); i++)
            {
                for (int j = 0; j < sudoku.state.GetLength(1); j++)
                {
                    // If unassigned variable
                    if(sudoku.state[i,j] == 0)
                    {
                        node.Add(Tuple.Create(i, j));
                    }
                }
            }

            Tuple<int, int> Xi;
            Tuple<int, int> Xj;
            // Get all the arc in the queue
            for (int i = 0; i < node.Count; i++)
            {
                Xi = node[i];
                for (int j = 0; j < node.Count; j++)
                {
                    if( j != i)
                    {
                        Xj = node[j];
                        queue.Enqueue(Tuple.Create(Xi, Xj));
                    }
                }
            }


            Tuple<Tuple<int, int>, Tuple<int, int>> top;
            while (queue.Count != 0)
            {
                top = queue.Dequeue();
                Xi = top.Item1;
                Xj = top.Item2;

                // If we modify the domain
                if ( removeInconsistentValues(sudoku, Xi, Xj))
                {
                    // Get all the neighbours of Xi
                    List<Tuple<int,int>> neigh = new List<Tuple<int, int>>();

                    int size = sudoku.state.GetLength(0);
                    int cellNumber = size * size;
                    int line_index = Xi.Item2 + Xi.Item1 * size;
                    int column_index = line_index + cellNumber;
                    int block_index = column_index + cellNumber;

                    List<Tuple<int, int>> lineNeighbour = sudoku.constraints[line_index].getNeighbour(sudoku.state);
                    List<Tuple<int, int>> columnNeighbour = sudoku.constraints[column_index].getNeighbour(sudoku.state);
                    List<Tuple<int, int>> blockNeighbour = sudoku.constraints[block_index].getNeighbour(sudoku.state);

                    neigh = lineNeighbour.Concat(columnNeighbour).ToList();
                    neigh = neigh.Concat(blockNeighbour).ToList();

                    foreach (Tuple<int, int> Xk in neigh)
                    {
                        queue.Enqueue(Tuple.Create(Xk, Xi)); // Add them to the queue
                    }
                }
            }
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        public bool removeInconsistentValues(Sudoku sudoku, Tuple<int,int> Xi, Tuple<int, int> Xj)
        {
            bool removed = false;

            foreach (int x in sudoku.domain[Xi.Item1, Xi.Item2])
            {
                foreach (int y in sudoku.domain[Xj.Item1, Xj.Item2])
                {
                    // If a value y in domain[Xi] allows (x,y) to satisfy the constraint Xi <=> Xj
                    if ( satisfy(Xi, x, Xj, y, sudoku) & satisfy(Xj, y, Xi, x, sudoku) )
                    {
                        return removed;
                    }
                }
                // Remove x from domain[Xi]
                sudoku.domain[Xi.Item1, Xi.Item2] = sudoku.domain[Xi.Item1, Xi.Item2].Where(val => val != x).ToArray();

            }
            return removed;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Check if the value x in variable Xi allows the value y in variable Xj given the constraints
        public bool satisfy(Tuple<int, int> Xi, int x, Tuple<int, int> Xj, int y, Sudoku sudoku)
        {
            bool b = false;

            int size = sudoku.domain.GetLength(0);
            int cellNumber = size * size;
            int line_index = Xi.Item2 + Xi.Item1 * size;
            int column_index = line_index + cellNumber;
            int block_index = column_index + cellNumber;

            sudoku.domainCopy = sudoku.constraints[line_index].update(x, sudoku.state, sudoku.domainCopy);
            sudoku.domainCopy = sudoku.constraints[column_index].update(x, sudoku.state, sudoku.domainCopy);
            sudoku.domainCopy = sudoku.constraints[block_index].update(x, sudoku.state, sudoku.domainCopy);

            if(sudoku.domainCopy[Xj.Item1, Xj.Item2].Contains(y))
            {
                b = true;
            }

            // Reset copy
            sudoku.syncDomaintoCopy();

            return b;

        }

        public int[,][] deepCopy(int[,][] array)
        {
            int[,][] copy = new int[array.GetLength(0), array.GetLength(1)][];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(0); j++)
                {
                    copy[i, j] = array[i, j];
                }
            }
            return copy;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Print the grid of the sudoku
        private void printGridState(int[,] state)
        {
            int size = (int) Math.Sqrt(state.GetLength(0));

            // Get grid matrix state
            string line;

            for (int i = 0; i < state.GetLength(0); i++)
            {
                if (i % size == 0)
                {
                    string horizontalborder = "*";
                    for (int j = 0; j < state.GetLength(0) + 2; j++)
                    {
                        horizontalborder += " - ";
                    }
                    horizontalborder += "*";
                    Console.WriteLine(horizontalborder);
                }
                line = "";
                for (int j = 0; j < state.GetLength(1); j++)
                {
                    if (j % size == 0)
                    {
                        line += " | " + state[i, j].ToString() + ' ';
                    }
                    else
                    {
                        line += ' ' + state[i, j].ToString() + ' ';
                    }

                }

                line += '|';

                Console.WriteLine(line);
            }

            string endline = "*";
            for (int j = 0; j < state.GetLength(0) + 2; j++)
            {
                endline += " - ";
            }
            endline += "*";
            Console.WriteLine(endline);
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

    }
}

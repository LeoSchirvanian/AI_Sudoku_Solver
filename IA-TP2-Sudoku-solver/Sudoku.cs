using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace IA_TP2_Sudoku_solver
{
    class Sudoku
    {

        // Attributes
        public int[,] state { get; }
        int[,] initState { get; }
        public int[,][] domain { get; set; }
        public int[,][] domainCopy { get; set; }
        public List<Constraint> constraints { get; }
        public Assignment assgn;

        public Sudoku(int[,] initial_state)
        {
            state = (int[,])initial_state.Clone();
            initState = (int[,])initial_state.Clone();

            constraints = generateSudokuConstraints();

            // Init domain 
            domain = createDomain();
            domain = initDomain(state, domain);

            domainCopy = createDomain();
            domainCopy = initDomain(state, domainCopy);

            assgn = new Assignment(initial_state);

        }



        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------- METHODS --------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Generate sudoku constraints
        private List<Constraint> generateSudokuConstraints()
        {
            List<Constraint> constraints = new List<Constraint>();
            constraints.AddRange(generateLineConstraints());
            constraints.AddRange(generateColumnConstraints());
            constraints.AddRange(generateBlockConstraints());

            return constraints;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Return domain
        public int[,][] getDomain()
        {
            return domain;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Update domain
        public void updateDomain()
        {

        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        public int[,][] getDomain(int[,] sudoku_state)
        {
            int[,][] ndom = new int[sudoku_state.GetLength(0), sudoku_state.GetLength(1)][];
            for (int i = 0; i < sudoku_state.GetLength(0); i++)
            {
                for (int j = 0; j < sudoku_state.GetLength(1); j++)
                {
                    int[,] scopy = (int[,])sudoku_state.Clone();
                    List<int> domrow = new List<int>();

                    for (int k = 0; k < sudoku_state.GetLength(0); k++)
                    {
                        scopy[i, j] = k;

                        if (isConsistent(scopy))
                        {
                            domrow.Add(k);
                        }
                    }

                    ndom[i, j] = domrow.ToArray();
                }
            }

            return ndom;

        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Check if each constraints are respected in the csp
        public bool isConsistent(int[,] sudoku_state)
        {
            foreach (Constraint constraint in constraints)
            {
                if (!constraint.check(state))
                {
                    return false;
                }
            }

            return true;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Generate a line constraints list for each cells of the sudoku
        private List<Constraints.LineConstraint> generateLineConstraints()
        {
            List<Constraints.LineConstraint> lineconstraints = new List<Constraints.LineConstraint>();
            for (int i=0; i < state.GetLength(0); i++)
            {
                for (int j=0; j < state.GetLength(1); j++)
                {
                    lineconstraints.Add(new Constraints.LineConstraint( new[] { i, j }, i ));
                }
            }
            return lineconstraints;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Generate a block constraints list for each cells of the sudoku
        private List<Constraints.BlockConstraint> generateBlockConstraints()
        {
            List<Constraints.BlockConstraint> blockconstraints = new List<Constraints.BlockConstraint>();

            int block_size = (int) Math.Sqrt(state.GetLength(0));

            for (int i=0; i < state.GetLength(0); i++)
            {
                int blockx = (i / block_size) * block_size;
                for (int j = 0; j < state.GetLength(1); j++)
                {
                    int blocky = (j / block_size) * block_size;
                    blockconstraints.Add(new Constraints.BlockConstraint(new[] { i, j }, new[] { blockx, blocky }, block_size));
                }
            }

            return blockconstraints;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Generate a column constraints list for each cells of the sudoku
        private List<Constraints.ColumnConstraint> generateColumnConstraints()
        {
            List<Constraints.ColumnConstraint> columnconstraints = new List<Constraints.ColumnConstraint>();
            for (int i = 0; i < state.GetLength(0); i++)
            {
                for (int j = 0; j < state.GetLength(1); j++)
                {
                    columnconstraints.Add(new Constraints.ColumnConstraint(new[] { i, j }, j));
                }
            }
            return columnconstraints;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Check domain
        private bool check(int x, int y, int v)
        {
            // Check if v is an allowed value for the variable cell x,y
            return domain[x, y].Contains(v);
        }

        /*
        private bool update(int x, int y, int v)
        {
            if( check(x, y, v) )
            {
                for (int i = 0; i < state.GetLength(0); i++)
                {

                }
            }
        }
        */

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Create the domain of the sudoku
        public int[,][] createDomain()
        {
            int[,][] domain = new int[state.GetLength(0), state.GetLength(1)][] ;//new int[state.GetLength(0), state.GetLength(1), state.GetLength(0)];
            for (int i = 0; i < state.GetLength(0); i++)
            {
                for (int j = 0; j < state.GetLength(1); j++)
                {
                    domain[i, j] = new int[state.GetLength(0)];
                    int[] values = new int[state.GetLength(0)];

                    for (int k = 0; k < state.GetLength(0); k++)
                    {
                        values[k] = k + 1;
                    }

                    domain[i, j] = values;
                }
            }

            return domain;
        }

        
        // Init the domain given a generated state
        public int[,][] initDomain(int[,] state, int[,][] domain)
        {
            for (int i = 0; i < state.GetLength(0); i++)
            {
                for (int j = 0; j < state.GetLength(1); j++)
                {
                    if(state[i,j] != 0)
                    {
                        domain[i,j] = new int [] { state[i, j] };

                        int size = state.GetLength(0);
                        int cellNumber = size * size;

                        int line_index = j + i*size;
                        int column_index = line_index + cellNumber;
                        int block_index = column_index + cellNumber;

                        domain = constraints[line_index].update(state[i, j], state, domain);
                        domain = constraints[column_index].update(state[i, j], state, domain);
                        domain = constraints[block_index].update(state[i, j], state, domain);

                    }
                }
            }

            return domain;
        }

        // Synchronize state with sudoku generator state
        public void syncDomaintoCopy()
        {
            for (int i = 0; i < domain.GetLength(0); i++)
            {
                for (int j = 0; j < domain.GetLength(1); j++)
                {
                    domainCopy[i, j] = domain[i, j];
                }
            }
        }


        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        /*
        public void printGridState(int size)
        {

            // Get grid matrix state
            string line;

            for (int i = 0; i < state.GetLength(0); i++)
            {
                if (i % size == 0)
                {
                    Console.WriteLine("*---------------------------------*");
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

            Console.WriteLine("*---------------------------------*");
        }

        /*
        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Synchronize state with sudoku generator state
        private void syncStateSG()
        {
            for (int i = 0; i < state.GetLength(0); i++)
            {
                for (int j = 0; j < state.GetLength(1); j++)
                {
                    state[i, j] = sg.state[i, j];
                }
            }
        }

        // Synchronize initState with sudoku generator state
        private void syncInitStateSG()
        {
            for (int i = 0; i < state.GetLength(0); i++)
            {
                for (int j = 0; j < state.GetLength(1); j++)
                {
                    initState[i, j] = state[i, j];
                }
            }
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Synchronize domain with sudoku generator state
        private void syncDomainSG()
        {
            for (int i = 0; i < state.GetLength(0); i++)
            {
                for (int j = 0; j < state.GetLength(0); j++)
                {

                    // If the cell has a value, then the domain is the value
                    if(state[i,j] != 0)
                    {
                        int[] values = new int[1];
                        values[0] = state[i, j];
                        domain[i, j] = values;
                    }

                    // If not, get the possibilities in a new list
                    else
                    {
                        // Get size of domain
                        int size = 0;
                        for (int k = 0; k < domain.GetLength(2); k++)
                        {
                            if(domain[i, j, k] == 1)
                            {
                                size++;
                            }
                        }

                        int[] values = new int[size];

                        // Fill domain
                        int compt = 0;
                        for (int k = 0; k < domain.GetLength(2); k++)
                        {
                            if (domain[i, j, k] == 1)
                            {
                                values[compt] = k + 1;
                                compt++;
                            }
                        }

                        domain[i, j] = values;


                    }
                }
            }
        }
        */

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

    }
}

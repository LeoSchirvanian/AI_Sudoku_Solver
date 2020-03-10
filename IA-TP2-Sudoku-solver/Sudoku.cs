using System;
using System.Collections.Generic;

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

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

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

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

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

    }
}

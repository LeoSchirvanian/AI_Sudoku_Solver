using System;
using System.Collections.Generic;
using System.Text;

namespace IA_TP2_Sudoku_solver
{
    class SudokuGen
    {
        // Attributes
        private SudokuGeneration.Grid grid { get; }

        // Constructor
        public SudokuGen(int size)
        {
            grid = new SudokuGeneration.Grid(size * size);
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------- METHODS --------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Generate random sudoku
        public int[,] generateSudoku(int size)
        {
            int res = grid.generateSudoku(size); ;
            
            // If we failed, we try till we achieve
            while (res == 0)
            {
                res = grid.generateSudoku(size);
            }

            return grid.state;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Import sudoku
        public int[,] importSudoku(int size)
        {
            grid.import(size);

            return grid.state;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //
    }
}

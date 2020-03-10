using System;

namespace IA_TP2_Sudoku_solver
{
    // Generate a random Sudoku
    class SudokuGen
    {
        // Attributes
        private SudokuGeneration.Grid grid { get; }
        private Random random = new Random();

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

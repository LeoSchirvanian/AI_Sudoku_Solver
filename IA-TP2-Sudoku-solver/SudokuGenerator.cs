using System;
using System.Collections.Generic;
using System.Text;


namespace IA_TP2_Sudoku_solver
{
    // This class allows to generate sudoku (random or imported) of size n
    class SudokuGenerator
    {

        // Attributes
        public int[,] state { get; }       // Current sudoku grid
        public int[,] initialstate { get; }
        public int[,,] domain { get; }     // Variables domain values
        private int[,,] copydomain { get; } // Copy of domain 
        
        private Random random = new Random();


        // Constructor
        public SudokuGenerator(int size)
        {
            int square_size = size * size;

            state = new int[square_size, square_size];

            domain = new int[square_size, square_size, square_size];
            copydomain = new int[square_size, square_size, square_size];

            for (int i = 0; i < square_size; i++)
            {
                for (int j = 0; j < square_size; j++)
                {
                    for (int k = 0; k < square_size; k++)
                    {
                        // All values allowed
                        domain[i, j, k] = 1;
                        copydomain[i, j, k] = 1;
                    }
                }
            }
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------- METHODS --------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Generate a sudoku
        public int[,] generateSudoku(int size)
        {
            // Try to generate a random sudoku, if succed returns 1, if not returns 0
            int res = trySudoku(size); ;
            
            // If we failed, we try till we achieve
            while (res == 0)
            {
                // Generate randomly a sudoku with a given block size (>= 2)
                res = trySudoku(size);
            }

            // Return the sudoku generated
            return state;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Try to generate a totally random sudoku
        private int trySudoku(int size)
        {
            int square_size = size * size;

            

            for (int i = 1; i <= square_size ; i++)
            {
                Tuple<int, int>[] block = getBlockXY(i, size); // Get coordinates of all cells of i-th block
                int compt = 0;
                int cell = random.Next(0, square_size);
                int value = -1;

                // Count the number of fails, if too big : stop the program and reload the function
                int failed = 0;
                // Maximum errors allowed
                int err = 10;

                // While we don't have n clues in this block
                while (compt < size)
                {
                    // Check if the random cell is already filled with a number
                    while (state[block[cell].Item1, block[cell].Item2] != 0)
                    {
                        cell = random.Next(0, square_size);  // Random cell
                    }
                    value = random.Next(1, square_size+1); // Random value

                    bool b = check(block[cell].Item1, block[cell].Item2, value, size); // Check if the value can be add in cell

                    // If we can add it
                    if (b == true)
                    {
                        // Update the value
                        int u = update(block[cell].Item1, block[cell].Item2, value, size, 2);

                        // If the update doesn't lead to an impossible sudoku we apply the change and increment compt
                        if (u == 1)
                        {
                            compt++;
                        }
                        // If we have failed to find a new cell to fill
                        else
                        {
                            // If too many fails
                            if (failed > err)
                            {
                                clearGridState();             // Don't forget to clear grid
                                clearGridDomain();            // Clear grid domain
                                return 0;                     // Stop the function
                            }
                            else
                            {
                                failed++; // Add +1 to failed
                            }

                        }

                    }
                    // If we have failed to find a new cell to fill
                    else
                    {
                        // If too many fails
                        if (failed > err)
                        {
                            clearGridState();             // Don't forget to clear grid
                            clearGridDomain();            // Clear grid domain
                            return 0;                     // Stop the function
                        }
                        else
                        {
                            failed++; // Add +1 to failed
                        }
                    }

                }
            }

            // We achieve to generate a correct sudoku
            return 1;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Clear grid domain and copydomain
        private void clearGridDomain()
        {
            for (int i = 0; i < domain.GetLength(0); i++)
            {
                for (int j = 0; j < domain.GetLength(1); j++)
                {
                    for (int k = 0; k < domain.GetLength(2); k++)
                    {
                        domain[i, j, k] = 1;
                        copydomain[i, j, k] = 1;
                    }

                }
            }
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Returns the number of value possible for a given empty cell
        private int getPossibilites(int x, int y)
        {
            int compt = 0;
            for (int i = 0; i < copydomain.GetLength(2); i++)
            {
                if (copydomain[x, y, i] == 1)
                {
                    compt++;
                }
            }
            return compt;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Synchronize copydomain to domain
        private void syncCopytoDomain()
        {
            for (int i = 0; i < domain.GetLength(0); i++)
            {
                for (int j = 0; j < domain.GetLength(1); j++)
                {
                    for (int k = 0; k < domain.GetLength(2); k++)
                    {
                        domain[i, j, k] = copydomain[i, j, k];
                    }
                }
            }
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Synchronize domain to copydomain
        private void syncDomaintoCopy()
        {
            for (int i = 0; i < domain.GetLength(0); i++)
            {
                for (int j = 0; j < domain.GetLength(1); j++)
                {
                    for (int k = 0; k < domain.GetLength(2); k++)
                    {
                        copydomain[i, j, k] = domain[i, j, k];
                    }
                }
            }
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Return the block number given a coordinate
        private int getBlocNumber(int x, int y, int square_size)
        {
            // Check if x and y have an acceptable value
            if (x < (square_size * square_size) & y < (square_size * square_size))
            {
                int BlocNumber = 1 + (x / square_size) * square_size + (y / square_size);
                return BlocNumber; // Return the block number
            }
            return 0;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Return a list of tuple which correspond to a given block
        private Tuple<int, int>[] getBlockXY(int number, int square_size)
        {
            Tuple<int, int>[] block = new Tuple<int, int>[square_size * square_size]; // Init array of tuple of coordinate cell in same block
            int n = 0;
            int compt = 0;

            // Columns then rows
            for (int i = 0; i < square_size * square_size; i++)
            {
                for (int j = 0; j < square_size * square_size; j++)
                {
                    n = getBlocNumber(i, j, square_size);
                    if (n == number)
                    {
                        block[compt] = Tuple.Create(i, j);
                        compt++;
                    }
                }
            }
            return block;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Check if a value v can be add in x,y
        private bool check(int x, int y, int v, int square_size)
        {

            // For import mode
            if (v == 0){
                return true;
            }

            else
            {
                // If the value is in the domain of the cell x,y
                if (domain[x, y, v - 1] == 0)
                {
                    return false;
                }
            }
            
            // If not, we return true, the value can be add
            return true;
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Update the domain matrix if the check method is true, return 0 if update an impossible sudoku
        private int update(int x, int y, int v, int size, int poss)
        {
            if (v == 0)
            {
                return 1;
            }
            else {
                int square_size = size * size;

                // Get the block number of the cell x,y
                int blockNumber = getBlocNumber(x, y, size);

                // Get all the coordinate of cells with the same block number than the cell x,y
                Tuple<int, int>[] block = getBlockXY(blockNumber, size);

                // Minimum number of possibilities allowed in a sudoku
                //int poss = 2;

                // We update the copydomain of cells on the same rows, columns and block
                for (int i = 0; i < square_size; i++){

                    // UPDATE ROW
                    if (copydomain[x, i, v - 1] != 2) // If the cell is not already filled
                    {
                        copydomain[x, i, v - 1] = 0;  // Update domain

                        // If this new update lead to less than 2 possibilites for the cell x,i return 0 and don't update (row)
                        if (getPossibilites(x, i) < poss)
                        {
                            syncDomaintoCopy();       //Reset the copydomain with domain
                            return 0;                            //Stop and return 0
                        }
                    }

                    // UPDATE COLUMN
                    if (copydomain[i, y, v - 1] != 2) // If the cell is not already filled
                    {
                        copydomain[i, y, v - 1] = 0;  // Update domain

                        // If this new update lead to less than 2 possibilites for the cell i,y return 0 and don't update (column)
                        if (getPossibilites(i, y) < poss)
                        {
                            syncDomaintoCopy();       // Reset the copydomain with domain
                            return 0;                            // Stop and return 0
                        }
                    }

                    // UPDATE BLOCKS
                    if (copydomain[block[i].Item1, block[i].Item2, v - 1] != 2) // If the cell is not already filled
                    {
                        copydomain[block[i].Item1, block[i].Item2, v - 1] = 0;  // Update domain

                        // If this new update lead to less than 2 possibilites for the cell block[i].Item1, block[i].Item2 return 0 and don't update (block)
                        if (getPossibilites(block[i].Item1, block[i].Item2) < poss)
                        {
                            syncDomaintoCopy();                                 // Reset the copydomain with domain
                            return 0;                                                      // Stop and return 0
                        }
                    }
                }

                // Updates are finished and don't lead to an impossible sudoku, we can update to domain

                // We fill with 2, which means this cell has been filled
                for (int i = 0; i < square_size; i++)
                {
                    copydomain[x, y, i] = 2;
                }
                syncCopytoDomain(); // Sync the changes

                // Update value of cell x,y
                state[x, y] = v;

                return 1;
            }
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Allow to the user to import his own sudoku in the solver
        public void import(int size)
        {
            String res = "";
            for (int i = 0; i < state.GetLength(0); i++)
            {
                for (int j = 0; j < state.GetLength(1); j++)
                {

                    printGridState(size);
                    Console.WriteLine("What is the value of the cell " + i.ToString() + ", " + j.ToString() + " ? ( write 0 for empty cells)");
                    res = Console.ReadLine();
                    int number;

                    bool b = true;

                    // Check if the user entered an integer superior than 2
                    while (b)
                    {
                        // If the user write an integer
                        if (Int32.TryParse(res, out number))
                        {
                            // If this integer is <0 or >size+1
                            if (number >= 0 & number <= state.GetLength(0))
                            {
                                //b = false;
                                //Console.Clear();
                                //state[i, j] = number;

                                // Check if good
                                bool c = check(i, j, number, size); // Check if the value can be add in cell

                                // If we can add it
                                if (c == true)
                                {
                                    // Update the value
                                    int u = update(i, j, number, size, 0);
                                    if (u == 1)
                                    {
                                        b = false;
                                        Console.Clear();
                                        state[i, j] = number;
                                    }
                                    else
                                    {
                                        Console.Clear();
                                        printGridState(size);
                                        Console.WriteLine("\nWarning ! Your value doesn't respect the constraints :");
                                        Console.WriteLine("Each row, column, and block can contain each number exactly once !\n");
                                        Console.WriteLine("What is the value of the cell " + i.ToString() + ", " + j.ToString() + " ? ( write 0 for empty cells)");
                                        res = Console.ReadLine();
                                    }
                                }
                                else
                                {
                                    Console.Clear();
                                    printGridState(size);
                                    Console.WriteLine("\nWarning ! Your value doesn't respect the constraints");
                                    Console.WriteLine("Each row, column, and block can contain each number exactly once !\n");
                                    Console.WriteLine("What is the value of the cell " + i.ToString() + ", " + j.ToString() + " ? ( write 0 for empty cells)");
                                    res = Console.ReadLine();
                                }
                            }
                            // If the integer is <0 or >size+1, then do a new request
                            else
                            {
                                Console.Clear();
                                printGridState(size);
                                Console.WriteLine("\n Incorrect value! \n");
                                Console.WriteLine("You must write down an integer between 0 and " + state.GetLength(0).ToString());
                                Console.WriteLine("What is the value of the cell " + i.ToString() + ", " + j.ToString() + " ? ( write 0 for empty cells)");
                                res = Console.ReadLine();
                            }
                        }
                        // If the user did not write down an integer, then do a new request
                        else
                        {
                            Console.Clear();
                            printGridState(size);
                            Console.WriteLine("\nYou must enter an integer ! \n");
                            Console.WriteLine("What is the value of the cell " + i.ToString() + ", " + j.ToString() + " ? ( write 0 for empty cells)");
                            res = Console.ReadLine();
                        }
                    }

                }
            }
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Clear grid state and initstate
        private void clearGridState()
        {
            for (int i = 0; i < state.GetLength(0); i++)
            {
                for (int j = 0; j < state.GetLength(1); j++)
                {
                    state[i, j] = 0;
                }
            }
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // This method allows to print the third dimension of the domain attributes
        private void printGridDomain()
        {
            for (int i = 0; i < domain.GetLength(0); i++)
            {
                for (int j = 0; j < domain.GetLength(0); j++)
                {
                    String line = "";
                    for (int k = 0; k < domain.GetLength(0); k++)
                    {
                        line += domain[i, j, k].ToString() + " ";
                    }
                    Console.WriteLine(line + "\n");
                }
            }
        }

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //

        // Print the grid of the sudoku
        private void printGridState(int size)
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

        // --------------------------------------------------------------------------------------------------------------- //
        // --------------------------------------------------------------------------------------------------------------- //
    }
}

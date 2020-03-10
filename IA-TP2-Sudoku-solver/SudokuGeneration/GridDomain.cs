
namespace IA_TP2_Sudoku_solver.SudokuGeneration
{
    // This class represents the domain of possibilites of a grid (domain[i,j,k] : if = 0 impossible value k for cell i,j , if = 1 possible value k for cell i,j)
    class GridDomain
    {
        // Attributes
        public int[,,] domain { get; set; }
        public int[,,] copydomain { get; set; }

        // Constructor
        public GridDomain(int size)
        {
            // We create a matrix size*size with a third dimension of size*size elements (with nxn sudoku, there are n^2 possibilities
            domain = new int[size, size, size];
            copydomain = new int[size, size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    for (int k = 0; k < size; k++)
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

        // Returns the number of value possible for a given empty cell
        public int getPossibilites(int x, int y)
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
        public void syncCopytoDomain()
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
        public void syncDomaintoCopy()
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

        // Clear grid domain and copydomain
        public void clearGridDomain()
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

    }

}

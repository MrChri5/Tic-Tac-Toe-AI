using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToeText
{
    class BoardLayout
    {

        //define board
        // layout  for back end analysis
        // display layout for text based input and output
        // both single and double indexing of grid
        // get each column/row/diag and index all in collection
        // get row/col/diag status (own set-up, opp set-up, contested, own single, opp single, empty) 

        private static int size;
        private static int maxSize = 9;
        private string[,] layout = new string[maxSize, maxSize];
        private string [] displayLayout = new string[maxSize * maxSize];
        private List<string[]> currentLines = new List<string[]>();
        private StatusType[] currentStatus;
        private string ownPiece = "X";
        private string oppPiece = "O";
        private string errorPiece = "e";

        public BoardLayout()
        {
            //default grid size is 3x3
            BoardInit(3);
        }
        public BoardLayout(int x)
        {
            if (x > maxSize)
            {
                x = maxSize;
            }
            BoardInit(x);
        }
        public void BoardInit(int x)
        {
            //initialize board to numbered cells left to right, top to bottom
            size = x;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int k = 1 + j + size * i;
                    layout[i, j] = k.ToString();
                    displayLayout[k - 1] = k.ToString();
                }
            }
            currentLines = GetLines(layout);
            currentStatus = GetStatus(currentLines);
        }

        public int Size
        {
            get { return size; }
        }

        public void SetOwn(int x)
        {
            //set own piece X
            int i = Math.DivRem(x-1, size, out int j);
            SetOwn(i , j);
        }
        public void SetOwn(int i, int j)
        {
            int k = j + size * i;
            if (!validBlank(layout[i, j]))
            {
                layout[i, j] = errorPiece;
                displayLayout[k] = errorPiece;
                return;
            }
            layout[i, j] = ownPiece;
            displayLayout[k] = ownPiece;
            currentLines = GetLines(layout);
            currentStatus = GetStatus(currentLines);
        }

        public void SetOpp(int x)
        {
            //set opponant piece O
            int i = Math.DivRem(x-1, size, out int j);
            SetOpp(i , j);
        }
        public void SetOpp(int i, int j)
        {
            int k = j + size * i;
            if (!validBlank(layout[i, j]))
            {
                layout[i, j] = errorPiece;
                displayLayout[k] = errorPiece;
                return;
            }
            layout[i, j] = oppPiece;
            displayLayout[k] = oppPiece;
            currentLines = GetLines(layout);
            currentStatus = GetStatus(currentLines);
        }

        public string[,] Layout
        {
            get { return layout; }
        }

        public string[] DisplayLayout
        {
            get { return displayLayout; }
        }

        public List<string[]> Lines
        {
            get { return currentLines; }
        }

        public StatusType[] Status
        {
            get { return currentStatus; }
        }

        public bool validBlank(int k)
            //check if the kth position on the board is available
            //if it has already been played then its entry in the display layout will be an 'X' or 'O'
        {
            if (k > 0 && k <= size * size)
            {
                return validBlank(displayLayout[k - 1]);
            }
            return false;
        }

        public bool validBlank(string x)
        {
            //check space is not taken, available spaces will contain numbers. spaces are filled with 'X' or 'O' when taken
            if (!int.TryParse(x,out int i))
            {
                return false;
            }
            if(i>0 && i <= size * size)
            {
                return true;
            }
            return false;    
        }

        private List<string[]> GetLines(string[,] getLinesLayout)
        {
            List<string[]> LinesList = new List<string[]>();
            //take layout and produce a list of lines
            string[] line = new string[size];
            LinesList.Clear();
            //rows
            for (int i = 0; i < size; i++)
            {
                line = new string[size];
                for (int j = 0; j < size; j++)
                {
                    line[j] = getLinesLayout[i, j];
                }
                LinesList.Add(line);
            }
            //columns
            for (int i = 0; i < size; i++)
            {
                line = new string[size];
                for (int j = 0; j < size; j++)
                {
                    line[j] = getLinesLayout[j, i];
                }
                LinesList.Add(line);
            }
            //diagonals
            line = new string[size];
            for (int i = 0; i < size; i++)
            {
                line[i] = getLinesLayout[i, i];
            }
            LinesList.Add(line);
            line = new string[size];
            for (int i = 0; i < size; i++)
            {
                line[i] = getLinesLayout[size - i - 1, i];
            }
            LinesList.Add(line);
            return LinesList;
        }
        
        public enum StatusType { own, opp, ownSetup, oppSetup, ownSetupFork, oppSetupFork, contested, ownOther, oppOther, empty, other, error};

        private StatusType[] GetStatus (string[,] statusLayout)
        {
            return GetStatus(GetLines(statusLayout));
        }
        private StatusType[] GetStatus (List<string[]> checkLines)
        {
            //analyse lines 
            StatusType[] status = new StatusType[checkLines.Count];
            int lineNum = 0;
              
            foreach(string [] line in checkLines)
            {
                int own = 0;
                int opp = 0;
                bool errorFlag = false;
                foreach (string piece in line)
                {
                    if (piece == errorPiece)
                    {
                    errorFlag = true;
                    }
                    if (piece == ownPiece)
                    {
                        ++own;
                    }
                    if (piece == oppPiece)
                    {
                        ++opp;
                    }
                }
                if (errorFlag || own + opp > size )
                {
                    status[lineNum] = StatusType.error;
                }
                else if (own == size)
                {
                    status[lineNum] = StatusType.own;
                }
                else if ( opp == size)
                {
                    status[lineNum] = StatusType.opp;
                }
                else if (own == 0 && opp == 0)
                {
                    status[lineNum] = StatusType.empty;
                }
                else if (own == 0 && opp == size -1 )
                {
                    status[lineNum] = StatusType.oppSetup;
                }
                else if (own == 0 && opp == size - 2)
                {
                    status[lineNum] = StatusType.oppSetupFork;
                }
                else if (own == 0 && opp > 0)
                {
                    status[lineNum] = StatusType.oppOther;
                }
                else if (opp == 0 && own == size - 1)
                {
                    status[lineNum] = StatusType.ownSetup;
                }
                else if (opp == 0 && own == size - 2)
                {
                    status[lineNum] = StatusType.ownSetupFork;
                }
                else if (opp == 0 && own > 0)
                {
                    status[lineNum] = StatusType.ownOther;
                }
                else if (opp > 0 && own > 0)
                {
                    status[lineNum] = StatusType.contested;
                }
                else
                {
                    status[lineNum] = StatusType.other;
                }
                    ++lineNum;
            }
            return status;
        }                
    }
}

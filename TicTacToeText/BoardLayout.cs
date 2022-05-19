using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToeText
{
    class BoardLayout
    {

        //BoardLayout class stores data about the pieces on the board        
        //there are also methods to return advanced data about the board used by the AI to determine its next move

        private static int size;
        //can support game boards up to maxSize x maxSize
        private static int maxSize = 9;
        //double indexed board
        private string[,] layout = new string[maxSize, maxSize];

        //variables for AI analysis of board
        private List<string[]> currentLines = new List<string[]>();
        private StatusType[] currentStatus;

        //defined player pieces
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
            //set size to max if size exceeds the maximum
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
                    int n = 1 + j + size * i;
                    layout[i, j] = n.ToString();
                }
            }
            //calculate the current lines and statuses
            currentLines = GetLines(layout);
            currentStatus = GetStatus(currentLines);
        }

        //return the size of the board 
        public int Size
        {
            get { return size; }
        }

        // method to set the player piece on the board (single index)
        public void SetOwn(int n)
        {            
            int i = Math.DivRem(n-1, size, out int j);
            SetOwn(i , j);
        }
        //method to set the player piece on the board (double index) 
        public void SetOwn(int i, int j)
        {
            //check if the space is available
            if (!validBlank(layout[i, j]))
            {
                layout[i, j] = errorPiece;
                return;
            }
            //set the piece
            layout[i, j] = ownPiece;
            //recalculate the current lines and statuses
            currentLines = GetLines(layout);
            currentStatus = GetStatus(currentLines);
        }

        //method to set the AI piece on th2 board (single index)
        public void SetOpp(int n)
        {            
            int i = Math.DivRem(n-1, size, out int j);
            SetOpp(i , j);
        }
        //method to set the AI piece on the board (double index)
        public void SetOpp(int i, int j)
        {
            //check if the space is available
            if (!validBlank(layout[i, j]))
            {
                layout[i, j] = errorPiece;
                return;
            }
            //set the piece
            layout[i, j] = oppPiece;
            //recalculate the current lines and statuses
            currentLines = GetLines(layout);
            currentStatus = GetStatus(currentLines);
        }

        //return the board layout
        public string[,] Layout
        {
            get { return layout; }
        }
        //return the list of lines
        public List<string[]> Lines
        {
            get { return currentLines; }
        }
        //return the line statuses
        public StatusType[] Status
        {
            get { return currentStatus; }
        }

        //check if the nth position on the board is available
        //if it has already been played then its entry in the display layout will be an 'X' or 'O'
        public bool validBlank(int n)            
        {            
            if (n > 0 && n <= size * size)
            {
                //calculate the double indexes
                int i = Math.DivRem(n - 1, size, out int j);
                return validBlank(layout[i,j]);
            }
            return false;
        }

        //check space is not taken, available spaces will contain numbers. spaces are filled with 'X' or 'O' when taken
        public bool validBlank(string n)
        {            
            if (!int.TryParse(n,out int i))
            {
                return false;
            }
            //check number is within range
            if(i>0 && i <= size * size)
            {
                return true;
            }
            return false;    
        }

        //return all possible lines on the board
        //the objective of the game is to fill one line with your pieces
        private List<string[]> GetLines(string[,] getLinesLayout)
        {
            List<string[]> LinesList = new List<string[]>();
            //use layout to produce a list of lines
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
        
        //list of possible line statuses
        public enum StatusType { own, opp, ownSetup, oppSetup, ownSetupFork, oppSetupFork, contested, ownOther, oppOther, empty, other, error};

        //return the line statuses using the list of lines
        private StatusType[] GetStatus (List<string[]> checkLines)
        {
            StatusType[] status = new StatusType[checkLines.Count];
            int lineNum = 0;
              
            //loop through each of the lines and analyse one
            foreach(string [] line in checkLines)
            {
                //'own' refers to the player when verses the AI, or player 1 in a 2 player game
                int own = 0;
                //'opp' refers to the AI when verses the AI, or player 2 in a 2 player game
                int opp = 0;
                bool errorFlag = false;
                foreach (string piece in line)
                {
                    if (piece == errorPiece)
                    {
                    errorFlag = true;
                    }
                    //count the number of Player pieces in the line
                    if (piece == ownPiece)
                    {
                        ++own;
                    }
                    //count the number of AI pieces in the line
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
                    //StatusType.own        - line is filled with player pieces
                    //i.e. the player has won
                    status[lineNum] = StatusType.own;
                }
                else if ( opp == size)
                {
                    //StatusType.opp        - line is filled with AI pieces
                    //i.e. the AI has won
                    status[lineNum] = StatusType.opp;
                }
                else if (own == 0 && opp == 0)
                {
                    //StatusType.empty      - line has no pieces in
                    //i.e. line is empty
                    status[lineNum] = StatusType.empty;
                }
                else if (own == 0 && opp == size -1 )
                {
                    //StatusType.oppSetup   - all spaces in the line have AI pieces except one which is empty
                    //i.e. the AI could win using this line on the next turn
                    status[lineNum] = StatusType.oppSetup;
                }
                else if (own == 0 && opp == size - 2)
                {
                    //StatusType.oppFork   - all spaces in the line have AI pieces except two which is empty
                    //i.e. the AI could win using this line in two turns if it intersects another line of this status
                    status[lineNum] = StatusType.oppSetupFork;
                }
                else if (own == 0 && opp > 0)
                {
                    //StatusType.oppOther   - all spaces in the line are either empty of have AI pieces in
                    //i.e. the AI could win using this line in the future
                    status[lineNum] = StatusType.oppOther;
                }
                else if (opp == 0 && own == size - 1)
                {
                    //StatusType.ownSetup   - all spaces in the line have Player pieces except one which is empty
                    //i.e. the Player could win using this line on the next turn
                    status[lineNum] = StatusType.ownSetup;
                }
                else if (opp == 0 && own == size - 2)
                {
                    //StatusType.ownFork   - all spaces in the line have Player pieces except two which is empty
                    //i.e. the Player could win using this line in two turns if it intersects another line of this status
                    status[lineNum] = StatusType.ownSetupFork;
                }
                else if (opp == 0 && own > 0)
                {
                    //StatusType.ownOther   - all spaces in the line are either empty of have Player pieces in
                    //i.e. the Player could win using this line in the future
                    status[lineNum] = StatusType.ownOther;
                }
                else if (opp > 0 && own > 0)
                {
                    //StatusType.contested   - this line contains AI pieces and Player pieces
                    //i.e. neither the Player or AI can win with this line
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

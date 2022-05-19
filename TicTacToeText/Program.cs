using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToeText
{
    class Program
    {
        static bool twoPlayer = false;
        static bool aiFirst = true;
        //static string gameLog;
        static void Main(string[] args)
        {           
            BoardLayout currentBoard = new BoardLayout();
            bool playerTurn = true;
            bool restart = true;
            bool settings = false;
            bool turnSwitch = false;
            string input = "";
            //display instructions for te selection of game mode
            Console.WriteLine("Choose mode:");
            Console.WriteLine("1 - Two Player");
            Console.WriteLine("2 - AI Second");
            Console.WriteLine("3 - AI First");
            Console.WriteLine("4 - AI Alternate");

            //loop reading the input line
            while ((input = Console.ReadLine()) != null && input != "exit")
            {
                if (!settings && input!="")
                {
                     //first time in game give option to change custom settings
                    switch (input[0])
                    {
                        case '1':
                            //two player game
                            twoPlayer = true;
                            
                            break;
                        case '2':
                            //Player vs AI - Player always first
                            twoPlayer = false;
                            aiFirst = false;
                            break;
                        case '3':
                            //Player vs AI - AI always first
                            twoPlayer = false;
                            aiFirst = true;
                            break;
                        case '4':
                            //Player vs AI - alternate
                            twoPlayer = false;
                            aiFirst = false;
                            turnSwitch = true;
                            break;
                        default:
                            break;
                    }
                    settings = true;
                }
                settings = true;
                
                if (restart)                   
                // if restart flag is set, initialize the board
                {
                    currentBoard = new BoardLayout();
                    playerTurn = true;
                    restart = false;
                    //gameLog = "v1" + DateTime.Now.ToString();
                    DisplayBoard(currentBoard);
                    //alternate who goes first if set to alternate 
                    if (turnSwitch)
                    {
                        aiFirst = !aiFirst;
                    }

                    //if restarting and AI is set to go first, play the AI's first turn
                    if (aiFirst && !twoPlayer)
                    {
                        //determine the AI move
                        int moveAI = AIOppMove(currentBoard);
                        Console.WriteLine("AI played position " + moveAI);
                        Console.WriteLine();
                        //double check the place the AI selected is valid
                        if (!currentBoard.validBlank(moveAI))
                        {
                            Console.WriteLine("AI error");
                            //gameLog += "AIerror";
                            //SaveLog();
                            restart = true;
                            continue;
                        }
                        //gameLog += moveAI.ToString();
                        //set the AI piece and redisplay the board
                        //no need to check if the AI won, it's their first turn
                        currentBoard.SetOpp(moveAI);
                        DisplayBoard(currentBoard);
                        Console.WriteLine("Your move");
                    }                 
                    continue;
                }

                //read the number input by the Player for their turn
                if (int.TryParse(input, out int inputNum) && inputNum <= currentBoard.Size * currentBoard.Size && inputNum > 0)
                {
                    if (currentBoard.validBlank(inputNum))
                    {
                        //process turn in a 2 player game
                        if (twoPlayer)
                        {
                            //if Player 1 turn set Player 1 piece (Own)
                            if (playerTurn)
                            {
                                currentBoard.SetOwn(inputNum);
                                playerTurn = false;
                            }
                            //if Player 2 turn set Player 2 piece (Opp)
                            else
                            {
                                currentBoard.SetOpp(inputNum);
                                playerTurn = true;
                            }
                            //redisplay the board and whose turn is next
                            DisplayBoard(currentBoard);
                            if (playerTurn)
                            {
                                Console.WriteLine("Player X's turn");
                            }
                            else
                            {
                                Console.WriteLine("Player O's turn");
                            }
                            //check if either player won
                            restart = CheckGameEnd(currentBoard);
                        }
                        //process Player turn in a Player verses AI game
                        if (!twoPlayer)
                        {
                            //gameLog += "H" + inputNum.ToString();
                            //set the Player piece, redisplay the board and check if the Player won
                            currentBoard.SetOwn(inputNum);
                            DisplayBoard(currentBoard);
                            restart = CheckGameEnd(currentBoard);
                            //if Player did not win then process the AIs turn
                            if (!restart)
                            {
                                //determine the AI move
                                int moveAI = AIOppMove(currentBoard);
                                Console.WriteLine("AI played position " + moveAI);
                                Console.WriteLine();
                                //double check the place the AI selected is valid
                                if (!currentBoard.validBlank(moveAI))
                                {
                                    Console.WriteLine("AI error");
                                    //gameLog += "AIerror";
                                    //SaveLog();
                                    restart = true;
                                    continue;
                                }
                                //gameLog += moveAI.ToString();
                                //set the AI piece, redisplay the board and check if the AI won
                                currentBoard.SetOpp(moveAI);
                                DisplayBoard(currentBoard);
                                restart = CheckGameEnd(currentBoard);
                                if (!restart)
                                {
                                    Console.WriteLine("Your turn");
                                }
                            }
                        }
                        //reset if someone won the game
                        if (restart)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Press ENTER to start a new game.");
                        }
                    }
                    //display error if the selected place is not valid
                    else
                    {
                        Console.WriteLine("Space already taken.");
                    }
                    
                }
                //display error if input is invalid
                else
                {
                    Console.WriteLine("Not valid input");
                }
            }
            //gameLog += "Exit";
            //SaveLog();
        }

        //check if either player won
        static bool CheckGameEnd(BoardLayout currentBoard)
        {
            //2 Player win messages
            string oppWinMsg = "O Wins";
            string ownWinMsg = "X Wins";
            string drawMsg = "You Drew";

            if (!twoPlayer)
            {
                //Player verses AI win messages
                ownWinMsg = "You Win";
                oppWinMsg = "I Win!! =)";
                drawMsg = "We Drew";
            }
            //check the status of each of the lines on the board
            for (int i = 0; i < currentBoard.Status.Length; i++)
            {
                switch (currentBoard.Status[i])
                {
                    //AI or Player 2 win case
                    case BoardLayout.StatusType.opp:
                        Console.WriteLine(oppWinMsg);
                        //gameLog += "AIWin";
                        //SaveLog();
                        return true;
                    //Player 1 win aase
                    case BoardLayout.StatusType.own:
                        Console.WriteLine(ownWinMsg);
                        //gameLog += "HWin";
                        //SaveLog();
                        return true;
                    default:
                        break;
                }
            }
            // check if there are any free spaces left, if not the the game ends in a draw
            for (int j=1; j <= currentBoard.Size * currentBoard.Size; j++)
            {
                if (currentBoard.validBlank(j))
                {
                    return false;
                }
            }
            Console.WriteLine(drawMsg);
            //gameLog += "Draw";
            //SaveLog();
            return true;
        }

        //display the current board in the console
        static void DisplayBoard(BoardLayout currentBoard)
        {
            //determine the length of each cell
            //e.g. in a 4x4 board the single digit numbers need zero padding to be 2 characters
            int cellLength = (currentBoard.Size * currentBoard.Size).ToString().Length;
            //grid character variables
            char gridVer = '│';
            char gridHor = '─';
            char gridInt = '┼';
            string gridLine = "";

            //build the horizontal gridline
            for (int i = 0; i < currentBoard.Size; i++)
            {
                gridLine += "".PadLeft(cellLength, gridHor);
                if (i < currentBoard.Size - 1)
                {
                    gridLine += gridInt;
                }
            }

            //output the whole grid
            for (int i = 0; i < currentBoard.Size; i++)
            {
                string printLine = "";
                for (int j = 0; j < currentBoard.Size; j++)
                {
                    //build each cell number, zero padded
                    printLine += currentBoard.Layout[i, j].PadLeft(cellLength,'0');
                    //add vertical line separators
                    if (j < currentBoard.Size - 1)
                    {
                        printLine += gridVer;
                    }
                }
                //output line of cell numbers
                Console.WriteLine(printLine);
                //output horizontal line separator
                if (i < currentBoard.Size - 1)
                {
                    Console.WriteLine(gridLine);
                }
            }
            Console.WriteLine("");
        }

        //determine the AIs next move
        static int AIOppMove(BoardLayout currentBoard)
        {
            // analyse each column/row/diagonal line in turn, check for the following:
            //1) can get a complete line
            //2) opponent can get a complete line, and should block            
            //3) can create fork
            //4) can block opponent creation of fork
            //5) prepare fork (check #7 for preferance)
            //6) any empty row
            //7) set up a complete line
            //8) go anywhere

            //variables for analysis
            int winAI = -1;
            int blockWin = -1;
            int[] setupFork = new int[currentBoard.Size * currentBoard.Status.Length];
            bool setupForkYes = false;
            int[] blockFork = new int[currentBoard.Size * currentBoard.Status.Length];
            int[] prepFork = new int[currentBoard.Size * currentBoard.Status.Length];
            bool prepForkYes = false;
            int[] prepForkLine = new int[currentBoard.Size * currentBoard.Status.Length];
            int setupForkPtr = 0;
            int blockForkPtr = 0;
            int prepForkPtr = 0;

            //gameLog += "AI";
                
            //gameLog+="s";
            for (int i = 0; i < currentBoard.Status.Length; i++)
            {
                //gameLog += ((int)currentBoard.Status[i]).ToString();
                switch (currentBoard.Status[i])
                {
                    //if a line is already completed return -1 as an error
                    case BoardLayout.StatusType.own:
                    case BoardLayout.StatusType.opp:
                    case BoardLayout.StatusType.error:
                        return -1;
                    //if AI can complete line, get index of line to complete
                    case BoardLayout.StatusType.oppSetup:
                        winAI = i;
                        break;
                    // if human player can get win, block this
                    case BoardLayout.StatusType.ownSetup:
                        blockWin = i;
                        break;
                    // find indexes of free spaces in following lines
                    case BoardLayout.StatusType.oppSetupFork:
                    case BoardLayout.StatusType.ownSetupFork:
                    case BoardLayout.StatusType.empty:
                    case BoardLayout.StatusType.oppOther:
                    string[] forkLine = currentBoard.Lines[i];

                        for (int j = 0; j < currentBoard.Size; j++)
                        {
                            if (currentBoard.validBlank(forkLine[j]))
                            {
                                //space to setup fork
                                if (currentBoard.Status[i] == BoardLayout.StatusType.oppSetupFork)
                                {
                                    setupFork[setupForkPtr] = int.Parse(forkLine[j]);
                                    setupForkPtr++;
                                    setupForkYes = true;
                                }
                                //space to block setup fork
                                else if (currentBoard.Status[i] == BoardLayout.StatusType.ownSetupFork)
                                {
                                    blockFork[blockForkPtr] = int.Parse(forkLine[j]);
                                    blockForkPtr++;
                                }
                                //spaces and line numbers to prepare forks to setup next turn
                                else
                                {
                                    prepFork[prepForkPtr] = int.Parse(forkLine[j]);
                                    prepForkLine[prepForkPtr] = i;
                                    prepForkPtr++;
                                    prepForkYes = true;
                                }
                            }
                        }
                        break;  
                      
                    default:
                        break;
                }
            }
            //gameLog += "s";

            // if either player can complete a line, get the index of the move to win or block
            if (winAI != -1 || blockWin != -1)
            {
                //gameLog += "a";
                int i = blockWin;
                if (winAI != -1)
                {
                    //gameLog += "b";
                    i = winAI;
                }
                string[] winLine = currentBoard.Lines[i];
                for (int j = 0; j < currentBoard.Size; j++)
                {
                    if (currentBoard.validBlank(winLine[j]))
                    {
                        return int.Parse(winLine[j]);
                    }
                }
            }

            // check for duplicates in setupFork. duplicate entries means we can create a fork
            for (int i = 0; i < setupForkPtr; i++)
            {
                for (int j = 0; j < setupForkPtr; j++)
                {
                    if (i != j && setupFork[i] == setupFork[j])
                    {
                        //gameLog += "c";
                        return setupFork[i];
                    }
                }
            }

            // similarly, check blockFork to block player fork.
            for (int i = 0; i < blockForkPtr; i++)
            {
                for (int j = 0; j < blockForkPtr; j++)
                {
                    if (i != j && blockFork[i] == blockFork[j])
                    {
                        //gameLog += "d";
                        return blockFork[i];
                    }
                }
            }

            //prepare fork to setup next turn
            int prepForkNum = 0;
            for (int i =0; i < setupForkPtr; i++)
            {
                for (int j =0; j< prepForkPtr; j++)
                {
                    if(setupFork[i] == prepFork[j])
                    {
                        for (int k = 0; k < currentBoard.Size; k++)
                        {
                            // if move also sets up a line, choose it now otherwise store it for later
                            if (int.TryParse(currentBoard.Lines[prepForkLine[j]][k], out prepForkNum) && 
                                prepForkNum != setupFork[i] && 
                                setupFork.Contains<int>(prepForkNum) && 
                                currentBoard.validBlank(prepForkNum))
                            {
                                //gameLog += "f";
                                return prepForkNum;                    
                            }
                        }
                    }
                }
            }
            //if a fork was prepared, choose it.
            if (currentBoard.validBlank(prepForkNum))
            {
                //gameLog += "g";
                return prepForkNum;
            }

            // else go in a random empty row
            int rndMove = 0;
            Random rnd = new Random();
            if (prepForkYes)
            {
                rndMove = rnd.Next(prepFork.Length);
                while (!currentBoard.validBlank(prepFork[rndMove]))
                {
                    rndMove = rnd.Next(prepFork.Length);
                }
                //gameLog += "h";
                return prepFork[rndMove];
            }

            //else make a move set up a line
            if(setupForkYes && currentBoard.validBlank(setupFork[0]))
            {
                //gameLog += "i";
                return setupFork[0];
            }

            // else go in any available space
            while (!currentBoard.validBlank(rndMove))
            {
             rndMove = rnd.Next(currentBoard.Size * currentBoard.Size)+1;
            }
            //gameLog += "j";
            return rndMove;
        }

        //static void SaveLog()
        //{
        //    if (!twoPlayer)
        //    {
        //        //string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TicTacToeLog.txt";
        //        string filePath = "TicTacToeLog.txt";
        //        System.IO.StreamWriter logWriter = new System.IO.StreamWriter(filePath, System.IO.File.Exists(filePath));
        //        logWriter.WriteLine(gameLog);
        //        logWriter.Close();
        //        gameLog = "v1" + DateTime.Now.ToString();
        //    }
        //}
    }
}

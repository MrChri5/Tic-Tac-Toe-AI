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
        static int boardSize = 3;
        static string gameLog = "v1";

        static void Main(string[] args)
        {           
            BoardLayout currentBoard = new BoardLayout(boardSize);
            bool playerTurn = true;
            bool restart = true;
            bool settings = false;
            bool turnSwitch = false;
            string input = "";
            Console.WriteLine("Choose mode:");
            Console.WriteLine("1 - Two Player");
            Console.WriteLine("2 - AI Second");
            Console.WriteLine("3 - AI First");
            Console.WriteLine("4 - AI Alternate");

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
                            //human vs ai - human always first
                            twoPlayer = false;
                            aiFirst = false;
                            break;
                        case '3':
                            // human vs ai - ai always first
                            twoPlayer = false;
                            aiFirst = true;
                            break;
                        case '4':
                            // human vs ai - alternate
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
                    currentBoard = new BoardLayout(boardSize);
                    playerTurn = true;
                    restart = false;
                    gameLog = "v1" + DateTime.Now.ToString();
                    DisplayBoard(currentBoard);
                    //alternate who goes first if set to alternate 
                    if (turnSwitch)
                    {
                        aiFirst = !aiFirst;
                    }

                    //if restarting and AI is set to go first, play the AI's first turn
                    if (aiFirst && !twoPlayer)
                    {
                        int moveAI = AIOppMove(currentBoard);
                        Console.WriteLine("AI played position " + moveAI);
                        Console.WriteLine();
                        if (!currentBoard.validBlank(moveAI))
                        {
                            Console.WriteLine("AI error");
                            gameLog += "AIerror";
                            SaveLog();
                            restart = true;
                            continue;
                        }
                        gameLog += moveAI.ToString();
                        currentBoard.SetOpp(moveAI);
                        DisplayBoard(currentBoard);
                        Console.WriteLine("Your move");
                    }                 
                    continue;
                }

                if (int.TryParse(input, out int inputNum) && inputNum <= boardSize * boardSize && inputNum > 0)
                {
                    if (currentBoard.validBlank(inputNum))
                    {
                        if (twoPlayer)
                        {
                            if (playerTurn)
                            {
                                currentBoard.SetOwn(inputNum);
                                playerTurn = false;
                            }
                            else
                            {
                                currentBoard.SetOpp(inputNum);
                                playerTurn = true;
                            }
                            DisplayBoard(currentBoard);
                            if (playerTurn)
                            {
                                Console.WriteLine("Player X's turn");
                            }
                            else
                            {
                                Console.WriteLine("Player O's turn");
                            }
                            restart = CheckGameEnd(currentBoard);
                        }
                        if (!twoPlayer)
                        {
                            gameLog += "H" + inputNum.ToString();
                            currentBoard.SetOwn(inputNum);
                            DisplayBoard(currentBoard);
                            restart = CheckGameEnd(currentBoard);
                            if (!restart)
                            {
                                int moveAI = AIOppMove(currentBoard);
                                Console.WriteLine("AI played position " + moveAI);
                                Console.WriteLine();
                                if (!currentBoard.validBlank(moveAI))
                                {
                                    Console.WriteLine("AI error");
                                    gameLog += "AIerror";
                                    SaveLog();
                                    restart = true;
                                    continue;
                                }
                                gameLog += moveAI.ToString();
                                currentBoard.SetOpp(moveAI);
                                DisplayBoard(currentBoard);
                                restart = CheckGameEnd(currentBoard);
                                if (!restart)
                                {
                                    Console.WriteLine("Your turn");
                                }
                            }
                        }
                        if (restart)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Press ENTER to start a new game.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Space already taken.");
                    }
                    
                }
                else
                {
                    Console.WriteLine("Not valid input");
                }
            }
            gameLog += "Exit";
            SaveLog();
        }

        static bool CheckGameEnd(BoardLayout currentBoard)
        {
            string oppWinMsg = "O Wins";
            string ownWinMsg = "X Wins";
            string drawMsg = "You Drew";

            if (!twoPlayer)
            {
                ownWinMsg = "You Win";
                oppWinMsg = "I Win!! =)";
                drawMsg = "We Drew";
            }
            for (int i = 0; i < currentBoard.Status.Length; i++)
            {
                switch (currentBoard.Status[i])
                {
                    case BoardLayout.StatusType.opp:
                        Console.WriteLine(oppWinMsg);
                        gameLog += "AIWin";
                        SaveLog();
                        return true;
                    case BoardLayout.StatusType.own:
                        Console.WriteLine(ownWinMsg);
                        gameLog += "HWin";
                        SaveLog();
                        return true;
                    default:
                        break;
                }
            }
            // check if there are any free spaces left
            for (int j=1; j <= currentBoard.Size * currentBoard.Size; j++)
            {
                if (currentBoard.validBlank(j))
                {
                    return false;
                }
            }
            Console.WriteLine(drawMsg);
            gameLog += "Draw";
            SaveLog();
            return true;
        }

        static void DisplayBoard(BoardLayout currentBoard)
        {
            int cellLength = (currentBoard.Size * currentBoard.Size).ToString().Length;
            char gridVer = '│';
            char gridHor = '─';
            char gridInt = '┼';
            string gridLine = "";

            for (int i = 0; i < currentBoard.Size; i++)
            {
                gridLine += "".PadLeft(cellLength, gridHor);
                if (i < currentBoard.Size - 1)
                {
                    gridLine += gridInt;
                }
            }

            for (int i = 0; i < currentBoard.Size; i++)
            {
                string printLine = "";
                for (int j = 0; j < currentBoard.Size; j++)
                {
                    printLine += currentBoard.Layout[i, j].PadLeft(cellLength,'0');
                    if (j < currentBoard.Size - 1)
                    {
                        printLine += gridVer;
                    }
                }
                Console.WriteLine(printLine);
                if (i < currentBoard.Size - 1)
                {
                    Console.WriteLine(gridLine);
                }
            }
            Console.WriteLine("");
        }

        static int AIOppMove(BoardLayout currentBoard)
        {
            //compute the AI (opponent) move
            // analyse each column/row/diag in turn, check for:
            //1) can get 3 in a row
            //2) opponent can get 3 , and should block            
            //3) can create fork
            //4) can block opponent creation of fork
            //5) prepare fork (check #7 for preferance)
            //6) any empty row
            //7) set up 3 in a row
            //8) go anywhere

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

            gameLog += "AI";
                
            gameLog+="s";
            for (int i = 0; i < currentBoard.Status.Length; i++)
            {
                gameLog += ((int)currentBoard.Status[i]).ToString();
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
            gameLog += "s";

            // if either player can complete a line, get the index of the move to win or block
            if (winAI != -1 || blockWin != -1)
            {
                gameLog += "a";
                int i = blockWin;
                if (winAI != -1)
                {
                    gameLog += "b";
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
                        gameLog += "c";
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
                        gameLog += "d";
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
                            // if move also sets up a line, choose it now other wise store it for later
                            if (int.TryParse(currentBoard.Lines[prepForkLine[j]][k], out prepForkNum) && 
                                prepForkNum != setupFork[i] && 
                                setupFork.Contains<int>(prepForkNum) && 
                                currentBoard.validBlank(prepForkNum))
                            {
                                gameLog += "f";
                                return prepForkNum;                    
                            }
                        }
                    }
                }
            }
            //if a fork was prepared, choose it.
            if (currentBoard.validBlank(prepForkNum))
            {
                gameLog += "g";
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
                gameLog += "h";
                return prepFork[rndMove];
            }

            //else make a move set up a line
            if(setupForkYes && currentBoard.validBlank(setupFork[0]))
            {
                gameLog += "i";
                return setupFork[0];
            }

            // else go in any available space
            while (!currentBoard.validBlank(rndMove))
            {
             rndMove = rnd.Next(currentBoard.Size * currentBoard.Size)+1;
            }
            gameLog += "j";
            return rndMove;
        }

        static void SaveLog()
        {
            if (!twoPlayer)
            {
                //string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TicTacToeLog.txt";
                string filePath = "TicTacToeLog.txt";
                System.IO.StreamWriter logWriter = new System.IO.StreamWriter(filePath, System.IO.File.Exists(filePath));
                logWriter.WriteLine(gameLog);
                logWriter.Close();
                gameLog = "v1" + DateTime.Now.ToString();
            }
        }
    }
}

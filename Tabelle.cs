using System;
using System.Threading;

namespace BattagliaNavale
{
    class Program
    {
        // Griglia 10 per 10 che scala per essere 50 x 25
        //
        //         1     2     3     4  
        // 4 da 2  **    **    **    **
        // 2 da 3  ***   ***
        // 2 da 4  ****  ****
        // 1 da 5  ***** ****
        enum HitStatus
        {
            hit,
            miss,
            occupied
        }
        static bool AreBoatsLeft(char[,] board)
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (board[i, j] == '+')
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        static void ClearLine(int top)
        {
            Console.SetCursorPosition(0, top);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, top);
        }
        static void DrawBoard(char[,] board)
        {
            for (int i = 1; i <= 10; i++)
            {
                for (int j = 1; j <= 10; j++)
                {
                    if (board[j - 1, i - 1] == '+')
                        Console.ForegroundColor = ConsoleColor.Green;
                    else if (board[j - 1, i - 1] == 'X')
                        Console.ForegroundColor = ConsoleColor.Red;
                    else if (board[j - 1, i - 1] == 'O')
                        Console.ForegroundColor = ConsoleColor.Blue;
                    else if (board[j - 1, i - 1] == '*')
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    DrawPosition(i, j, board[j - 1, i - 1] != default(char) ? board[j - 1, i - 1] : ' ');
                    Console.ResetColor();
                }
            }
        }
        static void DrawPosition(int x, int y, char c)
        {
            Console.CursorVisible = false;
            int top = Console.CursorTop;
            int left = Console.CursorLeft;
            if (x > 10 || x < 1 || y > 10 || y < 0)
            {
                throw new Exception("Wrong coordinates have been entered!");
            }
            x *= 2;
            y *= 2;
            Console.SetCursorPosition(x, y);
            Console.Write(c);
            Console.SetCursorPosition(left, top);
            Console.CursorVisible = true;
        }
        static bool IsColliding(int x, int y, int length, bool vertical, char[,] board)
        {
            for (int i = 0; i < length; i++)
            {
                try
                {
                    if (vertical)
                    {
                        if (board[y + i, x] == '+')
                            return true;
                    }
                    else
                    {
                        if (board[y, x - i] == '+')
                            return true;
                    }
                }
                catch (IndexOutOfRangeException e)
                {
                    return true;
                }
            }
            return false;
        }
        static bool NextBool()
        {
            Random r = new Random();
            return r.Next() > (Int32.MaxValue / 2);
        }
        static void PrintMessage(string message)
        {
            int cursorTop = Console.CursorTop;
            ClearLine(cursorTop);
            Console.SetCursorPosition(0, cursorTop);
            Console.Write(message);
        }
        static HitStatus Hit(char[,] board, char[,] hiddenBoard, int x, int y)
        {
            HitStatus status;
            switch (board[y, x])
            {
                case '+':
                    status = HitStatus.hit;
                    hiddenBoard[y, x] = 'X';
                    board[y, x] = 'X';
                    break;
                case 'X':
                case 'O':
                    status = HitStatus.occupied;
                    break;

                default:
                case default(char):
                    status = HitStatus.miss;
                    hiddenBoard[y, x] = 'O';
                    board[y, x] = 'O';
                    break;
            }
            return status;
        }


        static void BoatPositioningKeys(char[,] playerBoard, int boatLength)
        {
            int headX = 5;
            int headY = 5;


            void positionBoat(int x, int y, bool vertical, bool definitive = false)
            {
                for (int i = 0; i < boatLength; i++)
                {
                    if (vertical)
                        playerBoard[y + i, x] = definitive ? '+' : '*';
                    else
                        playerBoard[y, x - i] = definitive ? '+' : '*';
                }
                DrawBoard(playerBoard);
            }

            void clearBoat(int x, int y, bool vertical)
            {

                for (int i = 0; i < boatLength; i++)
                {
                    if (vertical)
                        playerBoard[y + i, x] = ' ';
                    else
                        playerBoard[y, x - i] = ' ';
                }
            }

            ConsoleKeyInfo key;

            bool vertical = false;

            while (IsColliding(headX, headY, boatLength, vertical, playerBoard))
            {
                Random r = new Random();
                headX = r.Next(boatLength, 10);
                headY = r.Next(boatLength, 10);
            }

            // Posizioni precedenti
            int sHeadX = headX;
            int sHeadY = headY;

            positionBoat(headX, headY, vertical);
            do
            {
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.LeftArrow && headX > 0)
                {
                    if (!vertical)
                    {
                        if (headX - boatLength + 1 > 0)
                            headX--;
                    }
                    else
                        headX--;
                }
                else if (key.Key == ConsoleKey.RightArrow && headX < 9)
                {
                    headX++;
                }
                else if (key.Key == ConsoleKey.DownArrow && headY < 9)
                {
                    if (vertical)
                    {
                        if (headY + boatLength - 1 < 9)
                            headY++;
                    }
                    else
                        headY++;
                }
                else if (key.Key == ConsoleKey.UpArrow && headY > 0)
                {
                    headY--;

                }
                else if (key.Key == ConsoleKey.R)
                {
                    if (((vertical && headX - boatLength + 2 > 0) || (!vertical && headY + boatLength - 2 < 9)) && !IsColliding(headX, headY, boatLength, !vertical, playerBoard))
                    {
                        vertical = !vertical;
                    }
                    else
                        PrintMessage("The boat cannot rotate!");
                }
                if (!IsColliding(headX, headY, boatLength, vertical, playerBoard))
                {
                    clearBoat(headX + (sHeadX - headX), headY + (sHeadY - headY), (key.Key == ConsoleKey.R &&
                        !IsColliding(headX, headY, boatLength, !vertical, playerBoard)) ? !vertical : vertical);
                    sHeadX = headX;
                    sHeadY = headY;
                    positionBoat(headX, headY, vertical);
                }
                else
                {
                    PrintMessage("There is a boat blocking the way!");
                    headX += (sHeadX - headX);
                    headY += (sHeadY - headY);
                }
            } while (key.Key != ConsoleKey.Enter || IsColliding(headX, headY, boatLength, vertical, playerBoard));
            positionBoat(headX, headY, vertical, true);
            ClearLine(Console.CursorTop);
        }
        static void SetupFase(char[,] playerBoard)
        {
            // Boat in 0 = **, boat in 1 = ***, boat in 2 = ****, boat in 3 = ***** 
            int[] boatCount = new int[] { 4, 2, 2, 1 };
            Console.WriteLine(Tabelle.Tabelle.Griglia(10, numerata: true, conLettere: true));
            Console.WriteLine("- BOAT POSITIONING PHASE -");
            for (int i = 0; i < boatCount.Length; i++)
            {
                for (int j = 0; j < boatCount[i]; j++)
                {
                    BoatPositioningKeys(playerBoard, i + 2);
                }
            }
            ClearLine(Console.CursorTop - 1);

        }
        static void GamePhase(char[,] playerBoard, char[,] enemyBoard)
        {
            int messageLine = Console.CursorTop;
            char[,] hiddenEnemyBoard = new char[10, 10];
            void ClearOutput(int lines)
            {
                for (int i = messageLine + lines; i >= messageLine; i--)
                {
                    ClearLine(i);
                }
            }

            void GetCoords(out int x, out int y)
            {
                string coords;
                PrintMessage("- Enemy Board -\n");
                DrawBoard(hiddenEnemyBoard);
                do
                {
                    do
                    {
                        PrintMessage("Insert the coordinates on which you want to shoot: ");
                        coords = Console.ReadLine().ToUpper();
                        ClearLine(messageLine + 1);
                    } while (coords.Length < 2 || coords.Length > 3);
                    try
                    {
                        x = coords.Length == 2 ? Convert.ToInt32(coords[1]) - 48 - 1 : Convert.ToInt32(coords.Substring(1, 2)) - 1;
                    }
                    catch (FormatException e)
                    {
                        x = -1;
                    }
                    y = 9 - (coords[0] - 65);
                    if (x < 0 || x > 9 || y < 0 || y > 9)
                    {
                        Console.SetCursorPosition(0, messageLine + 2);
                        PrintMessage("The coordinates you entered are incorrect!");
                        Console.SetCursorPosition(0, messageLine + 1);
                    }
                } while (x < 0 || x > 9 || y < 0 || y > 9);
                ClearOutput(3);
            }

            // Generate enemy boats
            int[] boatCount = new int[] { 4, 2, 2, 1 };
            Random r = new Random();

            for (int i = 0; i < boatCount.Length; i++)
            {
                for (int j = 0; j < boatCount[i]; j++)
                {
                    bool vertical = NextBool();
                    int x, y;
                    x = r.Next(i + 2, 10);
                    y = r.Next(0, 10 - (i + 2));
                    if (IsColliding(x, y, i + 2, vertical, enemyBoard))
                    {
                        j--;
                        continue;
                    }
                    for (int l = 0; l < i + 2; l++)
                    {
                        if (vertical)
                            enemyBoard[y + l, x] = '+';
                        else
                            enemyBoard[y, x - l] = '+';
                    }
                }
            }

            // Inzio ciclo di gioco
            int winner = 0;
            int turn = 0;
            int[] previousEnemyShot = new int[2];
            bool previousShotHitBoat = false;

            while (winner == 0)
            {
                // Turno giocatore
                if (turn % 2 == 0)
                {
                    HitStatus hit;
                    do
                    {
                        int x, y;
                        GetCoords(out x, out y);
                        hit = Hit(enemyBoard, hiddenEnemyBoard, x, y);
                        switch (hit)
                        {
                            case HitStatus.hit:
                                PrintMessage($"The shot hit an enemy ship in {(char)((9 - y) + 65)}{x + 1}! (Press any key to continue)");
                                break;
                            case HitStatus.miss:
                                PrintMessage($"The shot in {(char)((9 - y) + 65)}{x + 1} missed the enemy! (Press any key to continue)");
                                break;
                            case HitStatus.occupied:
                                PrintMessage($"Why are you shooting where you already shot?? (Press any key to continue)");
                                break;
                        }
                        DrawBoard(hiddenEnemyBoard);
                        Console.ReadKey();
                    } while (hit == HitStatus.occupied);
                    ClearOutput(3);
                    turn = 1;
                }
                // Turno AI
                else
                {
                    PrintMessage("- Player Board -\n");
                    DrawBoard(playerBoard);
                    HitStatus hit;
                    int x, y;
                    int retries = 0;
                    do
                    {
                        // Se l'ultimo colpo è andato a segno allora ci spara vicino
                        if (previousShotHitBoat && retries < 4)
                        {
                            do
                            {
                                int direction = r.Next(0, 4);
                                x = previousEnemyShot[0];
                                y = previousEnemyShot[1];
                                switch (direction)
                                {
                                    // Down
                                    case 0:
                                        if (y < 9)
                                        {
                                            y++;
                                        }
                                        break;
                                    // Up
                                    case 1:
                                        if (y > 0)
                                        {
                                            y--;
                                        }
                                        break;
                                    // Left
                                    case 2:
                                        if (x > 0)
                                        {
                                            x--;
                                        }
                                        break;
                                    // Right
                                    case 3:
                                        if (x < 9)
                                        {
                                            x++;
                                        }
                                        break;
                                }
                            } while (previousEnemyShot[0] != x && previousEnemyShot[1] != y);
                        }
                        else
                        {
                            x = r.Next(0, 10);
                            y = r.Next(0, 10);
                        }
                        hit = Hit(playerBoard, playerBoard, x, y);
                        retries++;
                    } while (hit == HitStatus.occupied);
                    previousEnemyShot = new int[] { x, y };
                    if (hit == HitStatus.hit)
                    {
                        PrintMessage($"The enemy hit one of your boats in {(char)((9 - y) + 65)}{x + 1}! (Press any key to continue)");
                        previousShotHitBoat = true;
                    }
                    else
                    {
                        PrintMessage($"The enemy's shot in {(char)((9 - y) + 65)}{x + 1} missed! (Press any key to continue)");
                        previousShotHitBoat = false;
                    }
                    DrawBoard(playerBoard);
                    Console.ReadKey();
                    ClearOutput(2);
                    turn = 0;
                }
                // Controllo condizioni vittoria
                if (!AreBoatsLeft(playerBoard))
                {
                    // The enemy is the winner
                    winner = 2;
                }
                if (!AreBoatsLeft(enemyBoard))
                {
                    // The player is the winner
                    winner = 1;
                }
            }
            PrintMessage($"The winner is {(winner == 1 ? "the player, you destroyed all of the enemy boats" : "the enemy, he destroyed all your boats")}!");

            PrintMessage($"Do you wanna play again?");
            string yes_no;
            yes_no = Console.ReadLine().ToLower();
            switch (yes_no)
            {
                case "yes":
                    Main();
                    break;
                case "y":
                    Main();
                    break;
                case "no";
                    break;
                case "n":
                    break;
                default:
                    PrintMessage("Not a valid yes or no");
                    break;
            }
        }

        static void Main()
        {
            bool exit = false;
            // TODO
            // Input coordinate su cui sparare
            // Cambio board e quindi messaggio sotto che indica la board corrente
            // - Funzione cambia board(boardCorrente)
            char[,] playerBoard = new char[10, 10];
            char[,] enemyBoard = new char[10, 10];
            SetupFase(playerBoard);
            GamePhase(playerBoard, enemyBoard);
            Console.ReadKey();
        }
    }
}

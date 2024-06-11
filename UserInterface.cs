namespace Ex02
{
    internal class UserInterface
    {
        private const string k_TryAgainStr = ", Please try again.";
        private const string k_ChooseSecondPlayer = "Press 1 to play against another Player.\nPress 2 to play against the computer.";
        private const string k_ChooseQuitOrRestart = "Press 1 to Restart the game.\nPress 2 to Quit.";
        private const string k_FirstChoice = "1";
        private const string k_SecondChoice = "2";
        private const byte k_MaxBoardSize = 6;
        private const byte k_MinBoardSize = 4;
        private bool m_QuitGame = false;

        public void RunMemoryGame()
        {
            MemoryGame memoryGame = new MemoryGame();
            byte playerNumber = 1;
            bool playingGame = true;

            memoryGame.InitializeHumanPlayer(getPlayerNameFromUser(playerNumber), playerNumber);
            if (handleDoubleChoiceQuestion(k_ChooseSecondPlayer) == true)
            {
                playerNumber++;
                memoryGame.InitializeHumanPlayer(getPlayerNameFromUser(playerNumber), playerNumber);
            }

            while (playingGame)
            {
                getBoardSizeFromUser(memoryGame);
                PlayMemoryGame(memoryGame);
                playingGame = shouldQuitOrRestart();
            }

            printExitMessage();
        }

        private void printExitMessage()
        {
            ConsoleUtils.Screen.Clear();
            System.Console.WriteLine("\n\nTHANK YOU FOR PLAYING MEMORY GAME!");
            System.Console.WriteLine("Press Enter to Exit...");
            System.Console.ReadLine();
        }

        private bool shouldQuitOrRestart()
        {
            bool shouldContinueGame = false;

            if (!m_QuitGame)
            {
                shouldContinueGame = handleDoubleChoiceQuestion(k_ChooseQuitOrRestart);
            }

            return shouldContinueGame;
        }

        private void getBoardSizeFromUser(MemoryGame io_MemoryGame)
        {
            int boardHeight, boardWidth;
            bool sizeValid = false;

            while(!sizeValid)
            {
                isValidBoardDimension(out boardHeight, "Height");
                isValidBoardDimension(out boardWidth, "Width");
                sizeValid = io_MemoryGame.SetBoardSize(boardHeight, boardWidth);
                if(sizeValid == false)
                {
                    System.Console.WriteLine($"Invalid Board Dimensions, odd dimensions are not allowed{k_TryAgainStr}");
                }
            }
        }

        private void isValidBoardDimension(out int o_Measurement, string i_DimensionType)
        {
            System.Console.WriteLine("Please enter board dimensions (even numbers only):");
            System.Console.WriteLine($"Please enter board {i_DimensionType}({k_MinBoardSize}-{k_MaxBoardSize}):");
            while (!int.TryParse(System.Console.ReadLine(), out o_Measurement) || 
                   !(o_Measurement <= k_MaxBoardSize && o_Measurement >= k_MinBoardSize))
            {
                ConsoleUtils.Screen.Clear();
                if(o_Measurement == 0)
                {
                    System.Console.WriteLine($"invalid {i_DimensionType}, input was not an integer{k_TryAgainStr}");
                }
                else
                {
                    System.Console.WriteLine($"invalid {i_DimensionType}, input was not in the required range{k_TryAgainStr}");
                }

                System.Console.WriteLine($"Please enter board {i_DimensionType}:");
            }

            ConsoleUtils.Screen.Clear();
        }

        private bool handleDoubleChoiceQuestion(string i_Question)
        {
            bool wasFirstChoiceTaken = false;
            string userChoice;

            System.Console.WriteLine(i_Question);
            userChoice = System.Console.ReadLine();
            while (userChoice != k_FirstChoice && userChoice != k_SecondChoice)
            {
                ConsoleUtils.Screen.Clear();
                System.Console.WriteLine($"Invalid input{k_TryAgainStr}");
                System.Console.WriteLine(i_Question);
                userChoice = System.Console.ReadLine();
            }

            ConsoleUtils.Screen.Clear();
            if (userChoice == k_FirstChoice)
            {
                wasFirstChoiceTaken = true;
            }

            return wasFirstChoiceTaken;
        }

        private string getPlayerNameFromUser(int i_PlayerNumber)
        {
            string playerName;

            System.Console.WriteLine($"Please enter player #{i_PlayerNumber} name:");
            playerName = System.Console.ReadLine();
            ConsoleUtils.Screen.Clear();

            return playerName;
        }

        public void PlayMemoryGame(MemoryGame i_MemoryGame)
        {
            i_MemoryGame.InitializeBoard();
            printBoard(i_MemoryGame.Board, i_MemoryGame.PossibleCardValues);
            while(!i_MemoryGame.IsGameOver())
            {
                chooseCardToFlip(i_MemoryGame);
                chooseCardToFlip(i_MemoryGame);
                if (m_QuitGame)
                {
                    break;
                }

                i_MemoryGame.CompareCardsAndHandleResult();
                printBoard(i_MemoryGame.Board, i_MemoryGame.PossibleCardValues);
            }

            if(!m_QuitGame)
            { 
                showResults(i_MemoryGame);
            }
        }

        private void showResults(MemoryGame i_MemoryGame)
        {
            string firstPlayerName = i_MemoryGame.GetPlayerName(1);
            string secondPlayerName = i_MemoryGame.GetPlayerName(2);
            int firstPlayerScore = i_MemoryGame.GetPlayerScore(1);
            int secondPlayerScore = i_MemoryGame.GetPlayerScore(2);

            System.Console.WriteLine($"{firstPlayerName}: {firstPlayerScore}\n{secondPlayerName}: {secondPlayerScore}\n");
        }

        private void getCardLocation(out int o_ChosenRow, out int o_ChosenColumn, MemoryGame i_MemoryGame)
        {
            if (i_MemoryGame.IsPlayerHuman())
            {
                getCardLocationFromUser(out o_ChosenRow, out o_ChosenColumn, i_MemoryGame);
            }
            else
            {
                i_MemoryGame.GetRandomCardLocation(out o_ChosenRow, out o_ChosenColumn);
            }
        }

        private void chooseCardToFlip(MemoryGame io_MemoryGame)
        {
            int chosenColumn, chosenRow;
            bool cardLocationOutOfBounds = true, cardAlreadyFlipped = true;

            if(!m_QuitGame)
            {
                while(cardAlreadyFlipped || cardLocationOutOfBounds)
                {
                    cardLocationOutOfBounds = true;
                    getCardLocation(out chosenRow, out chosenColumn, io_MemoryGame);
                    if (!m_QuitGame)
                    {
                        io_MemoryGame.TryToFlipCard(chosenRow, chosenColumn, ref cardLocationOutOfBounds, ref cardAlreadyFlipped);
                        sendInvalidCardLocationMessage(cardLocationOutOfBounds, cardAlreadyFlipped, io_MemoryGame);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private void sendInvalidCardLocationMessage(bool i_CardLocationOutOfBounds, bool i_CardAlreadyFlipped, MemoryGame i_MemoryGame)
        {
            printBoard(i_MemoryGame.Board, i_MemoryGame.PossibleCardValues);
            if (i_CardLocationOutOfBounds)
            {
                System.Console.WriteLine($"Invalid Card Location, the location entered is out of the board{k_TryAgainStr}");
            }
            else if(i_CardAlreadyFlipped && i_MemoryGame.IsPlayerHuman())
            {
                System.Console.WriteLine($"Invalid Card Location, the card in the entered location is already flipped{k_TryAgainStr}");
            }
        }

        private void getCardLocationFromUser(out int o_ChosenRow, out int o_ChosenColumn, MemoryGame i_MemoryGame)
        {
            string currentPlayerName = i_MemoryGame.GetCurrentPlayerName();
            string userInput;

            o_ChosenColumn = -1;
            o_ChosenRow = -1;
            System.Console.WriteLine($"{currentPlayerName}'s Turn: Please Choose a card Location(example B3):");
            userInput = System.Console.ReadLine();
            while(!isValidLocationInput(userInput))
            {
                printBoard(i_MemoryGame.Board, i_MemoryGame.PossibleCardValues);
                System.Console.WriteLine($"Invalid input, input is not in the required format{k_TryAgainStr}");
                System.Console.WriteLine($"{currentPlayerName}'s Turn: Please Choose a card Location(example B3):");
                userInput = System.Console.ReadLine();
            }

            if(!m_QuitGame)
            {
                parsCardLocation(out o_ChosenRow, out o_ChosenColumn, userInput);
            }
        }

        private void parsCardLocation(out int o_ChosenRow, out int o_ChosenColumn, string i_UserInput)
        {
            if(char.IsLower(i_UserInput[0]))
            {
                o_ChosenColumn = (int)(i_UserInput[0] - 'a' + 1);
            }
            else
            {
                o_ChosenColumn = (int)(i_UserInput[0] - 'A' + 1);
            }

            o_ChosenRow = (int)(i_UserInput[1] - '0');
        }

        private bool isValidLocationInput(string i_UserInput)
        {
            bool inputValid = false;

            if(i_UserInput == "q" || i_UserInput == "Q")
            {
                m_QuitGame = true;
                inputValid = true;
            }
            else if(i_UserInput.Length == 2)
            {
                if(char.IsDigit(i_UserInput, 1) && char.IsLetter(i_UserInput, 0))
                {
                    inputValid = true;
                }
            }

            return inputValid;
        }

        private void printColumnNames(int i_RowSize)
        {
            char columnName;

            for (int i = 0; i < i_RowSize; i++)
            {
                if (i > 1 && i % 4 == 0)
                {
                    columnName = (char)(i / 4 + 'A' - 1);
                    System.Console.Write(columnName.ToString());
                }
                else
                {
                    System.Console.Write(" ");
                }
            }
        }

        private void printBoard(Card[,] i_GameBoard, byte[] i_CardValues)
        {
            char valueToPrint;
            int boardRow, boardCol;
            int rowSize = 4 * i_GameBoard.GetLength(1) + 2;
            int colSize = (2 * i_GameBoard.GetLength(0)) + 1;

            ConsoleUtils.Screen.Clear();
            printColumnNames(rowSize);
            System.Console.WriteLine();
            for (int i = 0; i < colSize; i++)
            {
                for(int j = 0; j <= rowSize; j++)
                {
                    boardRow = i / 2;
                    boardCol = j / 4 - 1;
                    if(i % 2 == 0 && j > 1)
                    {
                        System.Console.Write("=");
                    }
                    else if (i % 2 == 1 && j == 0)
                    {
                        System.Console.Write((i / 2) + 1);
                    }
                    else if(j % 4 == 2 || j == rowSize)
                    {
                        System.Console.Write("|");
                    }
                    else if(j > 1 && j % 4 == 0 && i_GameBoard[boardRow, boardCol].IsValueVisible)
                    {
                        valueToPrint = (char)(i_CardValues[i_GameBoard[boardRow, boardCol].Value] + 'A');
                        System.Console.Write(valueToPrint); 
                    }
                    else
                    {
                        System.Console.Write(" ");
                    }
                }

                System.Console.WriteLine();
            }

            System.Console.WriteLine();
        }
    }
}

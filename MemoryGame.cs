namespace Ex02
{
    internal class MemoryGame
    {
        private const byte k_NumOfPlayers = 2;
        private const byte k_NumOfCardsFlippedPerTurn = 2;
        private const byte k_NumOfCardValues = 26;
        private readonly Player[] r_Players;
        private readonly LocationOnBoard[] r_CurrentTurnCardsLocations = new LocationOnBoard[k_NumOfCardsFlippedPerTurn];
        private Card[,] m_Board;
        private byte[] m_PossibleCardValues;
        private byte m_NumOfPairs = 0;
        private byte m_NumOfFlippedCardsInCurrentTurn = 0;
        private byte m_NumberOfCurrentlyPlayingPlayer = 0;

        private struct LocationOnBoard
        {
            private byte m_Row;
            private byte m_Col;

            public byte Row
            {
                get
                {
                    return m_Row;
                }
                set
                {
                    m_Row = value;
                }
            }

            public byte Col
            {
                get
                {
                    return m_Col;
                }
                set
                {
                    m_Col = value;
                }
            }
        }

        public MemoryGame()
        {
            this.r_Players = new Player[k_NumOfPlayers];
            for (int i = 0; i < k_NumOfPlayers; i++)
            {
                r_Players[i] = new Player();
            }
        }

        public byte[] PossibleCardValues
        {
            get
            {
                return m_PossibleCardValues;
            }
        }

        public Card[,] Board
        {
            get
            {
                return m_Board;
            }
        }

        public string GetCurrentPlayerName()
        {
            return GetPlayerName(this.m_NumberOfCurrentlyPlayingPlayer + 1);
        }

        public string GetPlayerName(int i_PlayerNumber)
        {
            return this.r_Players[i_PlayerNumber - 1].Name;
        }

        public int GetPlayerScore(int i_PlayerNumber)
        {
            return this.r_Players[i_PlayerNumber - 1].Score;
        }

        public void InitializeHumanPlayer(string i_PlayerName, int i_PlayerNumber)
        {
            this.r_Players[i_PlayerNumber - 1].IsHuman = true;
            this.r_Players[i_PlayerNumber - 1].Name = i_PlayerName;
        }

        public bool SetBoardSize(int i_BoardHeight, int i_BoardWidth)
        {
            bool validBoardSize;

            validBoardSize = isValidBoardSize(i_BoardHeight) && isValidBoardSize(i_BoardWidth);
            if(validBoardSize)
            {
                this.m_Board = new Card[i_BoardHeight, i_BoardWidth];
                this.m_NumOfPairs = (byte)(i_BoardHeight * i_BoardWidth / 2);
            }

            return validBoardSize;
        }

        private bool isValidBoardSize(int i_BoardSize)
        {
            return i_BoardSize % 2 == 0;
        }

        private byte getBoardHeight()
        {
            return (byte)this.m_Board.GetLength(0);
        }

        private byte getBoardWidth()
        {
            return (byte)this.m_Board.GetLength(1);
        }

        public void InitializeBoard()
        {
            int row, col;

            for (int i = 0; i < this.m_NumOfPairs * 2; i++)
            {
                this.GetRandomCardLocation(out row, out col);
                while(this.m_Board[row, col].Value != 0)
                {
                    this.GetRandomCardLocation(out row, out col);
                }
                
                this.m_Board[row, col].Value = (ushort)(i % (this.m_NumOfPairs));
            }

            generatePossibleCardValues();
        }

        private void generatePossibleCardValues()
        {
            bool[] wasValueGenerated = new bool[k_NumOfCardValues];
            this.m_PossibleCardValues = new byte[this.m_NumOfPairs];

            for(int i = 0; i < this.m_NumOfPairs; i++)
            {
                this.getRandomCardValue(i);
                while (wasValueGenerated[this.m_PossibleCardValues[i]] == true)
                {
                    this.getRandomCardValue(i);
                }

                wasValueGenerated[this.m_PossibleCardValues[i]] = true;
            }
        }

        private void getRandomCardValue(int i_ValueIndex)
        {
            System.Random randomValueGenerator = new System.Random();

            this.m_PossibleCardValues[i_ValueIndex] = (byte)randomValueGenerator.Next(0, k_NumOfCardValues);
        }

        public void GetRandomCardLocation(out int o_Row, out int o_Col)
        {
            System.Random randomLocationGenerator = new System.Random();

            o_Row = (byte)randomLocationGenerator.Next(0, this.getBoardHeight());
            o_Col = (byte)randomLocationGenerator.Next(0, this.getBoardWidth());
        }

        public void TryToFlipCard(
            int i_ChosenRow,
            int i_ChosenColumn,
            ref bool io_CardLocationOutOfBounds,
            ref bool io_CardAlreadyFlipped)
        {
            if(!isCardOutOfBounds(i_ChosenRow, i_ChosenColumn))
            {
                io_CardLocationOutOfBounds = false;
                if(!this.Board[i_ChosenRow - 1, i_ChosenColumn - 1].IsValueVisible)
                {
                    io_CardAlreadyFlipped = false;
                    this.Board[i_ChosenRow - 1, i_ChosenColumn - 1].IsValueVisible = true;
                    saveFlippedCardLocation(i_ChosenRow, i_ChosenColumn);
                }
            }
        }

        private void saveFlippedCardLocation(int i_ChosenRow, int i_ChosenColumn)
        {
            if (this.m_NumOfFlippedCardsInCurrentTurn == k_NumOfCardsFlippedPerTurn)
            {
                this.m_NumOfFlippedCardsInCurrentTurn = 0;
            }

            this.r_CurrentTurnCardsLocations[this.m_NumOfFlippedCardsInCurrentTurn].Row = (byte)(i_ChosenRow - 1);
            this.r_CurrentTurnCardsLocations[this.m_NumOfFlippedCardsInCurrentTurn].Col = (byte)(i_ChosenColumn - 1);
            this.m_NumOfFlippedCardsInCurrentTurn++;
        }

        private bool isCardOutOfBounds(int i_ChosenRow, int i_ChosenColumn)
        {
            return (i_ChosenRow > this.getBoardHeight() || i_ChosenRow <= 0) ||
                   (i_ChosenColumn > this.getBoardWidth() || i_ChosenColumn <= 0);
        }

        public void CompareCardsAndHandleResult()
        {
            byte firstCardRow = this.r_CurrentTurnCardsLocations[0].Row;
            byte firstCardCol = this.r_CurrentTurnCardsLocations[0].Col;
            byte secondCardRow = this.r_CurrentTurnCardsLocations[1].Row;
            byte secondCardCol= this.r_CurrentTurnCardsLocations[1].Col;

            if(this.Board[firstCardRow, firstCardCol].Value == this.Board[secondCardRow, secondCardCol].Value)
            {
                this.r_Players[this.m_NumberOfCurrentlyPlayingPlayer].Score++;
            }
            else
            {
                this.m_NumberOfCurrentlyPlayingPlayer++;
                this.m_NumberOfCurrentlyPlayingPlayer %= (byte)k_NumOfPlayers;
                System.Threading.Thread.Sleep(2000);
                this.Board[firstCardRow, firstCardCol].IsValueVisible = false;
                this.Board[secondCardRow, secondCardCol].IsValueVisible = false;
            }
        }

        public bool IsPlayerHuman()
        {
            return this.r_Players[this.m_NumberOfCurrentlyPlayingPlayer].IsHuman;
        }

        public bool IsGameOver()
        {
            int sum = 0;

            for(int i = 0; i < k_NumOfPlayers; i++)
            {
                sum += this.r_Players[i].Score;
            }

            return sum == this.m_NumOfPairs;
        }
    }
}

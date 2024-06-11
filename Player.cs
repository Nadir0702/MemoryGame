namespace Ex02
{
    internal class Player
    {
        private string m_Name = "Computer";
        private bool m_IsHuman = false;
        private byte m_Score = 0;

        public byte Score
        {
            get
            {
                return m_Score;
            }
            set
            {
                m_Score = value;
            }
        }

        public bool IsHuman
        {
            get
            {
                return m_IsHuman;
            }
            set
            {
                m_IsHuman = value;
            }
        }

        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }
    }
}

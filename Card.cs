namespace Ex02
{
    internal struct Card
    {
        private bool m_IsValueVisible;
        private ushort m_Value;

        public bool IsValueVisible
        {
            get
            {
                return m_IsValueVisible;
            }
            set
            {
                m_IsValueVisible = value;
            }
        }

        public ushort Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value;
            }
        }
    }
}

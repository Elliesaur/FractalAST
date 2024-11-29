namespace FractalAST.FASTBuilders
{
    public class TokenReader<TToken>
        where TToken : BaseToken
    {
        private readonly IList<TToken> _tokens;
        private int _currentIndex = -1;

        public bool ThereAreMoreTokens => _currentIndex < _tokens.Count - 1;

        public int CurrentIndex => _currentIndex;

        public TokenReader(IList<TToken> tokens)
        {
            _tokens = tokens;
        }

        public TToken GetNext()
        {
            ThrowIfEnd();
            return _tokens[++_currentIndex];
        }


        public TToken PeekNext(int skip = 1)
        {
            ThrowIfEndForPeek(skip);
            return _tokens[_currentIndex + skip];
        }

        public TToken PeekPrevious()
        {
            ThrowIfStartForPeek();
            return _tokens[_currentIndex - 1];
        }

        public TToken PeekPreviousSafe()
        {
            try
            {
                return PeekPrevious();
            }
            catch (Exception)
            {
                return default;
            }
        }

        public TToken PeekNextSafe(int skip = 1)
        {
            try
            {
                return PeekNext(skip);
            }
            catch (Exception)
            {
                return default;
            }
        }

        private void ThrowIfEndForPeek(int skip = 1)
        {
            var weCanPeek = _currentIndex + skip < _tokens.Count;
            if (!weCanPeek)
                throw new Exception("Cannot peek past the end of tokens list");
        }

        private void ThrowIfStartForPeek()
        {
            var weCanPeek = _currentIndex - 1 >= 0;
            if (!weCanPeek)
                throw new Exception("Cannot peek past the start of tokens list");
        }
        private void ThrowIfEnd()
        {
            if (!ThereAreMoreTokens)
                throw new Exception("Cannot read past the end of tokens list");
        }

        public bool IsNextOfType(ExprType type)
        {
            return PeekNext().Type == type;
        }

        public TToken GetCurrent()
        {
            return _tokens[_currentIndex];
        }
    }
}

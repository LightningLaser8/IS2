namespace ISL.Language.Expressions.Combined
{
    internal class BracketType(char open, char close, Func<List<Expression>, Expression> creator)
    {
        public char Open => open;
        public char Close => close;
        public Func<List<Expression>, Expression> Create { get; } = creator;
    }
}

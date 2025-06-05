namespace ISL.Interpreter
{
    public static class IslDebugOutput
    {
        private static string _msg = "";
        public static void Debug(string message)
        {
            _msg += message + "\n";
        }
        public static void Reset()
        {
            _msg = "";
        }
        public static string Message => _msg;
    }
}

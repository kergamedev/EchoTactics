namespace Echo.Common
{
    public static class Global
    {
        public static IGame Game;
        public static IConsole Console;
        public static IHome Home;
        public static IMatch Match;

        public static void Reset()
        {
            Game = null;
            Home = null;
            Match = null;
        }
    }
}
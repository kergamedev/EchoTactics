namespace Echo.Common
{
    public static class Global
    {
        public static IGame Game;
        public static IHome Home;

        public static void Reset()
        {
            Game = null;
            Home = null;
        }
    }
}
namespace MiniORM
{
    public class DbContext
    {
        public static Type[] AllowedSqlTypes =
        {
            typeof(string),
            typeof(DateTime),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(decimal),
            typeof(bool)
        };


    }
}

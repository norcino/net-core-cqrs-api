namespace Common.IoC
{
    public static class Constants
    {
        public static string ConfigConnectionStringName => "HouseKeeper";
    }

    public enum DatabaseType
    {
        SQLServer,
        SQLiteInMemory
    }
}

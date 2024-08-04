namespace BehaviorDesignPattern.StrategyPattern.Models;

public class Settings
{
    public static string claimDatabaseType = "databasetype";

    public DatabaseType DatabaseType { get; set; }
    
    public DatabaseType  GetDefaultDatabaseType => DatabaseType.SqlServer;
    
    
}
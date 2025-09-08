using Microsoft.EntityFrameworkCore;

namespace RBZ.Projekt.Database;

public class SQLiteContext : DbContext 
{
    public SQLiteContext(DbContextOptions options) : base(options)
    {

    }
}

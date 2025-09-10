using Microsoft.EntityFrameworkCore;

public static class DbSetExtension
{
    public static bool TryGet<T>(this DbSet<T> dbSet, int id, out T? value) where T : class
    {
        value = dbSet.Find(id);
        return value is not null;
    }


}

using RBZ.Projekt.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<SQLiteContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddDbContext<OldDbContext>(options =>
    options.UseSqlite("Data Source=old.db"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var newContext = scope.ServiceProvider.GetRequiredService<SQLiteContext>();
    var oldContext = scope.ServiceProvider.GetRequiredService<OldDbContext>();

    // import SQL into OldDb
    var sql = File.ReadAllText("../../../Data/movies.sql");

    sql = sql.Replace(
    "INSERT INTO MovieGenres",
    "INSERT OR IGNORE INTO MovieGenres"
    );

    oldContext.Database.EnsureDeleted();
    oldContext.Database.ExecuteSqlRaw(sql);

    newContext.Database.EnsureDeleted();
    newContext.Database.EnsureCreated();

    //migrate from old to new DB
    var migrator = new dbMigration(newContext, oldContext);
    migrator.translateOldDbDataToNew();

    // import JSON, CSV, XML files 
    var collector = new DataCollector(newContext);
    collector.collectAndValidateImports();
}



// Configure the HTTP request pipeline.


app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();


//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

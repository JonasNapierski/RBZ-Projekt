using System.IO;
using System.Xml;
using Microsoft.EntityFrameworkCore;
using RBZ.Projekt.Database;
using RBZ.Projekt.Models;
using Xunit;

namespace API.Tests;

public class DataCollectorTests
{
    private SQLiteContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<SQLiteContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new SQLiteContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    private string SetupTempDataFolder()
    {
        string folder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(folder);
        return folder + Path.DirectorySeparatorChar; // ensures trailing slash
    }

    [Fact]
    public void ImportJSON_InsertsMovieAndActor()
    {
        using var db = CreateInMemoryDb();
        string folder = SetupTempDataFolder();

        // prepare JSON
        string json = @"[
          {
            ""id"": ""m1"",
            ""title"": ""Inception"",
            ""year"": 2010,
            ""country"": ""USA"",
            ""cast"": [
              { ""name"": ""Leonardo DiCaprio"", ""role"": ""Cobb"" }
            ],
            ""ratings"": { ""IMDB"": 8.8 }
          }
        ]";
        File.WriteAllText(folder + "movies.json", json);
        File.WriteAllText(folder + "finances.csv", "id;year;budget;revDom;revInt;curr\n"); // empty header
        File.WriteAllText(folder + "festivals.xml", "<festivals></festivals>");

        var collector = new DataCollector(db, folder);
        collector.collectAndValidateImports();

        Assert.Single(db.Movies);
        Assert.Single(db.Actors);
        Assert.Single(db.Roles);
        Assert.Single(db.Ratings);
    }

    [Fact]
    public void ImportCSV_UpdatesMovieFinancials()
    {
        using var db = CreateInMemoryDb();
        string folder = SetupTempDataFolder();

        // seed movie
        db.Movies.Add(new Movie { MovieId = 1, Title = "Inception", Year = 2010 });
        db.SaveChanges();

        // prepare CSV
        File.WriteAllText(folder + "movies.json", "[]");
        File.WriteAllText(folder + "festivals.xml", "<festivals></festivals>");
        File.WriteAllText(folder + "finances.csv", "id;year;budget;revDom;revInt;curr\nm1;2010;100000;200000;300000;$");

        var collector = new DataCollector(db, folder);
        collector.collectAndValidateImports();

        var movie = db.Movies.First();
        Assert.Equal(100000, movie.Budget);
        Assert.Equal(200000, movie.RevenueDomestic);
        Assert.Equal(300000, movie.RevenueInternational);
        Assert.Single(db.Currencies);
    }

    [Fact]
    public void ImportXML_InsertsFestivalAndMovieFestival()
    {
        using var db = CreateInMemoryDb();
        string folder = SetupTempDataFolder();

        // seed movie
        db.Movies.Add(new Movie { MovieId = 1, Title = "Inception", Year = 2010 });
        db.SaveChanges();

        // prepare XML
        string xml = @"<festivals>
          <festival name='Cannes' year='2023' location='France'>
            <movie id='m1' category='Competition' status='Winner'/>
          </festival>
        </festivals>";
        File.WriteAllText(folder + "festivals.xml", xml);
        File.WriteAllText(folder + "movies.json", "[]");
        File.WriteAllText(folder + "finances.csv", "id;year;budget;revDom;revInt;curr\n");

        var collector = new DataCollector(db, folder);
        collector.collectAndValidateImports();

        Assert.Single(db.Festivals);
        Assert.Single(db.MovieFestivals);
        Assert.Single(db.Categories);
        Assert.Single(db.CategoryStatus);
    }

    [Fact]
    public void ImportJSON_DoesNotDuplicateMovie()
    {
        using var db = CreateInMemoryDb();
        string folder = SetupTempDataFolder();

        db.Movies.Add(new Movie { MovieId = 1, Title = "Inception", Year = 2010 });
        db.SaveChanges();

        string json = @"[
          { ""id"": ""m1"", ""title"": ""Inception"", ""year"": 2010,
            ""country"": ""USA"", ""cast"": [], ""ratings"": {} }
        ]";
        File.WriteAllText(folder + "movies.json", json);
        File.WriteAllText(folder + "finances.csv", "id;year;budget;revDom;revInt;curr\n");
        File.WriteAllText(folder + "festivals.xml", "<festivals></festivals>");

        var collector = new DataCollector(db, folder);
        collector.collectAndValidateImports();

        Assert.Single(db.Movies); // still one
    }
}


using System.Collections.Generic;
using System.Xml;
using RBZ.Projekt.Models;
using System.IO;
using Microsoft.VisualBasic;
using System.Linq;
using System;

public class DataCollector
{
    private readonly SQLiteContext _context;

    public DataCollector(SQLiteContext context)
    {
        _context = context;
    }

    public void collectAndValidateImports()
    {   
        importJSONData();
        importCSVData();
        importXMLData();
    }

    private void importXMLData()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load("../Data/festivals.xml");

        foreach (XmlNode festivalNode in doc.DocumentElement.ChildNodes)
        {
            string festivalName = festivalNode.Attributes["name"].Value;
            int year = int.Parse(festivalNode.Attributes["year"].Value);
            string location = festivalNode.Attributes["location"].Value;

            var festival = _context.Festivals
                .FirstOrDefault(f => f.Name == festivalName && f.Year == year && f.Location == location);

            if (festival == null)
            {
                festival = new Festival
                {
                    Name = festivalName,
                    Year = year,
                    Location = location
                };
                _context.Festivals.Add(festival);
                _context.SaveChanges();
            }

            foreach (XmlNode movieNode in festivalNode.ChildNodes)
            {
                int movieId = convertStringIdToInt(movieNode.Attributes["id"].Value);
                string categoryName = movieNode.Attributes["category"].Value;
                string statusName = movieNode.Attributes["status"].Value;

                var movie = _context.Movies.FirstOrDefault(m => m.MovieId == movieId);
                if (movie == null)
                {
                    Console.WriteLine("XML import: movie id " + movieId + "not found in DB ");
                    continue;
                }

                var category = _context.Categories.FirstOrDefault(c => c.Name == categoryName);
                if (category == null)
                {
                    category = new Category { Name = categoryName };
                    _context.Categories.Add(category);
                    _context.SaveChanges();
                }

                var status = _context.CategoryStatuses.FirstOrDefault(s => s.Name == statusName);
                if (status == null)
                {
                    status = new CategoryStatus { Name = statusName };
                    _context.CategoryStatuses.Add(status);
                    _context.SaveChanges();
                }

                if (!_context.MovieFestivals.Any(mf => mf.MovieId == movie.MovieId && mf.FestivalId == festival.Id))
                {
                    var movieFestival = new MovieFestival
                    {
                        Movie = movie,
                        Festival = festival,
                        Category = category,
                        CategoryStatus = status
                    };
                    _context.MovieFestivals.Add(movieFestival);
                }
            }
        }
        _context.SaveChanges();
    }

    private void importCSVData()
    {
        using (var reader = new StreamReader("../Data/finances.csv"))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');

                int refinedId = convertStringIdToInt(values[0]);
                int year = int.Parse(values[1]);
                long? budget = values[2].ToUpper() == "NULL" ? (long?)null : long.Parse(values[2]);
                long? revenueDomestic = values[3].ToUpper() == "NULL" ? (long?)null : long.Parse(values[3]);
                long? revenueInternational = values[4].ToUpper() == "NULL" ? (long?)null : long.Parse(values[4]);
                string currencySymbol = values[5];

                var movie = _context.Movies.FirstOrDefault(m => m.MovieId == refinedId);
                if (movie == null) continue;

                var dbCurrency = _context.Currencies.FirstOrDefault(c => c.Symbol == currencySymbol);
                if (dbCurrency == null)
                {
                    dbCurrency = new Currency { Symbol = currencySymbol };
                    _context.Currencies.Add(dbCurrency);
                }

                movie.Budget = budget;
                movie.RevenueDomestic = revenueDomestic;
                movie.RevenueInternational = revenueInternational;

                if (movie.Country != null && movie.Country.Currency == null)
                {
                    movie.Country.Currency = dbCurrency;
                }
            }

            _context.SaveChanges();
        }
    }

    private void importJSONData()
    {
        List<Item> items;

        using (StreamReader r = new StreamReader("../Data/movies.json"))
        {
            string json = r.ReadToEnd();
            items = JsonConvert.DeserializeObject<List<Item>>(json);
        }

        foreach (var item in items)
        {
            int refinedId = ConvertStringIdToInt(item.id.ToString());

            var movie = _context.Movies
                .FirstOrDefault(m => m.MovieId == refinedId && m.Title == item.title);

            Country country = null;
            if (!string.IsNullOrEmpty(item.country))
            {
                country = _context.Country.FirstOrDefault(c => c.Name == item.country);
                if (country == null)
                {
                    country = new Country { Name = item.country };
                    _context.Country.Add(country);
                    _context.SaveChanges();
                }
            }

            if (movie == null)
            {
                movie = new Movie
                {
                    MovieId = refinedId,
                    Title = item.title,
                    Year = item.year,
                    Country = country
                };
                _context.Movies.Add(movie);
            }

            // cast
            foreach (var castMember in item.cast)
            {
                string[] parts = castMember["name"].Split(" ");
                string firstName = parts[0];
                string lastName = parts[1];

                string roleName = castMember["role"];

                var actor = _context.Actors
                    .FirstOrDefault(a => a.FirstName == firstName && a.LastName == lastName);
                if (actor == null)
                {
                    actor = new Actor { FirstName = firstName, LastName = lastName };
                    _context.Actors.Add(actor);
                }

                var role = _context.Roles.FirstOrDefault(r => r.Name == roleName);
                if (role == null)
                {
                    role = new Role { Name = roleName };
                    _context.Roles.Add(role);
                }

                if (!_context.MovieActors.Any(ma =>
                    ma.MovieId == movie.MovieId &&
                    ma.ActorId == actor.Id &&
                    ma.RoleId == role.Id))
                {
                    var movieActor = new MovieActor
                    {
                        Movie = movie,
                        Actor = actor,
                        Role = role
                    };
                    _context.MovieActors.Add(movieActor);
                }
            }

            // ratings
            foreach (var pair in item.ratings)
            {
                string source = pair.Key;
                double value = pair.Value;

                var ratingInstitution = _context.RatingInstitution.FirstOrDefault(ri => ri.Name == source);
                if (ratingInstitution == null)
                {
                    ratingInstitution = new RatingInstitution { Name = source };
                    _context.RatingInstitution.Add(ratingInstitution);
                    _context.SaveChanges();
                }

                if (!_context.Rating.Any(r =>
                    r.MovieId == movie.MovieId &&
                    r.RatingInstitutionId == ratingInstitution.Id))
                {
                    var rating = new Rating
                    {
                        Movie = movie,
                        RatingInstitution = ratingInstitution,
                        Value = value
                    };
                    _context.MovieRatings.Add(rating);
                }
            }
        }
        _context.SaveChanges();
    }

    public class Item
    {
        public int id;
        public string title;
        public int year;
        public List<Dictionary<string, string>> cast;
        public Dictionary<string, double> ratings;
        public string country;
    }

    private int convertStringIdToInt(string str)
    {
        string numberPart = str.Substring(1);
        return int.Parse(numberPart);
    }

}

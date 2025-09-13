using System.Xml;
using RBZ.Projekt.Models;
using RBZ.Projekt.Database;
using Newtonsoft.Json;

public class DataCollector
{
    private readonly SQLiteContext _context;
    private readonly string _base_data_folder;

    public DataCollector(SQLiteContext context, string data_folder)
    {
        _context = context;
        _base_data_folder = data_folder;
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
        doc.Load(_base_data_folder + "festivals.xml");

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
                    Location = location,
                };
                _context.Festivals.Add(festival);
                _context.SaveChanges();
            }

            foreach (XmlNode movieNode in festivalNode.ChildNodes)
            {
                var idAttr = movieNode.Attributes["id"];
                if (idAttr == null || string.IsNullOrEmpty(idAttr.Value))
                {
                    continue;
                }
                int movieId = convertStringIdToInt(idAttr.Value);
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

                var status = _context.CategoryStatus.FirstOrDefault(s => s.Name == statusName);
                if (status == null)
                {
                    status = new CategoryStatus { Name = statusName };
                    _context.CategoryStatus.Add(status);
                    _context.SaveChanges();
                }

                if (!_context.MovieFestivals.Any(mf => mf.Id == movie.MovieId && mf.Id == festival.Id))
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
        using (var reader = new StreamReader(_base_data_folder + "finances.csv"))
        {
            if (!reader.EndOfStream) reader.ReadLine();

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

                // Currency holen oder neu anlegen
                var dbCurrency = _context.Currencies.FirstOrDefault(c => c.Symbol == currencySymbol);
                if (dbCurrency == null)
                {
                    dbCurrency = new Currency { Symbol = currencySymbol };
                    _context.Currencies.Add(dbCurrency);
                    _context.SaveChanges();
                }

                movie.Budget = budget;
                movie.RevenueDomestic = revenueDomestic;
                movie.RevenueInternational = revenueInternational;

                // Country mit Currency verbinden
                if (movie.Country != null)
                {
                    if (movie.Country.CurrencyId == null)
                    {
                        // Erstzuweisung: Currency setzen
                        movie.Country.Currency = dbCurrency;
                    }
                    else if (movie.Country.CurrencyId != dbCurrency.Id)
                    {
                        Console.WriteLine($"Währungs-Konflikt für Country={movie.Country.Name}: "
                            + $"bereits {movie.Country.Currency.Symbol}, neuer Wert wäre {dbCurrency.Symbol}");
                    }
                }
            }

            _context.SaveChanges();
        }
    }

    private void importJSONData()
    {
        List<Item> items;

        using (StreamReader r = new StreamReader(_base_data_folder + "movies.json"))
        {
            string json = r.ReadToEnd();
            items = JsonConvert.DeserializeObject<List<Item>>(json) ?? new List<Item>();
        }

        var currencyDict = new Dictionary<string, string>();
        foreach (var item in items)
        {
            int refinedId = convertStringIdToInt(item.id.ToString());

            var movie = _context.Movies
                .FirstOrDefault(m => m.MovieId == refinedId && m.Title == item.title);

            Country? country = null;
            if (!string.IsNullOrEmpty(item.production_country))
            {
                string normalizedName = _countryNormalization.ContainsKey(item.production_country)
                    ? _countryNormalization[item.production_country]
                    : item.production_country;

                country = _context.Countries.FirstOrDefault(c => c.Name == normalizedName);
                if (country == null)
                {
                    country = new Country { Name = normalizedName };

                    if (_countryCurrencyMap.TryGetValue(normalizedName, out var currencySymbol))
                    {
                        var dbCurrency = _context.Currencies.FirstOrDefault(c => c.Symbol == currencySymbol);
                        if (dbCurrency == null)
                        {
                            dbCurrency = new Currency { Symbol = currencySymbol };
                            _context.Currencies.Add(dbCurrency);
                            _context.SaveChanges();
                        }

                        country.Currency = dbCurrency;
                    }

                    _context.Countries.Add(country);
                    _context.SaveChanges();
                }
            }

            if (movie == null)
            {
                movie = new RBZ.Projekt.Models.Movie
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
                    _context.SaveChanges();
                }

                var role = _context.Roles.FirstOrDefault(r => r.Name == roleName);
                if (role == null)
                {
                    role = new Role { Name = roleName };
                    _context.Roles.Add(role);
                    _context.SaveChanges();
                }
                if (!_context.ChangeTracker.Entries<MovieActor>()
                    .Any(e => e.Entity.MovieId == movie.MovieId &&
                    e.Entity.ActorId == actor.Id &&
                    e.Entity.RoleId == role.Id))
                {
                    var movieActor = new MovieActor
                    {
                        Movie = movie,
                        Actor= actor,
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

                var ratingInstitution = _context.RatingInstitutions.FirstOrDefault(ri => ri.Name == source);
                if (ratingInstitution == null)
                {
                    ratingInstitution = new RatingInstitution { Name = source };
                    _context.RatingInstitutions.Add(ratingInstitution);
                }

                if (!_context.Ratings.Any(r =>
                    r.Movie.MovieId == movie.MovieId &&
                    r.RatingInstitution.Id == ratingInstitution.Id))
                {
                    var rating = new Rating
                    {
                        Movie = movie,
                        RatingInstitution = ratingInstitution,
                        Value = value
                    };
                    _context.Ratings.Add(rating);
                    _context.SaveChanges();
                }
            }
        }
        _context.SaveChanges();
    }

    public class Item
    {
        public string id;
        public string title;
        public int year;
        public List<Dictionary<string, string>> cast;
        public Dictionary<string, double> ratings;
        public string production_country;
    }

    private int convertStringIdToInt(string str)
    {
        string numberPart = str.Substring(1);
        return int.Parse(numberPart);
    }

    private static readonly Dictionary<string, string> _countryNormalization = new()
    {
        { "UK", "United Kingdom" },
        { "United Kingdom", "United Kingdom" },
        { "USA", "United States" },
        { "United States", "United States" },
        { "États-Unis", "United States" },
        { "Deutschland", "Germany" },
        { "Germany", "Germany" },

    };

    private static readonly Dictionary<string, string> _countryCurrencyMap = new()
{
    { "Germany", "EUR" },
    { "United Kingdom", "GBP" },
    { "France", "EUR" },
    { "United States", "USD" }
};
}

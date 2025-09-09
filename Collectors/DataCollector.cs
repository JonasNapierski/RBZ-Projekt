using System.Collections.Generic;
using System.Xml;
using RBZ.Projekt.Models;
using System.IO;
using Microsoft.VisualBasic;

public class DataCollector
{
    private readonly SQLiteContext _context;

    public DataCollector(SQLiteContext context)
    {
        _context = context;
    }

    public void collectAndValidateImports()
    {
        List<Festival> movies = importXMLData();

    }

    private List<Festival> importXMLData()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load("../Data/festivals.xml");

        List <Festival> festivals = new List<Festival>();

        foreach(XmlNode festivalNode in doc.DocumentElement.ChildNodes){
           string name = festivalNode.Attribute["name"].Value;
           int year = int.Parse(festivalNode.Attribute["year"].Value;
           string location = festivalNode.Attribute["location"].Value;
            List<Movie> movies = new List<Movie>();

           Festival festival = new Festival(year, name, location, movies);
           
           foreach(XmlNode movieNode in festivalNode.ChildNodes)
           {
                string id = movieNode.Attribute["id"].Value;
                string category = movieNode.Attribute["category"].Value;
                string status = movieNode.Attribute["status"].Value;
                string name = movieNode.InnerText;
                
                Movie movie = new Movie(id, category, status, name);

                festival.movies.Add(movie);
           }
            festivals.Add(festival);
        }
        return festivals;
    }

    public List<string> importCSVData()
    {
        using (var reader = new StreamReader("../Data/finances.csv"))
        {
            List<string> listA = new List<string>();
            List<string> listB = new List<string>();
           
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');

                string movieId = values[0];
                int year = int.Parse(values[1]);
                int? budget = values[2].ToUpper()=="NULL" ? (int?)null : int.Parse(values[2]);
                int? revenue_domestic = values[3].ToUpper() == "NULL" ? (int?)null : int.Parse(values[3]);
                int? revenue_international = values[4].ToUpper() == "NULL" ? (int?)null : int.Parse(values[4]);
                string currency = values[5];

                var movie = _context.Movie.FirstOrDefault(m => m.Id == movieId && m.Year == year);

                if (movie != null) 
                {
                    _context.Movie.Budget.Add(budget);
                    _context.Movie.Revenue_Domestic.Add(revenue_domestic);
                    _context.Movie.Revenue_International.Add(revenue_international);

                }

            }
        }
        
    }
}

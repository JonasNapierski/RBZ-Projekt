using System.Collections.Generic;
using System.Xml;

public class xmlCollector
{
    private readonly SQLiteContext _context;

    public XmlCollector(SQLiteContext context)
    {
        _context = context;
    }

    public void importData()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load("..\\Data\\festivals.xml");

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
                
                //todo: search for movie in DbContext bevore creating new Movie.
                //      are movies even saved with only .SaveChanges()?
                Movie movie = new Movie(id, category, status, name);

                festival.movies.Add(movie);
           }
            _context.Festival.Add(festival);
        }
        _context.SaveChanges();
    }
}

using System.Xml;

public class xmlCollector
{
    public override importData()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load("..\\Data\\festivals.xml");

        var festivals
        foreach(XmlNode festivalNode in doc.DocumentElement.ChildNodes){
           string name = festivalNode.Attribute["name"].Value;
           int year = festivalNode.Attribute["year"].Value;
           string location = festivalNode.Attribute["location"].Value;
           new Festival()

           foreach(XmlNode movieNode in festivalNode.ChildNodes)
           {
                string id = movieNode.Attribute["id"].Value
                string category = movieNode.Attribute["category"].Value
                string status = movieNode.Attribute["status"].Value
                string name
           }
        }
    }
}

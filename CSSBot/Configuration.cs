using System.Xml.Serialization;

namespace CSSBot
{
    /// <summary>
    /// This class is used to store configuration data
    /// That can be loaded (or reloaded, if needed)
    /// from some XML file. It is also possible to use JSON in a very similar manner.
    /// </summary>
    [XmlRoot("Configuration")]
    public class Configuration
    {
        [XmlElement("ConnectionToken")]
        public string ConnectionToken { get; set; }
    }
}

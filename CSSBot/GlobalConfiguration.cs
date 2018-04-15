using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace CSSBot
{
    /// <summary>
    /// This class should be used to contain data that is loaded
    /// from a file for configuration purposes, like the bot token.
    /// Also used for storing constants (CommandPrefix)
    /// </summary>
    public class GlobalConfiguration
    {
        /// <summary>
        /// What prefix should commands begin with?
        /// A good command prefix doesn't get triggered
        /// accidentally by normal conversation, and 
        /// is somewhat unique to other bots.
        /// </summary>
        public const char CommandPrefix = '?';

        // the path to the configuration file
        // that will be read and parsed
        private string m_ConfigFilePath = null;

        // our configuration data, loaded from xml file
        public Configuration Data { get; private set; }

        /// <summary>
        /// Constructor for GlobalConfiguration class
        /// </summary>
        /// <param name="path">The path to the file that will be opened and parsed</param>
        public GlobalConfiguration(string path)
        {
            // set the path
            SetConfigurationFilePath(path);
            // load from it
            LoadConfiguration();
        }

        /// <summary>
        /// Sets the Configuration File Path
        /// Validates that the path was not empty or invalid
        /// </summary>
        /// <param name="path"></param>
        private void SetConfigurationFilePath(string path)
        {
            if(string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("The path supplied was null or whitespace!");
            }

            m_ConfigFilePath = path;
        }

        /// <summary>
        /// Default constructor should be private
        /// </summary>
        private GlobalConfiguration() { }

        /// <summary>
        /// Loads (or Reloads) the Configuration file
        /// </summary>
        /// <param name="path"></param>
        public void LoadConfiguration()
        {
            // ensure that the config file path is valid
            if (string.IsNullOrWhiteSpace(m_ConfigFilePath))
            {
                throw new Exception("The configuration file path has not been set!");
            }

            // create a XML Serializer object
            var ser = new XmlSerializer(typeof(Configuration));

            // open the file at the given path
            using (var fs = new FileStream(m_ConfigFilePath, FileMode.Open))
            {
                // try to deserialize the contents of the file stream and hard-cast it to a Configuration type
                Data = (Configuration)ser.Deserialize(fs);
            }
        }
    }
}

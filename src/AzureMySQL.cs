using System;
using System.Collections.Generic;
using System.Linq;

namespace kedzior.io.ConnectionStringConverter
{
    public static class AzureMySQL
    {
        private static string DEFAULT_MYSQL_PORT = "3306";
        /// <summary>
        /// Converts non-standard MySQL Connection String used in Azure (MYSQLCONNSTR_localdb) to standard one.
        /// Non-standard: "Database={0};Data Source={1}:{2};User Id=30};Password={4}".
        /// Standard: "Database={0};Server={1}:{2};Uid=30};Pwd={4}".
        /// </summary>
        /// <param name="str">Non-standard connection string.</param>
        /// <returns>Standard connection string.</returns>
        public static string ToMySQLStandard(this string str)
        {
            if (string.IsNullOrEmpty(str))
                throw new ArgumentException("Connection String is empty.");

            string[] t = str.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, string> dictionary = t.Select(item => item.Split('=')).ToDictionary(s => s[0], s => s[1]);

            dictionary.ExtractPort("Data Source", ":", "Port");

            dictionary.ReplaceKey("Data Source", "Server");
            dictionary.ReplaceKey("User Id", "Uid");
            dictionary.ReplaceKey("Password", "Pwd");

            return dictionary.ToString("=", ";");

        }
        /// <summary>
        /// Extracts port number from "Data Source" section and creates individual section.
        /// </summary>
        /// <param name="source">List of sections.</param>
        /// <param name="serverKey">Data Source" key.</param>
        /// <param name="portSeparator">Character used to separate hostname from port.</param>
        /// <param name="portKey">Port key used for the new entry.</param>
        /// <returns>Modified dictionary</returns>
        private static Dictionary<string, string> ExtractPort(this Dictionary<string, string> source, string serverKey, string portSeparator, string portKey)
        {
            if (source == null)
                throw new ArgumentException("Parameter source can not be null.");

            if (source.ContainsKey(serverKey))
            {
                string portNumber = source[serverKey].Substring(source[serverKey].LastIndexOf(portSeparator) + 1);
                if (portNumber != source[serverKey])
                {
                    source.Add(portKey, portNumber);
                    source[serverKey] = source[serverKey].Replace(portSeparator + portNumber, "");
                }
            }

            if (!source.ContainsKey(portKey))
            {
                source.Add(portKey, DEFAULT_MYSQL_PORT);
            }

            return source;
        }
        /// <summary>
        /// Replaces dictionary key.
        /// </summary>
        /// <param name="source">Dictionary.</param>
        /// <param name="oldKey">Key to be replaced.</param>
        /// <param name="newKey">New key.</param>
        /// <returns>Modified dictionary.</returns>
        private static Dictionary<string, string> ReplaceKey(this Dictionary<string, string> source, string oldKey, string newKey)
        {
            if (source == null)
                throw new ArgumentException("Parameter source can not be null.");

            if (source.ContainsKey(oldKey))
            {
                source.Add(newKey, source[oldKey]);
                source.Remove(oldKey);
            }

            return source;
        }
        /// <summary>
        /// Joins keys and values of dictionary with separator.
        /// </summary>
        /// <param name="source">Dictionary.</param>
        /// <param name="keyValueSeparator">Separator between key and value of dictionary.</param>
        /// <param name="sequenceSeparator">Separator between dictionary elements.</param>
        /// <returns>Dictionary as string</returns>
        private static string ToString(this Dictionary<string, string> source, string keyValueSeparator, string sequenceSeparator)
        {
            if (source == null)
                throw new ArgumentException("Parameter source can not be null.");

            var pairs = source.OrderBy(x => x.Key).Select(x => string.Format("{0}{1}{2}", x.Key, keyValueSeparator, x.Value));

            return string.Join(sequenceSeparator, pairs);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using testTreeView.Model;

namespace testTreeView
{
    class fileLoader
    {
        private string _path = "";

        public fileLoader(string path)
        {
            this._path = path;
        }

        public List<NodeContext> GetContexts()
        {
            List<NodeContext> list = new List<NodeContext>();

            string result = "";
            using (StreamReader sr = new StreamReader(_path))
            {
                result = sr.ReadToEnd();
            }

            //Regex regex = new Regex(@".*""(.*)""\r\n.*""(.*)""\r\n.*""(.*)""");
            Regex regex = new Regex(".*\"(?<msgctxt>.*)\"\r\n.*\"(?<msgid>.*)\"\r\n.*\"(?<msgstr>.*)\"|.*\"(?<msgid>.*)\"\r\n.*\"(?<msgstr>.*)\"");
            MatchCollection matches = regex.Matches(result);
          
            foreach (Match match in matches)
            {

                NodeContext obj = new NodeContext() { 
                    context = match.Groups["msgctxt"].Success ? match.Groups["msgctxt"].Value.Split('.') : new string[0],
                    id = match.Groups["msgid"].Value,
                    str = match.Groups["msgstr"].Value };
                list.Add(obj);
            }

            return list;
        }

    }
}

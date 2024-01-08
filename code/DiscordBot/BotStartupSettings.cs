using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tortoise
{
    public class BotStartupSettingsJson
    {
        public string token = "Not Set";
        public string secret = "Not Set";
    }

    public class BotStartupSettings
    {
        public BotStartupSettingsJson json = new BotStartupSettingsJson();

        public bool ReadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }
            string contents = File.ReadAllText(filePath);
            BotStartupSettingsJson? botStartupSettingsJson = JsonConvert.DeserializeObject<BotStartupSettingsJson>(contents);
            if (botStartupSettingsJson is null)
            {
                return false;
            }
            json = botStartupSettingsJson;
            return true;
        }

        public void WriteToFile(string filePath)
        {
            string contents = JsonConvert.SerializeObject(json);
            File.WriteAllText(filePath, contents);
        }
    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TortoiseDiscordBot.code.CommandLineOptions;

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

        public bool Initialize(string filePath)
        {
            CommandLineOptions options = CommandLineOptionsManager.GetOptions();
            if (options.BotToken != null && options.BotSecret != null)
            {
                //Running using startup lines
                json.token = options.BotToken;
                json.secret = options.BotSecret;
                return true;
            }
            else
            {
                return ReadFromFile(filePath);
            }
        }

        public bool ReadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Logger.WriteLine_Error($"Failed to create BotStartupSettings because filepath does not exist '{filePath}'");
                return false;
            }
            string contents = File.ReadAllText(filePath);
            BotStartupSettingsJson? botStartupSettingsJson = JsonConvert.DeserializeObject<BotStartupSettingsJson>(contents);
            if (botStartupSettingsJson is null)
            {
                Logger.WriteLine_Error($"Failed to parse BotStartupSettings from filepath '{filePath}'");
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

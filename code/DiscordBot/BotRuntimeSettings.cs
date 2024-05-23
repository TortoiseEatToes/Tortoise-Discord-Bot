using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tortoise
{
    public class BotSettingsJson
    {
        public BotSettingsJson()
        {
            tortoiseBotSettings = new TortoiseBotSettings();
        }
        public List<string> onMessageReceivedHandlers = new List<string>();
        public List<string> onReactionAddedHandlers = new List<string>();
        public List<string> onReactionRemovedHandlers = new List<string>();
        public TortoiseBotSettings tortoiseBotSettings { get; set; }
    }

    public class TortoiseBotSettings
    {
        public string saveFilePath = "Not Set";
        public string messageRecievedCharacter = "Not Set";
        public ulong server_id_tortoise_jams = 0;
        public ulong logsChannel = 0;
        public ulong debugChannel = 0;
        public ulong defaultOutputChannel = 0;
        public ulong reactChannel = 0;
        public ulong reactMessage = 0;
        public ulong botOperator = 0;
        public string unityExecutableRoot = "Not Set";
        public Dictionary<string, ulong> reactRoles = new Dictionary<string, ulong>();
        public Dictionary<string, string> unityProjects = new Dictionary<string, string>();
    }

    public class BotRuntimeSettings
    {
        public BotSettingsJson json = new BotSettingsJson();

        public bool ReadFromFile(string filePath)
        {
            if(!File.Exists(filePath))
            {
                Logger.WriteLine_Error($"Failed to create BotRuntimeSettings because filepath does not exist '{filePath}'");
                return false;
            }
            string contents = File.ReadAllText(filePath);
            BotSettingsJson? botStartupSettingsJson = JsonConvert.DeserializeObject<BotSettingsJson>(contents);
            if (botStartupSettingsJson is null)
            {
                Logger.WriteLine_Error($"Failed to parse BotRuntimeSettings from filepath '{filePath}'");
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

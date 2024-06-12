using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTSBot;

[System.Serializable]
public class Commands
{

    public enum UserLevel { USER, VIP, MOD, STREAMER, ADMIN };

    public readonly string name;

    public UserLevel privilageLevel = UserLevel.USER;

    public string ttsComparisonCommand = "";

    public bool enabled = true;

    public string usage = "";

    public string description = "";

    public Commands(string name = "", UserLevel privilageLevel = UserLevel.USER, string ttsComparisonCommand = "", bool enabled = true, string usage = "", string description = "")
    {

        this.name = name;
        this.privilageLevel = privilageLevel;
        this.ttsComparisonCommand = ttsComparisonCommand;
        this.enabled = enabled;
        this.usage = usage;
        this.description = description;

    }


}
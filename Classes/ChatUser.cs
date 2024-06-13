using System;

namespace TTSBot;

[System.Serializable]
public class ChatUser{

    public string name { get; set; }
    public string? alias { get; set; }
    public int voiceNumber {get; set;}
	public string voiceName { get; set; }
    public float voiceRate { get; set; }
    public bool ignored {get; set;}
    public string origin { get; set; }

    public ChatUser(string name, string alias = null, int voiceNumber = 0, string voiceName = "", float voiceRate = (float)200.0, bool ignored = false, string origin = ""){

        this.name = name.ToLower();
        // set alias to name if not present
        if (alias == null)
        {alias = name;}
        this.alias = alias;
        this.voiceNumber = voiceNumber;
        this.voiceName = voiceName;
        this.voiceRate = voiceRate;
        this.ignored = ignored;
        this.origin = origin;

    }

    public override string ToString()
    {
        return String.Format("userName {0}, alias {1}, voiceNumber {2}, voiceName {3}, voiceRate {4}, ignored {5}, origin {6}",name, alias, voiceNumber.ToString(),voiceName, voiceRate.ToString(), ignored.ToString(), origin);
    }



}
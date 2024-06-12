using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Security.Cryptography;
using PusherClient;
using Newtonsoft.Json;

namespace TTSBot;

[System.Serializable]
public class Settings
    {

        private const string passphrase = "Sup3rS3curePass!";
        private const string oAuthKey = "oauth:6lwp9xs2oye948hx2hpv5hilldl68g"; // this is a public value 

        public string botName = "COHopponentBot"; // the bots chat userName
        [JsonPropertyAttribute] private string botOAuthKey = Encrypt(oAuthKey, passphrase); // stored in file as simple encrypted string.
        public string defaultJoinChannel = "xereborn"; // typically the broadcasters twitch channel
        public string twitchAdminUserName = "xereborn"; // incase you want to use the bot on someone elses channel only you will hear the tts.
        public string kickChannelUserName = "Xcom-Reborn";    
        public string kickChannelAdminUserName = "Xcom_Reborn";
        [JsonPropertyAttribute] private string kickPassword = "";// stored in file as simple encrypted string.
        [JsonPropertyAttribute] private string kick2FAToken = "";// stored in file as simple encrypted string.

    // Stores whose text messages the bot will speak.
    public string saidString = "said";

        public Dictionary<string, bool> settingDictionary = new Dictionary<string, bool>
    {
        {"broadcasterSpeaks" , true },
        {"modSpeaks" , true },
        {"vipSpeaks" , true },
        {"userSpeaks" , true },
        {"subscriberSpeaks" , true },
        {"substituteEnabled" , true },
        {"substituteRegexEnabled" , true },
        {"displayConnectionMessage" , true },
        {"displayDisconnectionMessage" , true },
        {"speakUserNameEnabled" , true },

    };

        //Volume
        public int volume = 100;

        //Keys for speech message stack manipulation
        public int pauseKey = ((int)Key.Add);
        public int skipKey = ((int)Key.Multiply);
        public int skipAllKey = ((int)Key.Divide);

    [JsonIgnore] public string KickPassword { get { return Decrypt(kickPassword, passphrase); } set { kickPassword = Encrypt(value, passphrase); } }
    [JsonIgnore] public string Kick2FAToken { get { return Decrypt(kick2FAToken, passphrase); } set { kick2FAToken = Encrypt(value, passphrase); } }
    [JsonIgnore] public string BotOAuthKey { get { return Decrypt(botOAuthKey, passphrase); } set { botOAuthKey = Encrypt(value, passphrase); } }

    public static string Encrypt(string plainText, string password = null)
    {
        if (String.IsNullOrEmpty(plainText)) return "";
        var data = Encoding.Default.GetBytes(plainText);
        var pwd = !string.IsNullOrEmpty(password) ? Encoding.Default.GetBytes(password) : Array.Empty<byte>();
        var cipher = ProtectedData.Protect(data, pwd, DataProtectionScope.CurrentUser);
        return Convert.ToBase64String(cipher);
    }

    public static string Decrypt(string cipherText, string password = null)
    {
        if (String.IsNullOrEmpty(cipherText)) return "";
        var cipher = Convert.FromBase64String(cipherText);
        var pwd = !string.IsNullOrEmpty(password) ? Encoding.Default.GetBytes(password) : Array.Empty<byte>();
        var data = ProtectedData.Unprotect(cipher, pwd, DataProtectionScope.CurrentUser);
        return Encoding.Default.GetString(data);
    }

    public override string ToString()
        {
            return "Object Contains TTS TwitchBot Settings";
        }

    }

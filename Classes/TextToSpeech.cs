using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

using System.Speech.Synthesis;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System;
using System.Linq;

using System.Threading;

using ExtensionMethods;

using System.Diagnostics;
using KickLib.Client;
using System.Windows;


namespace TTSBot {

    public class TextToSpeech {

        public TwitchBot? twitch_bot;

        public KickBot? kick_bot;

        private ChatUserManager users = new ChatUserManager();

        private IgnoredWords ignoredWords = new IgnoredWords();

        private SubstitutionWords substitutionWords = new SubstitutionWords();

        private string previousUserName = "";

        private SettingsManager botSettingManager;

        public bool running = true;

        public CommandsManager commands;

        public Thread MainLoopMessageStackCheckingThread;

        public CancellationTokenSource cts;

        public SpeechSynthesizer synth = new SpeechSynthesizer();

        public Queue<ChatData> messageBuffer = new Queue<ChatData>();


        public TextToSpeech()
        {

            this.commands = new CommandsManager(this);
            this.twitch_bot = new TwitchBot(this);
            this.kick_bot = new KickBot(this);

            this.botSettingManager = Application.Current.Properties["settings_manager"] as SettingsManager;

            cts = new CancellationTokenSource();

            run();
        }

        public void run()
        {


            cts.Dispose();

            cts = new CancellationTokenSource();

            MainLoopMessageStackCheckingThread = new Thread(() => MainLoop(cts.Token));

            MainLoopMessageStackCheckingThread.Start();

        }

        public void MainLoop(CancellationToken cancellationToken)
        {

            try
            {
                Debug.WriteLine(cancellationToken.IsCancellationRequested.ToString());

                while (this.running)
                {
                    if (cancellationToken.IsCancellationRequested) { break; }

                    if (this.messageBuffer.Count > 0)
                    {

                        ChatData chatData = this.messageBuffer.Dequeue();
                        // check for any text to speech chat commands 
                        CheckForChatCommands(chatData);
                        // Send to speech
                        Speak(chatData);

                    }

                    Thread.Sleep(100);

                }

                Debug.WriteLine("Exiting MainLoop on message buffer thread.");
            }
            catch (Exception e)
            {
                // Thread Abort Exception will throw here.
            }
        }

        public void ProcessChatMessage(ChatData chatData)
        {



            CheckForChatCommands(chatData);
            // Send to speech
            Speak(chatData);

        }

        public void Speak(ChatData chatData)
        {

            string user_name = chatData.user_name;

            Debug.WriteLine("LEVEL {}", chatData.user_level.ToString());

            //check for alias
            ChatUser user = new ChatUser(user_name, origin:chatData.origin);
            if (users.IsUserInList(user))
            {
                user = users.GetUser(user);
                user_name = user.alias;
            }

            // ensure message is not null
            if (chatData.message != null) {

                // if the user exists they might be set to ignore or message starts with ! char
                if ((!user.ignored) && (!(chatData.message[0] == "!"[0])) && (!(ignoredWords.ContainsIgnoredWord(chatData.message))))
                {
                    try
                    {

                            if (!this.botSettingManager.settings.settingDictionary["adminSpeaks"] && (chatData.user_level == Commands.UserLevel.ADMIN) ||
                                !this.botSettingManager.settings.settingDictionary["broadcasterSpeaks"] && (chatData.user_level == Commands.UserLevel.STREAMER) ||
                                !this.botSettingManager.settings.settingDictionary["modSpeaks"] && (chatData.user_level == Commands.UserLevel.MOD) ||
                                !this.botSettingManager.settings.settingDictionary["vipSpeaks"] && (chatData.user_level == Commands.UserLevel.VIP) ||
                                !this.botSettingManager.settings.settingDictionary["userSpeaks"] && (chatData.user_level == Commands.UserLevel.USER) ||
                                !this.botSettingManager.settings.settingDictionary["subscriberSpeaks"] && chatData.is_subscriber)
                            { return; }


                            //only use username said something, if not saying for first time in a row.
                            string spokenString = "";

                            //substitute any words in the user message for the ones in the substitution dictionary.
                            string messageToTextToSpeech = SubstituteWords(chatData);

                            System.Console.WriteLine("user_name : {0}", user_name);
                            System.Console.WriteLine("previousUserName : {0}", previousUserName);

                            if (previousUserName == user_name)
                            {

                                spokenString = messageToTextToSpeech;

                            }
                            else
                            {
                                // speak username if enabled
                                if (this.botSettingManager.settings.settingDictionary["speakUserNameEnabled"])
                                {
                                    spokenString = user_name + " " + this.botSettingManager.settings.saidString + " " + messageToTextToSpeech;
                                }
                                else
                                {
                                    spokenString = messageToTextToSpeech;
                                }

                            }
                            // Initialize a new instance of the SpeechSynthesizer.  
                            //synth = new SpeechSynthesizer();



                            // Configure the audio output.   
                            synth.SetOutputToDefaultAudioDevice();


                            // This requires testing to see if the voiceNumber index is correct.
                            // assign a random voice based on userName unless already set
                            int voiceNumber = user.name.GetIntFromString();
                            // make that number fall between the bounds of the voices available
                            ReadOnlyCollection<InstalledVoice> voicesAvailable = synth.GetInstalledVoices();
                            int voicesAvailableCount = voicesAvailable.Count;
                            int defaultVoiceIndex = (voiceNumber) % (voicesAvailableCount);


                            //get voice name based on default index
                            synth.SelectVoice(voicesAvailable[defaultVoiceIndex].VoiceInfo.Name);

                            // Set the voice based on Name
                            if (user.voiceName != "")
                            {
                                synth.SelectVoice(user.voiceName);
                            }

                            // Set the speech volume
                            synth.Volume = (int)this.botSettingManager.settings.volume;

                            //synth.SelectVoiceByHints(VoiceGender.NotSet,VoiceAge.NotSet,user.voiceNumber);
                            // Speak a string.  
                            synth.Speak(spokenString);

                            previousUserName = user_name;

                        }
                        // Don't speak message

                    catch
                    {

                    }

                }

            }

        }


        private string SubstituteWords(ChatData chat_message) {


            string output = chat_message.message;

            try {

                // check if subsitute is enabled
                if (this.botSettingManager.settings.settingDictionary["substituteEnabled"])
                {
                    if (substitutionWords.subwords.words != null)
                    {
                        if (substitutionWords.subwords.words.Count > 0)
                        {

                            try
                            {
                                var words = string.Join("|", substitutionWords.subwords.words.Keys);
                                System.Console.WriteLine("Word Sub Pattern Matches : " + $@"\b({words})\b");
                                output = Regex.Replace(chat_message.message, $@"\b({words})\b", delegate (Match m)
                                {

                                    return substitutionWords.subwords.words[Regex.Escape(m.Value)].PickRandom();
                                }

                                );
                            }
                            catch
                            {
                                System.Console.WriteLine("Regex match failed.");
                            }
                        }
                    }

                }

                // check if substituteregex enabled
                if (this.botSettingManager.settings.settingDictionary["substituteRegexEnabled"])
                {
                    if (substitutionWords.subwords.regularexpressions != null) {

                        if (substitutionWords.subwords.regularexpressions.Count > 0)
                        {

                            // this will iterate over regular expressions in the regular expression dictionary so care should be taken with the entered patterns.
                            foreach (KeyValuePair<string, List<string>> item in substitutionWords.subwords.regularexpressions)
                            {
                                try
                                {
                                    output = Regex.Replace(output, item.Key, item.Value.PickRandom());
                                    System.Console.WriteLine("Regex Sub Pattern Matches : " + item.Key + " : Sub : " + item.Value);
                                }
                                catch
                                {
                                    System.Console.WriteLine("Regex match failed.");
                                    System.Console.WriteLine("Regex Sub Pattern Matches : " + item.Key + " : Sub : " + item.Value);
                                }
                            }
                        }

                    }
                }

            }

            catch (Exception exp)
            {
                System.Console.WriteLine(exp.ToString());
                System.Console.WriteLine("Problems in SubstituteWords.");
                return "";

            }

            return output;
            
       }



        public void CheckForChatCommands(ChatData chatData)
        {

            string[] words = chatData.message.Split(' ');


            if (words.Length > 0)
            {

                // Create new dictionary only where command enabled == true
                Dictionary<Delegate, Commands> commmandDict = new Dictionary<Delegate, Commands>();
                foreach(var item in commands.commands){
                    if (item.Value.enabled == true){
                        commmandDict.Add(item.Key, item.Value);
                        
                    }
                }


                // Admin Only Commands
                if (chatData.user_level >= Commands.UserLevel.ADMIN)
                {
                    foreach (var item in commmandDict)
                    {
                        if ((item.Value.ttsComparisonCommand == words[0]) && (item.Value.privilageLevel == Commands.UserLevel.ADMIN))
                        {
                            
                            item.Key.DynamicInvoke(chatData);
                            return;
                        }

                    }
                }

                // Admin + Broadcaster Commands
                if (chatData.user_level >= Commands.UserLevel.STREAMER)
                {
                    foreach (var item in commmandDict){
                        if ((item.Value.ttsComparisonCommand == words[0]) && (item.Value.privilageLevel == Commands.UserLevel.STREAMER)){
                            item.Key.DynamicInvoke(chatData);
                            return;
                        }

                    }
                }

                // Admin + Broadcaster + Moderator commands 
                if (chatData.user_level >= Commands.UserLevel.MOD)

                {
                    foreach (var item in commmandDict){
                    if ((item.Value.ttsComparisonCommand == words[0]) && (item.Value.privilageLevel == Commands.UserLevel.MOD)){
                        item.Key.DynamicInvoke(chatData);
                        return;
                    }

                }                   
                }

                // Admin + Boardcaster + Mod + VIP commands 
                if (chatData.user_level >= Commands.UserLevel.VIP)

                {
                    foreach (var item in commmandDict)
                    {
                        if ((item.Value.ttsComparisonCommand == words[0]) && (item.Value.privilageLevel == Commands.UserLevel.VIP))
                        {
                            item.Key.DynamicInvoke(chatData);
                            return;
                        }

                    }
                }

                // USER/Everyone commands 

                foreach (var item in commmandDict)
                {
                    if ((item.Value.ttsComparisonCommand == words[0]) && (item.Value.privilageLevel == Commands.UserLevel.USER))
                    {
                        item.Key.DynamicInvoke(chatData);
                        return;
                    }

                }


            }

        }

        public bool SetRegex(ChatData chatData){

            string[] wordList = chatData.message.Split(' ');
            if (wordList.Length > 2){
                // typical input in the form of !regex \b8=*D\b|\b8-*D\b wub
                string pattern = wordList[1];
                string wordToSubstitute = String.Join(" ", wordList.Skip(2));

                substitutionWords.AddRegularExpressionSubPair(pattern, wordToSubstitute);
                substitutionWords.Save();

                var message1 = String.Format("{0} regex pattern will be substituted with {1} in text to speech.", pattern, wordToSubstitute);

                OutputMessage(chatData, message1);

                return true;

            }

            var message2 = String.Format("Please enter the command in the form : !regex pattern substitute");

            OutputMessage(chatData, message2);

            return false;

        }

        private void OutputMessage(ChatData chatData, string message) {

            if (chatData.origin == "TWITCH")
                twitch_bot.client.SendMessage(chatData.channel, message);
            if (chatData.origin == "KICK")
                kick_bot.SendMessage(chatData.channel, message);

        }

        public bool RemoveRegex(ChatData chatData){

            string[] wordList = chatData.message.Split(' ');
            if (wordList.Length > 1){
                string pattern = wordList[1];

                bool success = substitutionWords.RemoveRegularExpressionSubPair(pattern);

                if (success){

                    var message1 = String.Format("{0} pattern has been removed from the word regex substitution list.", pattern);
                        
                    OutputMessage(chatData, message1);

                    substitutionWords.Save();
                return true;
                }
                else{

                    var message2 = String.Format("Could not remove {0} pattern from the regex substitution list.", pattern);

                    OutputMessage(chatData, message2);
                    
                    return false;
                }

            }

            var message3 = String.Format("Please enter word to remove in the form of !removeregex word.");

            OutputMessage(chatData, message3);

            return false;


        }
        
        public bool RemoveSubstitute(ChatData chatData){

            string[] wordList = chatData.message.Split(' ');
            if (wordList.Length > 1){
                string wordToRemove = wordList[1];


                bool success = substitutionWords.RemoveWord(wordToRemove);

                if (success){

                    var message1 = String.Format("{0} has been removed from the word substitution list.", wordToRemove);
                    OutputMessage(chatData, message1);
                    
                substitutionWords.Save();
                return true;
                }
                else{

                    var message2 = String.Format("Could not remove {0} from the word substitution list.", wordToRemove);
                    OutputMessage(chatData, message2);
                    return false;
                }

            }

            var message3 = String.Format("Please enter word to remove in the form of !removesubstitute word.");
            OutputMessage(chatData, message3);
            return false;

        }

        public bool SetSubstituteWord(ChatData chatData){

            string[] wordList = chatData.message.Split(' ');
            if (wordList.Length > 2){
                // typical input in the form of !substitute <3 wub
                string word = wordList[1];
                if ((word.Length > 0)||(word != "")||(word != " "))
                {
                    System.Console.WriteLine(String.Format("word : {0}", word));
                    string wordToSubstitute = String.Join(" ", wordList.Skip(2));

                    substitutionWords.AddWordPair(word, wordToSubstitute);
                    substitutionWords.Save();

                    var message1 = String.Format("{0} will be substituted with {1} in text to speech.", word, wordToSubstitute);

                    OutputMessage(chatData,message1);
                    return true;
                }

            }

            var message2 = String.Format("Please enter the command in the form : !substitute word substitute");
            OutputMessage(chatData, message2);
            return false;

        }


        public bool DisplayBlackList(ChatData chatData){
           

            List<ChatUser> ignoredList = new List<ChatUser>();

            // get only users where ignore is true and put into new list

            foreach (ChatUser user in users.users){

                if (user.ignored == true){

                    ignoredList.Add(user);
                }

            }

            List<string> names = new List<string>();

            foreach (ChatUser user in ignoredList){

                names.Add(user.name);

            }

            string output = "";
            output = String.Join(",", names);

            var message1 = String.Format("Ignored Users : {0}.", output);

            OutputMessage (chatData, message1);

            return true;


        }

        public bool SetVoice(ChatData chatData){

            string[] wordList = chatData.message.Split(' ');
            if (wordList.Length > 1)
            {

                int voiceNumber = -1;
                string voiceNumberString = wordList[1]; 
                try{
                    voiceNumber = int.Parse(voiceNumberString);
                }
                catch{
                    var message1 = String.Format("{0} is not a vaild voice number.", voiceNumberString);

                    OutputMessage(chatData, message1);
                    System.Console.WriteLine("Problem parsing string to int in SetVoice.");
                    return false;
                }

                // Initialize a new instance of the SpeechSynthesizer.  
                SpeechSynthesizer synth = new SpeechSynthesizer();
                ReadOnlyCollection <InstalledVoice> installedVoices  = synth.GetInstalledVoices();
                if ( 0 <= voiceNumber && voiceNumber <= (installedVoices.Count - 1)){

                    ChatUser user = new ChatUser(chatData.user_name);
                    if (users.IsUserInList(user)){

                        user = users.GetUser(user);
                        users.RemoveUser(user);
                        user.voiceNumber = voiceNumber;
                        user.voiceName = installedVoices[voiceNumber].VoiceInfo.Name;
                        users.AddUser(user);

                    }
                    else{

                        user.voiceNumber = voiceNumber;
                        user.voiceName = installedVoices[voiceNumber].VoiceInfo.Name;
                        users.AddUser(user);
                        
                    }
                    var message2 = String.Format("{0} has selected voice : {1}", user.name, user.voiceName);
                    OutputMessage(chatData, message2);

                    users.Save();

                }

            }

            return true;


        }

        public bool SetUserVoice(ChatData chatData){

            string[] wordList = chatData.message.Split(' ');
            if (wordList.Length > 2)
            {   

                int voiceNumber = -1;
                string userName = wordList[1];
                string voiceNumberString = wordList[2]; 
                try{
                    voiceNumber = int.Parse(voiceNumberString);
                }
                catch{
                    var message1 = String.Format("{0} is not a vaild voice number.", voiceNumberString);
                    OutputMessage(chatData,message1);
                    System.Console.WriteLine("Problem parsing string to int in SetVoice.");
                    return false;
                }

                // Initialize a new instance of the SpeechSynthesizer.  
                SpeechSynthesizer synth = new SpeechSynthesizer();
                ReadOnlyCollection <InstalledVoice> installedVoices  = synth.GetInstalledVoices();
                if ( 0 <= voiceNumber && voiceNumber <= (installedVoices.Count - 1)){

                    ChatUser user = new ChatUser(userName);
                    if (users.IsUserInList(user)){

                        user = users.GetUser(user);
                        users.RemoveUser(user);
                        user.voiceNumber = voiceNumber;
                        user.voiceName = installedVoices[voiceNumber].VoiceInfo.Name;
                        users.AddUser(user);

                    }
                    else{

                        user.voiceNumber = voiceNumber;
                        user.voiceName = installedVoices[voiceNumber].VoiceInfo.Name;
                        users.AddUser(user);
                        
                    }
                    var message2 = String.Format("{0} voice has been set to voice : {1}", user.name, user.voiceName);
                    OutputMessage (chatData,message2);

                    users.Save();

                }

            }
            var message3 = String.Format("Correct useage in the form of !uservoice [UserName] [VoiceNumber]");
            OutputMessage(chatData,message3);

            return true;


        }

        public bool DisplayAvailableVoices(ChatData chatData){

            string voices = "";

            // Initialize a new instance of the SpeechSynthesizer.  
            SpeechSynthesizer synth = new SpeechSynthesizer();

            // Configure the audio output.   
            synth.SetOutputToDefaultAudioDevice();

            ReadOnlyCollection <InstalledVoice> installedVoices  = synth.GetInstalledVoices();

            int index = 0;
            try{
            foreach (InstalledVoice voice in installedVoices){

                string voiceName = voice.VoiceInfo.Name;
                voiceName = voiceName.Replace("Microsoft ", ""); //# remove the word microsoft
                //voiceName = voiceName.Replace(" English " , ""); //# remove the word English 
                voiceName = voiceName.Replace("Desktop", "" ); //# remove the word Desktop
                voiceName = voiceName.Replace("Mobile", ""); //# remove the word Mobile
                voiceName = voiceName.Replace("-" , ""); //# remove the character "-"
                voiceName = voiceName.Replace("(Canada)" , ""); // just removes canada for Eva if present
                string culture = "  ";
                try{
                    culture = voice.VoiceInfo.Culture.Name.Substring(3, 2);

                }catch{

                    System.Console.WriteLine("Problem getting last two characters of : " + voice.VoiceInfo.Culture.Name);

                }

                voices += " " + index.ToString() + ". " + voiceName + " (" + culture + ") ";
                //System.Console.WriteLine(voice.VoiceInfo.Name);
                index++;

            }
            }catch{
                
                System.Console.WriteLine("Problem gettings installed voices.");
            }

            var message1 = String.Format("Available Voices : {0}", voices.Substring(0, Math.Min(voices.Length, 500)));
            OutputMessage(chatData, message1);

            return true;
        }

        public bool SetIgnoreWord(ChatData chatData){


            
            string[] wordList = chatData.message.Split(' ');
            if (wordList.Length > 1)
            {

                ignoredWords.AddWord(wordList[1]);
                ignoredWords.Save();

                var message1 = String.Format("messages containing {0} will be ignored.", wordList[1]);

                OutputMessage(chatData, message1);

            }

            return true;


        }

        public bool SetUnignoreWord(ChatData chatData){

            string[] wordList = chatData.message.Split(' ');
            if (wordList.Length > 1)
            {

                ignoredWords.RemoveWord(wordList[1]);
                ignoredWords.Save();

                var message1 = String.Format("messages containing {0} will not be ignored.", wordList[1]);

                OutputMessage(chatData , message1);

            }   

            return true;         


        }

        public bool CloseTTS(ChatData chatData)
        {

            System.Console.WriteLine("Closing TTS.");
            try
            {
                if (this.botSettingManager.settings.settingDictionary["displayDisconnectionMessage"])
                {
                    var message1 = "Closing TTS TwitchBot.";
                    OutputMessage(chatData, message1);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            try
            {
                cts.Cancel();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            // Possibly need reference to WPF window to close keyboard hooks etc properly.
            // In the mean time.
            // BruteForce the Exit.
            Environment.Exit(0);

            return true;

        }

        public bool SetAlias(ChatData chatData)
        {

            string[] wordList = chatData.message.Split(' ');
            if (wordList.Length > 1)
            {

                string alias = Sanitize(String.Join(" ", wordList.Skip(1)));
                ChatUser user = new ChatUser(chatData.user_name, alias);
                if (user != null)
                {

                    if (users.IsUserInList(user))
                    {

                        user = users.GetUser(user);
                        users.RemoveUser(user);
                        user.alias = alias;
                        users.AddUser(user);
                    }
                    else
                    {

                        users.AddUser(user);

                    }

                    var message1 = String.Format("{0}'s alias has been set to {1}", chatData.user_name, alias);

                    OutputMessage(chatData, message1);

                    users.Save();

                    return true;

                }


            }

            return true;


        }

        public bool SetUserAlias(ChatData chatData){

            string[] wordList = chatData.message.Split(' ');
            if (wordList.Length > 2)
            {
                string userName = wordList[1];
                string alias = Sanitize(String.Join(" ", wordList.Skip(2)));
                ChatUser user = new ChatUser(userName, alias);
                if (user != null)
                {

                    if (users.IsUserInList(user))
                    {

                        user = users.GetUser(user);
                        users.RemoveUser(user);
                        user.alias = alias;
                        users.AddUser(user);
                    }
                    else
                    {

                        users.AddUser(user);

                    }

                    var message1 = String.Format("{0}'s alias has been set to {1}", userName, alias);
                    OutputMessage(chatData, message1);

                    users.Save();

                    return true;

                }


            }

            var message2 = String.Format("Correct usage in the form !useralias user_name alias");
            OutputMessage(chatData,message2);
            return false;
        }

        public bool SetIgnore(ChatData chatData)
        {
            string[] wordList = chatData.message.Split(' ');
            if (wordList.Length > 1)
            {
                ChatUser user = new ChatUser(wordList[1]);
                if (user != null)
                {

                    if (users.IsUserInList(user))
                    {

                        user = users.GetUser(user);
                        users.RemoveUser(user);
                        user.ignored = true;
                        users.AddUser(user);
                    }
                    else
                    {

                        user.ignored = true;
                        users.AddUser(user);

                    }

                    var message1 = String.Format("{0} will be ignored.", user.name);

                    OutputMessage (chatData, message1);
                    users.Save();

                }

            }

            return true;

        }

        public bool SetUnignore(ChatData chatData)
        {
            string[] wordList = chatData.message.Split(' ');
            if (wordList.Length > 1)
            {

                ChatUser user = new ChatUser(wordList[1]);
                if (user != null)
                {

                    if (users.IsUserInList(user))
                    {
                        user = users.GetUser(user);
                        users.RemoveUser(user);
                        user.ignored = false;
                        users.AddUser(user);
                    }
                    else
                    {
                        user.ignored = false;
                        users.AddUser(user);
                    }

                    var message1 = String.Format("{0} will not be ignored.", user.name);
                    OutputMessage(chatData, message1);
                    users.Save();

                }


            }

            return true;

        }

        private string Sanitize(string str)
        {

            // Allowed Characters only allowed in alias
            string pattern = @"[^a-zA-Z0-9 -.&,%£+=?*@!#]";
            // Create a Regex  
            Regex rg = new Regex(pattern);

            string match = rg.Replace(str, "");

            return match;

        }




    }


}
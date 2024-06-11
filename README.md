# Text to speech chatbot

 A chatbot bot to provide text to speech on Windows configurable with a graphical user interface created using WPF.

 Version v1.1

![Example12](https://github.com/XcomReborn/Text_To_Speech_Chatbot/assets/4015491/70d427ef-dfdb-42b1-9f03-036d562abe0a)

It may be ugly... but it works.
 

## User Instructions for Windows:

Download executable file here : https://github.com/XcomReborn/CSharpTwitchTTSBotGUI/releases

Download exe file under assets drop-down and run in a new folder.

Use the program graphical user interface and navigate to Settings:

Set the connection channel and admin name to your twitch name then press the connect button.


## Instructions for connecting to kick

This program uses the KickLib library provide here https://github.com/Bukk94/KickLib

Inorder to connect and send messages to kick.com chat an account with two factor authentication (2FA) is required.
Please follow the instructions laid out on the KickLib page to get hold of your authentication code which is needed
along with a valid user name and password.


## Bot Commands. - All commands can be set by the user and enabled/disabled in the commands menu.

### default Bot commands for Streamer/AdminUserName:

**!ignoreword [word]** - will mute a message containing a specific word eg: http or www.  
**!unignoreword [word]** - will remove the word from the ignore word list.  
**!closetts** - will close the program.  
**!substitute [word] [substitute]** - will substitute the words in the substitute for the word.  
**!removesubstitute [word]** - removes the word from the substitute dictionary. 

### ==Advanced Commands==

**!regex [pattern] [substitute]** - will substitute the words in the substitute for the regular expression match in the pattern. Care should be taken not to use a broad match search.  
**!removeregex [pattern]** - removes the pattern from the regex substitute dictionary.  

### Bot commands restricted to twitch channel moderators:

**!voices** - lists all available voices.  
**!voice #** - sets the users voice to an available voice listed in !voices.  
**!uservoice [userName] #** - sets the userName's voice to an available voice listed in !voices.  

**!alias [alias]** - sets mod username alias.  
**!useralias [userName] [alias]** - sets another users alias.  

**!ignore [userName]** - ignores the following user name.  
**!unignore [userName]** - unignores the following user name.  
**!blacklist** - lists all the usernames in the blacklist (!ignorelist).  

## Adding Extra Voices.

You can add extra voices following the tutorial currently avilable here: https://www.ghacks.net/2018/08/11/unlock-all-windows-10-tts-voices-system-wide-to-get-more-of-them/

Or you can download and execute the file for adding the extra keys here: https://github.com/XcomReborn/CSharpTwitchTTSBotGUI/tree/main/RegistryAddExtraEnglishVoices

Please read the README!.txt file first here: https://github.com/XcomReborn/CSharpTwitchTTSBotGUI/blob/main/RegistryAddExtraEnglishVoices/IMPORTANT_README!.txt


## Compilation Instructions.
 
 A simple bot that uses the twitchlib library and System.Speech.Synthesis windows SAPI5 to speak text written in twitch IRC Chat. 
 
 The current version targets net8.0-windows

 Dependancies:

 Requires the following libraries:

 TwitchLib 3.3.0 - https://github.com/TwitchLib/TwitchLib
 System.Speech - https://docs.microsoft.com/en-us/dotnet/api/system.speech.synthesis.speechsynthesizer?view=netframework-4.8

These can be installed in your IDE of choice, if you are using VSCode NUGet, I would recommend installing NuGet Gallery(v0.0.24) via the marketplace.

Library Install Instructions (VSCode):

Open market place (Ctrl+Shift+x)
search for and install NuGet.

In Command Pallet (Ctrl+Shift+P)
open NuGet Gallery
search for and install TwitchLib 3.3.0
search for and install System.Speech 6.0.0
search for and install KickLib 0.1.8

If using Visual Studio 2022.

Install Packages using: Tools -> NuGet Package Manager -> Manage NuGet Packages For Solution...
search for and install TwitchLib 3.3.0
search for and install System.Speech 6.0.0
search for and install KickLib 0.1.8

<OutputType> WinExe
<TargetFramework> net8.0-windows
<UseWPF> true




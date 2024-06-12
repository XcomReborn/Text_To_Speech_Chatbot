
using System.Collections;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;
using System;

namespace TTSBot;
public class ChatUserManager
{

    public List<ChatUser> users { get; set; } = new List<ChatUser>();

    public string dataPath = Directory.GetCurrentDirectory() + "\\data\\twitchUserData.json";

    public ChatUserManager()
    {

        // attempt to load settings if fails use defaults and create file
        if (!Load())
        {

            Save();

        }
        
    }

    public bool AddUser(string username, string alias)
    {

        try
        {
            users.Add(new ChatUser(username.ToLower(), alias));
        }
        catch
        {
            return false;
        }
        return true;
    }

    public bool AddUser(ChatUser user)
    {
        try
        {
            user.name.ToLower();
            users.Add(user);
        }
        catch { return false; }
        return true;
    }

    public bool RemoveUser(ChatUser user)
    {

        return users.Remove(user);

    }

    public 
    ChatUser GetUser(ChatUser user)
    {

        return users.Find(x => x.name.ToLower() == user.name.ToLower());

    }

    ChatUser GetUser(string userName){

        return users.Find(x => x.name.ToLower() == userName.ToLower());

    }

    public bool IsUserInList(ChatUser user)
    {

        if (users.Find(x => x.name.ToLower() == user.name.ToLower()) != null)
        {
            return true;
        }
        return false;
    }

      public bool IsUserInList(string userName)
    {

        if (users.Find(x => x.name.ToLower() == userName.ToLower()) != null)
        {
            return true;
        }
        return false;
    }

      


    public bool Load()
    {
        if (File.Exists(dataPath))
        {

            try
            {
                FileStream fs = new FileStream(dataPath, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                string str = sr.ReadLine();
                if (str != null)
                {
                    Console.WriteLine(str);
                    List<ChatUser>? twitchUsers = JsonSerializer.Deserialize<List<ChatUser>>(str);
                    this.users = twitchUsers;
                }
                sr.Close();
                fs.Close();
            }
            catch
            {
                System.Console.WriteLine("A problem occurred while trying to load twitchUsers");
                return false;
            }
        }

        return true;

    }

    public bool Save()
    {

        try{
            if (!File.Exists(dataPath)){
            Directory.CreateDirectory(Path.GetDirectoryName(dataPath));
            }
        }
        catch{

            System.Console.WriteLine("A problem occurred while trying to create the dataPath Directory.");
        }
        try{
        FileStream fs = new FileStream(dataPath, FileMode.Create, FileAccess.Write);
        StreamWriter sw = new StreamWriter(fs);
        string userJson = JsonSerializer.Serialize(users);
        sw.WriteLine(userJson);
        sw.Flush();
        sw.Close();
        fs.Close();
        }
        catch{
            System.Console.WriteLine("A problem occurred while trying to save twitchUsers.");
            return false;
        }
        return true;
    }

    public override string ToString()
    {
        string output = "";
        foreach (ChatUser user in users)
        {

            output += user.ToString() + "\n";
        }

        return output;
    }


}
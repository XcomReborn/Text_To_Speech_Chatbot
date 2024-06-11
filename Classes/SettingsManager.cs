using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Input;
using System.Collections.Generic;
using System.Diagnostics;

namespace TTSBot;

[System.Serializable]
public class SettingsManager
{

    public Settings settings = new Settings();

    public string userSettingsPath = Directory.GetCurrentDirectory() + "\\data\\userSettings.json";


    public SettingsManager()
    {

        // attempt to load settings if fails use defaults

        if (!Load())
        {

            Save();

        }
    }

    public bool Load()
    {

        if (File.Exists(userSettingsPath))
        {

            try
            {
                FileStream fs = new FileStream(userSettingsPath, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                string str = sr.ReadToEnd();
                if (str != null)
                {
                    Settings? settings = JsonConvert.DeserializeObject<Settings>(str);
                    if (settings != null)
                    {
                        this.settings = settings;
                    }
                }
                sr.Close();
                fs.Close();
            }
            catch
            {
                System.Console.WriteLine("A problem occurred while trying to load substitutionWords.json");
                return false;
            }
        }
        else
        {
            return false;
        }


        return true;


    }

    public bool Save()
    {

        try{
            if (!File.Exists(userSettingsPath)){
            Directory.CreateDirectory(Path.GetDirectoryName(userSettingsPath));
            }
        }
        catch{

            System.Console.WriteLine("A problem occurred while trying to create the userSettingsPath Directory.");
        }


            try
            {
                FileStream fs = new FileStream(userSettingsPath, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                string userSettingsJson = JsonConvert.SerializeObject(settings);
                sw.WriteLine(userSettingsJson);
                sw.Flush();
                sw.Close();
                fs.Close();
            }
            catch (Exception exception)
            {
                System.Console.WriteLine(exception.ToString());
                System.Console.WriteLine("A problem occurred while trying to save userSettings.");
                return false;
            }
            return true;

    }


}

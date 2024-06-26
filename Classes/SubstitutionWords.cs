using System.Text.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;
using System.IO;

namespace TTSBot;
public class SubstitutionWords{

    public SubWords subwords = new SubWords();

    public string substitutionWordsPath = AppDomain.CurrentDomain.BaseDirectory + "/data/substitutionWords.json";


    public SubstitutionWords(){

        // attempt to load settings if fails use defaults

        if (!this.Load())
        {

            this.Save();

        }
        

    }

    public bool AddWordPair(string word, string wordToSubstitute){

        try{
        subwords.words.Add(Regex.Escape(word), wordToSubstitute.Split(",").ToList());
        return true;
        }
        catch{
            return false;
        }
    }

    public bool RemoveWord(string word){

        try{
        subwords.words.Remove(word);
        return true;
        }
        catch{
            return false;
        }
    }

    public bool AddRegularExpressionSubPair(string pattern, string wordToSubstitute){

        try{
        subwords.regularexpressions.Add(pattern, wordToSubstitute.Split(",").ToList());
        return true;}
        catch{
            return false;
        }
    }

    public bool RemoveRegularExpressionSubPair(string pattern){

        try{
        subwords.regularexpressions.Remove(pattern);
        return true;}
        catch{
            return false;
        }

    }


    public bool Save()
    {

        try{
            if (!File.Exists(substitutionWordsPath)){
            Directory.CreateDirectory(Path.GetDirectoryName(substitutionWordsPath));
            }
        }
        catch{

            System.Console.WriteLine("A problem occurred while trying to create the substitutionWordsPath Directory.");
        }


        try
        {
            FileStream fs = new FileStream(substitutionWordsPath, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            string userJson = JsonConvert.SerializeObject(subwords);
            sw.WriteLine(userJson);
            sw.Flush();
            sw.Close();
            fs.Close();
        }
        catch
        {
            System.Console.WriteLine("A problem occurred while trying to save substitutionWords.json");
            return false;
        }

        return true;
    }

    public bool Load()
    {

       if (File.Exists(substitutionWordsPath))
        {

            try
            {
                FileStream fs = new FileStream(substitutionWordsPath, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                string str = sr.ReadToEnd();
                if (str != null)
                {

                    SubWords? wordDictionary = JsonConvert.DeserializeObject<SubWords>(str);
                    this.subwords = wordDictionary;

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
        else{
            return false;
        }

    
        return true;

    }

    public override string ToString()
    {
        return subwords.ToString();
    }


}


[System.Serializable]
public class SubWords{

    public Dictionary<string, List<string>> words = new Dictionary<string, List<string>>();
    public Dictionary<string, List<string>> regularexpressions = new Dictionary<string, List<string>>(); 

    public SubWords(){

        // No Construction Required

    }


    public override string ToString()
    {
        string output = "";
        if (words != null){
        var lines = words.Select(kvp => kvp.Key + ": " + kvp.Value);
        output = "words:\n" + string.Join(Environment.NewLine, lines);
        }
        if (regularexpressions != null){
        var lines = regularexpressions.Select(kvp => kvp.Key + ": " + kvp.Value);
        output = "regularexpressions:\n" + string.Join(Environment.NewLine, lines);
        return output;
        }        

        return "Dictionaries Empty.";
    }    


}
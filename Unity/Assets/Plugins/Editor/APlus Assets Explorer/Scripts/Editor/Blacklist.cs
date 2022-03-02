//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR

using System.IO;
using System.Collections.Generic;
using APlus;

public class Blacklist 
{
    private List<string> data = new List<string>();
    static string PATH = "ProjectSettings/APlusBlacklist.asset";

    public void Add(string s)
    {
        if(!Exists(s))
        {
            data.Add(s);
        }
    }

    public List<string> GetData()
    {
        return data;
    }

    public void Remove(string s)
    {
        if (Exists(s))
        {
            data.Remove(s);
        }
    }

    public bool Exists(string s)
    {
        return data.Contains(s);
    }

    public void Load()
    {
        data.Clear();
        if (File.Exists(PATH))
        {
            var lines = File.ReadAllLines(PATH);
            foreach (var line in lines)
            {
                if (line.StartsWith("#")) 
                {
                    continue;
                }

                data.Add(line.Trim());
            }
        } 
        else
        {
            data.Add("Assets/Plugins/APlus Assets Explorer");
            data.Add("Packages/");
            Save();
        }
    }

    public void Save()
    {
       var dataToStorage = string.Join("\n", data.ToArray());
       File.WriteAllText(PATH, dataToStorage);
    }

    public List<APAsset> GetAssets()
    {
        List<APAsset> list = new List<APAsset>();

        foreach (var path in data)
        {
            list.Add(APResources.GetBlackListAPAsset(path));
        }

        return list;
    }
}

#endif
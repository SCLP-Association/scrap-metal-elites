using System.Linq;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public static class JsonStore {
    public enum SaveTag {
        Player,
        Score
    }

    public static string[] ListByTag(SaveTag saveTag) {
         DirectoryInfo taskDirectory = new DirectoryInfo(Application.persistentDataPath);
         var pattern = saveTag.ToString() + "_*.json";
         FileInfo[] matchFiles = taskDirectory.GetFiles(pattern);
         List<string> names = new List<string>();
         for (var i=0; i<matchFiles.Length; i++) {
             var fn = Path.GetFileNameWithoutExtension(matchFiles[i].Name);
             string[] tokens = fn.Split('_');
             var final = string.Join("_", tokens.Skip(1).ToArray());
             names.Add(final);
         }
         return names.ToArray();
    }

    public static void Save(SaveTag saveTag, string name, string json) {
        var filename = saveTag.ToString() + "_" + name + ".json";
        var path = Path.Combine(Application.persistentDataPath, filename);
        File.WriteAllText(path, json);
    }

    public static string Load(SaveTag saveTag, string name) {
        var filename = saveTag.ToString() + "_" + name + ".json";
        var path = Path.Combine(Application.persistentDataPath, filename);
        if (File.Exists(path)) {
            var readText = File.ReadAllText(path);
            return readText;
        } else {
            return "";
        }
    }

}

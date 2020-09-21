#if UNITY_EDITOR
using UnityEditor;
using System.IO;
public static class EnumGenerator
{
    public static void Generate(string name, string[] entries)
    {
        string enumName = name;
        string[] enumEntries = entries;
        string filePathAndName = "Assets/FloofInternal/Playstate/" + enumName + ".cs";
        using (StreamWriter streamwriter = new StreamWriter(filePathAndName))
        {
            streamwriter.WriteLine("public enum " + enumName);
            streamwriter.WriteLine("{");
            for (int i = 0; i < enumEntries.Length; i++)
            {
                streamwriter.WriteLine("\t" + enumEntries[i] + ",");
            }
            streamwriter.WriteLine("}");
        }
        AssetDatabase.Refresh();
    }
}
#endif

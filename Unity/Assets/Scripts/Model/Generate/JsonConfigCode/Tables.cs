//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using SimpleJSON;

namespace cfg
{
   
public sealed partial class Tables
{
    public audioData audioData {get; }
    public roleData roleData {get; }

    public Tables(System.Func<string, JSONNode> loader)
    {
        var tables = new System.Collections.Generic.Dictionary<string, object>();
        audioData = new audioData(loader("audiodata")); 
        tables.Add("audioData", audioData);
        roleData = new roleData(loader("roledata")); 
        tables.Add("roleData", roleData);
        PostInit();

        audioData.Resolve(tables); 
        roleData.Resolve(tables); 
        PostResolve();
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        audioData.TranslateText(translator); 
        roleData.TranslateText(translator); 
    }
    
    partial void PostInit();
    partial void PostResolve();
}

}

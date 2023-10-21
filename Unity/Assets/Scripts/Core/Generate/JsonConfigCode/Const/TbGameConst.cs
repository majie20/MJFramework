//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using System.Collections.Generic;
using SimpleJSON;



namespace cfg.Const
{

public sealed partial class TbGameConst
{

     private readonly Const.GameConst _data;

    public TbGameConst(JSONNode _json)
    {
        if(!_json.IsArray)
        {
            throw new SerializationException();
        }
        if (_json.Count != 1) throw new SerializationException("table mode=one, but size != 1");
        _data = Const.GameConst.DeserializeGameConst(_json[0]);
        PostInit();
    }

     public int DefPlayer => _data.DefPlayer;
     public UnityEngine.Vector2 DefGravity => _data.DefGravity;

    public void Resolve(Dictionary<string, object> _tables)
    {
        _data.Resolve(_tables);
        PostResolve();
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        _data.TranslateText(translator);
    }

    
    partial void PostInit();
    partial void PostResolve();
}

}
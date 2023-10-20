//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using System.Collections.Generic;



namespace cfg.Role
{

public sealed partial class RoleType :  Bright.Config.BeanBase 
{
    public RoleType(ByteBuf _buf) 
    {
        Id = _buf.ReadInt();
        Name = _buf.ReadString();
        TypeCode = _buf.ReadString();
        PostInit();
    }

    public static RoleType DeserializeRoleType(ByteBuf _buf)
    {
        return new Role.RoleType(_buf);
    }

    /// <summary>
    /// 这是id
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// 角色类型编号
    /// </summary>
    public string TypeCode { get; private set; }

    public const int __ID__ = -2051636408;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<string, object> _tables)
    {
        PostResolve();
    }

    public  void TranslateText(System.Func<string, string, string> translator)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "Id:" + Id + ","
        + "Name:" + Name + ","
        + "TypeCode:" + TypeCode + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}

}
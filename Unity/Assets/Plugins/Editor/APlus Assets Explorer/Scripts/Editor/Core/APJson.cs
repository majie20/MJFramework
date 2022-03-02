//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR

namespace APlus
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Reflection;

    /// <summary>
    /// Data root for JsonSerializer
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class JSONRootAttribute : Attribute
    {
        public string Name { get; set; }

        public JSONRootAttribute(string name)
        {
            this.Name = name;
        }

        public JSONRootAttribute() : this("")
        {

        }
    }

    /// <summary>
    /// Date member for JsonSerializer
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class JSONDataMemberAttribute : Attribute
    {
        public string Name { get; set; }

        public JSONDataMemberAttribute(string name)
        {
            this.Name = name;
        }

        public JSONDataMemberAttribute() : this("")
        {

        }
    }

    public class APJsonSerializer
    {
        /// <summary>
        /// cache
        /// </summary>
        private static Dictionary<string, Dictionary<string, PropertyInfo>> cache = new Dictionary<string, Dictionary<string, PropertyInfo>>();

        public static string ToJson(object obj)
        {
            if(obj == null)
            {
                return string.Empty;
            }
            
            Type t = obj.GetType();
            
            if (!IsJsonSerializable(t))
            {
                return null;
            }

            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{");

            // use to stroage key-values and then concat them
            //
            List<string> keyValues = new List<string>();

            foreach (var keyVal in GetPropertyInfoList(t))
            {
                object val = keyVal.Value.GetValue(obj, null);
                string jsonValue = string.Empty;

                if (val == null)
                {
                    jsonValue = "null";
                    keyValues.Add(string.Format("\"{0}\":{1}", keyVal.Key, jsonValue));
                    continue;
                }

                // If mark JSONRootAttribute, the we use its json value
                // otherwise, we use its property values
                //
                if (IsJsonSerializable(val.GetType()))
                {
                    jsonValue = APJsonSerializer.ToJson(val);
                    keyValues.Add(string.Format("\"{0}\":{1}", keyVal.Key, jsonValue));
                }
                else if (val is IList)
                {
                    jsonValue = ToJsonArray(val as IList);
                    keyValues.Add(string.Format("\"{0}\":{1}", keyVal.Key, jsonValue));
                }
                else
                {
                    jsonValue = val.ToString();
                    keyValues.Add(string.Format("\"{0}\":\"{1}\"", keyVal.Key, jsonValue));
                }
            }

            jsonBuilder.Append(string.Join(",", keyValues.ToArray()));
            jsonBuilder.Append("}");

            return jsonBuilder.ToString();
        }

        public static string ToJsonArray(IList list)
        {
            if (list == null && list.Count == 0)
            {
                return "[]";
            }

            string[] KeyValues = new string[list.Count];
            
            for (int i = 0; i < list.Count; i++)
            {
                KeyValues[i] = ToJson(list[i]);
            }

            return string.Format("[{0}]", string.Join(",", KeyValues));
        }


        private static Dictionary<string, PropertyInfo> GetPropertyInfoList(Type t)
        {
            if (cache.ContainsKey(t.FullName))
            {
                return cache[t.FullName];
            }

            Dictionary<string, PropertyInfo> propertiesDict = new Dictionary<string, PropertyInfo>();
            
            foreach (var property in t.GetProperties())
            {
                string jsonKey = property.Name;
                
                // get customized json key name
                //
                foreach (var pa in property.GetCustomAttributes(false))
                {
                    if (pa is JSONDataMemberAttribute)
                    {
                        string newKeyName = (pa as JSONDataMemberAttribute).Name;
                        if (!string.IsNullOrEmpty(newKeyName))
                        {
                            jsonKey = newKeyName;
                        }

                        if (propertiesDict.ContainsKey(jsonKey))
                        {
                            throw new Exception("Key is already exist!");
                        }
                        else
                        {
                            propertiesDict.Add(jsonKey, property);
                        }
                        
                        break;
                    }
                }
            }

            cache.Add(t.FullName, propertiesDict);
            return propertiesDict;
        }

        private static bool IsJsonSerializable(Type t)
        {
            bool isJsonSerializable = false;
           
            foreach (var attribute in t.GetCustomAttributes(false))
            {
                if(attribute is JSONRootAttribute)
                {
                    isJsonSerializable = true;
                    break;
                }
            }

            return isJsonSerializable;
        }
    }
}
#endif
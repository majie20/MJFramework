using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using CatJson;
using Cysharp.Threading.Tasks;
using Model;
using UnityEngine;
using Component = Model.Component;

public class JsonFormatComponent : Component, IAwake
{
    /// <summary>
    /// 保存本地信息的文件夹
    /// </summary>
    private readonly string _dataJsonPath = $"{Application.persistentDataPath}/LocalizedData";
    /// <summary>
    /// 保存所有本地信息键值的文件
    /// </summary>
    private const string DATA_JSON_ALL_KEY_PATH = "DataJsonAllKey.json";
    /// <summary>
    /// 保存所有本地信息键值
    /// </summary>
    private List<string> _localizedDataKey;
    /// <summary>
    /// 临时本地信息键值
    /// </summary>
    private List<string> _temporaryKey;

    /// <summary>
    /// 所有的本地信息
    /// </summary>
    public Dictionary<string, string> DataJsonAllInfo
    {
        get;
        set;
    }

    public void Awake()
    {
        _localizedDataKey = new List<string>();
        _temporaryKey = new List<string>();
        DataJsonAllInfo = new Dictionary<string, string>();
        
        //暂时放在这进行初始化，用同步方法
        InitData();
    }

    public override void Dispose()
    {
        _localizedDataKey = null;
        _temporaryKey = null;
        DataJsonAllInfo = null;
        base.Dispose();
    }

    /// <summary>
    /// 异步保存Json信息
    /// </summary>
    /// <returns></returns>
    public async UniTask SaveJsonInfoAsync<T>(string name, T obj)
    {
        _temporaryKey.Add(name);
        var info = ObjToJsonString<T>(obj);
        await WriteFileAsync(name, info);
    }

    /// <summary>
    /// 保存Json信息
    /// </summary>
    /// <returns></returns>
    public void SaveJsonInfo<T>(string name, T obj)
    {
        _temporaryKey.Add(name);
        var info = ObjToJsonString<T>(obj);
        WriteFile(name, info);
    }

    /// <summary>
    /// 异步获取Json信息
    /// </summary>
    /// <returns></returns>
    public async UniTask<T> GetJsonInfoAsync<T>(string name)
    {
        var info = await ReadFileAsync(name);
        var obj = JsonStringToObj<T>(info);
        return obj;
    }

    /// <summary>
    /// 获取Json信息
    /// </summary>
    /// <returns></returns>
    public T GetJsonInfo<T>(string name)
    {
        var info = ReadFile(name);
        var obj = JsonStringToObj<T>(info);
        return obj;
    }

    /// <summary>
    /// 异步初始化加载所有信息
    /// </summary>
    /// <returns></returns>
    public async UniTask InitDataAsync()
    {
        string filePath = $"{_dataJsonPath}{DATA_JSON_ALL_KEY_PATH}";
        var text = await FileHelper.LoadFileByStreamAsync(filePath);
        var info = Convert.ToBase64String(text);
        _localizedDataKey = JsonStringToObj<List<string>>(info);
        foreach (var name in _localizedDataKey)
        {
            var content = await ReadFileAsync(name);
            DataJsonAllInfo.Add(name, content);
        }
    }

    /// <summary>
    /// 初始化加载所有信息
    /// </summary>
    /// <returns></returns>
    public void InitData()
    {
        string filePath = $"{_dataJsonPath}{DATA_JSON_ALL_KEY_PATH}";
        var text = FileHelper.LoadFileByStream(filePath);
        var info = Convert.ToBase64String(text);
        _localizedDataKey = JsonStringToObj<List<string>>(info);
        foreach (var name in _localizedDataKey)
        {
            var content = ReadFile(name);
            DataJsonAllInfo.Add(name, content);
        }
    }

    /// <summary>
    /// 异步保存信息的键
    /// </summary>
    /// <returns></returns>
    public async UniTask SaveKeyAsync()
    {
        var keys = _localizedDataKey;
        foreach (var key in _temporaryKey)
        {
            if (!keys.Contains(key))
            {
                keys.Add(key);
            }
        }
        await SaveJsonInfoAsync(DATA_JSON_ALL_KEY_PATH, keys);
    }

    /// <summary>
    /// 保存信息的键
    /// </summary>
    public void SaveAllKey()
    {
        var keys = _localizedDataKey;
        foreach (var key in _temporaryKey)
        {
            if (!keys.Contains(key))
            {
                keys.Add(key);
            }
        }
        SaveJsonInfo(DATA_JSON_ALL_KEY_PATH, keys);
    }

    /// <summary>
    /// 异步获取所有的键值
    /// </summary>
    /// <returns>所有的键值</returns>
    private async UniTask<List<string>> GetAllKeyAsync()
    {
        var text = await ReadFileAsync($"{_dataJsonPath}{DATA_JSON_ALL_KEY_PATH}");
        var info = JsonStringToObj<List<string>>(text);
        return info;
    }

    /// <summary>
    /// 获取所有的键值
    /// </summary>
    /// <returns>所有的键值</returns>
    private List<string> GetAllKey()
    {
        var text = ReadFile($"{_dataJsonPath}{DATA_JSON_ALL_KEY_PATH}");
        var info = JsonStringToObj<List<string>>(text);
        return info;
    }

    /// <summary>
    /// Json字符串转对象
    /// </summary>
    /// <returns></returns>
    public T JsonStringToObj<T>(string info)
    {
        var obj = JsonParser.ParseJson<T>(info);
        return obj;
    }

    /// <summary>
    /// 对象转Json字符串
    /// </summary>
    /// <returns></returns>
    public string ObjToJsonString<T>(T obj)
    {
        var info = JsonParser.ToJson<T>(obj);
        return info;
    }

    /// <summary>
    /// 异步读取文件信息
    /// </summary>
    /// <returns></returns>
    public async UniTask<string> ReadFileAsync(string name)
    {
        var filePath = $"{_dataJsonPath}{name}";
        var text = await FileHelper.LoadFileByStreamAsync(filePath);
        var info = Convert.ToBase64String(text);
        DataJsonAllInfo.Add(name, info);
        return info;
    }

    /// <summary>
    /// 读取文件信息
    /// </summary>
    /// <returns></returns>
    public string ReadFile(string name)
    {
        var filePath = $"{_dataJsonPath}{name}";
        var text = FileHelper.LoadFileByStream(filePath);
        var info = Convert.ToBase64String(text);
        DataJsonAllInfo.Add(name, info);
        return info;
    }

    /// <summary>
    /// 异步写入文件信息
    /// </summary>
    /// <returns></returns>
    public async UniTask WriteFileAsync(string name, string text)
    {
        var filePath = $"{_dataJsonPath}{name}";
        var info = Convert.FromBase64String(text);
        await FileHelper.SaveFileByStreamAsync(filePath, info);
    }

    /// <summary>
    /// 写入文件信息
    /// </summary>
    /// <returns></returns>
    public void WriteFile(string name, string text)
    {
        var filePath = $"{_dataJsonPath}{name}";
        var info = Convert.FromBase64String(text);
        FileHelper.SaveFileByStream(filePath, info);
    }
}
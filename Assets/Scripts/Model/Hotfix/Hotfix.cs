using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public sealed class Hotfix
{
#if ILRuntime
		private ILRuntime.Runtime.Enviorment.AppDomain appDomain;
		private MemoryStream dllStream;
		private MemoryStream pdbStream;
#else
    private Assembly assembly;
#endif

    public Hotfix Init()
    {

        return this;
    }

    public void Dispose()
    {
    }

    public void LoadHotfixAssembly()
    {

    }
}

public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	// UniTask.dll
	// Unity.Core.dll
	// mscorlib.dll
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>
	// Cysharp.Threading.Tasks.UniTask<object>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<object>
	// Model.Singleton<object>
	// }}

	public void RefMethods()
	{
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,Hotfix.UIHelper.<OpenUIView>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,Hotfix.UIHelper.<OpenUIView>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<Hotfix.UIHelper.<OpenUIView>d__1>(Hotfix.UIHelper.<OpenUIView>d__1&)
		// Model.Component Model.ObjectHelper.CreateComponent<object>(System.Type,Model.Entity,object,bool)
		// Cysharp.Threading.Tasks.UniTask<object> Model.ObjectHelper.CreateEntity<object>(Model.Entity,UnityEngine.Transform,string,bool,bool,bool)
		// object System.Activator.CreateInstance<object>()
	}
}
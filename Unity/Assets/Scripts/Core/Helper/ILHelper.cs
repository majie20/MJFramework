#if ILRuntime
using System;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.Utils;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ILRuntime.Runtime;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;

namespace Model
{
    public static class ILHelper
    {
        public static unsafe void InitILRuntime(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
        {
            // 注册委托
            appdomain.DelegateManager.RegisterMethodDelegate<float>();
            appdomain.DelegateManager.RegisterMethodDelegate<ILTypeInstance>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.String>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Cysharp.Threading.Tasks.UniTaskVoid>();

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((action) =>
            {
                return new UnityEngine.Events.UnityAction(() => { ((System.Action)action)(); });
            });

            // 注册适配器
            appdomain.RegisterCrossBindingAdaptor(new IDisposableAdapter());
            appdomain.RegisterCrossBindingAdaptor(new IAsyncStateMachineAdapter());
            appdomain.RegisterCrossBindingAdaptor(new BeanBaseAdapter());
            appdomain.RegisterCrossBindingAdaptor(new ComponentAdapter());
            appdomain.RegisterCrossBindingAdaptor(new UIBaseComponentAdapter());
            appdomain.RegisterValueTypeBinder(typeof(UnityEngine.Vector2), new Vector2Binder());
            appdomain.RegisterValueTypeBinder(typeof(UnityEngine.Vector3), new Vector3Binder());
            appdomain.RegisterValueTypeBinder(typeof(UnityEngine.Quaternion), new QuaternionBinder());

            // 注册重定向函数
            Model.Entity.RegisterILRuntimeCLRRedirection(appdomain);
            CatJson.ILRuntimeHelper.RegisterILRuntimeCLRRedirection(appdomain);
            //ProtoBuf.PType.RegisterILRuntimeCLRRedirection(appdomain);
            //ProtoBufRegister(appdomain);

            var debug_22 = typeof(NLog.Log).GetMethod("Debug", new System.Type[] { typeof(object) });
            var warn_22 = typeof(NLog.Log).GetMethod("Warn", new System.Type[] { typeof(object) });
            var error_22 = typeof(NLog.Log).GetMethod("Error", new System.Type[] { typeof(object) });
            var assert_22 = typeof(NLog.Log).GetMethod("Assert", new System.Type[] { typeof(bool), typeof(object) });

            appdomain.RegisterCLRMethodRedirection(debug_22, Debug_22);
            appdomain.RegisterCLRMethodRedirection(warn_22, Warn_22);
            appdomain.RegisterCLRMethodRedirection(error_22, Error_22);
            appdomain.RegisterCLRMethodRedirection(assert_22, Assert_22);
        }

        public static unsafe StackObject* Debug_22(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            //ILRuntime的调用约定为被调用者清理堆栈，因此执行这个函数后需要将参数从堆栈清理干净，并把返回值放在栈顶，具体请看ILRuntime实现原理文档
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            //这个是最后方法返回后esp栈指针的值，应该返回清理完参数并指向返回值，这里是只需要返回清理完参数的值即可
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            //取Log方法的参数，如果有两个参数的话，第一个参数是esp - 2,第二个参数是esp -1, 因为Mono的bug，直接-2值会错误，所以要调用ILIntepreter.Minus
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);

            //这里是将栈指针上的值转换成object，如果是基础类型可直接通过ptr->Value和ptr->ValueLow访问到值，具体请看ILRuntime实现原理文档
            //object message = typeof(object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            System.Object message =
                (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (ILRuntime.CLR.Utils.Extensions.TypeFlags)0);
            //所有非基础类型都得调用Free来释放托管堆栈
            __intp.Free(ptr_of_this_method);

            //在真实调用Debug.Log前，我们先获取DLL内的堆栈
            var stacktrace = __domain.DebugService.GetStackTrace(__intp);

            //我们在输出信息后面加上DLL堆栈
            NLog.Log.Debug($"{message}\n{stacktrace}");

            return __ret;
        }

        public static unsafe StackObject* Error_22(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            //ILRuntime的调用约定为被调用者清理堆栈，因此执行这个函数后需要将参数从堆栈清理干净，并把返回值放在栈顶，具体请看ILRuntime实现原理文档
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            //这个是最后方法返回后esp栈指针的值，应该返回清理完参数并指向返回值，这里是只需要返回清理完参数的值即可
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            //取Log方法的参数，如果有两个参数的话，第一个参数是esp - 2,第二个参数是esp -1, 因为Mono的bug，直接-2值会错误，所以要调用ILIntepreter.Minus
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);

            //这里是将栈指针上的值转换成object，如果是基础类型可直接通过ptr->Value和ptr->ValueLow访问到值，具体请看ILRuntime实现原理文档
            //object message = typeof(object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            System.Object message =
                (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (ILRuntime.CLR.Utils.Extensions.TypeFlags)0);
            //所有非基础类型都得调用Free来释放托管堆栈
            __intp.Free(ptr_of_this_method);

            //在真实调用Debug.Log前，我们先获取DLL内的堆栈
            var stacktrace = __domain.DebugService.GetStackTrace(__intp);

            //我们在输出信息后面加上DLL堆栈
            NLog.Log.Error($"{message}\n{stacktrace}");

            return __ret;
        }

        public static unsafe StackObject* Warn_22(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            //ILRuntime的调用约定为被调用者清理堆栈，因此执行这个函数后需要将参数从堆栈清理干净，并把返回值放在栈顶，具体请看ILRuntime实现原理文档
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            //这个是最后方法返回后esp栈指针的值，应该返回清理完参数并指向返回值，这里是只需要返回清理完参数的值即可
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            //取Log方法的参数，如果有两个参数的话，第一个参数是esp - 2,第二个参数是esp -1, 因为Mono的bug，直接-2值会错误，所以要调用ILIntepreter.Minus
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);

            //这里是将栈指针上的值转换成object，如果是基础类型可直接通过ptr->Value和ptr->ValueLow访问到值，具体请看ILRuntime实现原理文档
            //object message = typeof(object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            System.Object message =
                (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (ILRuntime.CLR.Utils.Extensions.TypeFlags)0);
            //所有非基础类型都得调用Free来释放托管堆栈
            __intp.Free(ptr_of_this_method);

            //在真实调用Debug.Log前，我们先获取DLL内的堆栈
            var stacktrace = __domain.DebugService.GetStackTrace(__intp);

            //我们在输出信息后面加上DLL堆栈
            NLog.Log.Warn($"{message}\n{stacktrace}");

            return __ret;
        }

        public static unsafe StackObject* Assert_22(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            //ILRuntime的调用约定为被调用者清理堆栈，因此执行这个函数后需要将参数从堆栈清理干净，并把返回值放在栈顶，具体请看ILRuntime实现原理文档
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;

            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);

            System.Object message =
                (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (ILRuntime.CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Boolean condition = ptr_of_this_method -> Value == 1;

            //在真实调用Debug.Log前，我们先获取DLL内的堆栈
            var stacktrace = __domain.DebugService.GetStackTrace(__intp);

            //我们在输出信息后面加上DLL堆栈
            NLog.Log.Assert(condition, $"{message}\n{stacktrace}");

            return __ret;
        }

        ////CLR重定向
        //public static unsafe void ProtoBufRegister(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
        //{
        //    //注册pb反序列化
        //    System.Type pbSerializeType = typeof(ProtoBuf.Serializer);
        //    var args = new[] { typeof(System.Type), typeof(Stream) };
        //    var pbDeserializeMethod = pbSerializeType.GetMethod("Deserialize", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, args, null);
        //    appdomain.RegisterCLRMethodRedirection(pbDeserializeMethod, Deserialize_1);
        //    args = new[] { typeof(ILTypeInstance) };
        //    Dictionary<string, List<MethodInfo>> genericMethods = new Dictionary<string, List<MethodInfo>>();
        //    List<MethodInfo> lst = null;

        //    foreach (var m in pbSerializeType.GetMethods())
        //    {
        //        if (m.IsGenericMethodDefinition)
        //        {
        //            if (!genericMethods.TryGetValue(m.Name, out lst))
        //            {
        //                lst = new List<MethodInfo>();
        //                genericMethods[m.Name] = lst;
        //            }

        //            lst.Add(m);
        //        }
        //    }

        //    if (genericMethods.TryGetValue("Deserialize", out lst))
        //    {
        //        foreach (var m in lst)
        //        {
        //            if (m.MatchGenericParameters(args, typeof(ILTypeInstance), typeof(Stream)))
        //            {
        //                var method = m.MakeGenericMethod(args);
        //                appdomain.RegisterCLRMethodRedirection(method, Deserialize_2);

        //                break;
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// pb net 反序列化重定向
        ///// </summary>
        ///// <param name="__intp"></param>
        ///// <param name="__esp"></param>
        ///// <param name="__mStack"></param>
        ///// <param name="__method"></param>
        ///// <param name="isNewObj"></param>
        ///// <returns></returns>
        //private static unsafe StackObject* Deserialize_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        //{
        //    ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        //    StackObject* ptr_of_this_method;
        //    StackObject* __ret = ILIntepreter.Minus(__esp, 2);

        //    ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        //    Stream source = (Stream)typeof(Stream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        //    __intp.Free(ptr_of_this_method);

        //    ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
        //    Type type = (Type)typeof(Type).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        //    __intp.Free(ptr_of_this_method);

        //    var result_of_this_method = ProtoBuf.Serializer.Deserialize(type, source);

        //    object obj_result_of_this_method = result_of_this_method;

        //    if (obj_result_of_this_method is CrossBindingAdaptorType adaptorType)
        //    {
        //        return ILIntepreter.PushObject(__ret, __mStack, adaptorType.ILInstance, true);
        //    }

        //    return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method, true);
        //}

        ///// <summary>
        ///// pb net 反序列化重定向
        ///// </summary>
        ///// <param name="__intp"></param>
        ///// <param name="__esp"></param>
        ///// <param name="__mStack"></param>
        ///// <param name="__method"></param>
        ///// <param name="isNewObj"></param>
        ///// <returns></returns>
        //private static unsafe StackObject* Deserialize_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        //{
        //    ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        //    StackObject* ptr_of_this_method;
        //    StackObject* __ret = ILIntepreter.Minus(__esp, 1);

        //    ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        //    Stream source = (Stream)typeof(Stream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        //    __intp.Free(ptr_of_this_method);

        //    var genericArgument = __method.GenericArguments;
        //    var type = genericArgument[0];
        //    var realType = type is CLRType ? type.TypeForCLR : type.ReflectionType;
        //    var result_of_this_method = ProtoBuf.Serializer.Deserialize(realType, source);

        //    return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        //}
    }
}
#endif
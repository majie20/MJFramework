using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class Model_Hotfix_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Model.Hotfix);
            args = new Type[]{};
            method = type.GetMethod("GetHotfixTypes", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetHotfixTypes_0);

            field = type.GetField("GameUpdate", flag);
            app.RegisterCLRFieldGetter(field, get_GameUpdate_0);
            app.RegisterCLRFieldSetter(field, set_GameUpdate_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_GameUpdate_0, AssignFromStack_GameUpdate_0);
            field = type.GetField("GameLateUpdate", flag);
            app.RegisterCLRFieldGetter(field, get_GameLateUpdate_1);
            app.RegisterCLRFieldSetter(field, set_GameLateUpdate_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_GameLateUpdate_1, AssignFromStack_GameLateUpdate_1);
            field = type.GetField("GameApplicationQuit", flag);
            app.RegisterCLRFieldGetter(field, get_GameApplicationQuit_2);
            app.RegisterCLRFieldSetter(field, set_GameApplicationQuit_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_GameApplicationQuit_2, AssignFromStack_GameApplicationQuit_2);


        }


        static StackObject* GetHotfixTypes_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Model.Hotfix instance_of_this_method = (Model.Hotfix)typeof(Model.Hotfix).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetHotfixTypes();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_GameUpdate_0(ref object o)
        {
            return ((Model.Hotfix)o).GameUpdate;
        }

        static StackObject* CopyToStack_GameUpdate_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Model.Hotfix)o).GameUpdate;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_GameUpdate_0(ref object o, object v)
        {
            ((Model.Hotfix)o).GameUpdate = (System.Action<System.Single>)v;
        }

        static StackObject* AssignFromStack_GameUpdate_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<System.Single> @GameUpdate = (System.Action<System.Single>)typeof(System.Action<System.Single>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((Model.Hotfix)o).GameUpdate = @GameUpdate;
            return ptr_of_this_method;
        }

        static object get_GameLateUpdate_1(ref object o)
        {
            return ((Model.Hotfix)o).GameLateUpdate;
        }

        static StackObject* CopyToStack_GameLateUpdate_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Model.Hotfix)o).GameLateUpdate;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_GameLateUpdate_1(ref object o, object v)
        {
            ((Model.Hotfix)o).GameLateUpdate = (System.Action)v;
        }

        static StackObject* AssignFromStack_GameLateUpdate_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @GameLateUpdate = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((Model.Hotfix)o).GameLateUpdate = @GameLateUpdate;
            return ptr_of_this_method;
        }

        static object get_GameApplicationQuit_2(ref object o)
        {
            return ((Model.Hotfix)o).GameApplicationQuit;
        }

        static StackObject* CopyToStack_GameApplicationQuit_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Model.Hotfix)o).GameApplicationQuit;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_GameApplicationQuit_2(ref object o, object v)
        {
            ((Model.Hotfix)o).GameApplicationQuit = (System.Action)v;
        }

        static StackObject* AssignFromStack_GameApplicationQuit_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @GameApplicationQuit = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((Model.Hotfix)o).GameApplicationQuit = @GameApplicationQuit;
            return ptr_of_this_method;
        }



    }
}

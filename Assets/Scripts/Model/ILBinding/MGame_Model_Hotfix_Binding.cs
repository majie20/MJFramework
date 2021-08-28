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
    unsafe class MGame_Model_Hotfix_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(MGame.Model.Hotfix);

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



        static object get_GameUpdate_0(ref object o)
        {
            return ((MGame.Model.Hotfix)o).GameUpdate;
        }

        static StackObject* CopyToStack_GameUpdate_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((MGame.Model.Hotfix)o).GameUpdate;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_GameUpdate_0(ref object o, object v)
        {
            ((MGame.Model.Hotfix)o).GameUpdate = (MGame.Model.GameUpdate)v;
        }

        static StackObject* AssignFromStack_GameUpdate_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            MGame.Model.GameUpdate @GameUpdate = (MGame.Model.GameUpdate)typeof(MGame.Model.GameUpdate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((MGame.Model.Hotfix)o).GameUpdate = @GameUpdate;
            return ptr_of_this_method;
        }

        static object get_GameLateUpdate_1(ref object o)
        {
            return ((MGame.Model.Hotfix)o).GameLateUpdate;
        }

        static StackObject* CopyToStack_GameLateUpdate_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((MGame.Model.Hotfix)o).GameLateUpdate;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_GameLateUpdate_1(ref object o, object v)
        {
            ((MGame.Model.Hotfix)o).GameLateUpdate = (MGame.Model.GameLateUpdate)v;
        }

        static StackObject* AssignFromStack_GameLateUpdate_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            MGame.Model.GameLateUpdate @GameLateUpdate = (MGame.Model.GameLateUpdate)typeof(MGame.Model.GameLateUpdate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((MGame.Model.Hotfix)o).GameLateUpdate = @GameLateUpdate;
            return ptr_of_this_method;
        }

        static object get_GameApplicationQuit_2(ref object o)
        {
            return ((MGame.Model.Hotfix)o).GameApplicationQuit;
        }

        static StackObject* CopyToStack_GameApplicationQuit_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((MGame.Model.Hotfix)o).GameApplicationQuit;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_GameApplicationQuit_2(ref object o, object v)
        {
            ((MGame.Model.Hotfix)o).GameApplicationQuit = (MGame.Model.GameApplicationQuit)v;
        }

        static StackObject* AssignFromStack_GameApplicationQuit_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            MGame.Model.GameApplicationQuit @GameApplicationQuit = (MGame.Model.GameApplicationQuit)typeof(MGame.Model.GameApplicationQuit).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((MGame.Model.Hotfix)o).GameApplicationQuit = @GameApplicationQuit;
            return ptr_of_this_method;
        }



    }
}

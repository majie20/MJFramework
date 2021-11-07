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
    unsafe class Model_ComponentView_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Model.ComponentView);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("set_isHotfix", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_isHotfix_0);

            field = type.GetField("dic", flag);
            app.RegisterCLRFieldGetter(field, get_dic_0);
            app.RegisterCLRFieldSetter(field, set_dic_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_dic_0, AssignFromStack_dic_0);


        }


        static StackObject* set_isHotfix_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Model.ComponentView instance_of_this_method = (Model.ComponentView)typeof(Model.ComponentView).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.isHotfix = value;

            return __ret;
        }


        static object get_dic_0(ref object o)
        {
            return ((Model.ComponentView)o).dic;
        }

        static StackObject* CopyToStack_dic_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Model.ComponentView)o).dic;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_dic_0(ref object o, object v)
        {
            ((Model.ComponentView)o).dic = (System.Collections.Generic.Dictionary<System.Object, System.Type>)v;
        }

        static StackObject* AssignFromStack_dic_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.Object, System.Type> @dic = (System.Collections.Generic.Dictionary<System.Object, System.Type>)typeof(System.Collections.Generic.Dictionary<System.Object, System.Type>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Model.ComponentView)o).dic = @dic;
            return ptr_of_this_method;
        }



    }
}

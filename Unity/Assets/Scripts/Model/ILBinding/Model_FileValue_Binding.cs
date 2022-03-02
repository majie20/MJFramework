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
    unsafe class Model_FileValue_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Model.FileValue);

            field = type.GetField("BUILD_AB_RES_PATH", flag);
            app.RegisterCLRFieldGetter(field, get_BUILD_AB_RES_PATH_0);
            app.RegisterCLRFieldSetter(field, set_BUILD_AB_RES_PATH_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_BUILD_AB_RES_PATH_0, AssignFromStack_BUILD_AB_RES_PATH_0);


        }



        static object get_BUILD_AB_RES_PATH_0(ref object o)
        {
            return Model.FileValue.BUILD_AB_RES_PATH;
        }

        static StackObject* CopyToStack_BUILD_AB_RES_PATH_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = Model.FileValue.BUILD_AB_RES_PATH;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_BUILD_AB_RES_PATH_0(ref object o, object v)
        {
            Model.FileValue.BUILD_AB_RES_PATH = (System.String)v;
        }

        static StackObject* AssignFromStack_BUILD_AB_RES_PATH_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @BUILD_AB_RES_PATH = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            Model.FileValue.BUILD_AB_RES_PATH = @BUILD_AB_RES_PATH;
            return ptr_of_this_method;
        }



    }
}

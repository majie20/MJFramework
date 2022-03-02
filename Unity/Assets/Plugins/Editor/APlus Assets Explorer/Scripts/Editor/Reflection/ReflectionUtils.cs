#if UNITY_EDITOR
using System;
using System.Reflection;

namespace APlus
{
    public class ReflectionUtils
    {
        public static BindingFlags BIND_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.GetProperty;
        public static object[] EMPTY_ARGUMENTS = new object[0];

        public static void RegisterMethod(Type type, string methodName, ref Action<object> registMethod)
        {
            var method = type.GetMethod(methodName);
            registMethod = instance =>
            {
                method.Invoke(instance, EMPTY_ARGUMENTS);
            };
        }

        public static void RegisterMethod<T>(Type type, string methodName, ref Action<object, T> registMethod)
        {
            var method = GetMethodFromType(type, methodName);
            registMethod = (instance, arg1) =>
            {
                method.Invoke(instance, new object[] { arg1 });
            };
        }

        public static void RegisterStaticMethod<T>(Type type, string methodName, ref Func<object, T> registMethod)
        {
            var method = GetMethodFromType(type, methodName);
            registMethod = arg1 =>
            {
                var retunVal = method.Invoke(null, new object[] { arg1 });
                return (T)retunVal;
            };
        }

        public static void RegisterStaticMethod<T1, T2>(Type type, string methodName, ref Func<object, T1, T2> registMethod)
        {
            var method = GetMethodFromType(type, methodName);
            registMethod = (arg1, arg2) =>
            {
                var retunVal = method.Invoke(null, new object[] { arg1, arg2 });
                return (T2)retunVal;
            };
        }

        public static void RegisterMethod<T>(Type type, string methodName, ref Func<object, object, T> registMethod)
        {
            var method = GetMethodFromType(type, methodName);
            registMethod = (instance, arg1) =>
            {
                var retunVal = method.Invoke(instance, new object[] { arg1 });
                return (T)retunVal;
            };
        }

        public static void RegisterMethod<T>(Type type, string methodName, ref Func<object, T> registMethod)
        {
            var method = GetMethodFromType(type, methodName);
            registMethod = (instance) =>
            {
                var retunVal = method.Invoke(instance, EMPTY_ARGUMENTS);
                return (T)retunVal;
            };
        }

        public static void RegisterMethod<T1, T2, T3>(Type type, string methodName, ref Func<object, T1, T2, T3> registMethod)
        {
            var method = GetMethodFromType(type, methodName);
            registMethod = (instance, arg1, arg2) =>
            {
                var retunVal = method.Invoke(instance, new object[] { arg1, arg2 });
                return (T3)retunVal;
            };
        }

        public static MethodInfo GetMethodFromType(Type type, string methodName)
        {
            var method = type.GetMethod(methodName, BIND_FLAGS);
            if (method == null)
            {
                throw new Exception(string.Format("method '{0}' is not in type {1}", methodName, type.ToString()));
            }

            return method;
        }
    }
}
#endif
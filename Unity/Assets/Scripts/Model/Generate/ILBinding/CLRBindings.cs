using System;
using System.Collections.Generic;
using System.Reflection;

namespace ILRuntime.Runtime.Generated
{
    class CLRBindings
    {

//will auto register in unity
#if UNITY_5_3_OR_NEWER
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        static private void RegisterBindingAction()
        {
            ILRuntime.Runtime.CLRBinding.CLRBindingUtils.RegisterBindingAction(Initialize);
        }

        internal static ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector2> s_UnityEngine_Vector2_Binding_Binder = null;
        internal static ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector3> s_UnityEngine_Vector3_Binding_Binder = null;
        internal static ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Quaternion> s_UnityEngine_Quaternion_Binding_Binder = null;

        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            System_Collections_Generic_Dictionary_2_String_Object_Binding.Register(app);
            System_Func_2_String_JSONNode_Binding.Register(app);
            System_Object_Binding.Register(app);
            System_String_Binding.Register(app);
            SimpleJSON_JSONNode_Binding.Register(app);
            Bright_Serialization_SerializationException_Binding.Register(app);
            System_Collections_Generic_List_1_Model_BeanBaseAdapter_Binding_Adapter_Binding.Register(app);
            System_Collections_Generic_List_1_Model_BeanBaseAdapter_Binding_Adapter_Binding_Enumerator_Binding.Register(app);
            System_IDisposable_Binding.Register(app);
            System_Boolean_Binding.Register(app);
            Bright_Common_StringUtil_Binding.Register(app);
            UnityEngine_Vector2_Binding.Register(app);
            System_Int32_Binding.Register(app);
            System_Collections_Generic_IEnumerable_1_JSONNode_Binding.Register(app);
            System_Collections_Generic_IEnumerator_1_JSONNode_Binding.Register(app);
            System_Collections_IEnumerator_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_Int32_Binding.Register(app);
            System_Func_3_String_String_String_Binding.Register(app);
            UnityEngine_Vector3_Binding.Register(app);
            UnityEngine_Vector4_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_Model_BeanBaseAdapter_Binding_Adapter_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_Int32_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_Int32_Int32_Binding.Register(app);
            System_Collections_Generic_List_1_Int32_Binding.Register(app);
            System_Collections_Generic_HashSet_1_Int32_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_Model_BeanBaseAdapter_Binding_Adapter_Binding_ValueCollection_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_Model_BeanBaseAdapter_Binding_Adapter_Binding_ValueCollection_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int64_Model_BeanBaseAdapter_Binding_Adapter_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Model_BeanBaseAdapter_Binding_Adapter_Binding.Register(app);
            System_ValueTuple_3_Int32_Int64_String_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_ValueTuple_3_Int32_Int64_String_Model_BeanBaseAdapter_Binding_Adapter_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int64_Int32_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Int32_Binding.Register(app);
            System_Nullable_1_Int32_Binding.Register(app);
            System_Collections_Generic_List_1_Int32_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_HashSet_1_Model_BeanBaseAdapter_Binding_Adapter_Binding.Register(app);
            System_Collections_Generic_HashSet_1_Int32_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_List_1_Int64_Binding.Register(app);
            System_Collections_Generic_List_1_String_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Component_Int32_Binding.Register(app);
            NLog_Log_Binding.Register(app);
            System_Func_2_ILTypeInstance_Boolean_Binding.Register(app);
            Model_Component_Binding.Register(app);
            Model_Singleton_1_Game_Binding.Register(app);
            Model_Game_Binding.Register(app);
            Model_Entity_Binding.Register(app);
            Model_FileValue_Binding.Register(app);
            Model_AssetsComponent_Binding.Register(app);
            UnityEngine_TextAsset_Binding.Register(app);
            SimpleJSON_JSON_Binding.Register(app);
            UnityEngine_Behaviour_Binding.Register(app);
            System_Threading_Monitor_Binding.Register(app);
            System_Activator_Binding.Register(app);
            System_Threading_Thread_Binding.Register(app);
            System_Collections_Generic_HashSet_1_Type_Binding.Register(app);
            Model_Hotfix_Binding.Register(app);
            System_Collections_Generic_List_1_Type_Binding.Register(app);
            System_Collections_Generic_List_1_Type_Binding_Enumerator_Binding.Register(app);
            System_Type_Binding.Register(app);
            System_Reflection_MemberInfo_Binding.Register(app);
            Model_UIBaseComponent_Binding.Register(app);
            Model_ObjectHelper_Binding.Register(app);
            Cysharp_Threading_Tasks_CompilerServices_AsyncUniTaskMethodBuilder_1_UIBaseComponent_Binding.Register(app);
            Cysharp_Threading_Tasks_UniTask_1_UIBaseComponent_Binding.Register(app);
            Cysharp_Threading_Tasks_UniTask_1_UIBaseComponent_Binding_Awaiter_Binding.Register(app);

            ILRuntime.CLR.TypeSystem.CLRType __clrType = null;
            __clrType = (ILRuntime.CLR.TypeSystem.CLRType)app.GetType (typeof(UnityEngine.Vector2));
            s_UnityEngine_Vector2_Binding_Binder = __clrType.ValueTypeBinder as ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector2>;
            __clrType = (ILRuntime.CLR.TypeSystem.CLRType)app.GetType (typeof(UnityEngine.Vector3));
            s_UnityEngine_Vector3_Binding_Binder = __clrType.ValueTypeBinder as ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector3>;
            __clrType = (ILRuntime.CLR.TypeSystem.CLRType)app.GetType (typeof(UnityEngine.Quaternion));
            s_UnityEngine_Quaternion_Binding_Binder = __clrType.ValueTypeBinder as ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Quaternion>;
        }

        /// <summary>
        /// Release the CLR binding, please invoke this BEFORE ILRuntime Appdomain destroy
        /// </summary>
        public static void Shutdown(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            s_UnityEngine_Vector2_Binding_Binder = null;
            s_UnityEngine_Vector3_Binding_Binder = null;
            s_UnityEngine_Quaternion_Binding_Binder = null;
        }
    }
}

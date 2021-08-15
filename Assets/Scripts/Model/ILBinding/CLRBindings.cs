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


        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            System_Collections_Generic_Dictionary_2_String_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_ILTypeInstance_Binding.Register(app);
            UnityEngine_Object_Binding.Register(app);
            UnityEngine_GameObject_Binding.Register(app);
            UnityEngine_Transform_Binding.Register(app);
            MGame_Model_ComponentView_Binding.Register(app);
            System_Collections_Generic_IEnumerable_1_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_IEnumerator_1_ILTypeInstance_Binding.Register(app);
            System_Collections_IEnumerator_Binding.Register(app);
            System_IDisposable_Binding.Register(app);
            System_Object_Binding.Register(app);
            System_Type_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Object_Type_Binding.Register(app);
            System_Linq_Enumerable_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_Queue_1_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Queue_1_ILTypeInstance_Binding.Register(app);
            System_Activator_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Queue_1_GameObject_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Transform_Binding.Register(app);
            System_Collections_Generic_Queue_1_GameObject_Binding.Register(app);
            MGame_Model_Singleton_1_Game_Binding.Register(app);
            MGame_Model_Game_Binding.Register(app);
            MGame_Model_Entity_Binding.Register(app);
            MGame_Model_ABComponent_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_ILTypeInstance_Binding_ValueCollection_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Type_ILTypeInstance_Binding_ValueCollection_Binding_Enumerator_Binding.Register(app);
            UnityEngine_Component_Binding.Register(app);
            MGame_Model_BodyConstructor_Binding.Register(app);
            System_Reflection_CustomAttributeExtensions_Binding.Register(app);
            System_Action_Binding.Register(app);
            DG_Tweening_DOTween_Binding.Register(app);
            System_Collections_Generic_IEnumerable_1_ILRuntime_Runtime_Adapters_AttributeAdapter_Binding_Adapter_Binding.Register(app);
            System_Collections_Generic_IEnumerator_1_ILRuntime_Runtime_Adapters_AttributeAdapter_Binding_Adapter_Binding.Register(app);
            UnityEngine_RectTransform_Binding.Register(app);
            UnityEngine_Rect_Binding.Register(app);
            UnityEngine_Vector2_Binding.Register(app);
            DG_Tweening_DOTweenModuleUI_Binding.Register(app);
            DG_Tweening_TweenSettingsExtensions_Binding.Register(app);
            System_Collections_Generic_List_1_Action_1_Object_Binding.Register(app);
            System_Collections_ArrayList_Binding.Register(app);
            UnityEngine_Vector3_Binding.Register(app);
            DG_Tweening_ShortcutExtensions_Binding.Register(app);
            UnityEngine_CanvasGroup_Binding.Register(app);
            UnityEngine_UI_Graphic_Binding.Register(app);
            UnityEngine_Color_Binding.Register(app);
            System_Action_1_Object_Binding.Register(app);
            System_Threading_Monitor_Binding.Register(app);
            System_Threading_Thread_Binding.Register(app);
        }

        /// <summary>
        /// Release the CLR binding, please invoke this BEFORE ILRuntime Appdomain destroy
        /// </summary>
        public static void Shutdown(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
        }
    }
}

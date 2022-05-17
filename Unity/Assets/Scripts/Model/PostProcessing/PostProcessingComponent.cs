using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Model
{
    [LifeCycle]
    public class PostProcessingComponent : Component, IAwake
    {
        private PostProcessVolume defaultPPV;

        private Dictionary<PostProcessVolume, Dictionary<Type, BaseEffect>> settings;

        public override void Dispose()
        {
            defaultPPV = null;
            settings = null;
            base.Dispose();
        }
        public void Awake()
        {
            defaultPPV = GameObject.Find("Main Camera").GetComponent<PostProcessVolume>();
            //defaultPPV = Camera.main.gameObject.GetComponent<PostProcessVolume>();
            settings =new Dictionary<PostProcessVolume, Dictionary<Type, BaseEffect>>();
        }

        public BaseEffect AddEffect<T>(PostProcessVolume v = null) where T : BaseEffect
        {
            var type = typeof(T);
            v = v ? v : defaultPPV;

            Dictionary<Type, BaseEffect> allEffects;
            BaseEffect effect;
            var parameters = new object[1];
            parameters[0] = v;
            BaseEffect typeEffect;
            try
            {
                typeEffect = Activator.CreateInstance(type, parameters) as BaseEffect;
                if (!settings.TryGetValue(v, out allEffects))
                {
                    var te = new Dictionary<Type, BaseEffect> { { type, typeEffect } };
                    settings.Add(v, te);
                }
                else if (!allEffects.TryGetValue(type, out effect))
                {
                    allEffects.Add(type, typeEffect);
                }
            }
            catch (Exception e)
            {
                typeEffect = null;
                NLog.Log.Error("禁止使用基类");
            }
            return typeEffect;
        }

        public void ShowEffect<T>(PostProcessVolume v=null)where T:BaseEffect
        {
            var type = typeof(T);
            v=v?v: defaultPPV;
            Dictionary<Type, BaseEffect> allEffects;
            BaseEffect effect;
            if (!(settings.TryGetValue(v, out allEffects) && allEffects.TryGetValue(type, out effect)))
            {
                effect = AddEffect<T>(v);
            }
            effect?.ShowEffect();
        }

        public void RecoverEffect<T>(PostProcessVolume v = null) where T : BaseEffect
        {
            var type = typeof(T);
            v = v ? v : defaultPPV;
            Dictionary<Type, BaseEffect> allEffects;
            BaseEffect effect;
            if (settings.TryGetValue(v, out allEffects) && allEffects.TryGetValue(type, out effect))
            {
                effect?.RecoverEffect();
            }
            else
            {
                Debug.Log("未添加" + type + "效果");
            }
        }

        public void ShowLerpEffect<T>(float t, PostProcessVolume v = null) where T : BaseEffect
        {
            var type = typeof(T);
            v = v ? v : defaultPPV;
            Dictionary<Type, BaseEffect> allEffects;
            BaseEffect effect;
            if (!(settings.TryGetValue(v, out allEffects) && allEffects.TryGetValue(type, out effect)))
            {
                effect = AddEffect<T>(v);
            }
            effect?.ShowLerpEffect(t);
        }

        public void RecoverLerpEffect<T>(float t,PostProcessVolume v = null) where T : BaseEffect
        {
            var type = typeof(T);
            v = v ? v : defaultPPV;
            Dictionary<Type, BaseEffect> allEffects;
            BaseEffect effect;
            if (settings.TryGetValue(v, out allEffects) && allEffects.TryGetValue(type, out effect))
            {
                effect?.RecoverLerpEffect(t);
            }
            else
            {
                Debug.Log("未添加" + type + "效果");
            }
        }
    }
}

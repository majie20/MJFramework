using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public class UnitDataComponent : Component, IAwake
    {
        private UnitInfoSettings _unitInfoSettings;

        private Dictionary<string, AssetReferenceSettings> _assetReferenceSettingsMap;

        public void Awake()
        {
            _assetReferenceSettingsMap = new Dictionary<string, AssetReferenceSettings>();

            AssetsComponent component = Game.Instance.Scene.GetComponent<AssetsComponent>();
            _unitInfoSettings = component.LoadSync<UnitInfoSettings>(ConstData.UNIT_INFO_SETTINGS);
        }

        public override void Dispose()
        {
            _assetReferenceSettingsMap = null;
            _unitInfoSettings = null;
            base.Dispose();
        }

        public UnitInfo GetInfoBySign(string sign)
        {
            if (_unitInfoSettings.UnitInfoMap.TryGetValue(sign, out UnitInfo info))
            {
                return info;
            }

            return null;
        }

        private Entity CreateUnit0(string sign, Entity eParent, Transform parent = null, bool isParentCanNull = false)
        {
            var info = GetInfoBySign(sign);

            if (info != null)
            {
                var settings = _assetReferenceSettingsMap[sign];

                var entity = ObjectHelper.CreateEntity<EventEntity>(eParent, parent, settings.InstantlyAssetDataList[0].Path, true, isParentCanNull);
                ObjectHelper.CreateComponent<UnitComponent>(entity);
                var unitComponent = entity.GetComponent<UnitComponent>();
                unitComponent.Type = info.Type;
                ObjectHelper.CreateComponents(entity, UnitValue.UnitTypeMap[info.Type]);

                return entity;
            }

            NLog.Log.Error($"{sign}，没有这个Unit的预制体！");

            return null;
        }

        public Entity CreateUnitLocal(string sign,
        Entity                               eParent,
        Vector3                              localPosition,
        Quaternion                           localRotation,
        Vector3                              localScale,
        Transform                            parent          = null,
        bool                                 isParentCanNull = false)
        {
            var entity = CreateUnit0(sign, eParent, parent, isParentCanNull);

            if (!(entity is null))
            {
                entity.Transform.localPosition = localPosition;
                entity.Transform.localRotation = localRotation;
                entity.Transform.localScale = localScale;
            }

            return entity;
        }

        public Entity CreateUnitWorld(string sign, Entity eParent, Vector3 position, Quaternion rotation, Vector3 localScale, Transform parent = null, bool isParentCanNull = false)
        {
            var entity = CreateUnit0(sign, eParent, parent, isParentCanNull);

            if (!(entity is null))
            {
                entity.Transform.SetPositionAndRotation(position, rotation);
                entity.Transform.localScale = localScale;
            }

            return entity;
        }

        public Entity CreateUnit(string sign, Entity eParent, Transform parent = null, Transform place = null, bool isParentCanNull = false)
        {
            var entity = CreateUnit0(sign, eParent, parent, isParentCanNull);

            if (!(entity is null))
            {
                if (place is null)
                {
                    entity.Transform.localPosition = Vector3.zero;
                    entity.Transform.localRotation = Quaternion.identity;
                    entity.Transform.localScale = Vector3.one;
                }
                else
                {
                    entity.Transform.SetPositionAndRotation(place.position, place.rotation);
                    entity.Transform.localScale = place.localScale;
                }
            }

            return entity;
        }

        public async UniTask LoadPlayerReferenceRes(string sign)
        {
            var info = GetInfoBySign(sign);

            if (info != null)
            {
                if (!_assetReferenceSettingsMap.ContainsKey(sign))
                {
                    AssetsComponent component = Game.Instance.Scene.GetComponent<AssetsComponent>();
                    await component.LoadAssetReferenceSettingsAsync(info.AssetReferenceSettingsPath);

                    if (!_assetReferenceSettingsMap.ContainsKey(sign))
                    {
                        _assetReferenceSettingsMap.Add(sign, component.LoadSync<AssetReferenceSettings>(info.AssetReferenceSettingsPath));
                    }
                }
            }
        }

        public void UnloadPlayerReferenceRes(string sign)
        {
            var info = GetInfoBySign(sign);

            if (info != null)
            {
                if (_assetReferenceSettingsMap.ContainsKey(sign))
                {
                    _assetReferenceSettingsMap.Remove(sign);
                    AssetsComponent component = Game.Instance.Scene.GetComponent<AssetsComponent>();
                    component.UnloadAssetReferenceSettings(info.AssetReferenceSettingsPath);
                }
            }
        }
    }
}
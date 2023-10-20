using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;

namespace M.ProductionPipeline
{
    public class FieldData
    {
        public string Data;
        public string Name;
        public string AssemblyName;
        public string TypeFullName;
    }

    public class StepSave
    {
        public int                   Index;
        public List<string>          FullNames;
        public List<List<FieldData>> Steps;
    }

    [Serializable]
    public class Step
    {
        [ValueDropdown("AllSteps", ExpandAllMenuItems = true)]
        public string FullName;

        public bool IsRunning = true;

        public string Fields;

        private static IEnumerable AllSteps = new ValueDropdownList<string>()
        {
            { nameof(AddDefineStep), typeof(AddDefineStep).FullName },
            { nameof(RemoveDefineStep), typeof(RemoveDefineStep).FullName },
            { nameof(BuildPlayModeStep), typeof(BuildPlayModeStep).FullName },
            { nameof(CheckResReferenceStep), typeof(CheckResReferenceStep).FullName },
            { nameof(CheckUIPrefabReferenceAtlasStep), typeof(CheckUIPrefabReferenceAtlasStep).FullName },
            { nameof(CollectAllUnitInfoStep), typeof(CollectAllUnitInfoStep).FullName },
            { nameof(DevelopPlayModeStep), typeof(DevelopPlayModeStep).FullName },
            { nameof(ExportAllAssetCollectorStep), typeof(ExportAllAssetCollectorStep).FullName },
            { nameof(ExportAllUISpriteAtlasStep), typeof(ExportAllUISpriteAtlasStep).FullName },
            { nameof(GenCodeBinStep), typeof(GenCodeBinStep).FullName },
            { nameof(GenCodeJsonStep), typeof(GenCodeJsonStep).FullName },
            { nameof(HotfixAnyStep), typeof(HotfixAnyStep).FullName },
            { nameof(HotfixEditorStep), typeof(HotfixEditorStep).FullName },
            { nameof(HybridCLRGenerateAllStep), typeof(HybridCLRGenerateAllStep).FullName },
            { nameof(OpenHybridCLRStep), typeof(OpenHybridCLRStep).FullName },
            { nameof(CloseHybridCLRStep), typeof(CloseHybridCLRStep).FullName },
            { nameof(ILRuntimeCLRBindingStep), typeof(ILRuntimeCLRBindingStep).FullName },
            { nameof(ILRuntimeCLRClearStep), typeof(ILRuntimeCLRClearStep).FullName },
            { nameof(OpenILRuntimeStep), typeof(OpenILRuntimeStep).FullName },
            { nameof(CloseILRuntimeStep), typeof(CloseILRuntimeStep).FullName },
            { nameof(SetAllResFormatStep), typeof(SetAllResFormatStep).FullName },
        };
    }
}
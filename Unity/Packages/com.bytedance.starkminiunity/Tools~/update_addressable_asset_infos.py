import fnmatch
import json
import os
import sys
import traceback

import subprocess

def install(package):
    subprocess.check_call([sys.executable, "-m", "pip", "install", "--user", package])


install("unityparser")

from unityparser import UnityDocument
from unityparser.constants import OrderedFlowDict


class ObjectID:
    def __init__(self, name="", guid="", file_id=0):
        self.name = name
        self.guid = guid
        self.file_id = file_id

    def __str__(self):
        return "name: {}, guid: {}, fileID: {}".format(self.name, self.guid, self.file_id)


def get_object_id_map(object_id_path):
    fp = open(object_id_path)
    json_data = json.load(fp)
    fp.close()
    object_map = dict()
    for item in json_data:
        object_map[item["name"]] = ObjectID(item["name"], item["guid"], item["fileID"])
    return object_map


def find_files(directory, pattern):
    for root, dirs, files in os.walk(directory):
        for basename in files:
            if fnmatch.fnmatch(basename, pattern):
                filename = os.path.join(root, basename)
                yield filename


def update_script_value(file_path, object_id, entry_name):
    doc = UnityDocument.load_yaml(file_path)
    asset_name = os.path.basename(file_path)
    asset_name_no_ext = os.path.splitext(asset_name)[0]
    target_entry = doc.entry
    if entry_name and entry_name != asset_name_no_ext and target_entry.m_Name == asset_name_no_ext:
        target_entry = doc.entry
        # print(" - use doc.entry for: \"{}\", {}".format(target_entry.m_Name, object_id))
    if entry_name and doc.entries:
        for entry in doc.entries:
            if entry.m_Name == entry_name:
                target_entry = entry
                # print(" - use entry for: \"{}\", {}".format(entry.m_Name, object_id))
    script_type = "type" in target_entry.m_Script and target_entry.m_Script["type"] or 3
    new_value = OrderedFlowDict()
    new_value.flow_style = True
    new_value["fileID"] = object_id.file_id
    new_value["guid"] = object_id.guid
    new_value["type"] = script_type
    target_entry.m_Script = new_value
    print(" - update asset: \"{}\", {}".format(asset_name, object_id))
    doc.dump_yaml()


def update_monobehaviour_script(object_id_map, root_dir, replace_item):
    entries = "entries" in replace_item and replace_item["entries"] or None
    file_pattern = replace_item["file_pattern"]
    if entries:
        for entries in entries:
            name = entries["name"]
            key = entries["key"]
            if key not in object_id_map:
                print(" - error: key: '{}' not found in object_id_map".format(key), file=sys.stderr)
                continue
            new_value = object_id_map[key]
            file_path = os.path.join(root_dir, file_pattern)
            update_script_value(file_path, new_value, name)
    else:
        key = replace_item["key"]
        if key not in object_id_map:
            print(" - error: key: '{}' not found in object_id_map".format(key), file=sys.stderr)
            return
        new_value = object_id_map[key]
        dir_name, filename = os.path.split(file_pattern)
        root_dir = os.path.join(root_dir, dir_name)
        candidate_files = find_files(root_dir, filename)
        for file_path in candidate_files:
            update_script_value(file_path, new_value, None)


def get_all_asset_files(dir_path):
    all_files = []
    for file_path in find_files(dir_path, '*.asset'):
        all_files.append(file_path)
    return all_files


def update_assembly_name(dst_asset_files, assembly_name):
    for file_path in dst_asset_files:
        doc = UnityDocument.load_yaml(file_path)
        all_fields = doc.entry.__dict__
        for field in all_fields:
            if isinstance(all_fields[field], OrderedFlowDict) and "m_AssemblyName" in all_fields[field]:
                item = getattr(doc.entry, field)
                if item["m_AssemblyName"]:
                    print(" - update assembly name {} -> {} in {}".format(item["m_AssemblyName"], assembly_name, file_path))
                    item["m_AssemblyName"] = assembly_name
                    setattr(doc.entry, field, item)
        doc.dump_yaml()


def main(addressable_data_dir, json_path, is_from_dll=True):
    replaces_map = [
        {"key": "AddressableAssetSettings", "file_pattern": "AddressableAssetSettings.asset"},
        {"key": "CacheInitializationSettings", "file_pattern": "CacheInitializationSettings.asset"},
        {"key": "AddressableAssetSettingsDefaultObject", "file_pattern": "DefaultObject.asset"},
        {"key": "BuildScriptFastMode", "file_pattern": "DataBuilders/BuildScriptFastMode.asset"},
        {"key": "BuildScriptVirtualMode", "file_pattern": "DataBuilders/BuildScriptVirtualMode.asset"},
        {"key": "BuildScriptPackedPlayMode", "file_pattern": "DataBuilders/BuildScriptPackedPlayMode.asset"},
        {"key": "BuildScriptPackedMode", "file_pattern": "DataBuilders/BuildScriptPackedMode.asset"},
        {"key": "AddressableAssetGroupTemplate", "file_pattern": "AssetGroupTemplates/Packed Assets.asset",
            "entries": [
                {"name": "Packed Assets", "key": "AddressableAssetGroupTemplate"},
                {"name": "BundledAssetGroupSchema", "key": "BundledAssetGroupSchema"},
                {"name": "ContentUpdateGroupSchema", "key": "ContentUpdateGroupSchema"},
            ] 
        },
        {"key": "AddressableAssetGroup", "file_pattern": "AssetGroups/*.asset"},
        {"key": "BundledAssetGroupSchema", "file_pattern": "AssetGroups/Schemas/*_BundledAssetGroupSchema.asset"},
        {"key": "ContentUpdateGroupSchema", "file_pattern": "AssetGroups/Schemas/*_ContentUpdateGroupSchema.asset"},
        {"key": "PlayerDataGroupSchema", "file_pattern": "AssetGroups/Schemas/*_PlayerDataGroupSchema.asset"},
        {"key": "AnalyzeResultData", "file_pattern": "AnalyzeData/AnalyzeRuleData.asset"}
    ]
    if is_from_dll:
        assembly_name = "starkmini, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
    else:
        assembly_name = "Unity.ResourceManager, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"

    object_id_map = get_object_id_map(json_path)

    print("update_monobehaviour_script ...")
    for item in replaces_map:
        update_monobehaviour_script(object_id_map, addressable_data_dir, item)
    dst_asset_files = get_all_asset_files(addressable_data_dir)

    print("update_assembly_name ...")
    update_assembly_name(dst_asset_files, assembly_name)

    print("update_data_builders ...")
    update_data_builders(addressable_data_dir)


def update_data_builders(addressable_data_dir):
    doc = UnityDocument.load_yaml(os.path.join(addressable_data_dir, "AddressableAssetSettings.asset"))
    data_builders = get_all_data_builders(addressable_data_dir, get_asset_item_template(doc))
    if len(data_builders) > 0:
        doc.entry.m_DataBuilders = data_builders
        doc.dump_yaml()


def read_guid_from_meta_file(file_path):
    try:
        basename = os.path.basename(file_path)
        fp = open(file_path)
        lines = fp.readlines()
        fp.close()
        for line in lines:
            line = line.strip()
            if line.startswith("guid:"):
                guid = line.replace("guid:", "").strip()
                # print("read \"{}\" guid: {}".format(basename, guid))
                return guid
    except Exception as e:
        print(e, file=sys.stderr)
        print("error read guid for file: {}".format(file_path), file=sys.stderr)
    return ""


def get_all_data_builders(addressable_data_dir, item_template):
    data_builders_dir = os.path.join(addressable_data_dir, "DataBuilders")
    data_builders = []
    # m_DataBuilders.Add(CreateScriptAsset<BuildScriptFastMode>());
    # m_DataBuilders.Add(CreateScriptAsset<BuildScriptVirtualMode>());
    # m_DataBuilders.Add(CreateScriptAsset<BuildScriptPackedPlayMode>());
    # m_DataBuilders.Add(CreateScriptAsset<BuildScriptPackedMode>());
    builtin_builders = [
        "BuildScriptFastMode",
        "BuildScriptVirtualMode",
        "BuildScriptPackedPlayMode",
        "BuildScriptPackedMode",
    ]
    builtin_builders_paths = []
    data_builder_meta_files = []
    for builtin_builder in builtin_builders:
        file_path = os.path.join(data_builders_dir, builtin_builder + ".asset.meta")
        # print(" - check builtin_builder: \"{}\", path: \"{}\"".format(builtin_builder, file_path))
        if os.path.isfile(file_path):
            builtin_builders_paths.append(file_path)
            data_builder_meta_files.append(file_path)
            print(" - append found builtin_builder: \"{}\"".format(builtin_builder))

    found_meta_files = find_files(data_builders_dir, "*.meta")
    for file_path in found_meta_files:
        # print("check found meta: \"{}\"".format(file_path))
        if file_path in builtin_builders_paths:
            continue
        data_builder_meta_files.append(file_path)
        print(" - append found meta: \"{}\"".format(os.path.basename(file_path)))

    for file_path in data_builder_meta_files:
        guid = read_guid_from_meta_file(file_path)
        if guid != "":
            item = OrderedFlowDict()
            item.flow_style = True
            item["fileID"] = item_template["fileID"]
            item["guid"] = guid
            item["type"] = item_template["type"]
            data_builders.append(item)
            print(" - add builder: \"{}\", {}".format(os.path.basename(file_path), guid))
    return data_builders


def get_asset_item_template(doc):
    if len(doc.entry.m_GroupAssets) < 1:
        item = OrderedFlowDict()
        item.flow_style = True
        item["fileID"] = 11400000
        item["guid"] = ""
        item["type"] = 2
        return item
    return doc.entry.m_GroupAssets[0]


if __name__ == '__main__':
    if len(sys.argv) < 4:
        sys.stderr.write("Usage: {} addressable_data_dir json_path is_from_dll\n".format(sys.argv[0]))
        exit(1)
    try:
        is_from_dll = sys.argv[3] == "true"
        main(sys.argv[1], sys.argv[2], is_from_dll)
        print("Update Addressable Settings Assets Success")
    except Exception as e:
        traceback.print_exc()
        sys.stderr.write("\nException: {}\n".format(e))
        exit(1)

// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Animancer.Editor
{
    partial class AnimancerToolsWindow
    {
        /// <summary>[Editor-Only] [Pro-Only] A base <see cref="Panel"/> for modifying <see cref="Sprite"/>s.</summary>
        /// <remarks>
        /// Documentation: <see href="https://kybernetik.com.au/animancer/docs/manual/tools">Animancer Tools</see>
        /// </remarks>
        /// https://kybernetik.com.au/animancer/api/Animancer.Editor/SpriteModifierPanel
        /// 
        [Serializable]
        public abstract class SpriteModifierPanel : Panel
        {
            /************************************************************************************************************************/

            private static readonly List<Sprite> SelectedSprites = new List<Sprite>();
            private static bool _HasGatheredSprites;

            /// <summary>The currently selected <see cref="Sprite"/>s.</summary>
            public static List<Sprite> Sprites
            {
                get
                {
                    if (!_HasGatheredSprites)
                    {
                        _HasGatheredSprites = true;
                        GatherSelectedSprites(SelectedSprites);
                    }

                    return SelectedSprites;
                }
            }

            /// <inheritdoc/>
            public override void OnSelectionChanged()
            {
                _HasGatheredSprites = false;
            }

            /************************************************************************************************************************/

            /// <summary>
            /// Adds all <see cref="Sprite"/>s in the <see cref="Selection.objects"/> or their sub-assets to the
            /// list of `sprites`.
            /// </summary>
            public static void GatherSelectedSprites(List<Sprite> sprites)
            {
                sprites.Clear();

                var selection = Selection.objects;
                for (int i = 0; i < selection.Length; i++)
                {
                    var selected = selection[i];
                    if (selected is Sprite sprite)
                    {
                        sprites.Add(sprite);
                    }
                    else if (selected is Texture2D texture)
                    {
                        sprites.AddRange(LoadAllSpritesInTexture(texture));
                    }
                }

                sprites.Sort(NaturalCompare);
            }

            /************************************************************************************************************************/

            /// <summary>The message to confirm that the user is certain they want to apply the changes.</summary>
            protected virtual string AreYouSure => "Are you sure you want to modify these Sprites?";

            /// <summary>Called immediately after the user confirms they want to apply changes.</summary>
            protected virtual void PrepareToApply() { }

            /// <summary>Applies the desired modifications to the `data` before it is saved.</summary>
            protected virtual void Modify(ref SpriteMetaData data, Sprite sprite) { }

            /// <summary>Applies the desired modifications to the `data` before it is saved.</summary>
            protected virtual void Modify(TextureImporter importer, List<Sprite> sprites)
            {
                var spriteSheet = importer.spritesheet;
                var hasError = false;

                for (int i = 0; i < sprites.Count; i++)
                {
                    var sprite = sprites[i];
                    var dataIndex = GetDataIndex(spriteSheet, sprite);

                    if (dataIndex < 0)
                        continue;

                    ref var spriteData = ref spriteSheet[dataIndex];
                    Modify(ref spriteData, sprite);
                    sprites.RemoveAt(i--);

                    if (!ValidateBounds(spriteData, sprite))
                        hasError = true;
                }

                if (!hasError)
                {
                    importer.spritesheet = spriteSheet;
                    EditorUtility.SetDirty(importer);
                    importer.SaveAndReimport();
                }
            }

            /************************************************************************************************************************/

            /// <summary>
            /// Tries to find the index of the <see cref="SpriteMetaData"/> matching the <see cref="Object.name"/> and
            /// <see cref="Sprite.rect"/>. Or if that fails, just the <see cref="Object.name"/>.
            /// </summary>
            /// <remarks>
            /// Returns -1 if there is no data matching the <see cref="Object.name"/>.
            /// <para></para>
            /// Returns -2 if there is more than one data matching the <see cref="Object.name"/> but no
            /// <see cref="Sprite.rect"/> match.
            /// </remarks>
            public static int GetDataIndex(SpriteMetaData[] spriteSheet, Sprite sprite)
            {
                var nameMatchIndex = -1;

                for (int i = 0; i < spriteSheet.Length; i++)
                {
                    ref var spriteData = ref spriteSheet[i];
                    if (spriteData.name == sprite.name)
                    {
                        if (spriteData.rect == sprite.rect)
                            return i;

                        if (nameMatchIndex == -1)// First name match.
                            nameMatchIndex = i;
                        else
                            nameMatchIndex = -2;// Already found 2 name matches.
                    }
                }

                if (nameMatchIndex == -1)
                {
                    Debug.LogError($"No {nameof(SpriteMetaData)} for '{sprite.name}' was found.", sprite);
                }
                else if (nameMatchIndex == -2)
                {
                    Debug.LogError($"More than one {nameof(SpriteMetaData)} for '{sprite.name}' was found" +
                        $" but none of them matched the {nameof(Sprite)}.{nameof(Sprite.rect)}." +
                        $" If the texture's Max Size is smaller than its actual size, increase the Max Size before performing this" +
                        $" operation so that the {nameof(Rect)}s can be used to identify the correct data.", sprite);
                }

                return nameMatchIndex;
            }

            /************************************************************************************************************************/

            public static bool ValidateBounds(SpriteMetaData data, Sprite sprite)
            {
                var widthScale = data.rect.width / sprite.rect.width;
                var heightScale = data.rect.height / sprite.rect.height;
                if (data.rect.xMin < 0 ||
                    data.rect.yMin < 0 ||
                    data.rect.xMax > sprite.texture.width * widthScale ||
                    data.rect.yMax > sprite.texture.height * heightScale)
                {
                    var path = AssetDatabase.GetAssetPath(sprite);
                    Debug.LogError($"This modification would have put '{sprite.name}' out of bounds" +
                        $" so '{path}' was not modified.", sprite);

                    return false;
                }

                return true;
            }

            /************************************************************************************************************************/

            /// <summary>
            /// Asks the user if they want to modify the target <see cref="Sprite"/>s and calls <see cref="Modify"/>
            /// on each of them before saving any changes.
            /// </summary>
            protected void AskAndApply()
            {
                if (!EditorUtility.DisplayDialog("Are You Sure?",
                    AreYouSure + "\n\nThis operation cannot be undone.",
                    "Modify", "Cancel"))
                    return;

                PrepareToApply();

                var pathToSprites = new Dictionary<string, List<Sprite>>();
                var sprites = Sprites;
                for (int i = 0; i < sprites.Count; i++)
                {
                    var sprite = sprites[i];

                    var path = AssetDatabase.GetAssetPath(sprite);

                    if (!pathToSprites.TryGetValue(path, out var spritesAtPath))
                        pathToSprites.Add(path, spritesAtPath = new List<Sprite>());

                    spritesAtPath.Add(sprite);
                }

                foreach (var asset in pathToSprites)
                {
                    var importer = (TextureImporter)AssetImporter.GetAtPath(asset.Key);

                    Modify(importer, asset.Value);

                    if (asset.Value.Count > 0)
                    {
                        var message = ObjectPool.AcquireStringBuilder()
                            .Append("Modification failed: unable to find data in '")
                            .Append(asset.Key)
                            .Append("' for ")
                            .Append(asset.Value.Count)
                            .Append(" Sprites:");

                        for (int i = 0; i < sprites.Count; i++)
                        {
                            message.AppendLine()
                                .Append(" - ")
                                .Append(sprites[i].name);
                        }

                        Debug.LogError(message.ReleaseToString(), AssetDatabase.LoadAssetAtPath<Object>(asset.Key));
                    }
                }
            }

            /************************************************************************************************************************/
        }
    }
}

#endif


using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using System.IO;
using System;

namespace Assets.Editor
{
    [ScriptedImporter(2, "gauss")]
    class GaussianImporter : ScriptedImporter
    {

        public override void OnImportAsset(AssetImportContext ctx)
        {
            using (var fs = new FileStream(ctx.assetPath,FileMode.Open))
            using (var br = new BinaryReader(fs))
            {
                var size = br.ReadInt32();
                var texture = new Texture2D(size, size, TextureFormat.RFloat, false);

                for (var y = 0; y < size; y++)
                {
                    for (var x = 0; x < size; x++)
                    {
                        var value = br.ReadSingle();
                        texture.SetPixel(x, y, new Color(value, value, value, 1));
                    }
                }
                texture.filterMode = FilterMode.Point;
                texture.wrapMode = TextureWrapMode.Clamp;
                ctx.AddObjectToAsset("Texture2D", texture);
                ctx.SetMainObject(texture);
            }
        }
    }
}
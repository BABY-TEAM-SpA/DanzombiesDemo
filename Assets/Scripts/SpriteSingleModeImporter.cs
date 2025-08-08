#if UNITY_EDITOR
using UnityEditor;

public class SpriteSingleModeImporter : AssetPostprocessor
{
    const int PostProcessOrder = 0;
    public override int GetPostprocessOrder() => PostProcessOrder;

    void OnPreprocessTexture()
    {
        var importer = assetImporter as TextureImporter;

        if (importer.importSettingsMissing is false)
            return;

        importer.spriteImportMode = SpriteImportMode.Single;
        importer.mipmapEnabled = false;
    }
}
#endif
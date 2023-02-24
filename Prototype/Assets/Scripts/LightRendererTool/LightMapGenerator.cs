using UnityEditor;
using UnityEngine;

public class LightMapGenerator : MonoBehaviour
{
    [MenuItem("Lighting Tool/Generate Lightmaps")]
    public static void GenerateLightMaps()
    {
        //Gather all lights in the scene
        var lights = GameObject.FindObjectsOfType<Light>();

        var lightMaps = new LightmapData[lights.Length];

        for (int i = 0; i < lights.Length; i++)
        {
            var light = lights[i];

            Texture2D lightMapTexture = new Texture2D(512, 512, TextureFormat.RGB24, false);

        }
    }
}

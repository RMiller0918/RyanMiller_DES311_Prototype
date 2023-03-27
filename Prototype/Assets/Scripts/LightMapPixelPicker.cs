using UnityEngine;
using UnityEngine.Rendering;

public class LightMapPixelPicker : MonoBehaviour
{

    public Color surfaceColor;
    public Color realtimeSurfaceColor;
    [Range(0,1)] public float brightness1; // http://stackoverflow.com/questions/596216/formula-to-determine-brightness-of-rgb-color 
    [Range(0,1)] public float brightness2; // http://www.nbdtech.com/Blog/archive/2008/04/27/Calculating-the-Perceived-Brightness-of-a-Color.aspx
    public LayerMask layerMask;
    [Range(0,2)]public int range;

    [SerializeField] private Renderer _hitRenderer;

    void Update()
    {
        // BRIGHTNESS APPROX
        brightness1 = (surfaceColor.r + surfaceColor.r + surfaceColor.b + surfaceColor.g + surfaceColor.g + surfaceColor.g) / 6;
        brightness1 = Mathf.Clamp(brightness1, 0f, 1f);
        // BRIGHTNESS
        brightness2 = Mathf.Sqrt((surfaceColor.r * surfaceColor.r * 0.2126f + surfaceColor.g * surfaceColor.g * 0.7152f + surfaceColor.b * surfaceColor.b * 0.0722f));
        brightness2 = Mathf.Clamp(brightness2, 0f, 1f);
    }

    private void FixedUpdate()
    {
        Raycast();
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10f, 10f, Screen.width, Screen.height));

        GUILayout.Label("R = " + string.Format("{0:0.00}", surfaceColor.r));
        GUILayout.Label("G = " + string.Format("{0:0.00}", surfaceColor.g));
        GUILayout.Label("B = " + string.Format("{0:0.00}", surfaceColor.b));

        GUILayout.Label("Brightness Approx = " + string.Format("{0:0.00}", brightness1));
        GUILayout.Label("Brightness = " + string.Format("{0:0.00}", brightness2));

        GUILayout.EndArea();
    }

    void Raycast()
    {
        // RAY TO PLAYER'S FEET
        Ray ray = new Ray(transform.position, Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction * range, Color.magenta);

        RaycastHit hitInfo;

        if (!Physics.Raycast(ray, out hitInfo, range, layerMask)) return;
        // GET RENDERER OF OBJECT HIT
        _hitRenderer = hitInfo.transform.GetComponent<Renderer>();
        //Debug.Log(_hitRenderer.lightmapIndex);

        // GET LIGHTMAP APPLIED TO OBJECT
        LightmapData lightmapData = LightmapSettings.lightmaps[_hitRenderer.lightmapIndex];

        var realtimeLightmap = LightmapSettings.lightmaps[_hitRenderer.realtimeLightmapIndex];
        // STORE LIGHTMAP TEXTURE
        Texture2D lightmapTex = lightmapData.lightmapColor;

        // GET LIGHTMAP COORDINATE WHERE RAYCAST HITS
        Vector2 pixelUV = hitInfo.lightmapCoord;
        //Debug.Log(pixelUV);
        // GET COLOR AT THE LIGHTMAP COORDINATE
        Color surfaceColor = lightmapTex.GetPixelBilinear(pixelUV.x, pixelUV.y);
        FindRealtimeLightmapUV(pixelUV);
        // APPLY
        this.surfaceColor = surfaceColor;

    }

    private void FindRealtimeLightmapUV(Vector2 pixelUV)
    {
        var realtimeLightMap = LightmapSettings.lightmaps[_hitRenderer.realtimeLightmapIndex];
        var lightmapTex = realtimeLightMap.lightmapColor;
        Color realsurfacecolor = lightmapTex.GetPixelBilinear(pixelUV.x, pixelUV.y);
        realtimeSurfaceColor = realsurfacecolor;
    }

}
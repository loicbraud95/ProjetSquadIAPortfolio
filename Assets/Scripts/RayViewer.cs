using UnityEngine;

public class RayViewer : MonoBehaviour
{
    public static RayViewer Instance;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
    }

    public float rayTimer;
    public float rayRange;
    public float rayStartThickness;
    public float rayEndThickness;
    public Material rayMat;

    public void AddRay(Ray ray)
    {
        CreateLineRenderer(ray.origin, ray.origin + ray.direction * rayRange);
    }

    public void AddRay(Ray ray, float range)
    {
        CreateLineRenderer(ray.origin, ray.origin + ray.direction * range);
    }

    void CreateLineRenderer(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("ShotLine");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startWidth = rayStartThickness;
        lr.endWidth = rayEndThickness;

        lr.material = rayMat;

        lr.startColor = Color.yellow;
        lr.endColor = Color.red;

        Destroy(lineObj, rayTimer);
    }
}

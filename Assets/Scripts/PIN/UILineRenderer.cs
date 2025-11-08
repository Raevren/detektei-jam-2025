using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/UILineRenderer")]
public class UILineRenderer : Graphic
{
    private List<Vector2[]> _points = new();
    
    [SerializeField] private float thickness = 25f;

    // Expose thickness so external code can adjust visibility
    public float Thickness => thickness;

    public void SetPoints(List<Vector2[]> newPoints)
    {
        _points = newPoints;
        SetVerticesDirty();
    }
    
    public void AddPoints(Vector2[] newPoints)
    {
        _points.Add(newPoints);
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        if (_points.Count == 0) return;

        var halfThickness = thickness / 2f;

        foreach (var vector2Se in _points)
        {
            for (var i = 0; i < vector2Se.Length - 1; i++)
            {
                var start = vector2Se[i];
                var end = vector2Se[i + 1];
                var dir = (end - start).normalized;
                var normal = new Vector2(-dir.y, dir.x) * halfThickness;

                var v1 = start - normal;
                var v2 = start + normal;
                var v3 = end + normal;
                var v4 = end - normal;

                var index = vh.currentVertCount;
                vh.AddVert(v1, color, Vector2.zero);
                vh.AddVert(v2, color, Vector2.zero);
                vh.AddVert(v3, color, Vector2.zero);
                vh.AddVert(v4, color, Vector2.zero);

                vh.AddTriangle(index + 0, index + 1, index + 2);
                vh.AddTriangle(index + 2, index + 3, index + 0);
            }   
        }
    }
}
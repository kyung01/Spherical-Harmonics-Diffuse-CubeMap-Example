using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class KVertex : MonoBehaviour {

    public float alpha, beta;
    MeshRenderer m_renderer;
    public Vector3 m_color;
    private void Awake()
    {
        m_renderer = this.GetComponent<MeshRenderer>();
        m_renderer.material = Instantiate<Material>(m_renderer.material);

    }
    public void setColor(Color color)
    {
        m_renderer.material.color = color;
        m_color = new Vector3(color.r, color.g, color.b);
    }
    public Vector3 Normal {
        get
        {
            return new Vector3(Mathf.Sin(alpha)*Mathf.Cos(beta), Mathf.Sin(alpha)*Mathf.Sin(beta),Mathf.Cos(alpha)).normalized;
        }
    }
  
    public void moveTo(Vector3 position)
    {
        this.transform.position = position + Normal;
        this.transform.rotation = Quaternion.FromToRotation(new Vector3(0, 0, -1), Normal);
    }
    public List<float> getSH()
    {
        List<float> HS = new List<float>();
        var normal = Normal;
        float Y00 = 0.282095f;
        float Y11 = 0.488603f * normal.x;
        float Y10 = 0.488603f * normal.z;
        float Y1_1 = 0.488603f * normal.y;
        float Y21 = 1.092548f * normal.x * normal.z;
        float Y2_1 = 1.092548f * normal.y * normal.z;
        float Y2_2 = 1.092548f * normal.y * normal.x;
        float Y20 = 0.946176f * normal.z * normal.z - 0.315392f;
        float Y22 = 0.546274f * (normal.x * normal.x - normal.y * normal.y);
        HS.Add(Y00);
        HS.Add(Y1_1);
        HS.Add(Y10);
        HS.Add(Y11);

        HS.Add(Y2_2);
        HS.Add(Y2_1);
        HS.Add(Y20);
        HS.Add(Y21);
        HS.Add(Y22);
        return HS;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoCustomCubeMapSHExample : MonoBehaviour
{
    public KVertex P_VERTEX;
    public Texture2D m_texture;
    public int iterationCount, diffuseVertexCount;
    public SimpleRotation m_sphereCubeMap, m_sphereCubeMapDiffuse;
    int iterationCountOld = 0, diffuseVertexCountOld=0;
    List<Vector3> coefficientsTexture = new List<Vector3>() { new Vector3(), new Vector3(), new Vector3(), new Vector3(), new Vector3(), new Vector3(), new Vector3(), new Vector3(), new Vector3(), };

    List<Vector3> exampleGraceCathedral = new List<Vector3>() {
        new Vector3(0.79f,0.44f,0.54f),
        new Vector3(.39f ,.35f ,.60f),new Vector3(-.34f, -.18f ,-.27f),new Vector3(-.29f, -.06f, .01f),
        new Vector3(-.11f, -.05f, -.12f),new Vector3(-.26f, -.22f, -.47f),new Vector3(-.16f, -.09f, -.15f),new Vector3(.56f, .21f, .14f),new Vector3(.21f, -.05f, -.30f) };
    // Use this for initialization
    List<KVertex> m_vertexs = new List<KVertex>();
    List<KVertex> m_vertexsDiffuse = new List<KVertex>();
    void Start()
    {

    }
    void updateColor(KVertex vertex, Texture2D texture)
    {
        float 
            x =( vertex.alpha/3.14f) * texture.width,
            y = (vertex.beta / (2*3.14f) ) * texture.height;
        Color c= texture.GetPixel((int)Mathf.Round(x), (int)Mathf.Round(y));
        vertex.setColor(c);
    }
    void updateColor(KVertex vertex, List<Vector3> coefficients, float scale)
    {
        var vertexSH = vertex.getSH();
        float a0 = 3.141593f;
        float a1 = 2.094395f;
        float a2 = 0.785398f;

        Vector3 color =
            a0 * vertexSH[0] * coefficients[0] +
            a1 * vertexSH[1] * coefficients[1] +
            a1 * vertexSH[2] * coefficients[2] +
            a1 * vertexSH[3] * coefficients[3] +
            a2 * vertexSH[4] * coefficients[4] +
            a2 * vertexSH[5] * coefficients[5] +
            a2 * vertexSH[6] * coefficients[6] +
            a2 * vertexSH[7] * coefficients[7] +
            a2 * vertexSH[8] * coefficients[8];

        vertex.setColor(new Color(color.x * scale, color.y * scale, color.z * scale));

    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            iterationCount++;

        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            iterationCount--;

        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            diffuseVertexCount++;

        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            diffuseVertexCount--;

        }
        if (diffuseVertexCountOld != diffuseVertexCount)
        {
            diffuseVertexCountOld = diffuseVertexCount;
            for (int i = m_vertexsDiffuse.Count - 1; i >= 0; i--)
            {
                GameObject.Destroy(m_vertexsDiffuse[i].gameObject);
            }
            m_vertexsDiffuse.Clear();
            for (float i = 0; i < diffuseVertexCount; i++)
            {
                for (float j = 0; j < diffuseVertexCount * 2; j++)
                {
                    float alpha = ((i + 1) / (diffuseVertexCount + 1)) * (3.14f);
                    float beta = ((j) / (diffuseVertexCount * 2)) * (3.14f * 2);
                    var vert = Instantiate<KVertex>(P_VERTEX);
                    vert.alpha = alpha;
                    vert.beta = beta;
                    vert.transform.parent = m_sphereCubeMapDiffuse.transform;
                    m_vertexsDiffuse.Add(vert);
                    vert.moveTo(new Vector3(1, 0, 0));
                    updateColor(vert, coefficientsTexture, 1);// (1.0f / m_vertexs.Count));
                }
            }
        }
        if (iterationCountOld != iterationCount)
        {
            iterationCountOld = iterationCount;
            for (int i = m_vertexs.Count - 1; i >= 0; i--)
            {
                GameObject.Destroy(m_vertexs[i].gameObject);
            }
            m_vertexs.Clear();
            for (float i = 0; i < iterationCount; i++)
            {
                for (float j = 0; j < iterationCount * 2; j++)
                {
                    float alpha = ((i + 1) / (iterationCount + 1)) * (3.14f);
                    float beta = ((j) / (iterationCount * 2)) * (3.14f * 2);
                    var vert = Instantiate<KVertex>(P_VERTEX);
                    vert.alpha = alpha;
                    vert.beta = beta;
                    vert.transform.parent = m_sphereCubeMap.transform;
                    m_vertexs.Add(vert);
                    vert.moveTo(new Vector3(-1,0,0));
                    updateColor(vert, m_texture);
                }
            }

            List<Vector3> coefficients = new List<Vector3>();
            for (int i = 0; i < 9; i++) coefficients.Add(new Vector3());

            float fwt = 0; //weights of all faces
            foreach (var v in m_vertexs)
            {
                var sphereSH = v.getSH();
                var normal = v.Normal;
                if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y) && Mathf.Abs(normal.x) > Mathf.Abs(normal.z))
                {
                    normal.x = 0;
                }
                else if (Mathf.Abs(normal.y) > Mathf.Abs(normal.x) && Mathf.Abs(normal.y) > Mathf.Abs(normal.z))
                {
                    normal.y = 0;
                }
                else if (Mathf.Abs(normal.z) > Mathf.Abs(normal.x) && Mathf.Abs(normal.z) > Mathf.Abs(normal.y))
                {
                    normal.z = 0;
                }else
                {
                    Debug.Log("Unknown case");
                }
                normal.Normalize();
                float fDiffSolid = 4.0f / ((1.0f + normal.x * normal.x + normal.y * normal.y + normal.z * normal.z) *
                                       Mathf.Sqrt(1.0f + normal.x * normal.x + normal.y * normal.y + normal.z * normal.z));
                fwt += fDiffSolid;
                for (int i = 0; i < 9; i++) coefficients[i] += (v.m_color ) * sphereSH[i];

            }
            float fNormProj = (4.0f * Mathf.PI) / fwt;
            //for (int i = 0; i < 9; i++) coefficients[i] *= fNormProj;
            for (int i = 0; i < 9; i++) Debug.Log("COEFFICIENTS " + i + " : " + coefficients[i]);
            //for (int i = 0; i < 9; i++) Debug.Log("COEFFICIENTS " + i + " : " + (coefficients[i].x / m_vertexs.Count)
            //   + " , " + (coefficients[i].y / m_vertexs.Count) + " , " + (coefficients[i].z / m_vertexs.Count)
            //
            //   );

            coefficientsTexture = coefficients;
        }

    }
}

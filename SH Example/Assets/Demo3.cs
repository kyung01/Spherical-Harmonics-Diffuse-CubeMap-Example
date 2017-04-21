using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo3 : MonoBehaviour
{
    public KVertex P_VERTEX;
    public Texture2D m_textureRight, m_textureLeft, m_textureUp, m_textureDown, m_textureFront, m_textureBack;
    public Texture2D m_texture;
    public int iterationCount, diffuseVertexCount;
    public SimpleRotation m_sphereCubeMap, m_sphereCubeMapDiffuse,m_sphereDiffuse2;
    int iterationCountOld = 0, diffuseVertexCountOld = 0;
    List<Vector3> coefficientsTexture = new List<Vector3>() { new Vector3(), new Vector3(), new Vector3(), new Vector3(), new Vector3(), new Vector3(), new Vector3(), new Vector3(), new Vector3(), };

    List<Vector3> exampleGraceCathedral = new List<Vector3>() {
        new Vector3(0.79f,0.44f,0.54f),
        new Vector3(.39f ,.35f ,.60f),new Vector3(-.34f, -.18f ,-.27f),new Vector3(-.29f, -.06f, .01f),
        new Vector3(-.11f, -.05f, -.12f),new Vector3(-.26f, -.22f, -.47f),new Vector3(-.16f, -.09f, -.15f),new Vector3(.56f, .21f, .14f),new Vector3(.21f, -.05f, -.30f) };
    // Use this for initialization
    List<KVertex> m_vertexs = new List<KVertex>();
    List<KVertex> m_vertexsDiffuse = new List<KVertex>();
    List<KVertex> m_vertexsDiffuse2 = new List<KVertex>();
    void Start()
    {

    }
    void updateColor(KVertex vertex, Texture2D texture)
    {
        float
            x = (vertex.alpha / 3.14f) * texture.width,
            y = (vertex.beta / (2 * 3.14f)) * texture.height;
        Color c = texture.GetPixel((int)Mathf.Round(x), (int)Mathf.Round(y));
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
    //value has to be -1 ~ 1
    //also the coordinate has to be in the middle of subtended area
    static float AreaElement(float x, float y)
    {
        return Mathf.Atan2(x * y, Mathf.Sqrt(x * x + y * y + 1));
    }
    float TexelCoordSolidAngle2(float U, float V, int a_Size)
    {
        //scale up to [-1, 1] range (inclusive), offset by 0.5 to point to texel center.
        //float U = (2.0f * ((float)a_U + 0.5f) / (float)a_Size) - 1.0f;
        //float V = (2.0f * ((float)a_V + 0.5f) / (float)a_Size) - 1.0f;

        float InvResolution = (2.0f / a_Size) * 0.5f;

        float x0 = U - InvResolution;
        float y0 = V - InvResolution;
        float x1 = U + InvResolution;
        float y1 = V + InvResolution;
        float SolidAngle = AreaElement(x0, y0) - AreaElement(x1, y0) + AreaElement(x1, y1) - AreaElement(x0, y1);

        return SolidAngle;
    }

    float solidAngleTotal = 0;
    KVertex createSphereAre(int face, int surfaceDividedBy, float u, float v, float pixelSize, List<Vector3> coefficients)
    {
        var vertex = Instantiate<KVertex>(P_VERTEX);

        vertex.transform.parent = m_sphereCubeMap.transform;
        Vector3 normal = new Vector3();
        float
            xDir = -1 + (u * 2) + pixelSize,
            yDir = -1 + (v * 2) + pixelSize;
        Texture2D texture = null;
        switch (face)
        {
            case 0: texture = m_textureRight; break; // +X
            case 1: texture = m_textureLeft; break; // -X
            case 2: texture = m_textureUp; break; // +Y
            case 3: texture = m_textureDown; break; // -Y
            case 4: texture = m_textureFront; break; // +Z
            case 5: texture = m_textureBack; break; // -Z
        }
        switch (face)
        {
            case 0: normal = new Vector3(+1, +yDir, -xDir); break; // +X
            case 1: normal = new Vector3(-1, +yDir, +xDir); break; // -X
            case 2: normal = new Vector3(+xDir, +1, -yDir); break; // +Y
            case 3: normal = new Vector3(+xDir, -1, +yDir); break; // -Y
            case 4: normal = new Vector3(+xDir, yDir, +1); break; // +Z
            case 5: normal = new Vector3(-xDir, yDir, -1); break; // -Z
        }
        normal.Normalize();
        vertex.transform.position = new Vector3(-2, 0, 0) + normal;
        vertex.transform.LookAt(new Vector3(-2, 0, 0));
        vertex.setColor(texture.GetPixel((int)(u * texture.width), (int)((v) * texture.height)));

        float A0 = 1.0f;// 3.141593f;
        const float A1 = 1.0f;// 2.095395f; // Stick with 1.0 for all of these!!!
        const float A2 = 1.0f; //0.785398f;

        float domega = TexelCoordSolidAngle2(u, v, surfaceDividedBy);
        solidAngleTotal += domega;
        coefficients[0] += 0.282095f * A0 * domega * vertex.m_color;

        // Band 1
        coefficients[1] += 0.488603f * normal.y * A1 * domega * vertex.m_color;
        coefficients[2] += 0.488603f * normal.z * A1 * domega * vertex.m_color;
        coefficients[3] += 0.488603f * normal.x * A1 * domega * vertex.m_color;

        // Band 2
        coefficients[4] += 1.092548f * normal.x * normal.y * A2 * domega * vertex.m_color;
        coefficients[5] += 1.092548f * normal.y * normal.z * A2 * domega * vertex.m_color;
        coefficients[6] += 0.315392f * (3.0f * normal.z * normal.z - 1.0f) * A2 * domega * vertex.m_color;
        coefficients[7] += 1.092548f * normal.x * normal.z * A2 * domega * vertex.m_color;
        coefficients[8] += 0.546274f * (normal.x * normal.x - normal.y * normal.y) * A2 * domega * vertex.m_color;
        return vertex;
    }
   
    void updateColor1(KVertex vertex, List<Vector3> coefficients)
    {
        var vertexSH = vertex.getSH();
        float a0 = 1;//athf.PI;// 1;//3.141593f;
        float a1 = 1;//athf.PI*(2.0f/3); ;//2.094395f;
        float a2 = 1;//Mathf.PI * (1.0f / 4);// 0.785398f;

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
        //color *= 0.5f;
        vertex.setColor(new Color(color.x, color.y, color.z));
    }
    void updateColor2(KVertex vertex, List<Vector3> coef)
    {
        /*
         * 
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
        vertex.setColor(new Color(color.x, color.y, color.z));
         * */
        var vertexSH = vertex.getSH();
        var N = vertex.Normal;
        float C1 = 0.429043f;
        float C2 = 0.511665f;
        float C3 = 0.743125f;
        float C4 = 0.886227f;
        float C5 = 0.247708f;

        var L00 = coef[0];
        var L1_1 = coef[1];
        var L10 = coef[2];
        var L11 = coef[3];
        var L2_2 = coef[4];
        var L2_1 = coef[5];
        var L20 = coef[6];
        var L21 = coef[7];
        var L22 = coef[8];

          // constant term, lowest frequency //////
          Vector3 color = C4 * coef[0] +

          // axis aligned terms ///////////////////
          2.0f * C2 * coef[1] * N.y +
          2.0f * C2 * coef[2] * N.z +
          2.0f * C2 * coef[3] * N.x +

          // band 2 terms /////////////////////////
          2.0f * C1 * coef[4] * N.x * N.y +
          2.0f * C1 * coef[5] * N.y * N.z +
          C3 * coef[6] * N.z * N.z - C5 * coef[6] +
          2.0f * C1 * coef[7] * N.x * N.z +
          C1 * coef[8] * (N.x * N.x - N.y * N.y);
        
    /*
    Vector3 irradianceColor =
        c1 * L22 * (N.x * N.x - N.y * N.y) +
        c3 * L20 * (N.z * N.z) +
        c4 * L00 -
        c5 * L20 +
        2 * c1 * (L2_2 * N.x * N.y + L21 * N.x * N.z + L2_1 * N.y * N.z) +
        2 * c2 * (L11 * N.x + L1_1 * N.y + L10 * N.z);

     * */
    vertex.setColor(new Color(color.x, color.y, color.z));

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

        if (iterationCountOld != iterationCount)
        {
            iterationCountOld = iterationCount;
            for (int i = 0; i < m_vertexs.Count; i++)
            {
                Destroy(m_vertexs[i].gameObject);
            }
            m_vertexs.Clear();
            solidAngleTotal = 0;
            coefficientsTexture = new List<Vector3>();
            for (int i = 0; i < 9; i++)
            {
                coefficientsTexture.Add(new Vector3());
            }
            for (int face = 0; face < 6; face++)
            {
                float pixelSize = 1.0f / (iterationCount);
                for (int i = 0; i < iterationCount; i++)
                {
                    for (int j = 0; j < iterationCount; j++)
                    {
                        float u = ((float)i) * pixelSize;
                        float v = ((float)j) * pixelSize;
                        m_vertexs.Add(
                        createSphereAre(face, iterationCount,
                            u, v,
                            pixelSize, coefficientsTexture)
                            );

                    }
                }
            }
            Debug.Log("Solidangle Total : " + solidAngleTotal);
            for(int i = 0; i < 9; i++)
            {
                coefficientsTexture[i] *= (4*Mathf.PI)/ solidAngleTotal;
            }
            for (int i = 0; i < 9; i++) Debug.Log("Cof " + i + " : " + coefficientsTexture[i].x + " , " + coefficientsTexture[i].y + " , " + coefficientsTexture[i].z);
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
                    vert.transform.parent = m_sphereCubeMapDiffuse.transform;
                    vert.alpha = alpha;
                    vert.beta = beta;
                    m_vertexsDiffuse.Add(vert);
                    vert.moveTo(new Vector3(0, 0, 0));
                    updateColor1(vert, coefficientsTexture);
                    //updateColor1(vert, exampleGraceCathedral);
                }
            }





            for (int i = m_vertexsDiffuse2.Count - 1; i >= 0; i--)
            {
                GameObject.Destroy(m_vertexsDiffuse2[i].gameObject);
            }
            m_vertexsDiffuse2.Clear();
            for (float i = 0; i < diffuseVertexCount; i++)
            {
                for (float j = 0; j < diffuseVertexCount * 2; j++)
                {
                    float alpha = ((i + 1) / (diffuseVertexCount + 1)) * (3.14f);
                    float beta = ((j) / (diffuseVertexCount * 2)) * (3.14f * 2);
                    var vert = Instantiate<KVertex>(P_VERTEX);
                     vert.transform.parent = m_sphereDiffuse2.transform;
                    vert.alpha = alpha;
                    vert.beta = beta;
                    m_vertexsDiffuse2.Add(vert);
                    vert.moveTo(new Vector3(2, 0, 0));
                    updateColor2(vert, coefficientsTexture);
                    //updateColor2(vert, exampleGraceCathedral);
                }
            }
        }

    }
}
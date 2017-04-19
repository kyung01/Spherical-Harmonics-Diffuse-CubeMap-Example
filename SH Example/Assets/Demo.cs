using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo : MonoBehaviour {
    public KVertex P_VERTEX;
    public int iterationCount;
    int iterationCountOld = 0;
    List<Vector3> exampleGraceCathedral = new List<Vector3>() {
        new Vector3(0.79f,0.44f,0.54f),
        new Vector3(.39f ,.35f ,.60f),new Vector3(-.34f, -.18f ,-.27f),new Vector3(-.29f, -.06f, .01f),
        new Vector3(-.11f, -.05f, -.12f),new Vector3(-.26f, -.22f, -.47f),new Vector3(-.16f, -.09f, -.15f),new Vector3(.56f, .21f, .14f),new Vector3(.21f, -.05f, -.30f) };
    // Use this for initialization
    List<KVertex> m_vertexs = new List<KVertex>();
	void Start () {
		
	}
	void updateColor(KVertex vertex, List<Vector3> coefficients)
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
        vertex.setColor(new Color(color.x, color.y, color.z));

    }
    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            iterationCount++;

        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            iterationCount--;

        }

        if (iterationCountOld != iterationCount)
        {
            iterationCountOld = iterationCount;
            for(int i = m_vertexs.Count-1; i >= 0; i--)
            {
                GameObject.Destroy(m_vertexs[i].gameObject);
            }
            m_vertexs.Clear();
            for (float i = 0; i < iterationCount; i++) {
                for(float j = 0; j < iterationCount*2; j++)
                {
                    float alpha = ((i + 1) / (iterationCount + 1)) * (3.14f);
                    float beta = ((j ) / (iterationCount*2)) * (3.14f*2);
                    var vert =Instantiate<KVertex>(P_VERTEX);
                    vert.alpha = alpha;
                    vert.beta = beta;
                    m_vertexs.Add(vert);
                    vert.moveTo(new Vector3());
                    updateColor(vert, exampleGraceCathedral);
                }
            }
        }
		
	}
}

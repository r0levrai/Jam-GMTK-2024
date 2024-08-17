using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw : MonoBehaviour
{
    public Camera m_camera;
    public GameObject[] brush;
    public bool drawable;

    List<LineRenderer> linesListUndo;
    List<LineRenderer> linesListRedo;
    LineRenderer currentLineRenderer;
    float currentWidth;
    int colorIndex;

    Vector2 lastPos;

    public static Draw Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        currentWidth = 0.2f;
        colorIndex = 0;
        linesListUndo = new List<LineRenderer>();
        linesListRedo = new List<LineRenderer>();

    }
    private void Update()
    {
        if (m_camera.ScreenToWorldPoint(Input.mousePosition)[1]<3 && drawable)
        {
            Drawing();
        }
    }

    void Drawing()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            CreateBrush();
            linesListRedo.Clear();
        }
        else if (Input.GetKey(KeyCode.Mouse0))
        {
            currentLineRenderer.startWidth = currentWidth;
            currentLineRenderer.endWidth = currentWidth;
            PointToMousePos();
        }
        else
        {
            if (currentLineRenderer != null)
            {
                linesListUndo.Add(currentLineRenderer);
                currentLineRenderer = null;
            }

        }
    }

    void CreateBrush()
    {
        GameObject brushInstance = Instantiate(brush[colorIndex], transform);
        currentLineRenderer = brushInstance.GetComponent<LineRenderer>();
        currentLineRenderer.startWidth = currentWidth;
        currentLineRenderer.endWidth = currentWidth;

        //because you gotta have 2 points to start a line renderer, 
        Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
        if (linesListUndo.Count == 0)
        {
            currentLineRenderer.SetPosition(0, new Vector3(mousePos[0], mousePos[1],1));
            currentLineRenderer.SetPosition(1, new Vector3(mousePos[0], mousePos[1],1));
        }
        else
        {
            currentLineRenderer.SetPosition(0, new Vector3(mousePos[0], mousePos[1], 1f / linesListUndo.Count - 1));
            currentLineRenderer.SetPosition(1, new Vector3(mousePos[0], mousePos[1], 1f / linesListUndo.Count - 1));
        }
    }

    void AddAPoint(Vector3 pointPos)
    {
        currentLineRenderer.positionCount++;
        int positionIndex = currentLineRenderer.positionCount - 1;
        currentLineRenderer.SetPosition(positionIndex, pointPos);
    }

    void PointToMousePos()
    {
        Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
        if (lastPos != mousePos)
        {
            if (linesListUndo.Count==0)
                AddAPoint(new Vector3(mousePos[0], mousePos[1], 1));
            else
                AddAPoint(new Vector3(mousePos[0], mousePos[1], 1f/linesListUndo.Count-1));
            lastPos = mousePos;
        }
    }

    void BrushWidthScale(float scale)
    {
        if (drawable)
            currentWidth *= scale;
    }

    public void OnButtonWidthUpPress()
    {
        if (drawable)
            BrushWidthScale(2);
    }

    public void OnButtonWidthDownPress()
    {
        if (drawable)
            BrushWidthScale(0.5f);
    }

    public void OnButtonBrushBlackPress()
    {
        if (drawable)
            colorIndex = 0;
    }

    public void OnButtonBrushRedPress()
    {
        if (drawable)
            colorIndex = 1;
    }

    public void OnButtonBrushWhitePress()
    {
        if (drawable)
            colorIndex = 2;
    }

    public void OnButtonUndoPress()
    {
        if (drawable)
        {
            if (linesListUndo.Count > 0)
            {
                LineRenderer action = linesListUndo[linesListUndo.Count - 1];
                linesListUndo.RemoveAt(linesListUndo.Count - 1);
                linesListRedo.Add(action);
                action.enabled = false;
            }
        }
    }

    public void OnButtonRedoPress()
    {
        if (drawable)
        {
            if (linesListRedo.Count > 0)
            {
                LineRenderer action = linesListRedo[linesListRedo.Count - 1];
                linesListRedo.RemoveAt(linesListRedo.Count - 1);
                linesListUndo.Add(action);
                action.enabled = true;
            }
        }
    }

    public struct LineRendererData
    {
        public LineRendererData(Vector3[][] a, float[] b, int[] c )
        {
            linesPoints = a;
            linesWidth = b;
            linesColorIndex = c; 
        }
        public Vector3[][] linesPoints { get;}
        public float[] linesWidth { get; }
        public int[] linesColorIndex { get; }
    }

    public LineRendererData GetDrawingData()
    {
        Vector3[][] linesPoints = new Vector3[linesListUndo.Count][];
        float[] linesWidth = new float[linesListUndo.Count];
        int[] linesColorIndex = new int[linesListUndo.Count];
        for (int i = 0; i < linesListUndo.Count;i++)
        {
            LineRenderer action = linesListRedo[linesListRedo.Count - 1];
            linesWidth[i] = action.startWidth;
            if (action.material.name == "BrushMatBlack (Instance)")
                linesColorIndex[i] = 0;
            if (action.material.name == "BrushMatRed (Instance)")
                linesColorIndex[i] = 1;
            if (action.material.name == "BrushMatWhite (Instance)")
                linesColorIndex[i] = 2;
            linesPoints[i] = new Vector3[action.positionCount];
            for (int j = 0; j < action.positionCount;j++)
            {
                linesPoints[i][j] = action.GetPosition(j);
            }
        }

        LineRendererData data = new LineRendererData(linesPoints, linesWidth, linesColorIndex);
        return data;
    }

    public void SetDrawingData(LineRendererData data)
    {
        Vector3[][] linesPoints = data.linesPoints;
        float[] linesWidth = data.linesWidth;
        int[] linesColorIndex = data.linesColorIndex;
        for (int i = 0; i < linesPoints.Length; i++)
        {
            colorIndex = linesColorIndex[i];
            currentWidth = linesWidth[i];
            CreateBrush();
            for (int j = 0; j < linesPoints[i].Length;j++)
            {
                AddAPoint(linesPoints[i][j]);
            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw : MonoBehaviour
{
    [System.NonSerialized] public Camera m_camera;
    public GameObject[] brush;
    public bool drawable;

    [System.Serializable]
    public struct UnitySuxxWith2DVector
    {
        public Vector3[] points;
        public UnitySuxxWith2DVector(Vector3[] a)
        {
            points = a;
        }
    }

    [System.Serializable]
    public struct LineRendererData
    {
        public UnitySuxxWith2DVector[] linesPoints;
        public float[] linesWidth;
        public int[] linesColorIndex;
        public LineRendererData(UnitySuxxWith2DVector[] a, float[] b, int[] c)
        {
            linesPoints = a;
            linesWidth = b;
            linesColorIndex = c;
        }
    }

    public List<LineRenderer> linesListUndo;
    List<LineRenderer> linesListRedo;
    LineRenderer currentLineRenderer;
    float currentWidth;
    int colorIndex;
    bool enoughPoint;

    Vector2 lastPos;

    public bool isEraserActive = false;


    public static Draw Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        m_camera = Camera.main;

        currentWidth = 0.4f;
        colorIndex = 0;
        enoughPoint = false;
        linesListUndo = new List<LineRenderer>();
        linesListRedo = new List<LineRenderer>();

    }
    
    public void ToggleEraser()
    {
        isEraserActive = !isEraserActive;
    }

    private void Update()
    {
        if (drawable)
        {
            if (isEraserActive)
            {
                EraserTool();
            }
            else
            {
                Drawing();
            }
        }
    }

    void EraserTool()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
            EraseAtPosition(mousePos, 0.5f);
        }
    }


    void EraseAtPosition(Vector2 position, float eraserRadius)
    {
        for (int i = linesListUndo.Count - 1; i >= 0; i--)
        {
            LineRenderer line = linesListUndo[i];
            int positionCount = line.positionCount;
            List<Vector3> newPositions = new List<Vector3>();
            List<List<Vector3>> splitSegments = new List<List<Vector3>>();

            bool isErasing = false;

            for (int j = 0; j < positionCount; j++)
            {
                Vector3 pointPosition = line.GetPosition(j);
                float distance = Vector2.Distance(new Vector2(pointPosition.x, pointPosition.y), position);

                if (distance > eraserRadius)
                {
                    if (isErasing && newPositions.Count > 0)
                    {
                        splitSegments.Add(new List<Vector3>(newPositions));
                        newPositions.Clear();
                    }
                    isErasing = false;
                    newPositions.Add(pointPosition);
                }
                else
                {
                    isErasing = true;
                }
            }
            if (newPositions.Count > 0)
            {
                splitSegments.Add(new List<Vector3>(newPositions));
            }

            if (splitSegments.Count == 0)
            {
                Destroy(line.gameObject);
                linesListUndo.RemoveAt(i);
            }
            else
            {
                line.positionCount = splitSegments[0].Count;
                line.SetPositions(splitSegments[0].ToArray());

                for (int k = 1; k < splitSegments.Count; k++)
                {
                    CreateNewLineSegment(splitSegments[k], line);
                }
            }
        }
    }

    void CreateNewLineSegment(List<Vector3> segmentPoints, LineRenderer originalLine)
    {
        if (segmentPoints.Count < 2) return;

        GameObject newLineObject = new GameObject("LineSegment");
        newLineObject.transform.parent = originalLine.transform.parent;

        LineRenderer newLine = newLineObject.AddComponent<LineRenderer>();
        newLine.material = originalLine.material;
        newLine.startWidth = originalLine.startWidth;
        newLine.endWidth = originalLine.endWidth;
        newLine.positionCount = segmentPoints.Count;
        newLine.SetPositions(segmentPoints.ToArray());

        linesListUndo.Add(newLine);
    }

    void Drawing()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            CreateBrush(transform.position,transform.rotation, transform.localScale);
            linesListRedo.Clear();
        }
        else if (Input.GetKey(KeyCode.Mouse0))
        {
            currentLineRenderer.startWidth = currentWidth;
            currentLineRenderer.endWidth = currentWidth;
            PointToMousePos();
            SoundManager.Instance.PlaySound("drawing");
        }
        else
        {
            if (currentLineRenderer != null)
            {
				SoundManager.Instance.StopSound();
				linesListUndo.Add(currentLineRenderer);
                currentLineRenderer = null;
            }

        }
    }

    void CreateBrush(Vector3 pos, Quaternion rotation, Vector3 scale)
    {
        GameObject brushInstance = Instantiate(brush[colorIndex], transform);
        currentLineRenderer = brushInstance.GetComponent<LineRenderer>();
        currentLineRenderer.startWidth = currentWidth;
        currentLineRenderer.endWidth = currentWidth;
        enoughPoint = false;
        if (!drawable)
        {
            currentLineRenderer.transform.position = pos;
            currentLineRenderer.transform.rotation = rotation;
            //currentLineRenderer.transform.localScale = scale;
            currentLineRenderer.widthMultiplier *= transform.lossyScale.y;
            print(transform.lossyScale.y);
        }

        if (drawable)
        {
            //because you gotta have 2 points to start a line renderer, 
            Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
            if (linesListUndo.Count == 0)
            {
                currentLineRenderer.SetPosition(0, new Vector3(mousePos[0], mousePos[1],-0.15f));
            }
            else if (linesListUndo.Count == 1)
            {
                currentLineRenderer.SetPosition(0, new Vector3(mousePos[0], mousePos[1],-0.2f));
            }
            else
            {
                currentLineRenderer.SetPosition(0, new Vector3(mousePos[0], mousePos[1], 1f / linesListUndo.Count - 1));
            }
            currentLineRenderer.enabled = false;
        }
    }

    void AddAPoint(Vector3 pointPos)
    {
        if (!enoughPoint)
        {
            int positionIndex = currentLineRenderer.positionCount - 1;
            currentLineRenderer.SetPosition(positionIndex, pointPos);
            currentLineRenderer.enabled = true;
            enoughPoint = true;
        }
        else
        {
            currentLineRenderer.positionCount++;
            int positionIndex = currentLineRenderer.positionCount - 1;
            currentLineRenderer.SetPosition(positionIndex, pointPos);
        }
    }
    void AddAPointSet(int positionIndex, Vector3 pointPos)
    {
        if (positionIndex>=2)
            currentLineRenderer.positionCount++;
        currentLineRenderer.SetPosition(positionIndex, pointPos);
    }

    void PointToMousePos()
    {
        Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
        if (lastPos != mousePos)
        {
            if (linesListUndo.Count==0)
                AddAPoint(new Vector3(mousePos[0], mousePos[1], -0.15f));
            else if (linesListUndo.Count == 1)
                AddAPoint(new Vector3(mousePos[0], mousePos[1], -0.2f));
            else
                AddAPoint(new Vector3(mousePos[0], mousePos[1], 1f/linesListUndo.Count-1));
            lastPos = mousePos;
        }
    }

    void BrushWidthScale(float scale)
    {
        currentWidth *= scale;
    }

    public void OnButtonWidthUpPress()
    {
        BrushWidthScale(2);
	}

	public void OnButtonWidthDownPress()
    {
        BrushWidthScale(0.5f);
    }

    public void OnSmallBrushPress()
    {
        currentWidth = 0.1f;
    }
    public void OnMediumBrushPress()
    {
        currentWidth = 0.4f;
    }
    public void OnLargeBrushPress()
    {
        currentWidth = 0.8f;
    }

    public void OnButtonBrushBlackPress()
    {
        colorIndex = 0;
    }

    public void OnButtonBrushRedPress()
    {
        colorIndex = 1;
    }

    public void OnButtonBrushWhitePress()
    {
        colorIndex = 2;
    }

    public void OnButtonBrushBluePress()
    {
        colorIndex = 3;
    }

    public void OnClearButtonPress()
    {
        if (linesListUndo.Count > 0)
        {
            for (int i = 0; i < linesListUndo.Count; i++)
            {
                Destroy(linesListUndo[i].gameObject);
            }
        }
        linesListUndo.Clear();
        linesListRedo.Clear();
    }

    public void OnButtonUndoPress()
    {
        if (linesListUndo.Count > 0)
        {
            LineRenderer action = linesListUndo[linesListUndo.Count - 1];
            linesListUndo.RemoveAt(linesListUndo.Count - 1);
            linesListRedo.Add(action);
            action.enabled = false;
        }
    }

    public void OnButtonRedoPress()
    {
        if (linesListRedo.Count > 0)
        {
            LineRenderer action = linesListRedo[linesListRedo.Count - 1];
            linesListRedo.RemoveAt(linesListRedo.Count - 1);
            linesListUndo.Add(action);
            action.enabled = true;
        }
    }

    public LineRendererData GetDrawingData()
    {
        UnitySuxxWith2DVector[] linesPoints = new UnitySuxxWith2DVector[linesListUndo.Count];
        float[] linesWidth = new float[linesListUndo.Count];
        int[] linesColorIndex = new int[linesListUndo.Count];
        for (int i = 0; i < linesListUndo.Count;i++)
        {
            LineRenderer action = linesListUndo[i];
            linesWidth[i] = action.startWidth;
            if (action.material.name == "BrushMatBlack (Instance)")
                linesColorIndex[i] = 0;
            if (action.material.name == "BrushMatRed (Instance)")
                linesColorIndex[i] = 1;
            if (action.material.name == "BrushMatWhite (Instance)")
                linesColorIndex[i] = 2;
            if (action.material.name == "BrushMatBlue (Instance)")
                linesColorIndex[i] = 3;
            linesPoints[i] = new UnitySuxxWith2DVector(new Vector3[action.positionCount]);
            for (int j = 0; j < action.positionCount;j++)
            {
                linesPoints[i].points[j] = action.GetPosition(j);
            }
        }

        LineRendererData data = new LineRendererData(linesPoints, linesWidth, linesColorIndex);
        return data;
    }

    public void SetDrawingData(LineRendererData data)
    {
        if (drawable) { Debug.LogWarning("[Draw] Untick drawable before calling SetDrawingData"); }
        Vector3 pos = transform.position;
        Quaternion rotation = transform.rotation;
        Vector3 scale = transform.localScale;
        //clear the previous objects.
        if (linesListUndo.Count>0)
        {
            for (int i = 0; i < linesListUndo.Count; i++)
            {
                Destroy(linesListUndo[i].gameObject);
            }
        }
        linesListUndo.Clear();
        linesListRedo.Clear();
        UnitySuxxWith2DVector[] linesPoints = data.linesPoints;
        float[] linesWidth = data.linesWidth;
        int[] linesColorIndex = data.linesColorIndex;
        for (int i = 0; i < linesPoints.Length; i++)
        {
            colorIndex = linesColorIndex[i];
            currentWidth = linesWidth[i];
            CreateBrush(pos,rotation,scale);
            for (int j = 0; j < linesPoints[i].points.Length;j++)
            {
                Vector3 previouspos = linesPoints[i].points[j];
                AddAPointSet(j,previouspos);
            }
            linesListUndo.Add(currentLineRenderer);
            currentLineRenderer = null;
        }
    }

}

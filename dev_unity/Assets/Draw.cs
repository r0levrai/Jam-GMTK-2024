using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw : MonoBehaviour
{
    public Camera m_camera;
    public GameObject[] brush;

    List<LineRenderer> linesListUndo;
    List<LineRenderer> linesListRedo;
    LineRenderer currentLineRenderer;
    float currentWidth;
    int colorIndex;


    Vector2 lastPos;

    private void Awake()
    {
        currentWidth = 0.2f;
        colorIndex = 0;
        linesListUndo = new List<LineRenderer>();
        linesListRedo = new List<LineRenderer>();
    }
    private void Update()
    {
        if (m_camera.ScreenToWorldPoint(Input.mousePosition)[1]<3)
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
            currentLineRenderer.startWidth = currentWidth;
            currentLineRenderer.endWidth = currentWidth;
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
        GameObject brushInstance = Instantiate(brush[colorIndex]);
        currentLineRenderer = brushInstance.GetComponent<LineRenderer>();

        //because you gotta have 2 points to start a line renderer, 
        Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);

        currentLineRenderer.SetPosition(0, mousePos);
        currentLineRenderer.SetPosition(1, mousePos);

    }

    void AddAPoint(Vector2 pointPos)
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
            AddAPoint(mousePos);
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
}

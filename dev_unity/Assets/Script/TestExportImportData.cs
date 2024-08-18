using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestExportImportData : MonoBehaviour
{
    // Start is called before the first frame update
    public Draw drawable;
    public Draw replicator;
    public Draw.LineRendererData debug;

    public void OnClick()
    {
        Draw.LineRendererData data = drawable.GetDrawingData();
        debug = data;
        replicator.SetDrawingData(data);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Net.Http;

public class NetworkTester : MonoBehaviour
{
    public Draw[] drawings;

    static Draw.LineRendererData emptyDrawingData = new Draw.LineRendererData(new Draw.UnitySuxxWith2DVector[0], new float[0], new int[0]);
    NetworkedDrawing currentDrawing;

    [TextArea(4, 1000)] public string usageComment = "S: save to variable\n" +
                                                     "R: restore from variable\n" +
                                                     "Shift + S: send to server\n" +
                                                     "Shift + R: receive from server";

    // Update is called once per frame
    async void Update()
    {
        if (Input.GetKeyDown("s"))
        {
            if (!Input.GetKey(KeyCode.LeftShift))  // 's' pressed
            {
                print("saving drawing in variable");
                currentDrawing = new NetworkedDrawing(drawings[0].GetDrawingData(),
                    "guest", "A book", "Human", 8.3f
                );
                print(currentDrawing.ToJson());
            }
            else // Shift + S pressed
            {
                print("sending drawing to server");
                var drawing = new NetworkedDrawing(drawings[0].GetDrawingData(),
                    "guest", "A book", "Human", 8.3f
                );
                print(drawing.ToJson());
                await drawing.Send();
            }

            print("clearing drawing");
            drawings[0].SetDrawingData(emptyDrawingData);
        }
        if (Input.GetKeyDown("r"))
        {
            if (!Input.GetKey(KeyCode.LeftShift))  // 'r' pressed
            {
                print("restoring drawing from variable");
                drawings[1].SetDrawingData(currentDrawing.GetDrawingData());
            }
            else  // Shift + R pressed
            {
                print("receiving drawing from server");
                int n = 3;
                NetworkedDrawing[] received = await NetworkedDrawing.ReceiveRandom(n);
                print($"received {received.Length} drawings, asked for {n}");
                if (received.Length > 0)
                {
                    print($"displaying first:");
                    print(received[0].ToJson());
                    drawings[1].SetDrawingData(received[0].GetDrawingData());
                }
            }
        }
    }
}

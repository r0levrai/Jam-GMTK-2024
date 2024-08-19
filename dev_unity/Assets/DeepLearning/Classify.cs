using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine;
using Unity.Barracuda;

public class Classify : MonoBehaviour
{
    public NNModel modelAsset;
    private Model m_RuntimeModel;
    //public Texture2D imageToRecognise;
    //public MyPair[] result;

    private IWorker m_Worker;
    private string[] classes;

    [System.Serializable]
    public struct MyPair
    {
        public string label;
        public float score;
        public MyPair(string a, float b)
        {
            label = a;
            score = b;
        }
    }


    int compareTuple(MyPair a, MyPair b)
    {
        if (a.score == b.score)
            return 0;
        else if (a.score < b.score)
            return 1;
        else
            return -1;
    }

    void Start()
    {
        classes = new string[250]{ "airplane","alarm clock","angel","ant","apple","arm","armchair","ashtray","axe","backpack","banana","barn","baseball bat","basket","bathtub","bear (animal)","bed","bee","beer-mug","bell","bench","bicycle","binoculars","blimp","book","bookshelf","boomerang","bottle opener","bowl","brain","bread","bridge","bulldozer","bus","bush","butterfly","cabinet","cactus","cake","calculator","camel","camera","candle","cannon","canoe","car (sedan)","carrot","castle","cat","cell phone","chair","chandelier","church","cigarette","cloud","comb","computer monitor","computer-mouse","couch","cow","crab","crane (machine)","crocodile","crown","cup","diamond","dog","dolphin","donut","door","door handle","dragon","duck","ear","elephant","envelope","eye","eyeglasses","face","fan","feather","fire hydrant","fish","flashlight","floor lamp","flower with stem","flying bird","flying saucer","foot","fork","frog","frying-pan","giraffe","grapes","grenade","guitar","hamburger","hammer","hand","harp","hat","head","head-phones","hedgehog","helicopter","helmet","horse","hot air balloon","hot-dog","hourglass","house","human-skeleton","ice-cream-cone","ipod","kangaroo","key","keyboard","knife","ladder","laptop","leaf","lightbulb","lighter","lion","lobster","loudspeaker","mailbox","megaphone","mermaid","microphone","microscope","monkey","moon","mosquito","motorbike","mouse (animal)","mouth","mug","mushroom","nose","octopus","owl","palm tree","panda","paper clip","parachute","parking meter","parrot","pear","pen","penguin","person sitting","person walking","piano","pickup truck","pig","pigeon","pineapple","pipe (for smoking)","pizza","potted plant","power outlet","present","pretzel","pumpkin","purse","rabbit","race car","radio","rainbow","revolver","rifle","rollerblades","rooster","sailboat","santa claus","satellite","satellite dish","saxophone","scissors","scorpion","screwdriver","sea turtle","seagull","shark","sheep","ship","shoe","shovel","skateboard","skull","skyscraper","snail","snake","snowboard","snowman","socks","space shuttle","speed-boat","spider","sponge bob","spoon","squirrel","standing bird","stapler","strawberry","streetlight","submarine","suitcase","sun","suv","swan","sword","syringe","t-shirt","table","tablelamp","teacup","teapot","teddy-bear","telephone","tennis-racket","tent","tiger","tire","toilet","tomato","tooth","toothbrush","tractor","traffic light","train","tree","trombone","trousers","truck","trumpet","tv","umbrella","van","vase","violin","walkie talkie","wheel","wheelbarrow","windmill","wine-bottle","wineglass","wrist-watch","zebra" };
        m_RuntimeModel = ModelLoader.Load(modelAsset);
        m_Worker = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, m_RuntimeModel);
    }

    public MyPair[] Classification(Texture2D imageToRecognise)
    {
		Texture2D newTex = imageToRecognise;
		TextureScale.Bilinear(newTex, 224, 224);
		Tensor input = new Tensor(newTex, channels: 3);
        m_Worker.Execute(input);
        Tensor output = m_Worker.PeekOutput();
        MyPair[] result = new MyPair[output.length];
        for (int i = 0; i < output.length; i++)
        {
            MyPair pair = new MyPair(classes[i], output[i]);
            result[i] = pair;
        }
        System.Array.Sort<MyPair>(result, new System.Comparison<MyPair>(
                  (i1, i2) => compareTuple(i1, i2)));
		input.Dispose();
		return result;
    }
}

public class TextureScale
{
	public class ThreadData
	{
		public int start;
		public int end;
		public ThreadData(int s, int e)
		{
			start = s;
			end = e;
		}
	}

	private static Color[] texColors;
	private static Color[] newColors;
	private static int w;
	private static float ratioX;
	private static float ratioY;
	private static int w2;
	private static int finishCount;
	private static Mutex mutex;

	public static void Point(Texture2D tex, int newWidth, int newHeight)
	{
		ThreadedScale(tex, newWidth, newHeight, false);
	}

	public static void Bilinear(Texture2D tex, int newWidth, int newHeight)
	{
		ThreadedScale(tex, newWidth, newHeight, true);
	}

	private static void ThreadedScale(Texture2D tex, int newWidth, int newHeight, bool useBilinear)
	{
		texColors = tex.GetPixels();
		newColors = new Color[newWidth * newHeight];
		if (useBilinear)
		{
			ratioX = 1.0f / ((float)newWidth / (tex.width - 1));
			ratioY = 1.0f / ((float)newHeight / (tex.height - 1));
		}
		else
		{
			ratioX = ((float)tex.width) / newWidth;
			ratioY = ((float)tex.height) / newHeight;
		}
		w = tex.width;
		w2 = newWidth;
		var cores = Mathf.Min(SystemInfo.processorCount, newHeight);
		var slice = newHeight / cores;

		finishCount = 0;
		if (mutex == null)
		{
			mutex = new Mutex(false);
		}
		if (cores > 1)
		{
			int i = 0;
			ThreadData threadData;
			for (i = 0; i < cores - 1; i++)
			{
				threadData = new ThreadData(slice * i, slice * (i + 1));
				ParameterizedThreadStart ts = useBilinear ? new ParameterizedThreadStart(BilinearScale) : new ParameterizedThreadStart(PointScale);
				Thread thread = new Thread(ts);
				thread.Start(threadData);
			}
			threadData = new ThreadData(slice * i, newHeight);
			if (useBilinear)
			{
				BilinearScale(threadData);
			}
			else
			{
				PointScale(threadData);
			}
			while (finishCount < cores)
			{
				Thread.Sleep(1);
			}
		}
		else
		{
			ThreadData threadData = new ThreadData(0, newHeight);
			if (useBilinear)
			{
				BilinearScale(threadData);
			}
			else
			{
				PointScale(threadData);
			}
		}

		tex.Reinitialize(newWidth, newHeight);
		tex.SetPixels(newColors);
		tex.Apply();

		texColors = null;
		newColors = null;
	}

	public static void BilinearScale(System.Object obj)
	{
		ThreadData threadData = (ThreadData)obj;
		for (var y = threadData.start; y < threadData.end; y++)
		{
			int yFloor = (int)Mathf.Floor(y * ratioY);
			var y1 = yFloor * w;
			var y2 = (yFloor + 1) * w;
			var yw = y * w2;

			for (var x = 0; x < w2; x++)
			{
				int xFloor = (int)Mathf.Floor(x * ratioX);
				var xLerp = x * ratioX - xFloor;
				newColors[yw + x] = ColorLerpUnclamped(ColorLerpUnclamped(texColors[y1 + xFloor], texColors[y1 + xFloor + 1], xLerp),
													   ColorLerpUnclamped(texColors[y2 + xFloor], texColors[y2 + xFloor + 1], xLerp),
													   y * ratioY - yFloor);
			}
		}

		mutex.WaitOne();
		finishCount++;
		mutex.ReleaseMutex();
	}

	public static void PointScale(System.Object obj)
	{
		ThreadData threadData = (ThreadData)obj;
		for (var y = threadData.start; y < threadData.end; y++)
		{
			var thisY = (int)(ratioY * y) * w;
			var yw = y * w2;
			for (var x = 0; x < w2; x++)
			{
				newColors[yw + x] = texColors[(int)(thisY + ratioX * x)];
			}
		}

		mutex.WaitOne();
		finishCount++;
		mutex.ReleaseMutex();
	}

	private static Color ColorLerpUnclamped(Color c1, Color c2, float value)
	{
		return new Color(c1.r + (c2.r - c1.r) * value,
						  c1.g + (c2.g - c1.g) * value,
						  c1.b + (c2.b - c1.b) * value,
						  c1.a + (c2.a - c1.a) * value);
	}
}

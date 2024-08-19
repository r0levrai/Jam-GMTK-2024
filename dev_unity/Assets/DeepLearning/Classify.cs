using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;

public class Classify : MonoBehaviour
{
    public NNModel modelAsset;
    private Model m_RuntimeModel;
    public Texture2D imageToRecognise;
    public MyPair[] result;

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
        classes =new string[250]{ "airplane","alarm clock","angel","ant","apple","arm","armchair","ashtray","axe","backpack","banana","barn","baseball bat","basket","bathtub","bear (animal)","bed","bee","beer-mug","bell","bench","bicycle","binoculars","blimp","book","bookshelf","boomerang","bottle opener","bowl","brain","bread","bridge","bulldozer","bus","bush","butterfly","cabinet","cactus","cake","calculator","camel","camera","candle","cannon","canoe","car (sedan)","carrot","castle","cat","cell phone","chair","chandelier","church","cigarette","cloud","comb","computer monitor","computer-mouse","couch","cow","crab","crane (machine)","crocodile","crown","cup","diamond","dog","dolphin","donut","door","door handle","dragon","duck","ear","elephant","envelope","eye","eyeglasses","face","fan","feather","fire hydrant","fish","flashlight","floor lamp","flower with stem","flying bird","flying saucer","foot","fork","frog","frying-pan","giraffe","grapes","grenade","guitar","hamburger","hammer","hand","harp","hat","head","head-phones","hedgehog","helicopter","helmet","horse","hot air balloon","hot-dog","hourglass","house","human-skeleton","ice-cream-cone","ipod","kangaroo","key","keyboard","knife","ladder","laptop","leaf","lightbulb","lighter","lion","lobster","loudspeaker","mailbox","megaphone","mermaid","microphone","microscope","monkey","moon","mosquito","motorbike","mouse (animal)","mouth","mug","mushroom","nose","octopus","owl","palm tree","panda","paper clip","parachute","parking meter","parrot","pear","pen","penguin","person sitting","person walking","piano","pickup truck","pig","pigeon","pineapple","pipe (for smoking)","pizza","potted plant","power outlet","present","pretzel","pumpkin","purse","rabbit","race car","radio","rainbow","revolver","rifle","rollerblades","rooster","sailboat","santa claus","satellite","satellite dish","saxophone","scissors","scorpion","screwdriver","sea turtle","seagull","shark","sheep","ship","shoe","shovel","skateboard","skull","skyscraper","snail","snake","snowboard","snowman","socks","space shuttle","speed-boat","spider","sponge bob","spoon","squirrel","standing bird","stapler","strawberry","streetlight","submarine","suitcase","sun","suv","swan","sword","syringe","t-shirt","table","tablelamp","teacup","teapot","teddy-bear","telephone","tennis-racket","tent","tiger","tire","toilet","tomato","tooth","toothbrush","tractor","traffic light","train","tree","trombone","trousers","truck","trumpet","tv","umbrella","van","vase","violin","walkie talkie","wheel","wheelbarrow","windmill","wine-bottle","wineglass","wrist-watch","zebra" };
        m_RuntimeModel = ModelLoader.Load(modelAsset);
        m_Worker = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, m_RuntimeModel);
        Tensor input = new Tensor(imageToRecognise, channels: 3);
        m_Worker.Execute(input);
        Tensor output = m_Worker.PeekOutput();
        result = new MyPair[output.length];
        for (int i = 0; i < output.length; i++)
        {
            MyPair pair = new MyPair(classes[i], output[i]);
            result[i] = pair;
        }
        System.Array.Sort<MyPair>(result, new System.Comparison<MyPair>(
                  (i1, i2) => compareTuple(i1,i2)));

    }

    public void Classification()
    {
        Tensor input = new Tensor(1, 224, 224, 3);
        m_Worker.Execute(input);
        Tensor output = m_Worker.PeekOutput();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndcardManager : MonoBehaviour
{
    public EndCard endcard;
    public int nCards = 11;

    // Start is called before the first frame update
    async void Start()
    {
        NetworkedDrawing[] drawings = await NetworkedDrawing.ReceiveLasts(nCards);

        SpawnCards();
        
        EndCard[] cards = GetComponents<EndCard>();
        Debug.Assert(cards.Length == nCards);
        PopulateCards(cards, drawings);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnCards()
    {
        for (int i = 0; i < 4; i++)
        {
            EndCard ec2 = Instantiate(endcard, new Vector3(0, 0, 0), Quaternion.identity);
            float rot2 = Random.Range(2.0f, 10.0f);
            if (Random.Range(0.0f, 1.0f) < 0.5f) rot2 *= -1;
            ec2.setupRotation(rot2 * 3, -rot2);
            ec2.setupScale(0.5f, 0.5f);
            ec2.setupPosition(new Vector3(30 / 5.0f * (i + 1) - 15, 7.5f), new Vector3(18 / 5.0f * (i + 1) - 9, 3.5f));
            ec2.time_ = -1.3f - Random.Range(0.0f, 0.25f);
        }

        for (int i = 0; i < 4; i++)
        {
            EndCard ec2 = Instantiate(endcard, new Vector3(0, 0, 0), Quaternion.identity);
            float rot2 = Random.Range(2.0f, 10.0f);
            if (Random.Range(0.0f, 1.0f) < 0.5f) rot2 *= -1;
            ec2.setupRotation(rot2 * 3, -rot2);
            ec2.setupScale(0.5f, 0.5f);
            ec2.setupPosition(new Vector3(30 / 5.0f * (i + 1) - 15, -7.5f), new Vector3(18 / 5.0f * (i + 1) - 9, -3.5f));
            ec2.time_ = -1.3f - Random.Range(0.0f, 0.25f);
        }

        EndCard ec = Instantiate(endcard, new Vector3(0, 0, 0), Quaternion.identity);
        float rot = Random.Range(2.0f, 10.0f);
        if (Random.Range(0.0f, 1.0f) < 0.5f) rot *= -1;
        ec.setupRotation(rot * 3, -rot);
        ec.setupScale(0.5f, 0.5f);
        ec.setupPosition(new Vector3(-15, 0), new Vector3(18 / 5.0f - 9, 0));
        ec.time_ = -1.3f - Random.Range(0.0f, 0.25f);

        ec = Instantiate(endcard, new Vector3(0, 0, 0), Quaternion.identity);
        rot = Random.Range(2.0f, 10.0f);
        if (Random.Range(0.0f, 1.0f) < 0.5f) rot *= -1;
        ec.setupRotation(rot * 3, -rot);
        ec.setupScale(0.5f, 0.5f);
        ec.setupPosition(new Vector3(15, 0), new Vector3(18 / 5.0f * 4 - 9, 0));
        ec.time_ = -1.3f - Random.Range(0.0f, 0.25f);

        ec = Instantiate(endcard, new Vector3(0, 0, 0), Quaternion.identity);
        ec.setupRotation(2, -2);
        ec.setupScale(0.35f, 0.7f);
        ec.setupPosition(new Vector3(-0, -0, 0), new Vector3(0, 0, 0));
        ec.time_ = -1;
    }

    void PopulateCards(EndCard[] cards, NetworkedDrawing[] drawings)
    {
        for (int i = 0; i < drawings.Length; i++)
        {
            cards[i].timeAgoText.text = drawings[i].GetTimeDifference();
            int iBg = int.TryParse(drawings[i].data.background, out iBg) ? iBg : 0;
            cards[i].background.sprite = Constants.Instance.GetCurrentImage(iBg);
            cards[i].draw.SetDrawingData(drawings[i].GetDrawingData());
        }
    }
}

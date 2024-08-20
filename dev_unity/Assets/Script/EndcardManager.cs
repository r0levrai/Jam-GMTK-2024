using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndcardManager : MonoBehaviour
{
    public EndCard endcard;
    public int nCards = 11;

    private EndCard playerCard;
    private List<EndCard> otherCards;
    private bool canGetPlayerDrawing;

    // Start is called before the first frame update
    async void Start()
    {
        canGetPlayerDrawing = Draw.Instance != null;  // skip populating main player drawing when
                                                      // directly launching the GameOver scene

        playerCard = Instantiate(endcard, new Vector3(0, 0, 0), Quaternion.identity);
        playerCard.setupRotation(2, -2);
        playerCard.setupScale(0.35f, 0.7f);
        playerCard.setupPosition(new Vector3(-0, -0, 0), new Vector3(0, 0, 0));
        playerCard.time_ = -1;

        if (canGetPlayerDrawing)
        {
            PopulateCard(playerCard, Constants.Instance.GetPlayerDrawing(), customText: "Now :)");
        }

        // next line will take a while
        NetworkedDrawing[] drawings = await NetworkedDrawing.ReceiveLasts(nCards);

        SpawnOtherCards(drawings);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnOtherCards(NetworkedDrawing[] drawings)
    {
        otherCards = new List<EndCard>();

        for (int i = 0; i < 4; i++)
        {
            if (drawings.Length <= i) { return; }
            EndCard ec2 = Instantiate(endcard, new Vector3(0, 0, 0), Quaternion.identity);
            float rot2 = Random.Range(2.0f, 10.0f);
            if (Random.Range(0.0f, 1.0f) < 0.5f) rot2 *= -1;
            ec2.setupRotation(rot2 * 3, -rot2);
            ec2.setupScale(0.5f, 0.5f);
            ec2.setupPosition(new Vector3(30 / 5.0f * (i + 1) - 15, 7.5f), new Vector3(18 / 5.0f * (i + 1) - 9, 3.5f));
            ec2.time_ = -1.3f - Random.Range(0.0f, 0.25f);
            PopulateCard(ec2, drawings[i]);
            otherCards.Add(ec2);
        }

        for (int i = 0; i < 4; i++)
        {
            if (drawings.Length <= 4+i) { return; }
            EndCard ec2 = Instantiate(endcard, new Vector3(0, 0, 0), Quaternion.identity);
            float rot2 = Random.Range(2.0f, 10.0f);
            if (Random.Range(0.0f, 1.0f) < 0.5f) rot2 *= -1;
            ec2.setupRotation(rot2 * 3, -rot2);
            ec2.setupScale(0.5f, 0.5f);
            ec2.setupPosition(new Vector3(30 / 5.0f * (i + 1) - 15, -7.5f), new Vector3(18 / 5.0f * (i + 1) - 9, -3.5f));
            ec2.time_ = -1.3f - Random.Range(0.0f, 0.25f);
            PopulateCard(ec2, drawings[i + 4]);
            otherCards.Add(ec2);
        }

        if (drawings.Length < 9) { return; }
        EndCard ec = Instantiate(endcard, new Vector3(0, 0, 0), Quaternion.identity);
        float rot = Random.Range(2.0f, 10.0f);
        if (Random.Range(0.0f, 1.0f) < 0.5f) rot *= -1;
        ec.setupRotation(rot * 3, -rot);
        ec.setupScale(0.5f, 0.5f);
        ec.setupPosition(new Vector3(-15, 0), new Vector3(18 / 5.0f - 9, 0));
        ec.time_ = -1.3f - Random.Range(0.0f, 0.25f);
        PopulateCard(ec, drawings[8]);
        otherCards.Add(ec);

        if (drawings.Length < 10) { return; }
        ec = Instantiate(endcard, new Vector3(0, 0, 0), Quaternion.identity);
        rot = Random.Range(2.0f, 10.0f);
        if (Random.Range(0.0f, 1.0f) < 0.5f) rot *= -1;
        ec.setupRotation(rot * 3, -rot);
        ec.setupScale(0.5f, 0.5f);
        ec.setupPosition(new Vector3(15, 0), new Vector3(18 / 5.0f * 4 - 9, 0));
        ec.time_ = -1.3f - Random.Range(0.0f, 0.25f);
        PopulateCard(ec, drawings[9]);
        otherCards.Add(ec);
    }

    void PopulateCard(EndCard card, NetworkedDrawing drawing, string customText = null)
    {
        card.timeAgoText.text = customText != null ? customText : drawing.GetTimeDifference();
        int iBg = int.TryParse(drawing.data.background, out iBg) ? iBg : 0;
        card.background.sprite = Constants.Instance.GetCurrentImage(iBg);
        card.draw.SetDrawingData(drawing.GetDrawingData());
    }
}

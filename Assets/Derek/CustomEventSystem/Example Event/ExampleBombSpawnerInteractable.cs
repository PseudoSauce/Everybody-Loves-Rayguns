using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTypes;

public class ExampleBombSpawnerInteractable : Interactable {
    [SerializeField] GameObject bombPrefab;
    [SerializeField] int[] numbersRequired;

    private List<int> receivedNumbers;

    ////////////////////////////////
    //     Interactable Stuff 
    ///////////////////////////////
	protected override void Init()
    {
        receivedNumbers = new List<int>();

        AssignInteractionType(Interaction.EXAMPLEBOMB);
        AssignStart(MyStart);

        // assigns required for this particular interactable
        AssignCustomEventReceiveNotify(ReceiveCustomEvent, ReceiveManagerEvent);
    }

    private void MyStart()
    {
        EventBeacon.RegisterEvents((uint)CustomEventExamples.BombTriggerEvent);
    }

    ////////////////////////////////
    //     Custom Event Stuff 
    ///////////////////////////////
    private void ReceiveCustomEvent(CustomEventPacket handlerPacket)
    {
        var eventID = handlerPacket.Handler.EventID;
        print("dasfasf");
        // received the proper event, so triggers the spawn of a bomb,
        // once conditions are met (ie. all 3 switches pressed)
        if ((CustomEventExamples)eventID == CustomEventExamples.BombTriggerEvent)
        {
            BombTriggerEvent handler = (BombTriggerEvent)handlerPacket.Handler;

            if (!receivedNumbers.Contains(handler.switchNumber))
            {
                receivedNumbers.Add(handler.switchNumber);
            }

            if (receivedNumbers.Count == numbersRequired.Length)
            {
                SpawnBomb();
            }
        }
    }

    private void ReceiveManagerEvent(ICustomEventManagerHandler handler)
    {

    }

    ////////////////////////////////
    //     Other Stuff
    ///////////////////////////////
    private void SpawnBomb()
    {
        GameObject obj = Instantiate(bombPrefab, transform);
    }
}

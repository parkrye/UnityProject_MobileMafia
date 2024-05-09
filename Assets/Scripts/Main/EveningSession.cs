using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EveningSession : Session
{
    protected override void AwakeSelf()
    {
        base.AwakeSelf();

        Time = 30f;
    }

    public override void StartSession()
    {
        base.StartSession();


    }

    public override void EndSession(int mostVotedIndex)
    {
        base.EndSession(mostVotedIndex);


    }
}

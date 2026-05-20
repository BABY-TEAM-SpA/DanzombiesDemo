using System;
using UnityEngine;

public class PlayerBeatReciever : BeatReciever //zombies ahora usan AnimBeatReceiver para lo que antes hacía este script.
{
    [SerializeField] PlayerManager managerComp;
    [SerializeField] PlayerDanceMemory danceMemoryComp;

    bool danceExecutedThisBeat; /*si se ha hecho un baile en este beat. Se reinicia en pre-beats. Se usa para saber cuando el jugador debe mandar 
    DanceStep.None al DanceMemory. Las zonas de zombies TIENEN SU PROPIA FORMA DE CHECKEAR ESTO y NO usan esta variable (TODO: hacer que ambas cosas
    usen la misma forma de checkear para que sea imposible que desacuerden?)*/

    void Start()
    {
        managerComp.OnDanceEvent.AddListener(RaiseDanceThisBeatFlag);
    }

    public override void PreBeatAction(int counter)
    {
        base.PreBeatAction(counter);
        danceExecutedThisBeat = false;
    }

    public override void BeatAction(int counter)
    {
        base.BeatAction(counter);
    }
    
    public override void PostBeatAction(int counter)
    {
        base.PostBeatAction(counter);
        if (!danceExecutedThisBeat)
        {
            danceMemoryComp.RememberStep(managerComp, DanceStep.None);
        }
    }

    void RaiseDanceThisBeatFlag(PlayerManager throwaway1, DanceStep throwaway2)
    {
        danceExecutedThisBeat = true;
    }
}

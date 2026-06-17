using System;
using UnityEngine;

public class PlayerBeatReciever : BeatReciever //zombies ahora usan AnimBeatReceiver para lo que antes hacía este script.
{
    PlayerManager managerComp;
    PlayerDanceMemory danceMemoryComp;
    PlayerMovementController moveController;

    bool danceExecutedThisBeat; /*si se ha hecho un baile en este beat. Se reinicia en pre-beats. Se usa para saber cuando el jugador debe mandar 
    DanceStep.None al DanceMemory. Las zonas de zombies TIENEN SU PROPIA FORMA DE CHECKEAR ESTO y NO usan esta variable (TODO: hacer que ambas cosas
    usen la misma forma de checkear para que sea imposible que desacuerden?)*/

    public void SetReferences(PlayerManager newManager, PlayerDanceMemory newDanceMemory, PlayerMovementController newMoveCont)
    {  //Allows these refs to be set from PlayerManager to ensure they always agree (and only have to be set once in the inspector)
        if (managerComp != null)
        {
            managerComp.OnDanceEvent.RemoveListener(RaiseDanceThisBeatFlag);
        }
        newManager.OnDanceEvent.AddListener(RaiseDanceThisBeatFlag);

        managerComp = newManager;
        danceMemoryComp = newDanceMemory;
        moveController =  newMoveCont;
    }

    public override void PreBeatAction(int counter)
    {
        base.PreBeatAction(counter);
        danceExecutedThisBeat = false;
    }

    public override void BeatAction(int counter)
    {
        base.BeatAction(counter);
        moveController.PassBeat();
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

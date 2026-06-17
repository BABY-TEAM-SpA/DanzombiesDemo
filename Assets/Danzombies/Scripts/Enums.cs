//poniendo los enums publicos aca para mas orden.
//ignorando los que son definidos dentro de una clase

using System.Linq; //para Array.Cast en Extensions

public enum SortingLayers{ // De Position3d.cs
    OnStart,
    OnUpdate,
    None,
}

public enum UiEasingType{ // De UIEasing.cs
    Linear,
    EaseInQuad,
    EaseOutQuad,
    EaseInOutQuad,
    EaseInCubic,
    EaseOutCubic,
    EaseInOutCubic,
    EaseInQuart,
    EaseOutQuart,
    EaseInOutQuart,
    EaseOutBounce,
    EaseOutElastic
}

public enum UiTimeMode{ // De UiAnimation.cs
    Scaled,
    Unscaled,
    DSP
}

public enum SeguridadState // De PlayerManager.cs
{
    Normal,
    Insecure,
    Flow
}

public enum DanceState{ // De PlayerAnimatorController.cs
	None,
	North,
	South,
	West,
	East
}

public enum ActionToPlaySong{ // De MusicLevelController
    Enqueue,
    Interrupt
}


public enum DanceStep{ // De RhythmPuzzle.cs
    None,
    L_North,
    R_North,
    L_South,
    R_South,
    L_West,
    R_West,
    L_East,
    R_East,
    Any //permite tener un input que responde a cualquier paso por parte del player
}

public enum Timing{ // De RhythmPuzzle.cs
    Early,
    Late,
    Miss
}

public enum BeatType{ // De BeatManager.cs
    Negra,
    Blanca,
    Redonda
}

public static class Extensions{ //Extensiones para dar algo de funcionalidad extra a estos enums
    public static DanceStep DetermineStep(this DanceStep step) //si Any, retorna un paso aleatorio que no sea None ni Any. Si no, retorna el paso recibido
    { 
        if (step != DanceStep.Any)
        {
            return step;
        } 
        else
        {
            var validSteps = System.Enum.GetValues(typeof(DanceStep))
                                .Cast<DanceStep>()
                                .Where(s => s != DanceStep.None && s != DanceStep.Any)
                                .ToArray(); //crea un array con todos los pasos que no sean None ni Any
            return validSteps[UnityEngine.Random.Range(0, validSteps.Length)]; // y retorna uno aleatorio entre esos.
        }
    }
}
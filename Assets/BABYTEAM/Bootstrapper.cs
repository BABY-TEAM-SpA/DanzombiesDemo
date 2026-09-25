using UnityEngine;

/// <summary>
/// En una carpeta Resources, crear un Prefab con el nombre "[Systems]".
/// Systems deberá contener todos los objetos que se desea sean persistentes en todo el proyecto,
/// como un AudioManager o la cámara principal.
/// </summary>
public static class Bootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute() => Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("[Systems]")));
}
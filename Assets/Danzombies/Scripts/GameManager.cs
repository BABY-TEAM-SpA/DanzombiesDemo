using UnityEngine;

public class GameManager : Service<GameManager>
{
    [SerializeField][Range(0, 2)] private int alza = 1;
    public int Alza => alza;

    public Languages Language { get; set; }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class DanceStepEvaluator
{
    #region [VARIABLES]
    private readonly List<StepRegist> expected = new();
    private readonly List<StepRegist> player = new();

    public event Action<BeatReciever.BeatFeedback> OnFeedback;

    private float Window => BeatManager.Instance.BeatDurationMs / 3f; // <- [Frco] Equivalente a 2/6 de un Beat
    #endregion

    #region [METHODS]
    #region Add
    public void AddExpected(DanceStep step, int timeMs)
    {
        Expire(timeMs);
        Insert(new StepRegist(step, timeMs), expected, player);
    }

    public void AddPlayer(DanceStep step, int timeMs)
    {
        Expire(timeMs);
        Insert(new StepRegist(step, timeMs), player, expected);
    }
    #endregion

    #region Insert
    /// <summary>
    /// Cuando se añade un StepRegist nuevo a una de las listas, puede ocurrir que:
    /// A. El jugador hace un input precipitado, su lista tiene un StepRegist que todavía no existe en la del puzzle.
    ///    Resultado: se inserta su StepRegist en 'player' hasta que se consuma o expire.
    /// B. El jugador hace un input atrasado, la lista del puzzle ya cuenta con un StepRegist con el mismo DanceStep.
    ///    Resultado: se omite la inserción y directamente se consumen ambos StepRegist.
    /// 
    /// Funcionamiento:
    /// 1. Busca si en el momento de la inserción, existe un StepRegist en 'other' con el mismo DanceStep.
    /// 2. Mantiene en 'bestDelta' el menor tiempo entre el StepRegist insertado y la coincidencia en 'other'.
    /// 3. Si hubo coincidencia con 'bestDelta' < 'Window', se "consume" el StepRegist de 'other' y se llama a Grade()
    ///    para que evalúe el BeatFeedback a retornar.
    /// 4. Si no ocurrió (3), se inserta en 'own'.
    /// </summary>
    /// <param name="reg">StepRegist recién llegado.</param>
    /// <param name="own">Lista de destino.</param>
    /// <param name="other">Lista a comparar.</param>
    private void Insert(StepRegist reg, List<StepRegist> own, List<StepRegist> other)
    {
        int best = -1;
        int bestDelta = int.MaxValue;
        for (int i = 0; i < other.Count; i++)
        {
            if (other[i].danceStep != reg.danceStep) continue;
            int delta = Math.Abs(other[i].stepTimeMs - reg.stepTimeMs);
            if (delta <= Window && delta < bestDelta)
            {
                best = i;
                bestDelta = delta;
            }
        }

        if (best >= 0)
        {
            other.RemoveAt(best);
            OnFeedback?.Invoke(Grade(bestDelta));
        }
        else own.Add(reg);
    }

    private BeatReciever.BeatFeedback Grade(int absDelta)
    {
        BeatManager bm = BeatManager.Instance;
        float max = bm.BeatDurationMs * bm.margenPercentOnBeat;
        float great = max * bm.greatPercentOnMargin;
        float perfect = great * bm.perfectPercentOnMargin;

        if (absDelta <= perfect) return BeatReciever.BeatFeedback.Perfect;
        if (absDelta <= great) return BeatReciever.BeatFeedback.Good;
        return BeatReciever.BeatFeedback.Ok;
    }
    #endregion

    #region Expire
    private void Expire(int nowMs)
    {
        RemoveExpired(expected, player, nowMs);
        RemoveExpired(player, expected, nowMs);
    }

    private void RemoveExpired(List<StepRegist> own, List<StepRegist> other, int nowMs)
    {
        for (int i = own.Count - 1; i >= 0; i--)
        {
            StepRegist reg = own[i];
            if (nowMs - reg.stepTimeMs <= Window) continue;

            own.RemoveAt(i);
            if (reg.penalized) continue;

            OnFeedback?.Invoke(BeatReciever.BeatFeedback.Bad);
            PenalizeNeighbor(reg, other);
        }
    }

    /// <summary>
    /// Busca en la otra lista el StepRegist más cercano en tiempo al registro que expiró y lo marca como 'penalized'
    /// para evitar que se considere como Bad al expirar.
    /// </summary>
    /// <param name="reg">StepRegist de la otra lista, expirado y que fue considerado como Bad.</param>
    /// <param name="other">Lista opuesta y en la que se busca en StepRegist más próximo a expirar.</param>
    private void PenalizeNeighbor(StepRegist reg, List<StepRegist> other)
    {
        StepRegist best = null;
        int bestDelta = int.MaxValue;
        foreach (StepRegist candidate in other)
        {
            if (candidate.penalized) continue;
            int delta = Math.Abs(candidate.stepTimeMs - reg.stepTimeMs);
            if (delta <= Window && delta < bestDelta)
            {
                best = candidate;
                bestDelta = delta;
            }
        }
        if (best != null) best.penalized = true;
    }
    #endregion

    #region Helpers
    public void Tick(int nowMs) => Expire(nowMs);

    public void Clear()
    {
        expected.Clear();
        player.Clear();
    }
    #endregion
    #endregion
}

public class StepRegist
{
    public DanceStep danceStep;
    public int stepTimeMs;
    public bool penalized; // <- [Frco] Marcar como verdadero para evitar que la expiración del StepRegist lance un Bad

    public StepRegist(DanceStep step, int timeMs)
    {
        danceStep = step;
        stepTimeMs = timeMs;
    }
}
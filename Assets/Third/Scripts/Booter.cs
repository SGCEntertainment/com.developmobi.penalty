using UnityEngine;

public static class Booter
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        Object.Instantiate(Resources.Load("loading"));
    }
}
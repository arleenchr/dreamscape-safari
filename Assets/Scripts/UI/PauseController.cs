using BondomanShooter.Game;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    public static void PauseGame()
    {
        TimeControl.Instance.IsPaused = true;
    }

    public static void ContinueGame()
    {
        TimeControl.Instance.IsPaused = false;
    }
}

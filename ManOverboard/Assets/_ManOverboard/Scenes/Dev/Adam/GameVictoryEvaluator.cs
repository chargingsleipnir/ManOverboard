using UnityEngine;
using UnityEngine.UI;

public class GameVictoryEvaluator : MonoBehaviour {

    public Text VictoryMessageText;

    public GameCtrl GameController;

    public AdamLevelManager LevelManager;

    public void EvaluateConditions()
    {
        int charLoss = LevelManager.PassengerStartingCount - LevelManager.passengerSet.Count;

        if (charLoss <= GameController.GetLevelMaxCharLoss(3))
        {
            VictoryMessageText.text = "You're a winner! 3 star play!";
        }
        else if (charLoss <= GameController.GetLevelMaxCharLoss(2))
        {
            VictoryMessageText.text = "You're a winner! 2 star play!";
        }
        else if (charLoss <= GameController.GetLevelMaxCharLoss(1))
        {
            VictoryMessageText.text = "You're a winner! 1 star play!";
        }
    }
}

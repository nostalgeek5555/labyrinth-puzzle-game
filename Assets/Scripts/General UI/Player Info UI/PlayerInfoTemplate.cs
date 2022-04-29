using UnityEngine;
using TMPro;

public class PlayerInfoTemplate : MonoBehaviour
{
    public Pion pion;
    public TextMeshProUGUI playerCardInfoText;

    public void Init(Pion _pion)
    {
        pion = _pion;
        _pion.playerInfoTemplate = this;
        UpdateInfo();
    }

    public void UpdateInfo()
    {
        playerCardInfoText.text = pion.type.ToString() + " " + pion.playerId.ToString() + " = " + pion.cards.Count + " / " + pion.GetCardCount();
        Debug.Log(playerCardInfoText.text);
    }
}

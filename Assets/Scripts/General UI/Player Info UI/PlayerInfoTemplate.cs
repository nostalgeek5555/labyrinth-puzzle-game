using UnityEngine;
using TMPro;

public class PlayerInfoTemplate : MonoBehaviour
{
    public Pion pion;
    public TextMeshProUGUI playerCardInfoText;

    private Pion.Type _type;
    private string _playerName;
    private int _playerId;
    private int _playerCardCount;

    public void Init(Pion _pion)
    {
        pion = _pion;
        _type = _pion.type;
        _playerName = _pion.playerName;
        _playerId = _pion.playerId;
        _playerCardCount = _pion.cards.Count;
        _pion.playerInfoTemplate = this;
        UpdateInfo();
    }

    public void UpdateInfo()
    {
        if (_type == Pion.Type.AI)
        {
            playerCardInfoText.text = _type.ToString() + " " + _playerId.ToString() + " = " + _playerCardCount + " / " + pion.GetCardCount();
            Debug.Log(playerCardInfoText.text);
        }
        
        else
        {
            Debug.Log($"player name = {_playerName}");
            if (_playerName != "")
            {
                playerCardInfoText.text = _playerName + " = " + _playerCardCount + " / " + pion.GetCardCount();
                Debug.Log(playerCardInfoText.text);
            }
            
            else
            {
                playerCardInfoText.text = _type.ToString() + " " + _playerId.ToString() + " = " + _playerCardCount + " / " + pion.GetCardCount();
                Debug.Log(playerCardInfoText.text);
            }
        }
    }
}

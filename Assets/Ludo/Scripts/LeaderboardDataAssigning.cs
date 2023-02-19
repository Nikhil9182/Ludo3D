using UnityEngine.UI;
using UnityEngine;

public class LeaderboardDataAssigning : MonoBehaviour
{
    [SerializeField] Text _name;
    [SerializeField] Text _rank;
    [SerializeField] Text _amount;

    public void AssignText(string name, string rank, string amount)
    {
        _name.text = name;
        _rank.text = rank;
        _amount.text = amount;
    }
}

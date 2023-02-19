using UnityEngine;

[CreateAssetMenu(fileName ="UserData",menuName ="Inventory/ScrptableData")]
public class UserAuthData : ScriptableObject
{
    public string sessionToken;
    public string userName;
    public int userId;
    public string phoneNumber;
    public string emailId;
    public string refCode;
    public bool is_completed;
}

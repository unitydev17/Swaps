using UnityEngine;

public class UserDataStorage : IUserRepository
{
    private const string UserDataParam = "UserData";

    public void Save(UserData data)
    {
        var serialized = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(UserDataParam, serialized);
        PlayerPrefs.Save();
    }

    public UserData Load()
    {
        var data = PlayerPrefs.GetString(UserDataParam);
        return string.IsNullOrEmpty(data) ? new UserData() : JsonUtility.FromJson<UserData>(data);
    }
}
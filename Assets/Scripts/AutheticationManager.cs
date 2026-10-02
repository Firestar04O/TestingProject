using UnityEngine;

public class AuthenticationManager : MonoBehaviour
{
    public bool ValidateLogin(string username, string password)
    {
        return username == "gato" && password == "123456789";
    }
}
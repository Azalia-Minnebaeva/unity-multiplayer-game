using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script
{
    public class LoginButton : MonoBehaviour
    {
        public TMP_InputField usernameInput;
        public TMP_InputField passwordInput;
        public Button loginButton;

        private void Start()
        {
            loginButton.onClick.AddListener(OnLoginClicked);
        }

        private async void OnLoginClicked()
        {
            string username = usernameInput.text.Trim();
            string password = passwordInput.text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Debug.LogWarning("Заполните логин и пароль");
                return;
            }

            bool success = await APIManager.Instance.SignIn(username, password);
            if (success)
            {
                SceneTransitionManager.Instance.LoadScene("MainMenuThree");
            }
            else
            {
                Debug.Log("Неверный логин или пароль");
            }
        }
    }
}


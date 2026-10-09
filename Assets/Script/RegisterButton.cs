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
    public class RegisterButton : MonoBehaviour
    {
        public TMP_InputField usernameInput;
        public TMP_InputField passwordInput;
        public Button registerButton;

        private void Start()
        {
            registerButton.onClick.AddListener(OnRegisterClicked);
        }

        private async void OnRegisterClicked()
        {
            string username = usernameInput.text.Trim();
            string password = passwordInput.text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Debug.LogWarning("Заполните логин и пароль");
                return;
            }

            bool success = await APIManager.Instance.SignUp(username, password);
            if (success)
            {
                SceneTransitionManager.Instance.LoadScene("MainMenuThree");
            }
            else
            {
                Debug.Log("Ошибка регистрации. Возможно, имя пользователя занято.");
            }
        }
    }
}

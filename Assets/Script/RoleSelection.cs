using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Script
{
    public class RoleSelection : MonoBehaviour
    {
        public void OnHostClicked()
        {
            SceneTransitionManager.Instance.SetRoleAndLoadLevelSelection("Host");
        }

        public void OnClientClicked()
        {
            SceneTransitionManager.Instance.SetRoleAndLoadLevelSelection("Client");
        }
    }
}

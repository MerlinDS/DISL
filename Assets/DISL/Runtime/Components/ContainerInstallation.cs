using DISL.Runtime.Base;
using UnityEngine;

namespace DISL.Runtime.Components
{
    public sealed class ContainerInstallation : MonoBehaviour
    {
        private void Awake()
        {
            var installer = GetComponentInParent<ContainerInstaller>();
            if (installer == null)
            {
                Debug.LogWarning("ContainerInstaller not found in parent hierarchy");
                enabled = false;
                return;
            }

            installer.InstallTo(GetComponents<IContainerReceiver>());
        }
    }
}
using System.Collections.Generic;
using DISL.Runtime.Base;
using DISL.Runtime.Installers;
using UnityEngine;

namespace DISL.Runtime.Components
{
    public sealed class ContainerInstaller : MonoBehaviour, IContainerInstaller
    {
        private IContainer _container;
        private readonly Queue<IContainerReceiver> _receivers = new();

        public void Install(IContainer container)
        {
            _container = container;
            Install();
        }

        public void InstallTo(IEnumerable<IContainerReceiver> receivers)
        {
            foreach (var receiver in receivers) 
                _receivers.Enqueue(receiver);
            Install();
        }

        private void Install()
        {
            if(_container is null)
                return;

            while (_receivers.TryDequeue(out var receiver)) 
                receiver.Receive(_container);
        }
    }
}
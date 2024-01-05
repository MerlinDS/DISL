using System;
using DISL.Runtime.Base;
using DISL.Runtime.Installers;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class ScenesInstaller : IContainerInstaller, IDisposable
{
    private IContainer _container;

    public ScenesInstaller()
    {
        if (Application.isPlaying)
            SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <inheritdoc />
    public void Install(IContainer container)
    {
        _container = container;
        
        for (var i = 0; i < SceneManager.loadedSceneCount; i++) 
            OnSceneLoaded(SceneManager.GetSceneAt(i), LoadSceneMode.Single);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        _container = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (var gameObject in scene.GetRootGameObjects())
        {
            if (gameObject.TryGetComponent(out IContainerInstaller installer))
                installer.Install(_container);
        }
    }
}
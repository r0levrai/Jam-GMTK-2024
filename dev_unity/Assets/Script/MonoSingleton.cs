using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T m_Instance;
    private static bool applicationIsQuitting = false;
    private static bool hasCalledAwake = false;

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                    "' already destroyed on application quit." +
                    " Won't create again - returning null.");
                return null;
            }
            if (object.ReferenceEquals((object)MonoSingleton<T>.m_Instance, (object)null))
            {
                MonoSingleton<T>.m_Instance = Object.FindObjectOfType(typeof(T)) as T;
                if (object.ReferenceEquals((object)MonoSingleton<T>.m_Instance, (object)null))
                {
                    new GameObject("" + (object)typeof(T)).AddComponent<T>();
                    MonoSingleton<T>.m_Instance = Object.FindObjectOfType(typeof(T)) as T;
                    if (!hasCalledAwake)
                    {
                        MonoSingleton<T>.m_Instance.OnAwake();
                        hasCalledAwake = true;
                    }
                }
                else
                {
                    if (!hasCalledAwake)
                    {
                        MonoSingleton<T>.m_Instance.OnAwake();
                        hasCalledAwake = true;
                    }
                }
            }
            return MonoSingleton<T>.m_Instance;
        }
    }

    public static bool IsInstanceValid
    {
        get
        {
            return !object.ReferenceEquals((object)MonoSingleton<T>.m_Instance, (object)null);
        }
    }

    protected MonoSingleton()
    {

    }

    private void Awake()
    {
        if (!object.ReferenceEquals((object)MonoSingleton<T>.m_Instance, (object)null))
            return;
        MonoSingleton<T>.m_Instance = this as T;
        if (!hasCalledAwake)
        {
            MonoSingleton<T>.m_Instance.OnAwake();
            hasCalledAwake = true;
        }
    }

    protected virtual void OnAwake()
    {
    }

    private void OnApplicationQuit()
    {
        applicationIsQuitting = true;
        MonoSingleton<T>.m_Instance = (T)null;
    }

    protected virtual void DoOnDestroy()
    {
    }

    private void OnDestroy()
    {
        this.DoOnDestroy();
        if (!object.ReferenceEquals((object)MonoSingleton<T>.m_Instance, (object)this))
            return;
        MonoSingleton<T>.m_Instance = (T)null;
    }
}
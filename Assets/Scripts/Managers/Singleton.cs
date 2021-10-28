using UnityEngine;

namespace Managers
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = FindObjectOfType<T>();
                    if (m_Instance == null)
                    {
                        m_Instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
                    }
                }

                return m_Instance;
            }
        }

        private static T m_Instance;

        private void OnApplicationQuit()
        {
            m_Instance = null;
        }
    }
}

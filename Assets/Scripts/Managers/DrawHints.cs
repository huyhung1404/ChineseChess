using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Managers
{
    public class DrawHints : MonoBehaviour
    {
        #region Singleton

        public static DrawHints Instance { get; set; }

        private void Awake()
        {
            Instance = this;
        }

        #endregion

        [SerializeField] private GameObject hintPrefab;
    
        public void DrawHintStraight(Vector3[] position)
        {
            for (float i = position[0].z; i <= position[1].z + 0.5f; i++)
            {
                Instantiate(hintPrefab, new Vector3(position[0].x, 0.1f, i), Quaternion.identity);
            }

            for (float i = position[2].x; i <= position[3].x + 0.5f; i++)
            {
                Instantiate(hintPrefab, new Vector3(i, 0.1f, position[3].z), Quaternion.identity);
            }
        }

        public void DrawHintStraight(Vector3[] position, Vector3[] ignoreVectors)
        {
            for (int i = 0; i < 4; i++)
            {
                if (ignoreVectors[i] == -Vector3.one)
                {
                    ignoreVectors[i] = position[i];
                    continue;
                }

                Instantiate(hintPrefab, new Vector3(position[i].x, 0.1f, position[i].z), Quaternion.identity);
            }

            for (float i = ignoreVectors[0].z; i <= ignoreVectors[1].z + 0.5f; i++)
            {
                Instantiate(hintPrefab, new Vector3(ignoreVectors[0].x, 0.1f, i), Quaternion.identity);
            }

            for (float i = ignoreVectors[2].x; i <= ignoreVectors[3].x + 0.5f; i++)
            {
                Instantiate(hintPrefab, new Vector3(i, 0.1f, ignoreVectors[3].z), Quaternion.identity);
            }
        }

        public void DrawHintsPoints(List<Vector3> position)
        {
            foreach (Vector3 vector3 in position.Distinct().ToArray())
            {
                Instantiate(hintPrefab, new Vector3(vector3.x, 0.1f, vector3.z), Quaternion.identity);
            }
        }

        public void DrawOne(Vector3 position)
        {
            Instantiate(hintPrefab, new Vector3(position.x, 0.1f, position.z), Quaternion.identity);
        }

        public  void TurnOffHints()
        {
            GameObject[] hints = GameObject.FindGameObjectsWithTag("Hints");
            foreach (GameObject o in hints)
            {
                Destroy(o);
            }
        }
    }
}
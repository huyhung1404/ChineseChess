using System.Collections;
using UnityEngine;

namespace Player
{
    public class ClientMoveChess : MonoBehaviour
    {
        private bool takeMakeMove;
        private Rigidbody chessSelectRigidbody;
        private GameObject chessSelect;
        private Vector3 destination;
        private void FixedUpdate()
        {
            ChangeToMakeMove();
        }
        private void ChangeToMakeMove()
        {
            if (!takeMakeMove) return;
            if (Mathf.Abs(destination.x - chessSelect.transform.position.x) < 0.1f &&
                Mathf.Abs(destination.z - chessSelect.transform.position.z) < 0.1f)
            {
                //Hạ cờ
                StartCoroutine(ChessPieceDown());
                takeMakeMove = false;
                return;
            }
            //Di chuyển 
            chessSelect.transform.position +=
                (Vector3.right * (destination.x - chessSelect.transform.position.x) +
                 Vector3.forward * (destination.z - chessSelect.transform.position.z)) * (Time.smoothDeltaTime * 5);
        }

        public IEnumerator MakeMove(GameObject _chessSelect,Vector3 _destination)
        {
            chessSelect = _chessSelect;
            destination = _destination;
            chessSelectRigidbody = _chessSelect.GetComponent<Rigidbody>();
            chessSelectRigidbody.velocity += Vector3.up * 5;
            yield return new WaitForSeconds(0.35f);
            chessSelectRigidbody.constraints = RigidbodyConstraints.FreezePositionY;
            takeMakeMove = true;
        }
        private IEnumerator ChessPieceDown()
        {
            chessSelectRigidbody.constraints = RigidbodyConstraints.None;
            Vector3 position = chessSelectRigidbody.transform.position;
            Vector3 _lastPosition = new Vector3(Mathf.Round(position.x), 0.2f, Mathf.Round(position.z));
            yield return new WaitForSeconds(0.7f);
            chessSelectRigidbody.transform.position = _lastPosition;
        }
    }
}

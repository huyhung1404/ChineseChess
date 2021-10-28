using System.Collections;
using Managers;
using UnityEngine;

namespace Player
{
    public class PickUpChess : MonoBehaviour
    {
        private Rigidbody rigidbodyChessPiece;

        public void PickUpChessPiece(GameObject _pickUp)
        {
            rigidbodyChessPiece = _pickUp.GetComponent<Rigidbody>();
            StartCoroutine(SelectChessPiece());
        }

        public void SetDownChessPiece()
        {
            DrawHints.Instance.TurnOffHints();
            StartCoroutine(ChessPieceDown());
        }

        public void SetDownAndPickUpChessPiece(GameObject _pickUp)
        {
            StartCoroutine(ChessPieceDownAndPickUpNew(_pickUp.GetComponent<Rigidbody>()));
        }

        private IEnumerator SelectChessPiece()
        {
            rigidbodyChessPiece.velocity += Vector3.up * 5;
            yield return new WaitForSeconds(0.35f);
            rigidbodyChessPiece.constraints = RigidbodyConstraints.FreezePositionY;
            yield return new WaitForSeconds(0.2f);
            rigidbodyChessPiece.GetComponent<ChessPiece.ChessPiece>().DrawHint();
        }

        private IEnumerator ChessPieceDown()
        {
            Rigidbody _lastRigidbody = rigidbodyChessPiece;
            rigidbodyChessPiece = null;
            _lastRigidbody.constraints = RigidbodyConstraints.None;
            Vector3 position = _lastRigidbody.transform.position;
            Vector3 _lastPosition = new Vector3(Mathf.Round(position.x), 0.2f, Mathf.Round(position.z));
            yield return new WaitForSeconds(0.7f);
            _lastRigidbody.transform.position = _lastPosition;
        }

        private IEnumerator ChessPieceDownAndPickUpNew(Rigidbody _rigidbody)
        {
            DrawHints.Instance.TurnOffHints();
            rigidbodyChessPiece.constraints = RigidbodyConstraints.None;
            Vector3 position = rigidbodyChessPiece.transform.position;
            Vector3 _lastPosition = new Vector3(Mathf.Round(position.x), 0.2f, Mathf.Round(position.z));
            yield return new WaitForSeconds(0.5f);
            rigidbodyChessPiece.transform.position = _lastPosition;
            rigidbodyChessPiece = _rigidbody;
            rigidbodyChessPiece.velocity += Vector3.up * 5;
            yield return new WaitForSeconds(0.35f);
            rigidbodyChessPiece.constraints = RigidbodyConstraints.FreezePositionY;
            yield return new WaitForSeconds(0.2f);
            rigidbodyChessPiece.GetComponent<ChessPiece.ChessPiece>().DrawHint();
        }
    }
}
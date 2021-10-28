using Managers;
using UnityEngine;

namespace ChessPiece
{
    public abstract class ChessPiece : MonoBehaviour
    {
        public TEAM Team;
        public abstract bool CheckPosition(Vector3 _position,Vector3 _origin);
        public abstract void DrawHint();
        
        
        protected bool CheckNotObjectInCenter(Vector3 _position,Vector3 _origin)
        {
            
            if (Physics.Linecast(new Vector3(_origin.x,0.25f, _origin.z),
                new Vector3(_position.x, 0.25f, _position.z), out var hit))
            {
                return hit.transform.position == _position 
                       && Team != hit.collider.GetComponent<ChessPiece>().Team;
            }
            return true;
        }
        //
        public void MoveChessPiece(Vector3 _position,ref bool _isMove)
        {
            if (Mathf.Abs(_position.x -transform.position.x) < 0.05f 
                && Mathf.Abs(_position.z - transform.position.z) < 0.05f)
            {
                _isMove = false;
                return;
            }
            
            Vector3 _point = _position - transform.position + Vector3.up;
            transform.Translate(_point * (Time.smoothDeltaTime * 5));
        }

        protected bool CheckCrossMove(Vector3 _position)
        {
            Vector3 positionChess = transform.position;
            return Mathf.Abs(_position.x - positionChess.x) > 0.2f &&
                   Mathf.Abs(_position.z - positionChess.z) > 0.2f;
        }
        
        protected bool CheckStraightMove(Vector3 _position)
        {
            Vector3 positionChess = transform.position;
            return Mathf.Abs(_position.x - positionChess.x) < 0.2f ||
                   Mathf.Abs(_position.z - positionChess.z) < 0.2f;
        }

        protected bool CheckGoOneDirection(Vector3 _position,Vector3 _origin)
        {
            float _distanceX = Mathf.Round(Mathf.Abs(_origin.x - transform.position.x));
            float _distanceZ = Mathf.Round(Mathf.Abs(_origin.z - transform.position.z));
            if (_distanceX == 0f && _distanceZ == 0f)
                return false;
            
            if (_distanceX == 0f && _distanceZ > 0f)
                return Mathf.Round(Mathf.Abs(_position.x - transform.position.x)) > 0f;
            
            if (_distanceX > 0f && _distanceZ == 0f)
                return Mathf.Round(Mathf.Abs(_position.z - transform.position.z)) > 0f;
            return false;
        }
        private void OnCollisionEnter(Collision _other)
        {
            //Kiểm tra quân cờ
            if (!_other.gameObject.CompareTag("ChessPiece") && !_other.gameObject.CompareTag("King")) return;
            //Kiểm tra quân ở trên, quân ở dưới
            if (_other.gameObject.transform.position.y > transform.position.y) return;
            //Xoá quân ở dưới
            //Kiểm tra xem có phải vua hay không
            if (_other.gameObject.CompareTag("King"))
            {
                GameManager.Instance.GameOver(_other.collider.GetComponent<ChessPiece>().Team);
            }
            _other.gameObject.SetActive(false);
        }
    }
}
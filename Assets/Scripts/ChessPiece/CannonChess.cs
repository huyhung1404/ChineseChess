using Managers;
using UnityEngine;

namespace ChessPiece
{
    public class CannonChess : ChessPiece
    {
        private Vector3[] positionHints;
        private Vector3 ignore;
        private void Start()
        {
            positionHints = new Vector3[4];
        }
        public override bool CheckPosition(Vector3 _position, Vector3 _origin)
        {
            if (CheckCrossMove(_position)) 
                return false;
            if (CheckGoOneDirection(_position, _origin))
                return false;
            return CheckNotObjectInCenter(_position,_origin);
        }
        private new bool CheckNotObjectInCenter(Vector3 _position,Vector3 _origin)
        {
            var _lastPosition = new Vector3(_origin.x,0.25f, _origin.z);
            var _pos = new Vector3(_position.x, 0.25f, _position.z);
            
            if (!Physics.Linecast(_lastPosition, _pos, out var _hit)) 
                return true;
            if (_hit.transform.position == _position)
                return false;
            //Nếu có vật cản,bắn tiếp 1 tia 
            if (Physics.Linecast(new Vector3(_hit.point.x,0.25f,_hit.point.z)+(_pos - _lastPosition).normalized/2,
                _pos,out var enemy))
            {
                return enemy.transform.position == _position 
                       && Team != enemy.collider.GetComponent<ChessPiece>().Team;
            }
            return false;
        }

        public override void DrawHint()
        {
            Vector3[] _ignoreVectors = new Vector3[4];
            Vector3 _checkTransform = new Vector3(transform.position.x, 0.25f, transform.position.z);
            positionHints[0] = CheckHintsPosition(_checkTransform, Vector3.back,
                new Vector3(_checkTransform.x, _checkTransform.y, Border.BOTTOM));
            _ignoreVectors[0] = ignore;
            positionHints[1] = CheckHintsPosition(_checkTransform, Vector3.forward,
                new Vector3(_checkTransform.x, _checkTransform.y, Border.TOP));
            _ignoreVectors[1] = ignore;
            positionHints[2] = CheckHintsPosition(_checkTransform, Vector3.left,
                new Vector3(Border.LEFT, _checkTransform.y, _checkTransform.z));
            _ignoreVectors[2] = ignore;
            positionHints[3] = CheckHintsPosition(_checkTransform, Vector3.right,
                new Vector3(Border.RIGHT, _checkTransform.y, _checkTransform.z));
            _ignoreVectors[3] = ignore;
            DrawHints.Instance.DrawHintStraight(positionHints,_ignoreVectors);
        }

        private Vector3 CheckHintsPosition(Vector3 _checkTransform, Vector3 _direction, Vector3 _defaultVector)
        {
            if (!Physics.Raycast(_checkTransform, _direction, out RaycastHit _hit, 10))
            {
                ignore = - Vector3.one;
                return _defaultVector;
            }
            if (!Physics.Raycast(_hit.transform.position + _direction*0.5f, _direction, out RaycastHit hit2, 10))
            {
                ignore = - Vector3.one;
                return _hit.transform.position - _direction;
            }
            if (hit2.collider.GetComponent<ChessPiece>().Team == Team)
            {
                ignore = - Vector3.one;
                return _hit.transform.position - _direction;
            }
            ignore = _hit.transform.position - _direction;
            return hit2.transform.position;
        }
    }
}
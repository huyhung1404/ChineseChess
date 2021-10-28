using Managers;
using Player;
using UnityEngine;

namespace ChessPiece
{
    public class ChariotChess : ChessPiece
    {
        private Vector3[] positionHints;
        private void Start()
        {
            positionHints = new Vector3[4];
        }
        public override bool CheckPosition(Vector3 _position,Vector3 _origin)
        {
            if (CheckCrossMove(_position)) 
                return false;
            
            if (CheckGoOneDirection(_position, _origin))
                return false;
         
            return CheckNotObjectInCenter(_position,_origin);
        }

        public override void DrawHint()
        {
            Vector3 checkTransform = new Vector3(transform.position.x, 0.25f, transform.position.z);
            positionHints[0] = CheckHintsPosition(checkTransform, Vector3.back,
                new Vector3(checkTransform.x, checkTransform.y, Border.BOTTOM));
            positionHints[1] = CheckHintsPosition(checkTransform, Vector3.forward,
                new Vector3(checkTransform.x, checkTransform.y, Border.TOP));
            positionHints[2] = CheckHintsPosition(checkTransform, Vector3.left,
                new Vector3(Border.LEFT, checkTransform.y, checkTransform.z));
            positionHints[3] = CheckHintsPosition(checkTransform, Vector3.right,
                new Vector3(Border.RIGHT, checkTransform.y, checkTransform.z));
            DrawHints.Instance.DrawHintStraight(positionHints);
        }

        private Vector3 CheckHintsPosition(Vector3 _checkTransform, Vector3 _direction, Vector3 _defaultVector)
        {
            if (!Physics.Raycast(_checkTransform, _direction, out RaycastHit _hit, 10)) 
                return _defaultVector;
            if (_hit.collider.GetComponent<ChessPiece>().Team == Team)
            {
                return _hit.transform.position - _direction;
            }
            return _hit.transform.position;
        }
    }
}

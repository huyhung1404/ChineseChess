using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace ChessPiece
{
    public class AdvisorChess : ChessPiece
    {
        private Vector3 origin;
        private Vector3 position;
        private List<Vector3> listPosition;
        private void Start()
        {
            listPosition = new List<Vector3>();
        }
        public override bool CheckPosition(Vector3 _position,Vector3 _origin)
        {
            origin = _origin;
            position = _position;
            if (CheckStraightMove(_position)) 
                return false;
            
            if (!CheckAdvisorMove()) 
                return false;
            return CheckNotObjectInCenter(_position,_origin);
        }
        private bool CheckAdvisorMove()
        {
            //Kiểm tra ở biên trái
        if (Mathf.Round(origin.x) == Border.PALACE_LEFT)
        {
            if (!CheckBorder(1)) return false;
        }
        
        //Kiểm tra ở biên phải
        if (Mathf.Round(origin.x) == Border.PALACE_RIGHT)
        {
            if (!CheckBorder(-1)) return false;
        }
        
        //Kiểm tra đi quá 1 ô
        return !(Mathf.Abs(position.z - origin.z) > 1.2f)
               && !(Mathf.Abs(position.x - origin.x) > 1.2f);
        }
        
        private bool CheckBorder(int _signed)
        {
            if (_signed * (origin.x - position.x) > 0.5f) return false;
        
            //Góc dưới
            if (Mathf.Round(origin.z) == Border.PALACE_TOP_BLACK)
            {
                if (origin.z - position.z > 0.5f) return false;
            }
        
            //Góc trên
            if (Mathf.Round(origin.z) != Border.PALACE_TOP_RED) return true;
            return !(position.z - origin.z > 0.5f);
        }

        public override void DrawHint()
        {
            Vector3 _checkTransform = new Vector3(transform.position.x, 0.25f, transform.position.z);
            bool[] _isDefault = new bool[4];
            if (Mathf.Round(_checkTransform.x) == Border.PALACE_LEFT)
            {
                _isDefault[1] = _isDefault[2] = true;
            }

            if (Mathf.Round(_checkTransform.x) == Border.PALACE_RIGHT)
            {
                _isDefault[0] = _isDefault[3] = true;
            }

            if (Mathf.Round(_checkTransform.z) == Border.PALACE_TOP_BLACK 
                || Mathf.Round(_checkTransform.z) == Border.BOTTOM)
            {
                _isDefault[0] = _isDefault[2] = true;
            }

            if (Mathf.Round(_checkTransform.z) == Border.PALACE_TOP_RED 
                || Mathf.Round(_checkTransform.z) == Border.TOP)
            {
                _isDefault[1] = _isDefault[3] = true;
            }
            
            if (!_isDefault[0])
                listPosition.Add(CheckHintsPosition(_checkTransform, Vector3.back + Vector3.right));
            if (!_isDefault[1])
                listPosition.Add(CheckHintsPosition(_checkTransform, Vector3.forward + Vector3.left));
            if (!_isDefault[2])
                listPosition.Add(CheckHintsPosition(_checkTransform, Vector3.left + Vector3.back));
            if (!_isDefault[3])
                listPosition.Add(CheckHintsPosition(_checkTransform, Vector3.right + Vector3.forward));
            listPosition.Add(_checkTransform);
            DrawHints.Instance.DrawHintsPoints(listPosition);
            listPosition.Clear();
        }

        private Vector3 CheckHintsPosition(Vector3 _checkTransform, Vector3 _direction)
        {
            if (!Physics.Raycast(_checkTransform, _direction, out RaycastHit _hit, 1))
                return _checkTransform + _direction;
            if (_hit.collider.GetComponent<ChessPiece>().Team == Team)
            {
                return _hit.transform.position - _direction;
            }

            return _hit.transform.position;
        }
    }
}

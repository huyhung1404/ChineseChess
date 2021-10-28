using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace ChessPiece
{
    public class ElephantChess : ChessPiece
    {
        private List<Vector3> listPosition;
        private Vector3 origin;
        private void Start()
        {
            listPosition = new List<Vector3>();
        }
        public override bool CheckPosition(Vector3 _position, Vector3 _origin)
        {
            origin = _origin;
            if (CheckStraightMove(_position)) 
                return false;
            
            if (!CheckElephantMove(_position,_origin)) 
                return false;
            return CheckNotObjectInCenter(_position,_origin);
        }
        private bool CheckElephantMove(Vector3 _position,Vector3 _origin)
        {
            //Kiểm tra biên
            if (Mathf.Round(_origin.z) == Border.RIVERBANK_RED)
            {
                if (_position.z - _origin.z> 0.5f) return false;
            }
            if (Mathf.Round(_origin.z) == Border.RIVERBANK_BLACK)
            {
                if (_origin.z - _position.z > 0.5f) return false;
            }
            //Kiểm tra đi ít hơn 1 ô
            if (Mathf.Abs(_position.z - _origin.z) < 1.8f &&
                Mathf.Abs(_position.z - _origin.z) > 0.2f ||
                Mathf.Abs(_position.x - _origin.x) < 1.8f &&
                Mathf.Abs(_position.x - _origin.x) > 0.2f) return false;
            //Kiểm tra đi quá 2 ô
            return !(Mathf.Abs(_position.z - _origin.z) > 2.2f)
                   && !(Mathf.Abs(_position.x - _origin.x) > 2.2f);
        }

        public override void DrawHint()
        {
         Vector3 checkTransform = new Vector3(transform.position.x, 0.25f, transform.position.z);
            bool[] isDefault = new bool[4];
        
            if (Mathf.Round(checkTransform.x) == Border.LEFT)
            {
                isDefault[1] = isDefault[2] = true;
            }
            
            if (Mathf.Round(checkTransform.x) == Border.RIGHT)
            {
                isDefault[0] = isDefault[3] = true;
            }
            //Kiểm tra sông
            if (Mathf.Round(checkTransform.z) == Border.RIVERBANK_BLACK 
                || Mathf.Round(checkTransform.z) == Border.BOTTOM)
            {
                isDefault[0] = isDefault[2] = true;
            }

            if (Mathf.Round(checkTransform.z) == Border.RIVERBANK_RED 
                || Mathf.Round(checkTransform.z) == Border.TOP)
            {
                isDefault[1] = isDefault[3] = true;
            }
            //Nếu đúng đặt giá trị raycast nếu sai đặt giá trị mặc định
            if (!isDefault[0])
                listPosition.Add(CheckHintsPosition(checkTransform, Vector3.back + Vector3.right));
            if (!isDefault[1])
                listPosition.Add(CheckHintsPosition(checkTransform, Vector3.forward + Vector3.left));
            if (!isDefault[2])
                listPosition.Add(CheckHintsPosition(checkTransform, Vector3.left + Vector3.back));
            if (!isDefault[3])
                listPosition.Add(CheckHintsPosition(checkTransform, Vector3.right + Vector3.forward));
            listPosition.Add(checkTransform);
            DrawHints.Instance.DrawHintsPoints(listPosition);
            listPosition.Clear();
        }

        private Vector3 CheckHintsPosition(Vector3 checkTransform, Vector3 direction)
        {
            if (!Physics.Raycast(checkTransform, direction, out RaycastHit hit, 2.5f))
                return checkTransform + direction*2;
            if (Mathf.Abs(hit.transform.position.x - origin.x) < 1.5f) return checkTransform;
            return hit.collider.GetComponent<ChessPiece>().Team == Team ? checkTransform : hit.transform.position;
        }
    }
}

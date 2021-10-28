using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace ChessPiece
{
    public class SoldierChess : ChessPiece
    {
        private List<Vector3> listPositions;
        private void Start()
        {
            listPositions = new List<Vector3>();
        }
        public override bool CheckPosition(Vector3 _position, Vector3 _origin)
        {
            if (CheckCrossMove(_position)) 
                return false;
            if (CheckGoOneDirection(_position, _origin))
                return false;
            
            if (!CheckSoldierMove(_position,_origin)) 
                return false;
            return CheckNotObjectInCenter(_position,_origin);
        }

        private bool CheckSoldierMove(Vector3 _position,Vector3 _origin)
        {
            //Kiểm tra team
            if (Team == TEAM.RED_TEAM)
            {
                //Kiểm tra đi lùi
                if (_position.z < _origin.z) return false;
                //Kiểm tra đi quá 1 ô
                if (_position.z - _origin.z > 1.2f) return false;
                //Kiểm tra có thể đi ngang
                if (_origin.z > 4.8f) return !(Mathf.Abs(_position.x - _origin.x) > 1.2f);
                //Nếu không thể đi ngang
                return !(Mathf.Abs(_position.x - _origin.x) > 0.2f);
            }

            //Kiểm tra đi lùi
            if (_position.z > _origin.z) return false;
            //Kiểm tra đi qua 1 ô
            if (_origin.z - _position.z > 1.2f) return false;
            //Kiểm tra có thể đi ngang
            if (_origin.z < 4.2f) return !(Mathf.Abs(_position.x - _origin.x) > 1.2f);
            //Nếu không thể đi ngang
            return !(Mathf.Abs(_position.x - _origin.x) > 0.2f);
        }
        public override void DrawHint()
        {
            Vector3 checkTransform = new Vector3(transform.position.x, 0.25f, transform.position.z);
            //Kiểm tra team
            if (Team == TEAM.RED_TEAM)
            {
                //Kiểm tra cuối
                if (Mathf.Round(checkTransform.z) != 9)
                    listPositions.Add(CheckHintsPosition(checkTransform, Vector3.forward));
                //Kiểm tra đi phải
                if (checkTransform.z > 4.8f && checkTransform.x < 7.8)
                    listPositions.Add(CheckHintsPosition(checkTransform, Vector3.right));
                //Kiểm tra đi trái
                if (checkTransform.z >4.8f && checkTransform.x > 0.2)
                    listPositions.Add(CheckHintsPosition(checkTransform, Vector3.left));
                listPositions.Add(checkTransform);
                DrawHints.Instance.DrawHintsPoints(listPositions);
                listPositions.Clear();
                return;
            }
            if (Mathf.Round(checkTransform.z) != 0)
                listPositions.Add(CheckHintsPosition(checkTransform, Vector3.back));
            if (checkTransform.z < 4.2f && checkTransform.x < 7.8)
                listPositions.Add(CheckHintsPosition(checkTransform, Vector3.right));
            if (checkTransform.z < 4.2f && checkTransform.x > 0.2)
                listPositions.Add(CheckHintsPosition(checkTransform, Vector3.left));
            listPositions.Add(checkTransform);
            DrawHints.Instance.DrawHintsPoints(listPositions);
            listPositions.Clear();
        }

        private Vector3 CheckHintsPosition(Vector3 checkTransform, Vector3 direction)
        {
            //Kiểm tra có vật thể trước mặt hay không nếu ko có trả về vị trí có thể di chuyển
            if (!Physics.Raycast(checkTransform, direction, out RaycastHit hit, 1))
                return checkTransform + direction;
            //Kiểm tra cùng team hay không, nếu khác team có thể ăn
            return hit.collider.GetComponent<ChessPiece>().Team == Team ? checkTransform : hit.transform.position;
        }
    }
}

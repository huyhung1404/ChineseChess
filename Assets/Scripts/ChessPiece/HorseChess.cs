using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace ChessPiece
{
    public class HorseChess : ChessPiece
    {
        private List<Vector3> listPosition;

        private void Start()
        {
            listPosition = new List<Vector3>();
        }
        public override bool CheckPosition(Vector3 _position, Vector3 _origin)
        {
            if (!CheckEnemy(_position))
                return false;
            if (CheckStraightMove(_position)) 
                return false;
            //Kiểm tra di chuyển
            if (!CheckHorseMove(_position,_origin)) 
                return false;
            
            return CheckNotObjectInCenter(_position,_origin);
        }

        private bool CheckEnemy(Vector3 _position)
        {
            if (!Physics.Raycast(_position + Vector3.up, Vector3.down, out RaycastHit hit,2,1<<8)) 
                return true;
            return hit.collider.GetComponent<ChessPiece>().Team != Team;
        }

        private bool CheckHorseMove(Vector3 _position, Vector3 _origin)
        {
            //Kiểm tra đi ít hơn 1 ô
            if (Mathf.Abs(_position.z - _origin.z) < 1.8f &&
                Mathf.Abs(_position.z - _origin.z) > 0.2f &&
                Mathf.Abs(_position.x - _origin.x) < 1.8f &&
                Mathf.Abs(_position.x - _origin.x) > 0.2f) return false;
            //Kiểm tra đi chéo
            return Mathf.Abs(_position.x - _origin.x) < 2.2 &&
                   Mathf.Abs(_position.z - _origin.z) < 1.2f ||
                   Mathf.Abs(_position.x - _origin.x) < 1.2 &&
                   Mathf.Abs(_position.z - _origin.z) < 2.2f;
        }

        private new bool CheckNotObjectInCenter(Vector3 _position,Vector3 _origin)
        {
            //Gốc toạ độ
            var vectorStart = new Vector3(_origin.x,0.25f, _origin.z);
            //Điểm đến
            var vectorPos = new Vector3(_position.x, 0, _position.z);
            //Tìm vector hướng
            var vectorNormalize = vectorStart - vectorPos;
            //Tìm vector đối diện
            var vectorEnd = vectorPos + (vectorNormalize.x > 0 ? Vector3.right : Vector3.left) +
                            (vectorNormalize.z > 0 ? Vector3.forward : Vector3.back) + Vector3.up * 0.25f;
            //Nếu có vật thể chắn trước mặt, trả về false
            return !Physics.Linecast(vectorStart, vectorEnd);
        }
        
        public override void DrawHint()
        {
        Vector3 checkTransform = new Vector3(transform.position.x, 0.25f, transform.position.z);
            bool[] isDefault = new bool[8];
            //Kiểm tra biên trái
            if (checkTransform.x < 1.2f)
            {
                isDefault[2] = isDefault[3] = true;
                if (checkTransform.x < 0.2)
                {
                    isDefault[1] = isDefault[4] = true;
                }
            }

            //Kiểm tra biên phải
            if (checkTransform.x > 6.8f)
            {
                isDefault[6] = isDefault[7] = true;
                if (checkTransform.x > 7.8f)
                {
                    isDefault[0] = isDefault[5] = true;
                }
            }

            //Kiểm tra biên dưới
            if (checkTransform.z < 1.2f)
            {
                isDefault[0] = isDefault[1] = true;
                if (checkTransform.z < 0.2f)
                {
                    isDefault[2] = isDefault[7] = true;
                }
            }

            //Kiểm tra biên trên
            if (checkTransform.z > 7.8f)
            {
                isDefault[4] = isDefault[5] = true;
                if (checkTransform.z > 8.8f)
                {
                    isDefault[3] = isDefault[6] = true;
                }
            }

            if (!Physics.Raycast(checkTransform + Vector3.back * 0.5f, Vector3.back, 0.5f))
            {
                if (!isDefault[0])
                {
                    listPosition.Add(CheckHintsPosition(checkTransform, Vector3.back, Vector3.right));
                }

                if (!isDefault[1])
                {
                    listPosition.Add(CheckHintsPosition(checkTransform, Vector3.back, Vector3.left));
                }
            }

            if (!Physics.Raycast(checkTransform + Vector3.left * 0.5f, Vector3.left, 0.5f))
            {
                if (!isDefault[2])
                {
                    listPosition.Add(CheckHintsPosition(checkTransform, Vector3.left, Vector3.back));
                }

                if (!isDefault[3])
                {
                    listPosition.Add(CheckHintsPosition(checkTransform, Vector3.left, Vector3.forward));
                }
            }

            if (!Physics.Raycast(checkTransform + Vector3.forward * 0.5f, Vector3.forward, 0.5f))
            {
                if (!isDefault[4])
                {
                    listPosition.Add(CheckHintsPosition(checkTransform, Vector3.forward, Vector3.left));
                }

                if (!isDefault[5])
                {
                    listPosition.Add(CheckHintsPosition(checkTransform, Vector3.forward, Vector3.right));
                }
            }

            if (!Physics.Raycast(checkTransform + Vector3.right * 0.5f, Vector3.right, 0.5f))
            {
                if (!isDefault[6])
                {
                    listPosition.Add(CheckHintsPosition(checkTransform, Vector3.right, Vector3.forward));
                }

                if (!isDefault[7])
                {
                    listPosition.Add(CheckHintsPosition(checkTransform, Vector3.right, Vector3.back));
                }
            }

            listPosition.Add(checkTransform);
            DrawHints.Instance.DrawHintsPoints(listPosition);
            listPosition.Clear();
        }

        private Vector3 CheckHintsPosition(Vector3 checkTransform, Vector3 direction1, Vector3 direction2)
        {
            if (!Physics.Raycast(checkTransform + direction1, direction1 + direction2, out RaycastHit hit, 1))
                return checkTransform + 2 * direction1 + direction2;

            return hit.collider.GetComponent<ChessPiece>().Team == Team ? checkTransform : hit.transform.position;
        }
    }
}

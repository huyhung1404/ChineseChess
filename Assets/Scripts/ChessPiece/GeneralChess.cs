using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace ChessPiece
{
    public class GeneralChess : ChessPiece
    {
        private Vector3 position;
        private Vector3 origin;
        private List<Vector3> listPosition;
        private void Start()
        {
            listPosition = new List<Vector3>();
        }
        public override bool CheckPosition(Vector3 _position, Vector3 _origin)
        {
            position = _position;
            origin = _origin;
            if (CheckCrossMove(_position))
                return false;

            if (CheckOppositeGeneral())
                return true;

            if (CheckGoOneDirection(_position, _origin))
                return false;
            if (!CheckGeneralMove())
                return false;

            return CheckNotObjectInCenter(_position, _origin);
        }

        private bool CheckOppositeGeneral()
        {
            var direction = Team == TEAM.RED_TEAM ? Vector3.forward : Vector3.back;
            //Bắn raycast từ tướng
            if (!Physics.Raycast(new Vector3(origin.x, 0.25f, origin.z + direction.z * 0.5f),
                direction, out var hit, 10)) return false;
            //Nếu tìm thấy vật
            // ReSharper disable once Unity.UnknownTag
            if (!hit.collider.CompareTag("King")) return false;
            //Nếu đúng là vua thì có thể ăn
            return position == hit.transform.position;
        }

        private bool CheckGeneralMove()
        {
            //Kiểm tra ở biên trái
            if (Mathf.Round(origin.x) == Border.PALACE_LEFT)
            {
                if (origin.x - position.x > 0.5f) return false;
            }

            //Kiểm tra ở biên phải
            if (Mathf.Round(origin.x) == Border.PALACE_RIGHT)
            {
                if (position.x - origin.x > 0.5f) return false;
            }

            //Kiểm tra team
            if (Team == TEAM.RED_TEAM)
            {
                // Kiểm tra biên trên
                if (Mathf.Round(origin.z) == Border.PALACE_TOP_RED)
                {
                    if (position.z - origin.z > 0.5f) return false;
                }

                //Kiểm tra đi dọc 1 ô
                if (Mathf.Abs(position.z - origin.z) > 1.2f) return false;
                //Kiểm tra đi ngang 1 ô
                return !(Mathf.Abs(position.x - origin.x) > 1.2f);
            }

            // Kiểm tra biên dưới
            if (Mathf.Round(origin.z) == Border.PALACE_TOP_BLACK)
            {
                if (origin.z - position.z > 0.5f) return false;
            }

            //Kiểm tra đi dọc 1 ô
            if (Mathf.Abs(position.z - origin.z) > 1.2f) return false;
            //kiểm tra đi ngang 1 ô
            return !(Mathf.Abs(position.x - origin.x) > 1.2f);
        }
        
        public override void DrawHint()
        {
         Vector3 checkTransform = new Vector3(transform.position.x, 0.25f, transform.position.z);
            //Lưu trữ các biến kiểm tra giá trị biên
            bool[] isDefault = new bool[4];
            //Biên trái
            if (Mathf.Round(checkTransform.x) == Border.PALACE_LEFT)
            {
                isDefault[2] = true;
            }
            //Biên phải
            if (Mathf.Round(checkTransform.x) == Border.PALACE_RIGHT)
            {
                isDefault[3] = true;
            }
            //Biên dưới
            if (Mathf.Round(checkTransform.z) == Border.PALACE_TOP_BLACK 
                || Mathf.Round(checkTransform.z) == Border.BOTTOM)
            {
                isDefault[0] = true;
            }
            //Biên trên
            if (Mathf.Round(checkTransform.z) == Border.PALACE_TOP_RED 
                || Mathf.Round(checkTransform.z) == Border.TOP)
            {
                isDefault[1] = true;
            }
            //Nếu ở biên lấy giá trị mặc định, ngược lại lấy giá trị đã check raycast
            if (!isDefault[0])
                listPosition.Add(CheckHintsPosition(checkTransform, Vector3.back));
            if (!isDefault[1])
                listPosition.Add(CheckHintsPosition(checkTransform, Vector3.forward));
            if (!isDefault[2])
                listPosition.Add(CheckHintsPosition(checkTransform, Vector3.left));
            if (!isDefault[3])
                listPosition.Add(CheckHintsPosition(checkTransform, Vector3.right));
            listPosition.Add(checkTransform);
            CheckOpposite();
            DrawHints.Instance.DrawHintsPoints(listPosition);
            listPosition.Clear();
        }

        private Vector3 CheckHintsPosition(Vector3 checkTransform, Vector3 direction)
        {
            //Kiểm tra có vật thể trước mặt hay không nếu ko có trả về vị trí có thể di chuyển
            if (!Physics.Raycast(checkTransform, direction, out RaycastHit hit, 1))
                return checkTransform + direction;
            //Kiểm tra cùng team hay không
            if (hit.collider.GetComponent<ChessPiece>().Team == Team)
            {
                //Nếu cùng team vị trí từ đi 1
                return hit.transform.position - direction;
            }
            //Nếu khác team có thể ăn
            return hit.transform.position;
        }
        private void CheckOpposite()
        {
            var direction = Team == TEAM.RED_TEAM ? Vector3.forward : Vector3.back;
            //Bắn raycast từ tướng
            if (!Physics.Raycast(
                new Vector3(origin.x, 0.25f,origin.z + direction.z *0.5f),
                direction, out var hit, 10)) return;
            //Nếu tìm thấy vật
            if (!hit.collider.CompareTag("King")) return;
            //Nếu đúng là vua thì có thể ăn
            DrawHints.Instance.DrawOne(hit.transform.position);
        }
    }
}
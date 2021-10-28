using System;
using UnityEngine;

namespace Player
{
    public class ReturnChess : MonoBehaviour
    {
        private bool startReturnOldPosition;
        private GameObject chessPieceSelect;
        private Vector3 positionIsSelect;
        private GameObject nextChessPiece;
        private PickUpChess pickUpChess;

        private void Start()
        {
            pickUpChess = GetComponent<PickUpChess>();
        }

        public void SetChessReturn(GameObject _chessPieceSelect,Vector3 _positionIsSelect,GameObject _nextChessPiece)
        {
            chessPieceSelect = _chessPieceSelect;
            positionIsSelect = _positionIsSelect;
            nextChessPiece = _nextChessPiece;
            startReturnOldPosition = true;
            // GameManager.TurnOffHints();
        }
        private void FixedUpdate()
        {
            //Kiểm tra trở lại vị trí ban đầu hay không, Nếu sai return;
            if (!startReturnOldPosition) 
                return;
            //Nếu đúng, kiếm tra đã trở lại vị trí ban đầu hay chưa
            if (Mathf.Abs(positionIsSelect.x - chessPieceSelect.transform.position.x) < 0.1f &&
                Mathf.Abs(positionIsSelect.z - chessPieceSelect.transform.position.z) < 0.1f)
            {
                //Hạ cờ
                startReturnOldPosition = false;
                pickUpChess.SetDownAndPickUpChessPiece(nextChessPiece);
                return;
            }
        
            //Di chuyển về vị trí ban đầu
            Vector3 _point = positionIsSelect - chessPieceSelect.transform.position + Vector3.up;
            chessPieceSelect.transform.Translate(_point * (Time.smoothDeltaTime * 5));
        }
    }
}

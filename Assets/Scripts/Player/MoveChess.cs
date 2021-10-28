using System;
using UnityEngine;

namespace Player
{
    public class MoveChess : MonoBehaviour
    {
        private Vector3 positionClick;
        private ChessPiece.ChessPiece chessPiece;
        private bool isMove;
        public void SetInfoClick(Vector3 _position,GameObject _selectGameObject,Vector3 _origin)
        {
            if (isMove)
                return;
            positionClick = _position;
            chessPiece = _selectGameObject.GetComponent<ChessPiece.ChessPiece>();
            isMove = chessPiece.CheckPosition(_position,_origin);
        }

        private void FixedUpdate()
        {
            if (!isMove) 
                return;
            chessPiece.MoveChessPiece(positionClick, ref isMove);
        }
    }
}

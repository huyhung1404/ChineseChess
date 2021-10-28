using System;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PickUpChess))]
    [RequireComponent(typeof(ReturnChess))]
    [RequireComponent(typeof(MoveChess))]
    [RequireComponent(typeof(NetworkPlayer))]
    public class SelectChess : MonoBehaviour
    {
        public static int CurrentTeam = -1;
        public static bool IsLocalGame = true;
        public static int LastTeamSelected;

        private Camera mainCamera;
        private GameObject chessIsSelect;
        private Vector3 positionIsSelect;

        private Vector3 positionClick;

        private PickUpChess pickUpChess;
        private ReturnChess returnChess;
        private MoveChess moveChess;
        private NetworkPlayer networkPlayer;
        private Action<Vector3,GameObject,Vector3> sendPosition;

        private void Start()
        {
            mainCamera = Camera.main;
            chessIsSelect = null;
            LastTeamSelected = 1;
            pickUpChess = GetComponent<PickUpChess>();
            returnChess = GetComponent<ReturnChess>();
            moveChess = GetComponent<MoveChess>();
            networkPlayer = GetComponent<NetworkPlayer>();
        }

        private void Update()
        {
            //Click chuột
            if (!Input.GetMouseButtonDown(0)) return;
            
            Ray _ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            //Kiểm tra bắn raycast
            if (!Physics.Raycast(_ray.origin, _ray.direction, out var _hit)) return;
            //Lưu vị trí click chuột
            positionClick = _hit.collider.transform.position;
            sendPosition?.Invoke(positionClick,chessIsSelect,positionIsSelect);
            //Chọn cờ
            if (_hit.collider.CompareTag("ChessPiece") || _hit.collider.CompareTag("King"))
            {
                CheckChessPiece(_hit.collider.GetComponent<ChessPiece.ChessPiece>(),
                    _hit.collider.gameObject);
            }

        }

        private void CheckChessPiece(ChessPiece.ChessPiece _chessPiece, GameObject _chessGameObject)
        {
            if (chessIsSelect == null)
            {
                int _team = (int)_chessPiece.Team;
                //Kiểm tra có phải lượt team mình không
                if (!IsLocalGame)
                {
                    if (_team != CurrentTeam) return;
                }
                if (_team - LastTeamSelected == 0) return;
                //
                chessIsSelect = _chessGameObject;
                positionIsSelect = chessIsSelect.transform.position;
                sendPosition += moveChess.SetInfoClick;
                pickUpChess.PickUpChessPiece(chessIsSelect);
                return;
            }

            //Hạ cờ đã chọn
            if (chessIsSelect == _chessGameObject)
            {
                // Kiểm tra vị trị hạ cờ
                if (Mathf.Abs(positionIsSelect.x - positionClick.x) > 0.2f ||
                    Mathf.Abs(positionIsSelect.z - positionClick.z) > 0.2f)
                {
                    //Khác vị trí ban đầu, team tiếp theo được đánh
                    if (!IsLocalGame)
                    {
                        networkPlayer.SendMakeMove(positionIsSelect, positionClick);
                    }
                    LastTeamSelected = (int) chessIsSelect.GetComponent<ChessPiece.ChessPiece>().Team;
                }

                //Hạ cờ
                pickUpChess.SetDownChessPiece();
                sendPosition -= moveChess.SetInfoClick;
                chessIsSelect = null;
                return;
            }

            //Kiểm tra vị trí đã chọn có cờ quân địch hay không
            if (chessIsSelect.GetComponent<ChessPiece.ChessPiece>().Team != _chessPiece.Team) 
                return;
            
            // Hạ cờ về vị trí ban đầu
            returnChess.SetChessReturn(chessIsSelect,positionIsSelect,_chessGameObject);
            //Chọn cờ mới
            chessIsSelect = _chessGameObject;
            positionIsSelect = chessIsSelect.transform.position;
        }
    }
}
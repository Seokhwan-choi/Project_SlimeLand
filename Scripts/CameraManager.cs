using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;
using System.Linq;

namespace MLand
{
    public enum FollowType
    {
        None,
        SlimeKing,        // 슬라임왕
        Slime,            // 슬라임
        Building,         // 건물
    }

    struct FollowInfo
    {
        public Transform FollowTm;
        public FollowType FollowType;
        public bool IsSlimeKing => FollowType == FollowType.SlimeKing;
        public void Reset()
        {
            FollowTm = null;
            FollowType = FollowType.None;
        }
    }

    public enum MoveType
    {
        Original,
        Motion
    }

    class CameraManager : MonoBehaviour
    {
        int mUILayer;
        int mBuildingLayer;
        int mCharacterLayer;

        float mWidth;
        float mHeight;
        bool mInZoom;
        MoveType mCurrentMoveType;
        Vector3 mMovePos;
        FollowInfo mFollowInfo;

        public Transform FollowTm => mFollowInfo.FollowTm;
        Camera mMainCamera => Camera.main;
        Transform mMainCameraTm => mMainCamera.transform;
        public void Init()
        {
            mHeight = mMainCamera.orthographicSize;
            mWidth = mHeight * Screen.width / Screen.height;
            mUILayer = LayerMask.NameToLayer("UI");
            mBuildingLayer = LayerMask.NameToLayer("Building");
            mCharacterLayer = LayerMask.NameToLayer("Character");

            mCurrentMoveType = MoveType.Original;
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            if (mFollowInfo.FollowTm != null && mFollowInfo.FollowType != FollowType.None)
            {
                inScrolling = false;

                mMovePos = GetFollowPos();

                CheckOtherTouchInFollowing();
            }
            else
            {
                if (MLand.Lobby.InTutorial())
                    return;

                if (Input.touchSupported)
                {
                    UpdateTouchAction();
                }
                else
                {
                    UpdateMouseMove();
                    UpdateWheelZoom();
                }
            }

            MoveToTarget(mMovePos, dt);

            ClampCameraPos();
        }

        void CheckOtherTouchInFollowing()
        {
            Vector2 touchPos;

            if (Input.touchSupported)
            {
                if (Input.touchCount == 1)
                {
                    Touch firstTouch = Input.GetTouch(0);
                    if (firstTouch.phase == TouchPhase.Began)
                    {
                        touchPos = firstTouch.position;

                        var results = GetTouchPosRaycastHitResults(touchPos);

                        // 건물이나 캐릭터를 터치한게 아니라면 카메라 포커스를 바로 풀어주자
                        if (results.Count(x =>
                        x.gameObject.layer == mCharacterLayer ||
                        x.gameObject.layer == mBuildingLayer ||
                        x.gameObject.layer == mUILayer) <= 0)
                        {
                            ResetFollowInfo();
                        }
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    touchPos = Input.mousePosition;

                    List<RaycastResult> results = GetTouchPosRaycastHitResults(touchPos);

                    // 건물이나 캐릭터를 터치한게 아니라면 카메라 포커스를 바로 풀어주자
                    if (results.Count(x =>
                    x.gameObject.layer == mCharacterLayer ||
                    x.gameObject.layer == mBuildingLayer ||
                    x.gameObject.layer == mUILayer) <= 0)
                    {
                        ResetFollowInfo();
                    }
                }
            }
        }

        public void SetCameraSize(float size)
        {
            mMainCamera.orthographicSize = size;
        }

        public void ResetFollowInfo()
        {
            mFollowInfo.Reset();

            inScrolling = false;

            if (Input.touchSupported)
            {
                mPreCamPos = mMainCameraTm.position;
                mNowCamPos = mMainCameraTm.position;
            }
            else
            {
                mBeginCamPos = mMainCameraTm.position;
                mMouseClickPoint = Input.mousePosition;
            }
        }

        Coroutine mZoomRoutine;
        public void PlayResetZoomRoutine(float zoomSpeed = 5f)
        {
            if (mZoomRoutine != null)
                StopCoroutine(mZoomRoutine);

            mZoomRoutine = StartCoroutine(ZoomRoutine(MLand.GameData.CameraCommonData.originalCameraSize, zoomSpeed));
        }

        public void PlayHideCloudZoomRoutine(float zoomSpeed = 5f)
        {
            if (mZoomRoutine != null)
                StopCoroutine(mZoomRoutine);

            mZoomRoutine = StartCoroutine(ZoomRoutine(MLand.GameData.CameraCommonData.cameraMaxSize, zoomSpeed));
        }

        public void PlaySlimeZoomRoutine(float zoomSpeed = 5f)
        {
            if (mZoomRoutine != null)
                StopCoroutine(mZoomRoutine);

            mZoomRoutine = StartCoroutine(ZoomRoutine(MLand.GameData.CameraCommonData.cameraMinSize, zoomSpeed));
        }

        public void SetFollowInfo(FollowInfo info)
        {
            if (mFollowInfo.FollowTm != info.FollowTm)
                SoundPlayer.PlayFocus();

            mFollowInfo = info;

            if (mFollowInfo.FollowType == FollowType.Slime)
            {
                PlaySlimeZoomRoutine();
            }
            else
            {
                PlayResetZoomRoutine();
            }
        }

        public void ChangeMoveType(MoveType type)
        {
            mCurrentMoveType = type;
        }

        Vector3 GetFollowPos()
        {
            float height = mFollowInfo.IsSlimeKing ? 
                MLand.GameData.CameraCommonData.slimeKingHeight : 
                MLand.GameData.CameraCommonData.otherHeight;

            float calcSize = (mMainCamera.orthographicSize * height) / (MLand.Lobby.CanvasSize.y * 0.5f);

            float followOffsetY = -(calcSize * 0.4f);

            Vector3 followPos = mFollowInfo.FollowTm.position;

            return new Vector3(followPos.x, followPos.y + followOffsetY);
        }

        void ClampCameraPos()
        {
            float limitX = MLand.GameData.CameraCommonData.mapSizeX - mWidth;
            float clampX = Mathf.Clamp(mMainCameraTm.position.x, -limitX, limitX);

            float limitY = MLand.GameData.CameraCommonData.mapSizeY - mHeight;
            float clampY = Mathf.Clamp(mMainCameraTm.position.y, -limitY, limitY);

            float orgZ = MLand.GameData.CameraCommonData.originalCameraPosZ;

            mMainCameraTm.position = new Vector3(clampX, clampY, orgZ);
        }

        public bool IsPointerOverUIObject(Vector2 touchPos)
        {
            List<RaycastResult> results = GetTouchPosRaycastHitResults(touchPos);

            return results.Count(x => x.gameObject.layer == mUILayer)  > 0;
        }

        List<RaycastResult> GetTouchPosRaycastHitResults(Vector2 touchPos)
        {
            PointerEventData eventDataCurrentPosition
                = new PointerEventData(EventSystem.current);

            eventDataCurrentPosition.position = touchPos;

            List<RaycastResult> results = new List<RaycastResult>();

            EventSystem.current
            .RaycastAll(eventDataCurrentPosition, results);

            return results;
        }

#region 마우스 클릭 관련
        Vector3 mBeginCamPos;
        Vector3 mMouseClickPoint;

        bool inMouseScroll;
        void UpdateMouseMove()
        {
            Vector3 mousePos = Input.mousePosition;

            // UI 터치 중이면 무시하자
            if (IsPointerOverUIObject(mousePos))
            {
                inMouseScroll = false;

                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                inMouseScroll = true;

                mBeginCamPos = mMainCameraTm.position;

                mMouseClickPoint = mousePos;
            }
            else if (Input.GetMouseButton(0))
            {
                if (inMouseScroll)
                {
                    Vector3 mouseDelta = mMainCamera.ScreenToViewportPoint(mousePos - mMouseClickPoint);

                    mMovePos = mBeginCamPos - (mouseDelta * CalcCameraMovePower());
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                inMouseScroll = false;
            }
        }

        void UpdateWheelZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel") * MLand.GameData.CameraCommonData.zoomSpeed;
            if (scroll != 0)
            {
                if (mZoomRoutine != null)
                    StopCoroutine(mZoomRoutine);
            }

            float clampSize = Mathf.Clamp(mMainCamera.orthographicSize - scroll,
                MLand.GameData.CameraCommonData.cameraMinSize,
                MLand.GameData.CameraCommonData.cameraMaxSize);

            SetCameraSize(clampSize);
        }
#endregion

#region 핸드폰 터치 관련
        Vector3 mPreCamPos;
        Vector2 mNowCamPos;
        Coroutine mStopPinchZoomRoutine;

        bool inPinchZoom;
        bool inScrolling;
        void UpdateTouchAction()
        {
            if (Input.touchCount == 1)
            {
                Touch firstTouch = Input.GetTouch(0);

                // UI 터치 중이면 무시하자
                if (IsPointerOverUIObject(firstTouch.position))
                {
                    inScrolling = false;

                    return;
                }

                if (inPinchZoom)
                {
                    if (mStopPinchZoomRoutine != null)
                    {
                        StopCoroutine(mStopPinchZoomRoutine);

                        mStopPinchZoomRoutine = null;
                    }

                    mStopPinchZoomRoutine = StartCoroutine(StopPinchZoom());

                    return;
                }

                if (firstTouch.phase == TouchPhase.Began)
                {
                    inScrolling = true;

                    mPreCamPos = mMainCameraTm.position;

                    mNowCamPos = firstTouch.position;
                }
                else if (firstTouch.phase == TouchPhase.Moved)
                {
                    if (inScrolling)
                    {
                        Vector3 mouseDelta = mMainCamera.ScreenToViewportPoint(firstTouch.position - mNowCamPos);

                        mMovePos = mPreCamPos - (mouseDelta * CalcCameraMovePower());
                    }
                }
                else if (firstTouch.phase == TouchPhase.Ended)
                {
                    inScrolling = false;
                }
            }

            if (Input.touchCount == 2) // 핀치 줌 인 & 아웃 구현, 두 손가락으로 화면을 터치 했을 때 확인
            {
                inPinchZoom = true;
                inScrolling = false;

                Touch touchFirst = Input.GetTouch(0);    // 첫번째 손가락 좌표
                Touch touchSecond = Input.GetTouch(1);   // 두번째 손가락 좌표

                // UI 터치 중이면 무시하자
                if (IsPointerOverUIObject(touchFirst.position) ||
                    IsPointerOverUIObject(touchSecond.position))
                    return;

                // deltaposition은 deltatime과 동일하게 delta만큼의 시간 동안 움직인 거리를 말한다.
                // 즉, 현재 position에서 이전 delta값을 빼주면 움직이기 전의 손가락 위치가 된다.
                Vector2 touchFirstPrevPos = touchFirst.position - touchFirst.deltaPosition;
                Vector2 touchSecondPrevPos = touchSecond.position - touchSecond.deltaPosition;

                // 현재와 이전값의 움직임의 크기를 구한다.
                float prevTouchDeltaMag = (touchFirstPrevPos - touchSecondPrevPos).magnitude;
                float touchDeltaMag = (touchFirst.position - touchSecond.position).magnitude;

                // 두 값의 차이는 확대/축소할 때 얼만큼 많이 확대/축소가 진행되어야 하는지를 결정한다.
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                if (deltaMagnitudeDiff != 0)
                {
                    if (mZoomRoutine != null)
                        StopCoroutine(mZoomRoutine);
                }

                float calcSize = deltaMagnitudeDiff * MLand.GameData.CameraCommonData.zoomSpeed * 0.01f;
                float clampSize = Mathf.Clamp(mMainCamera.orthographicSize + calcSize,
                    MLand.GameData.CameraCommonData.cameraMinSize,
                    MLand.GameData.CameraCommonData.cameraMaxSize);

                SetCameraSize(clampSize);
            }
        }

        IEnumerator StopPinchZoom()
        {
            yield return new WaitForEndOfFrame();

            inPinchZoom = false;
        }

#endregion

        void MoveToTarget(Vector3 target, float dt)
        {
#if UNITY_EDITOR
            if (inTestMode)
                return;
#endif
            Vector3 endValue = new Vector3(target.x, target.y, MLand.GameData.CameraCommonData.originalCameraPosZ);

            mMainCameraTm.position = Vector3.Lerp(mMainCameraTm.position, endValue, dt * GetMovePower());

            // 거리를 계산해서 도착이라고 하자
            //if ( Vector3.Magnitude(mMainCameraTm.position - endValue) <= 0.05f )
            //{

            //}
        }

        float GetMovePower()
        {
            if (mCurrentMoveType == MoveType.Original)
            {
                return MLand.GameData.CameraCommonData.moveSpeed * 2f;
            }
            else
            {
                return MLand.GameData.CameraCommonData.moveSpeed * 5f;
            }
        }

        float CalcCameraMovePower()
        {
            return MLand.GameData.CameraCommonData.moveSpeed * mMainCamera.orthographicSize;
        }

        IEnumerator ZoomRoutine(float size, float zoomSpeed)
        {
            mInZoom = true;

            while(true)
            {
                if (mMainCamera.orthographic)
                {
                    SetCameraSize(Mathf.Lerp(mMainCamera.orthographicSize, size, Time.deltaTime * zoomSpeed));

                    if (Mathf.Abs(mMainCamera.orthographicSize - size) <= 0.01f)
                    {
                        SetCameraSize(size);

                        mInZoom = false;
                    }
                }

                if (mInZoom == false)
                    break;

                yield return null;
            }
        }

        bool inTestMode;
        public void TestMoveByKeyBoard()
        {
            SetTestMode(true);

            float dt = Time.deltaTime;
            float keyH = Input.GetAxis("Horizontal");
            float keyV = Input.GetAxis("Vertical");
            keyH = keyH * dt * GetMovePower() * 1f;
            keyV = keyV * dt * GetMovePower() * 1f;

            mMainCameraTm.Translate(Vector3.right * keyH);
            mMainCameraTm.Translate(Vector3.up * keyV);
        }

        public void SetTestMode(bool testMode)
        {
            inTestMode = testMode;
        }

        public void PlayTestZoomOutRoutine(float zoomSpeed = 5f)
        {
            StartCoroutine(ZoomRoutine(MLand.GameData.CameraCommonData.cameraMaxSize, zoomSpeed));
        }
    }
}


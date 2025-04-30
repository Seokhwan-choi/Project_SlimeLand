using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

namespace MLand
{
    class MarsoManager
    {
        Transform mContainer;

        List<Marso> mMarsoList;
        List<Marso> mRemoveList;
        public int ActiveMarsoCount => mMarsoList.Count(x => x.IsDisappearing == false);
        public void Init()
        {
            mContainer = GameObject.Find("SlimeField")?.transform;
            mMarsoList = new List<Marso>();
            mRemoveList = new List<Marso>();
        }

        public void OnUpdate(float dt)
        {
            foreach (Marso marso in mMarsoList)
            {
                marso.OnUpdate(dt);
            }

            foreach(Marso remove in mRemoveList)
            {
                mMarsoList.Remove(remove);
            }

            mRemoveList.Clear();
        }

        public void CreateMarso(Vector3 startPos, double amount)
        {
            GameObject go = MLand.ObjectPool.AcquireObject("OBJ/Marso", mContainer);

            Marso marso = go.GetOrAddComponent<Marso>();
            marso.Init(this, amount);

            go.transform.position = startPos;

            DoJumpMarso(go.transform);

            mMarsoList.Add(marso);
        }

        void DoJumpMarso(Transform marso)
        {
            bool isLeft = Util.Dice();
            float offsetX = isLeft ? -2.5f : 2.5f;
            float offsetY = Random.Range(-3f, 1.5f);

            Vector3 offset = new Vector2(offsetX, offsetY);
            Vector3 jumpStartPos = GetJumpPos(marso.position, offset);

            // 자연스럽게 통통 튀는 느낌을 위해서 아래와 같이 하드 코딩
            marso.DOJump(jumpStartPos, 1f, 1, 0.3f).OnComplete(() =>
            {
                jumpStartPos = GetJumpPos(marso.position, new Vector2(isLeft ? -0.3f : 0.3f, 0));

                marso.DOJump(jumpStartPos, 0.5f, 1, 0.25f).OnComplete(() =>
                {
                    jumpStartPos = GetJumpPos(marso.position, new Vector2(isLeft ? -0.1f : 0.1f, 0));

                    marso.DOJump(jumpStartPos, 0.25f, 1, 0.125f).OnComplete(() =>
                    {
                        //jumpStartPos = GetJumpPos(marso.position, new Vector2(isLeft ? -0.1f : 0.1f, 0));

                        //marso.DOJump(jumpStartPos, 0.125f, 1, 0.125f);
                    });
                });
            });
        }

        Vector2 GetJumpPos(Vector3 startPos, Vector3 offset)
        {
            return new Vector2(startPos.x + offset.x, startPos.y + offset.y);
        }

        public void RemoveMarso(Marso removeMarso)
        {
            mRemoveList.Add(removeMarso);
        }

        // 제일 가까운 마소를 찾는다.
        public Marso FindClosestMarso(Character finder)
        {
            float minDistance = float.MaxValue;
            Marso closestMarso = null;

            foreach(Marso marso in mMarsoList.Where(x => x.IsDisappearing == false))
            {
                float distance = Util.GetDistanceBy2D(finder.transform.position, marso.transform.position);

                if (minDistance > distance)
                {
                    minDistance = distance;
                    closestMarso = marso;
                }
            }

            return closestMarso;
        }
    }
}
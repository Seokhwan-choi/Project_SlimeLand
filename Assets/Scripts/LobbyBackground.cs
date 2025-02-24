using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class LobbyBackground
    {
		GameObject mBgObj;
		int mCurrArea;
		
        public void Init()
        {
			//ChangeArea(Bear.LocalData.CurrentArea);
		}

        public void ChangeArea(int area)
        {
			if (area == mCurrArea)
				return;

			GameObject newBg = CreateAreaBg(area);
			if (newBg == null)
				return;

			if (mBgObj != null)
			{
				// 기존 배경이 있으면 슬라이드 연출
				const float SlideDist = 6.2f;
				const float SlideDuration = 0.3f;

				bool rightToLeft = mCurrArea < area;
				float dist = (rightToLeft ? +1 : -1) * SlideDist;

				Transform prevTm = mBgObj.transform;
				Transform newTm = newBg.transform;

				// To Do : UIBlock
				//mGameManager.StartCoroutine(ChangeArea());

				IEnumerator ChangeArea()
				{
					float elapsed = 0.0f;
					while (true)
					{
						elapsed += Time.deltaTime;

						float t = Mathf.Clamp(elapsed / SlideDuration, 0, 1f);
						float f = Mathf.Lerp(0, 1f, t);

						Action(f);

						if (t == 1.0f)
						{
							OnFinish();

							break;
						}

						yield return null;
					}

					void Action(float f)
					{
						float curve = Mathf.Lerp(Mathf.Cos(1f), Mathf.Cos(0), f);
						float pos = curve * dist;

						newTm.localPosition = new Vector3(pos - dist, 0, 0);
						prevTm.localPosition = new Vector3(pos, 0, 0);
					}

					void OnFinish()
					{
						// To Do : UI UnBlock

						ChangeMesh();
					}

					yield return null;
				}
			}
			else
			{
				ChangeMesh();
			}

			void ChangeMesh()
			{
				Release();

				mCurrArea = area;
				mBgObj = newBg;
			}
		}

		public void Release()
		{
			if (mBgObj != null)
			{
				mBgObj.Destroy();
				mBgObj = null;

				mCurrArea = 0;
			}
		}

		public static GameObject CreateAreaBg(int area)
		{
			string bgName = $"Background{area:D2}";

			return Util.Instantiate(bgName);
		}
	}
}
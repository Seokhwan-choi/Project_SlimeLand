using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MLand
{
    class PatternMover : MonoBehaviour
    {
        RawImage mImage;

        public float MoveSpeed = 0.05f;
        void Start()
        {
            mImage = GetComponent<RawImage>();
        }

        void Update()
        {
            Rect uvRect = mImage.uvRect;

            uvRect.x += Time.deltaTime * MoveSpeed;
            uvRect.y += Time.deltaTime * MoveSpeed;

            mImage.uvRect = uvRect;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MLand
{
    class ShowMessage : TutorialAction
    {
        bool mTextSpeechMotionComplete;
        public override void OnStart()
        {
            MLand.GameManager.EndTouchBlock();

            mIsFinish = false;
            mTextSpeechMotionComplete = false;

            TutorialData data = MLand.GameData.TutorialData.Where(x => x.id == mParent.CurrentTutorialId && x.step == mStep).FirstOrDefault();
            if (data != null)
            {
                string speechMessage = StringTableUtil.GetTutorialMessage(data.message);

                mParent.ShowSpeech(OnModalTouch)
                    .SetTextSpeech(speechMessage, 1f, OnComplete)
                    .SetCharacterImg(data.character);
            }
            else
            {
                Finish();
            }
        }

        void OnModalTouch()
        {
            if (mTextSpeechMotionComplete)
            {
                Finish();
            }
        }

        void Finish()
        {
            mIsFinish = true;

            if (mParent.CurrentTutorialStepList.Count > 0)
            {
                bool nextActionIsSpeech = mParent.CurrentTutorialStepList[0] is ShowMessage;

                return;
            }

            mParent.SetActiveSpeechObj(false);

            MLand.GameManager.StartTouchBlock(100f);
        }

        void OnComplete()
        {
            mTextSpeechMotionComplete = true;
        }
    }
}
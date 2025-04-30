
namespace MLand
{
    class Focus_MiniGame : TutorialAction
    {
        public override void OnStart()
        {
            base.OnStart();

            MLand.GameManager.SetFollowMiniGame();

            mIsFinish = true;
        }
    }
}
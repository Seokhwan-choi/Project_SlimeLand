namespace MLand
{
    class Action_Spawn : CharacterAction
    {
        float mTime;
        public override void OnStart(ActionManager parent)
        {
            base.OnStart(parent);

            mTime = MLand.GameData.SlimeCommonData.spawnTime;

            mParent.Character.Anim.PlayIdle(EmotionType.Excited);
        }

        public override void OnUpdate(float dt)
        {
            mTime -= dt;
            if (mTime <= 0f)
            {
                mIsFinish = true;
            }
        }
    }
}
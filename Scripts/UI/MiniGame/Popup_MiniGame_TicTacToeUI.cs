using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;
using System.Linq;

namespace MLand
{
    enum PlayerSymbol
    {
        None,
        O,
        X,
    }

    enum GameResultType
    {
        Win,
        Draw,
        Lose,

        Count
    }

    class Popup_MiniGame_TicTacToeUI : Popup_MiniGameBase
    {
        GameObject mTurnObj;
        WinLineItem[] mWinLines;
        BoardItem[,] mBoardItems;
        PlayerSymbol mAISymbol;
        PlayerSymbol mPlayerSymbol;
        PlayerSymbol mCurrentTurnSymbol;
        public PlayerSymbol PlayerSymbol => mPlayerSymbol;
        public bool IsPlayerTurn => mCurrentTurnSymbol == mPlayerSymbol;
        public void Init()
        {
            Init(MiniGameType.TicTacToe);

            mBoardItems = new BoardItem[3, 3];
            for (int row = 0; row < 3; ++row)
            {
                for (int col = 0; col < 3; ++col)
                {
                    GameObject boardItemObj = gameObject.FindGameObject($"BoardItem_{row + 1}X{col + 1}");

                    BoardItem boardItem = boardItemObj.GetOrAddComponent<BoardItem>();

                    boardItem.Init(this);

                    mBoardItems[row, col] = boardItem;
                }
            }

            mTurnObj = gameObject.FindGameObject("Turn");

            mWinLines = new WinLineItem[(int)WinLineType.Count - 1];

            // None을 무시하기 위해서..
            var winLinesParent = gameObject.FindGameObject("WinLines");
            for(int i = 1; i < (int)WinLineType.Count; ++i)
            {
                WinLineType type = (WinLineType)i;

                var winLineObj = winLinesParent.FindGameObject($"Image_{type}");

                mWinLines[i - 1] = winLineObj.GetOrAddComponent<WinLineItem>();
                mWinLines[i - 1].Init(type);
            }
        }

        public void OnSelectedBoardItem()
        {
            (PlayerSymbol winner, bool isFull) result = CheckFinish();

            if (result.winner != PlayerSymbol.None || result.isFull)
            {
                bool isDraw = result.isFull && result.winner == PlayerSymbol.None;

                mIsPlay = false;

                StartCoroutine(PlayEndGameMotion(result.winner == mPlayerSymbol, isDraw, base.EndGame));
            }
            else
            {
                ChangeTurn();

                if (mCurrentTurnSymbol != mPlayerSymbol)
                {
                    StartCoroutine(PlayAI());
                }
            }

            (PlayerSymbol winner, bool isFull) CheckFinish()
            {
                PlayerSymbol[,] symbols = ExtractSymbols();

                return (symbols.GetWinner(), symbols.IsFull());
            }

            void ChangeTurn()
            {
                mCurrentTurnSymbol = mCurrentTurnSymbol.ChangeSymbol();

                RefreshTurnUI();
            }
        }

        IEnumerator PlayAI()
        {
            float randWaitTime = UnityEngine.Random.Range(1f, 2f);

            yield return new WaitForSeconds(randWaitTime);

            // 약 50퍼의 확률로 멍청하게 아무대나 두자
            if (Util.Dice())
            {
                TicTacToe_MiniMaxAI miniMaxAI = new TicTacToe_MiniMaxAI(ExtractSymbols(), mCurrentTurnSymbol);

                (int x, int y, PlayerSymbol symbol) result = miniMaxAI.Play();

                mBoardItems[result.x, result.y].OnSelect(result.symbol);
            }
            else
            {
                IEnumerable<BoardItem> array = Change2DArrayToArray(mBoardItems).Where(x => x.Symbol == PlayerSymbol.None);

                IOrderedEnumerable<BoardItem> shuffleArray = DataUtil.GetShuffleDatas(array);

                BoardItem boardItem = shuffleArray.FirstOrDefault();

                boardItem?.OnSelect(mAISymbol);

                IEnumerable<BoardItem> Change2DArrayToArray(BoardItem[,] boardItems)
                {
                    foreach (var boardItem in boardItems)
                        yield return boardItem;
                }
            }
        }

        PlayerSymbol[,] ExtractSymbols()
        {
            PlayerSymbol[,] symbols = new PlayerSymbol[3, 3];

            for(int x = 0; x < 3; ++x)
            {
                for(int y = 0; y < 3; ++y)
                {
                    symbols[x, y] = mBoardItems[x, y].Symbol;
                }
            }

            return symbols;
        }

        void RefreshTurnUI()
        {
            bool isTurnO = mCurrentTurnSymbol == PlayerSymbol.O;
            bool isMyTurn = mCurrentTurnSymbol == mPlayerSymbol;

            var turnO = mTurnObj.FindGameObject("Turn_O");
            turnO.SetActive(isTurnO);

            var turnX = mTurnObj.FindGameObject("Turn_X");
            turnX.SetActive(!isTurnO);

            var textTurn = isTurnO ?
                turnO.FindComponent<TextMeshProUGUI>("Text_TurnO") :
                turnX.FindComponent<TextMeshProUGUI>("Text_TurnX");

            textTurn.text = isMyTurn ? 
                    StringTableUtil.Get("MiniGameTicTacToe_MyTurn") :
                    StringTableUtil.Get("MiniGameTicTacToe_SlimeTurn");

            mTurnObj.transform.DOPunchScale(Vector3.one * 0.25f, 0.5f, elasticity: 0.2f);

            SoundPlayer.PlayMiniGame_TicTacToe_ChangeTurn();
        }

        public override void OnStart()
        {
            base.OnStart();

            mTurnObj.SetActive(true);

            MLand.SavePoint.CheckQuests(QuestType.PlayAnyMiniGame);

            foreach (var boardItem in mBoardItems)
            {
                boardItem?.OnReset();
            }

            foreach (var winLineItem in mWinLines)
            {
                winLineItem?.Refresh();
            }

            mCurrentTurnSymbol = Util.Dice() ? PlayerSymbol.X : PlayerSymbol.O;
            mPlayerSymbol = Util.Dice() ? mCurrentTurnSymbol : mCurrentTurnSymbol.ChangeSymbol();
            mAISymbol = mPlayerSymbol.ChangeSymbol();

            RefreshTurnUI();

            if (mCurrentTurnSymbol != mPlayerSymbol)
            {
                StartCoroutine(PlayAI());
            }

            // 게임 시작!
            mIsPlay = true;
            mIsPause = false;
        }

        IEnumerator PlayEndGameMotion(bool isPlayerWin, bool isDraw, Action onEndMotion)
        {
            yield return new WaitForSeconds(0.5f);

            SoundPlayer.PlayMiniGame_TicTacToe_Finish();

            yield return new WaitForSeconds(1f);
            
            ShowGameResultLine();

            yield return new WaitForSeconds(1.5f);

            onEndMotion?.Invoke();

            if (isDraw)
                SoundPlayer.PlayMiniGame_TicTacToe_Draw();
            else if (isPlayerWin)
                SoundPlayer.PlayMiniGame_TicTacToe_Win();
            else
                SoundPlayer.PlayMiniGame_TicTacToe_Lose();

            mTurnObj.SetActive(false);

            CalcScore(isPlayerWin, isDraw);

            ShowGameResultImg(isPlayerWin, isDraw);
            
            RewardData reward = DataUtil.GetMiniGameRewardData(GetScore());
            if (reward != null)
            {
                var rewardObj = mGameEnd.FindGameObject("RewardInfo");
                rewardObj.SetActive(true);
                rewardObj.transform.DORewind();
                rewardObj.transform.DOPunchScale(Vector3.one * 0.5f, 0.5f, elasticity: 0.25f);

                ItemInfo itemInfo = ItemInfo.CreateRewardInfo(reward);

                Button buttonReward = rewardObj.FindComponent<Button>("Button_Reward");
                buttonReward.SetButtonAction(() => MonsterLandUtil.ShowDescPopup(itemInfo));

                Image imgReward = rewardObj.FindComponent<Image>("Image_Reward");
                imgReward.sprite = itemInfo.GetIconImg();

                TextMeshProUGUI textRewardAmount = rewardObj.FindComponent<TextMeshProUGUI>("Text_Amount");

                StringParam param = new StringParam("amount", itemInfo.GetAmountString());

                string amountText = StringTableUtil.Get("UIString_Amount", param);

                textRewardAmount.text = amountText;
            }

            yield return new WaitForSeconds(0.5f);
        }

        void ShowGameResultLine()
        {
            PlayerSymbol[,] symbols = ExtractSymbols();

            WinLineType winLine = symbols.GetWinLine();

            for(int i = 0; i < mWinLines.Length; ++i)
            {
                mWinLines[i].Refresh();

                if (mWinLines[i].Type == winLine)
                    mWinLines[i].PlayLineDrawMotion();
            }
        }

        void ShowGameResultImg(bool isPlayerWin, bool isDraw)
        {
            GameObject gameResultObj = mGameEnd.FindGameObject("GameResult");
            Image[] resultimgs = gameResultObj.GetComponentsInChildren<Image>();
            foreach (var resultImg in resultimgs)
            {
                resultImg.gameObject.SetActive(false);
            }

            string resultName = isDraw ? "Image_Draw" :
                isPlayerWin ? "Image_Win" : "Image_Lose";

            Image result = gameResultObj.FindComponent<Image>(resultName);

            result.gameObject.SetActive(true);

            result.transform.localScale = Vector3.one * 3f;
            result.transform.DORewind();
            result.transform.DOScale(Vector3.one, 0.25f)
                .SetAutoKill(false);
        }

        int mScore;
        public void CalcScore(bool isPlayerWin, bool isDraw)
        {
            mScore = isDraw ? MLand.GameData.MiniGameTicTacToeData.drawScore :
                ( isPlayerWin ? MLand.GameData.MiniGameTicTacToeData.winScore : MLand.GameData.MiniGameTicTacToeData.loseScore );
        }

        public override int GetScore()
        {
            return mScore;
        }
    }

    class WinLineItem : MonoBehaviour
    {
        Image mImage;
        Tweener mDrawTweener;
        WinLineType mType;
        public WinLineType Type => mType;
        public void Init(WinLineType type)
        {
            mType = type;

            mImage = GetComponent<Image>();

            Refresh();
        }

        public void Refresh()
        {
            gameObject.SetActive(false);
        }

        public void PlayLineDrawMotion()
        {
            gameObject.SetActive(true);

            mDrawTweener?.Rewind();
            mImage.fillAmount = 0f;
            mDrawTweener = DOTween.To((f) => mImage.fillAmount = f, 0f, 1f, 1f)
                .SetAutoKill(false);

            SoundPlayer.PlayMiniGame_TicTacToe_LineDraw();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class TicTacToe_MiniMaxAI
    {
        int mResultIndexX;
        int mResultIndexY;
        PlayerSymbol mResultSymbol;

        PlayerSymbol[,] mSymbols;
        PlayerSymbol mCurrentSymbol;
        public TicTacToe_MiniMaxAI(PlayerSymbol[,] symbols, PlayerSymbol currentSymbol)
        {
            mSymbols = symbols;
            mCurrentSymbol = currentSymbol;
        }

        public TicTacToe_MiniMaxAI DeepCopy()
        {
            return new TicTacToe_MiniMaxAI((PlayerSymbol[,])mSymbols.Clone(), mCurrentSymbol);
        }

        public (int x, int y, PlayerSymbol symbol) Play()
        {
            MiniMax(0);

            return (mResultIndexX, mResultIndexY, mResultSymbol);
        }

        public void Check(int x, int y)
        {
            mSymbols[x, y] = mCurrentSymbol;

            mCurrentSymbol = mCurrentSymbol.ChangeSymbol();
        }

        int MiniMax(int depth)
        {
            PlayerSymbol winner = mSymbols.GetWinner();

            // Player°¡ O
            if (winner == PlayerSymbol.O)
            {
                return depth - 10;
            }
            // Computer°¡ X
            else if (winner == PlayerSymbol.X)
            {
                return 10 - depth;
            }
            else if (mSymbols.IsFull())
            {
                return 0;
            }

            int maxScore = int.MinValue;
            int maxX = -1, maxY = -1;

            int minScore = int.MaxValue;
            int minX = -1, minY = -1;

            for (int x = 0; x < 3; ++x)
            {
                for (int y = 0; y < 3; ++y)
                {
                    if (mSymbols.IsEmpty(x, y))
                    {
                        TicTacToe_MiniMaxAI newMiniMax = this.DeepCopy();

                        newMiniMax.Check(x, y);

                        int score = newMiniMax.MiniMax(depth + 1);

                        if (score > maxScore)
                        {
                            maxScore = score;
                            maxX = x;
                            maxY = y;
                        }

                        if (score < minScore)
                        {
                            minScore = score;
                            minX = x;
                            minY = y;
                        }
                    }
                }
            }

            if (mCurrentSymbol == PlayerSymbol.X)
            {
                mResultIndexX = maxX;
                mResultIndexY = maxY;
                mResultSymbol = mCurrentSymbol;

                Check(maxX, maxY);

                return maxScore;
            }
            else
            {
                mResultIndexX = minX;
                mResultIndexY = minY;
                mResultSymbol = mCurrentSymbol;

                Check(minX, minY);

                return minScore;
            }
        }
    }
}


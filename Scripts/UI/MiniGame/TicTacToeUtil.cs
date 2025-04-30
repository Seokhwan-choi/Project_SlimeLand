using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    enum WinLineType
    {
        None,
        Row_1_LeftToRight,
        Row_2_LeftToRight,
        Row_3_LeftToRight,
        Col_1_TopToBottom,
        Col_2_TopToBottom,
        Col_3_TopToBottom,
        Cross_LeftTopToRightBottom,
        Cross_RightTopToLeftBottom,

        Count,
    }

    static class TicTacToeUtil
    {
        public static PlayerSymbol ChangeSymbol(this PlayerSymbol symbol)
        {
            if (symbol == PlayerSymbol.X)
                return PlayerSymbol.O;
            else
                return PlayerSymbol.X;
        }

        public static PlayerSymbol GetWinner(this PlayerSymbol[,] symbols)
        {
            for (int i = 0; i < 3; ++i)
            {
                // ���� �� Ȯ��
                if (symbols[i, 0] == symbols[i, 1] &&
                    symbols[i, 1] == symbols[i, 2])
                {
                    if (symbols[i,0] != PlayerSymbol.None)
                        return symbols[i, 0];
                }

                // ���� �� Ȯ��
                if (symbols[0, i] == symbols[1, i] &&
                    symbols[1, i] == symbols[2, i])
                {
                    if (symbols[0, i] != PlayerSymbol.None)
                        return symbols[0, i];
                }
            }

            // �¿��� �� �밢�� Ȯ��
            if (symbols[0, 0] == symbols[1, 1] &&
                symbols[1, 1] == symbols[2, 2])
            {
                if (symbols[0, 0] != PlayerSymbol.None)
                    return symbols[0, 0];
            }

            // �쿡�� �� �밢�� Ȯ��
            if (symbols[2, 0] == symbols[1, 1] &&
                symbols[1, 1] == symbols[0, 2])
            {
                if (symbols[2, 0] != PlayerSymbol.None)
                    return symbols[2, 0];
            }

            // �̰�쿡�� ���� �ºΰ� ���� �ʾҴٰ� ���� ��
            return PlayerSymbol.None;
        }

        public static WinLineType GetWinLine(this PlayerSymbol[,] symbols)
        {
            for (int i = 0; i < 3; ++i)
            {
                // ���� �� Ȯ��
                if (symbols[i, 0] == symbols[i, 1] &&
                    symbols[i, 1] == symbols[i, 2] && 
                    symbols[i, 0] != PlayerSymbol.None)
                {
                    if (i == 0)
                        return WinLineType.Row_1_LeftToRight;
                    else if (i == 1)
                        return WinLineType.Row_2_LeftToRight;
                    else
                        return WinLineType.Row_3_LeftToRight;
                }

                // ���� �� Ȯ��
                if (symbols[0, i] == symbols[1, i] &&
                    symbols[1, i] == symbols[2, i] &&
                    symbols[0, i] != PlayerSymbol.None)
                {
                    if (i == 0)
                        return WinLineType.Col_1_TopToBottom;
                    else if (i == 1)
                        return WinLineType.Col_2_TopToBottom;
                    else
                        return WinLineType.Col_3_TopToBottom;
                }
            }

            // �¿��� �� �밢�� Ȯ��
            if (symbols[0, 0] == symbols[1, 1] &&
                symbols[1, 1] == symbols[2, 2] &&
                symbols[0, 0] != PlayerSymbol.None)
            {
                return WinLineType.Cross_LeftTopToRightBottom;
            }

            // �쿡�� �� �밢�� Ȯ��
            if (symbols[2, 0] == symbols[1, 1] &&
                symbols[1, 1] == symbols[0, 2] &&
                symbols[2, 0] != PlayerSymbol.None)
            {
                return WinLineType.Cross_RightTopToLeftBottom;
            }

            // �̰�쿡�� ���� �ºΰ� ���� �ʾҴٰ� ���� ��
            return WinLineType.None;
        }

        public static bool IsEmpty(this PlayerSymbol[,] symbols, int x, int y)
        {
            return symbols[x, y] == PlayerSymbol.None;
        }

        public static bool IsFull(this PlayerSymbol[,] symbols)
        {
            for (int x = 0; x < 3; ++x)
            {
                for (int y = 0; y < 3; ++y)
                {
                    if (symbols[x, y] == PlayerSymbol.None)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
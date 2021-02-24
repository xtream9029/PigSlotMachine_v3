using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayLine : GameControl
{
    private int[,] map = new int[3, 5];//슬롯머신의 결과를 받아올 3x5 배열
    public static bool Bonus_flag = false;
    public ulong pv;

    #region 슬롯머신 맵핑 초기화
    public PayLine()
    {
        //슬롯의 결과를 받아올 3X5배열 0으로 다 밀어넣음
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                map[i, j] = 0;//0~14의 값을 가짐
            }
        }
    }
    #endregion

    #region 슬롯머신 결과 맵핑
    public void Mapping_symbol()
    {
        //슬롯머신 돌린 결과를 2차원 배열에 맵핑하는 함수(점수 계산을 위해)
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (i == 0)
                {
                    //high
                    map[i, j] = rows[j].row_stoppedSlot3;
                }
                else if (i == 1)
                {
                    //mid
                    map[i, j] = rows[j].row_stoppedSlot2;
                }
                else if (i == 2)
                {
                    //low
                    map[i, j] = rows[j].row_stoppedSlot1;
                }
            }
        }
    }
    #endregion

    public void IsBonus()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                int k = map[i, j];
                if (k == (int)Symbols.bonus)
                {
                    //2,3,4번째 릴에서 보너스 심볼이 1번이라도 나왔을 경우에 프리스핀을 돌림
                    //원 게임에서는 보너스심볼이 2 3 4 릴에서 1번씩 모두 나왔을 경우에 프리스핀을 돌림
                    if (j == 1 && j == 2 && j == 3)
                    {
                        Bonus_flag = true;
                    }
                }
            }
        }
    }

    public ulong Calculate_Score(int[] A)
    {
        pv = 0;
        //페이라인만 체크해서 계산하는 부분
        int max_symbol = -1;
        int max_cnt = -1;

        for (int j = 0; j < 18; j++)//심볼이 0~17
        {
            if (max_cnt < A[j])
            {
                max_cnt = A[j];
                max_symbol = j;
            }
        }//for

        max_cnt += A[14];//와일드심볼 추가
        ulong gold = bettingGold / 50;//배팅한 금액을 페이라인 수로 나눠줌

        switch (max_symbol)
        {
            case (int)Symbols.pig_red:
                if (max_cnt == 3)
                {
                    pv += gold * 10;
                }
                else if (max_cnt == 4)
                {
                    pv += gold * 50;
                }
                else if (max_cnt == 5)
                {
                    pv += gold * 250;
                }
                break;

            case (int)Symbols.pig_blue:
                if (max_cnt == 3)
                {
                    pv += gold * 10;
                }
                else if (max_cnt == 4)
                {
                    pv += gold * 35;
                }
                else if (max_cnt == 5)
                {
                    pv += gold * 150;
                }
                break;

            case (int)Symbols.pig_green:
                if (max_cnt == 3)
                {
                    pv += gold * 10;
                }
                else if (max_cnt == 4)
                {
                    pv += gold * 30;
                }
                else if (max_cnt == 5)
                {
                    pv += gold * 100;
                }
                break;

            case (int)Symbols.del:
                if (max_cnt == 3)
                {
                    pv += gold * 8;
                }
                else if (max_cnt == 4)
                {
                    pv += gold * 25;
                }
                else if (max_cnt == 5)
                {
                    pv += gold * 80;
                }
                break;

            case (int)Symbols.box:
                if (max_cnt == 3)
                {
                    pv += gold * 5;
                }
                else if (max_cnt == 4)
                {
                    pv += gold * 20;
                }
                else if (max_cnt == 5)
                {
                    pv += gold * 50;
                }
                break;

            case (int)Symbols.cart:
                if (max_cnt == 3)
                {
                    pv += gold * 5;
                }
                else if (max_cnt == 4)
                {
                    pv += gold * 20;
                }
                else if (max_cnt == 5)
                {
                    pv += gold * 50;
                }
                break;


            case (int)Symbols.A:
                if (max_cnt == 3)
                {
                    pv += gold * 3;
                }
                else if (max_cnt == 4)
                {
                    pv += gold * 5;
                }
                else if (max_cnt == 5)
                {
                    pv += gold * 20;
                }
                break;

            case (int)Symbols.K:
                if (max_cnt == 3)
                {
                    pv += gold * 3;
                }
                else if (max_cnt == 4)
                {
                    pv += gold * 5;
                }
                else if (max_cnt == 5)
                {
                    pv += gold * 20;
                }
                break;

            case (int)Symbols.Q:
                if (max_cnt == 3)
                {
                    pv += gold * 3;
                }
                else if (max_cnt == 4)
                {
                    pv += gold * 5;
                }
                else if (max_cnt == 5)
                {
                    pv += gold * 10;
                }
                break;

            case (int)Symbols.J:
                if (max_cnt == 3)
                {
                    pv += gold * 3;
                }
                else if (max_cnt == 4)
                {
                    pv += gold * 5;
                }
                else if (max_cnt == 5)
                {
                    pv += gold * 10;
                }
                break;
        }//switch
        return pv;
    }//Calculate_Score

    #region 페이라인 애니메이션
    //몇번째 릴인지에 대한 정보와 어떤 심볼인지를 매개변수로 받아옴
    public IEnumerator Three_Win_Animation(int symbol_value, int a, int b, int c)
    {
        switch (symbol_value)
        {
            case (int)Symbols.pig_green:
                GameObject.Find($"Row_{a}/Green_Pig_Pair").transform.Find("green_pig").gameObject.SetActive(false);
                GameObject.Find($"Row_{a}/Green_Pig_Pair").transform.Find("green_pig_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Green_Pig_Pair").transform.Find("green_pig").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Green_Pig_Pair").transform.Find("green_pig_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Green_Pig_Pair").transform.Find("green_pig").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Green_Pig_Pair").transform.Find("green_pig_ani").gameObject.SetActive(true);
                yield return new WaitForSeconds(6);
                GameObject.Find($"Row_{a}/Green_Pig_Pair").transform.Find("green_pig").gameObject.SetActive(true);
                GameObject.Find($"Row_{a}/Green_Pig_Pair").transform.Find("green_pig_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Green_Pig_Pair").transform.Find("green_pig").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Green_Pig_Pair").transform.Find("green_pig_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Green_Pig_Pair").transform.Find("green_pig").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Green_Pig_Pair").transform.Find("green_pig_ani").gameObject.SetActive(false);
                break;

            case (int)Symbols.pig_blue:
                GameObject.Find($"Row_{a}/Blue_Pig_Pair").transform.Find("blue_pig").gameObject.SetActive(false);
                GameObject.Find($"Row_{a}/Blue_Pig_Pair").transform.Find("blue_pig_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Blue_Pig_Pair").transform.Find("blue_pig").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Blue_Pig_Pair").transform.Find("blue_pig_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Blue_Pig_Pair").transform.Find("blue_pig").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Blue_Pig_Pair").transform.Find("blue_pig_ani").gameObject.SetActive(true);
                yield return new WaitForSeconds(6);
                GameObject.Find($"Row_{a}/Blue_Pig_Pair").transform.Find("blue_pig").gameObject.SetActive(true);
                GameObject.Find($"Row_{a}/Blue_Pig_Pair").transform.Find("blue_pig_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Blue_Pig_Pair").transform.Find("blue_pig").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Blue_Pig_Pair").transform.Find("blue_pig_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Blue_Pig_Pair").transform.Find("blue_pig").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Blue_Pig_Pair").transform.Find("blue_pig_ani").gameObject.SetActive(false);
                break;

            case (int)Symbols.pig_red:
                GameObject.Find($"Row_{a}/Red_Pig_Pair").transform.Find("red_pig").gameObject.SetActive(false);
                GameObject.Find($"Row_{a}/Red_Pig_Pair").transform.Find("red_pig_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Red_Pig_Pair").transform.Find("red_pig").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Red_Pig_Pair").transform.Find("red_pig_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Red_Pig_Pair").transform.Find("red_pig").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Red_Pig_Pair").transform.Find("red_pig_ani").gameObject.SetActive(true);
                yield return new WaitForSeconds(6);
                GameObject.Find($"Row_{a}/Red_Pig_Pair").transform.Find("red_pig").gameObject.SetActive(true);
                GameObject.Find($"Row_{a}/Red_Pig_Pair").transform.Find("red_pig_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Red_Pig_Pair").transform.Find("red_pig").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Red_Pig_Pair").transform.Find("red_pig_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Red_Pig_Pair").transform.Find("red_pig").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Red_Pig_Pair").transform.Find("red_pig_ani").gameObject.SetActive(false);
                break;

            case (int)Symbols.A:
                GameObject.Find($"Row_{a}/A_Pair").transform.Find("A").gameObject.SetActive(false);
                GameObject.Find($"Row_{a}/A_Pair").transform.Find("A_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/A_Pair").transform.Find("A").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/A_Pair").transform.Find("A_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/A_Pair").transform.Find("A").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/A_Pair").transform.Find("A_ani").gameObject.SetActive(true);
                yield return new WaitForSeconds(6);
                GameObject.Find($"Row_{a}/A_Pair").transform.Find("A").gameObject.SetActive(true);
                GameObject.Find($"Row_{a}/A_Pair").transform.Find("A_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/A_Pair").transform.Find("A").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/A_Pair").transform.Find("A_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/A_Pair").transform.Find("A").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/A_Pair").transform.Find("A_ani").gameObject.SetActive(false);
                break;

            case (int)Symbols.K:
                GameObject.Find($"Row_{a}/K_Pair").transform.Find("K").gameObject.SetActive(false);
                GameObject.Find($"Row_{a}/K_Pair").transform.Find("K_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/K_Pair").transform.Find("K").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/K_Pair").transform.Find("K_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/K_Pair").transform.Find("K").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/K_Pair").transform.Find("K_ani").gameObject.SetActive(true);
                yield return new WaitForSeconds(6);
                GameObject.Find($"Row_{a}/K_Pair").transform.Find("K").gameObject.SetActive(true);
                GameObject.Find($"Row_{a}/K_Pair").transform.Find("K_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/K_Pair").transform.Find("K").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/K_Pair").transform.Find("K_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/K_Pair").transform.Find("K").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/K_Pair").transform.Find("K_ani").gameObject.SetActive(false);
                break;

            case (int)Symbols.Q:
                GameObject.Find($"Row_{a}/Q_Pair").transform.Find("Q").gameObject.SetActive(false);
                GameObject.Find($"Row_{a}/Q_Pair").transform.Find("Q_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Q_Pair").transform.Find("Q").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Q_Pair").transform.Find("Q_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Q_Pair").transform.Find("Q").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Q_Pair").transform.Find("Q_ani").gameObject.SetActive(true);
                yield return new WaitForSeconds(6);
                GameObject.Find($"Row_{a}/Q_Pair").transform.Find("Q").gameObject.SetActive(true);
                GameObject.Find($"Row_{a}/Q_Pair").transform.Find("Q_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Q_Pair").transform.Find("Q").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Q_Pair").transform.Find("Q_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Q_Pair").transform.Find("Q").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Q_Pair").transform.Find("Q_ani").gameObject.SetActive(false);
                break;

            case (int)Symbols.J:
                GameObject.Find($"Row_{a}/J_Pair").transform.Find("J").gameObject.SetActive(false);
                GameObject.Find($"Row_{a}/J_Pair").transform.Find("J_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/J_Pair").transform.Find("J").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/J_Pair").transform.Find("J_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/J_Pair").transform.Find("J").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/J_Pair").transform.Find("J_ani").gameObject.SetActive(true);
                yield return new WaitForSeconds(6);
                GameObject.Find($"Row_{a}/J_Pair").transform.Find("J").gameObject.SetActive(true);
                GameObject.Find($"Row_{a}/J_Pair").transform.Find("J_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/J_Pair").transform.Find("J").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/J_Pair").transform.Find("J_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/J_Pair").transform.Find("J").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/J_Pair").transform.Find("J_ani").gameObject.SetActive(false);
                break;

            case (int)Symbols.del:
                GameObject.Find($"Row_{a}/Ccocal_Pair").transform.Find("Ccocal").gameObject.SetActive(false);
                GameObject.Find($"Row_{a}/Ccocal_Pair").transform.Find("Ccocal_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Ccocal_Pair").transform.Find("Ccocal").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Ccocal_Pair").transform.Find("Ccocal_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Ccocal_Pair").transform.Find("Ccocal").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Ccocal_Pair").transform.Find("Ccocal_ani").gameObject.SetActive(true);
                yield return new WaitForSeconds(6);
                GameObject.Find($"Row_{a}/Ccocal_Pair").transform.Find("Ccocal").gameObject.SetActive(true);
                GameObject.Find($"Row_{a}/Ccocal_Pair").transform.Find("Ccocal_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Ccocal_Pair").transform.Find("Ccocal").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Ccocal_Pair").transform.Find("Ccocal_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Ccocal_Pair").transform.Find("Ccocal").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Ccocal_Pair").transform.Find("Ccocal_ani").gameObject.SetActive(false);
                break;

            case (int)Symbols.box:
                GameObject.Find($"Row_{a}/Box_Pair").transform.Find("Box").gameObject.SetActive(false);
                GameObject.Find($"Row_{a}/Box_Pair").transform.Find("Box_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Box_Pair").transform.Find("Box").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Box_Pair").transform.Find("Box_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Box_Pair").transform.Find("Box").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Box_Pair").transform.Find("Box_ani").gameObject.SetActive(true);
                yield return new WaitForSeconds(6);
                GameObject.Find($"Row_{a}/Box_Pair").transform.Find("Box").gameObject.SetActive(true);
                GameObject.Find($"Row_{a}/Box_Pair").transform.Find("Box_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Box_Pair").transform.Find("Box").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Box_Pair").transform.Find("Box_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Box_Pair").transform.Find("Box").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Box_Pair").transform.Find("Box_ani").gameObject.SetActive(false);
                break;

            case (int)Symbols.cart:
                GameObject.Find($"Row_{a}/Cart_Pair").transform.Find("Cart").gameObject.SetActive(false);
                GameObject.Find($"Row_{a}/Cart_Pair").transform.Find("Cart_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Cart_Pair").transform.Find("Cart").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Cart_Pair").transform.Find("Cart_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Cart_Pair").transform.Find("Cart").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Cart_Pair").transform.Find("Cart_ani").gameObject.SetActive(true);
                yield return new WaitForSeconds(6);
                GameObject.Find($"Row_{a}/Cart_Pair").transform.Find("Cart").gameObject.SetActive(true);
                GameObject.Find($"Row_{a}/Cart_Pair").transform.Find("Cart_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Cart_Pair").transform.Find("Cart").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Cart_Pair").transform.Find("Cart_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Cart_Pair").transform.Find("Cart").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Cart_Pair").transform.Find("Cart_ani").gameObject.SetActive(false);
                break;

            case (int)Symbols.bonus:
                GameObject.Find($"Row_{a}/Scatter_Pair").transform.Find("Scatter").gameObject.SetActive(false);
                GameObject.Find($"Row_{a}/Scatter_Pair").transform.Find("Scatter_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Scatter_Pair").transform.Find("Scatter").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Scatter_Pair").transform.Find("Scatter_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Scatter_Pair").transform.Find("Scatter").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Scatter_Pair").transform.Find("Scatter_ani").gameObject.SetActive(true);
                yield return new WaitForSeconds(6);
                GameObject.Find($"Row_{a}/Scatter_Pair").transform.Find("Scatter").gameObject.SetActive(true);
                GameObject.Find($"Row_{a}/Scatter_Pair").transform.Find("Scatter_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Scatter_Pair").transform.Find("Scatter").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Scatter_Pair").transform.Find("Scatter_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Scatter_Pair").transform.Find("Scatter").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Scatter_Pair").transform.Find("Scatter_ani").gameObject.SetActive(false);
                break;

            case (int)Symbols.wild:
                GameObject.Find($"Row_{a}/Wild_Pair").transform.Find("Wild").gameObject.SetActive(false);
                GameObject.Find($"Row_{a}/Wild_Pair").transform.Find("Wild_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Wild_Pair").transform.Find("Wild").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Wild_Pair").transform.Find("Wild_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Wild_Pair").transform.Find("Wild").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Wild_Pair").transform.Find("Wild_ani").gameObject.SetActive(true);
                yield return new WaitForSeconds(6);
                GameObject.Find($"Row_{a}/Wild_Pair").transform.Find("Wild").gameObject.SetActive(true);
                GameObject.Find($"Row_{a}/Wild_Pair").transform.Find("Wild_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Wild_Pair").transform.Find("Wild").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Wild_Pair").transform.Find("Wild_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Wild_Pair").transform.Find("Wild").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Wild_Pair").transform.Find("Wild_ani").gameObject.SetActive(false);
                break;
        }//switch
    }

    public IEnumerator Four_Win_Animation(int symbol_value, int a, int b, int c, int d)
    {
        switch (symbol_value)
        {
            case (int)Symbols.pig_green:
                GameObject.Find($"Row_{a}/Green_Pig_Pair").transform.Find("green_pig").gameObject.SetActive(false);
                GameObject.Find($"Row_{a}/Green_Pig_Pair").transform.Find("green_pig_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Green_Pig_Pair").transform.Find("green_pig").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Green_Pig_Pair").transform.Find("green_pig_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Green_Pig_Pair").transform.Find("green_pig").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Green_Pig_Pair").transform.Find("green_pig_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{d}/Green_Pig_Pair").transform.Find("green_pig").gameObject.SetActive(false);
                GameObject.Find($"Row_{d}/Green_Pig_Pair").transform.Find("green_pig_ani").gameObject.SetActive(true);
                yield return new WaitForSeconds(6);
                GameObject.Find($"Row_{a}/Green_Pig_Pair").transform.Find("green_pig").gameObject.SetActive(true);
                GameObject.Find($"Row_{a}/Green_Pig_Pair").transform.Find("green_pig_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Green_Pig_Pair").transform.Find("green_pig").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Green_Pig_Pair").transform.Find("green_pig_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Green_Pig_Pair").transform.Find("green_pig").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Green_Pig_Pair").transform.Find("green_pig_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{d}/Green_Pig_Pair").transform.Find("green_pig").gameObject.SetActive(true);
                GameObject.Find($"Row_{d}/Green_Pig_Pair").transform.Find("green_pig_ani").gameObject.SetActive(false);
                break;

            case (int)Symbols.pig_blue:
                GameObject.Find($"Row_{a}/Blue_Pig_Pair").transform.Find("blue_pig").gameObject.SetActive(false);
                GameObject.Find($"Row_{a}/Blue_Pig_Pair").transform.Find("blue_pig_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Blue_Pig_Pair").transform.Find("blue_pig").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Blue_Pig_Pair").transform.Find("blue_pig_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Blue_Pig_Pair").transform.Find("blue_pig").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Blue_Pig_Pair").transform.Find("blue_pig_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{d}/Blue_Pig_Pair").transform.Find("blue_pig").gameObject.SetActive(false);
                GameObject.Find($"Row_{d}/Blue_Pig_Pair").transform.Find("blue_pig_ani").gameObject.SetActive(true);
                yield return new WaitForSeconds(6);
                GameObject.Find($"Row_{a}/Blue_Pig_Pair").transform.Find("blue_pig").gameObject.SetActive(true);
                GameObject.Find($"Row_{a}/Blue_Pig_Pair").transform.Find("blue_pig_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Blue_Pig_Pair").transform.Find("blue_pig").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Blue_Pig_Pair").transform.Find("blue_pig_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Blue_Pig_Pair").transform.Find("blue_pig").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Blue_Pig_Pair").transform.Find("blue_pig_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{d}/Blue_Pig_Pair").transform.Find("blue_pig").gameObject.SetActive(true);
                GameObject.Find($"Row_{d}/Blue_Pig_Pair").transform.Find("blue_pig_ani").gameObject.SetActive(false);
                break;

            case (int)Symbols.pig_red:
                GameObject.Find($"Row_{a}/Red_Pig_Pair").transform.Find("red_pig").gameObject.SetActive(false);
                GameObject.Find($"Row_{a}/Red_Pig_Pair").transform.Find("red_pig_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Red_Pig_Pair").transform.Find("red_pig").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Red_Pig_Pair").transform.Find("red_pig_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Red_Pig_Pair").transform.Find("red_pig").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Red_Pig_Pair").transform.Find("red_pig_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{d}/Red_Pig_Pair").transform.Find("red_pig").gameObject.SetActive(false);
                GameObject.Find($"Row_{d}/Red_Pig_Pair").transform.Find("red_pig_ani").gameObject.SetActive(true);
                yield return new WaitForSeconds(6);
                GameObject.Find($"Row_{a}/Red_Pig_Pair").transform.Find("red_pig").gameObject.SetActive(true);
                GameObject.Find($"Row_{a}/Red_Pig_Pair").transform.Find("red_pig_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Red_Pig_Pair").transform.Find("red_pig").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Red_Pig_Pair").transform.Find("red_pig_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Red_Pig_Pair").transform.Find("red_pig").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Red_Pig_Pair").transform.Find("red_pig_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{d}/Red_Pig_Pair").transform.Find("red_pig").gameObject.SetActive(true);
                GameObject.Find($"Row_{d}/Red_Pig_Pair").transform.Find("red_pig_ani").gameObject.SetActive(false);
                break;

            case (int)Symbols.A:
                GameObject.Find($"Row_{a}/A_Pair").transform.Find("A").gameObject.SetActive(false);
                GameObject.Find($"Row_{a}/A_Pair").transform.Find("A_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/A_Pair").transform.Find("A").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/A_Pair").transform.Find("A_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/A_Pair").transform.Find("A").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/A_Pair").transform.Find("A_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{d}/A_Pair").transform.Find("A").gameObject.SetActive(false);
                GameObject.Find($"Row_{d}/A_Pair").transform.Find("A_ani").gameObject.SetActive(true);
                yield return new WaitForSeconds(6);
                GameObject.Find($"Row_{a}/A_Pair").transform.Find("A").gameObject.SetActive(true);
                GameObject.Find($"Row_{a}/A_Pair").transform.Find("A_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/A_Pair").transform.Find("A").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/A_Pair").transform.Find("A_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/A_Pair").transform.Find("A").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/A_Pair").transform.Find("A_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{d}/A_Pair").transform.Find("A").gameObject.SetActive(true);
                GameObject.Find($"Row_{d}/A_Pair").transform.Find("A_ani").gameObject.SetActive(false);
                break;

            case (int)Symbols.K:
                GameObject.Find($"Row_{a}/K_Pair").transform.Find("K").gameObject.SetActive(false);
                GameObject.Find($"Row_{a}/K_Pair").transform.Find("K_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/K_Pair").transform.Find("K").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/K_Pair").transform.Find("K_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/K_Pair").transform.Find("K").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/K_Pair").transform.Find("K_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{d}/K_Pair").transform.Find("K").gameObject.SetActive(false);
                GameObject.Find($"Row_{d}/K_Pair").transform.Find("K_ani").gameObject.SetActive(true);
                yield return new WaitForSeconds(6);
                GameObject.Find($"Row_{a}/K_Pair").transform.Find("K").gameObject.SetActive(true);
                GameObject.Find($"Row_{a}/K_Pair").transform.Find("K_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/K_Pair").transform.Find("K").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/K_Pair").transform.Find("K_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/K_Pair").transform.Find("K").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/K_Pair").transform.Find("K_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{d}/K_Pair").transform.Find("K").gameObject.SetActive(true);
                GameObject.Find($"Row_{d}/K_Pair").transform.Find("K_ani").gameObject.SetActive(false);
                break;

            case (int)Symbols.Q:
                GameObject.Find($"Row_{a}/Q_Pair").transform.Find("Q").gameObject.SetActive(false);
                GameObject.Find($"Row_{a}/Q_Pair").transform.Find("Q_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Q_Pair").transform.Find("Q").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Q_Pair").transform.Find("Q_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Q_Pair").transform.Find("Q").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Q_Pair").transform.Find("Q_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{d}/Q_Pair").transform.Find("Q").gameObject.SetActive(false);
                GameObject.Find($"Row_{d}/Q_Pair").transform.Find("Q_ani").gameObject.SetActive(true);
                yield return new WaitForSeconds(6);
                GameObject.Find($"Row_{a}/Q_Pair").transform.Find("Q").gameObject.SetActive(true);
                GameObject.Find($"Row_{a}/Q_Pair").transform.Find("Q_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Q_Pair").transform.Find("Q").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Q_Pair").transform.Find("Q_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Q_Pair").transform.Find("Q").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Q_Pair").transform.Find("Q_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{d}/Q_Pair").transform.Find("Q").gameObject.SetActive(true);
                GameObject.Find($"Row_{d}/Q_Pair").transform.Find("Q_ani").gameObject.SetActive(false);
                break;

            case (int)Symbols.J:
                GameObject.Find($"Row_{a}/J_Pair").transform.Find("J").gameObject.SetActive(false);
                GameObject.Find($"Row_{a}/J_Pair").transform.Find("J_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/J_Pair").transform.Find("J").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/J_Pair").transform.Find("J_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/J_Pair").transform.Find("J").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/J_Pair").transform.Find("J_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{d}/J_Pair").transform.Find("J").gameObject.SetActive(false);
                GameObject.Find($"Row_{d}/J_Pair").transform.Find("J_ani").gameObject.SetActive(true);
                yield return new WaitForSeconds(6);
                GameObject.Find($"Row_{a}/J_Pair").transform.Find("J").gameObject.SetActive(true);
                GameObject.Find($"Row_{a}/J_Pair").transform.Find("J_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/J_Pair").transform.Find("J").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/J_Pair").transform.Find("J_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/J_Pair").transform.Find("J").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/J_Pair").transform.Find("J_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{d}/J_Pair").transform.Find("J").gameObject.SetActive(true);
                GameObject.Find($"Row_{d}/J_Pair").transform.Find("J_ani").gameObject.SetActive(false);
                break;

            case (int)Symbols.del:
                GameObject.Find($"Row_{a}/Ccocal_Pair").transform.Find("Ccocal").gameObject.SetActive(false);
                GameObject.Find($"Row_{a}/Ccocal_Pair").transform.Find("Ccocal_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Ccocal_Pair").transform.Find("Ccocal").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Ccocal_Pair").transform.Find("Ccocal_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Ccocal_Pair").transform.Find("Ccocal").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Ccocal_Pair").transform.Find("Ccocal_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{d}/Ccocal_Pair").transform.Find("Ccocal").gameObject.SetActive(false);
                GameObject.Find($"Row_{d}/Ccocal_Pair").transform.Find("Ccocal_ani").gameObject.SetActive(true);
                yield return new WaitForSeconds(6);
                GameObject.Find($"Row_{a}/Ccocal_Pair").transform.Find("Ccocal").gameObject.SetActive(true);
                GameObject.Find($"Row_{a}/Ccocal_Pair").transform.Find("Ccocal_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Ccocal_Pair").transform.Find("Ccocal").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Ccocal_Pair").transform.Find("Ccocal_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Ccocal_Pair").transform.Find("Ccocal").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Ccocal_Pair").transform.Find("Ccocal_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{d}/Ccocal_Pair").transform.Find("Ccocal").gameObject.SetActive(true);
                GameObject.Find($"Row_{d}/Ccocal_Pair").transform.Find("Ccocal_ani").gameObject.SetActive(false);
                break;

            case (int)Symbols.box:
                GameObject.Find($"Row_{a}/Box_Pair").transform.Find("Box").gameObject.SetActive(false);
                GameObject.Find($"Row_{a}/Box_Pair").transform.Find("Box_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Box_Pair").transform.Find("Box").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Box_Pair").transform.Find("Box_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Box_Pair").transform.Find("Box").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Box_Pair").transform.Find("Box_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{d}/Box_Pair").transform.Find("Box").gameObject.SetActive(false);
                GameObject.Find($"Row_{d}/Box_Pair").transform.Find("Box_ani").gameObject.SetActive(true);
                yield return new WaitForSeconds(6);
                GameObject.Find($"Row_{a}/Box_Pair").transform.Find("Box").gameObject.SetActive(true);
                GameObject.Find($"Row_{a}/Box_Pair").transform.Find("Box_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Box_Pair").transform.Find("Box").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Box_Pair").transform.Find("Box_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Box_Pair").transform.Find("Box").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Box_Pair").transform.Find("Box_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{d}/Box_Pair").transform.Find("Box").gameObject.SetActive(true);
                GameObject.Find($"Row_{d}/Box_Pair").transform.Find("Box_ani").gameObject.SetActive(false);
                break;

            case (int)Symbols.cart:
                GameObject.Find($"Row_{a}/Cart_Pair").transform.Find("Cart").gameObject.SetActive(false);
                GameObject.Find($"Row_{a}/Cart_Pair").transform.Find("Cart_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Cart_Pair").transform.Find("Cart").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Cart_Pair").transform.Find("Cart_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Cart_Pair").transform.Find("Cart").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Cart_Pair").transform.Find("Cart_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{d}/Cart_Pair").transform.Find("Cart").gameObject.SetActive(false);
                GameObject.Find($"Row_{d}/Cart_Pair").transform.Find("Cart_ani").gameObject.SetActive(true);
                yield return new WaitForSeconds(6);
                GameObject.Find($"Row_{a}/Cart_Pair").transform.Find("Cart").gameObject.SetActive(true);
                GameObject.Find($"Row_{a}/Cart_Pair").transform.Find("Cart_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Cart_Pair").transform.Find("Cart").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Cart_Pair").transform.Find("Cart_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Cart_Pair").transform.Find("Cart").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Cart_Pair").transform.Find("Cart_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{d}/Cart_Pair").transform.Find("Cart").gameObject.SetActive(true);
                GameObject.Find($"Row_{d}/Cart_Pair").transform.Find("Cart_ani").gameObject.SetActive(false);
                break;

            case (int)Symbols.bonus:
                GameObject.Find($"Row_{a}/Scatter_Pair").transform.Find("Scatter").gameObject.SetActive(false);
                GameObject.Find($"Row_{a}/Scatter_Pair").transform.Find("Scatter_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Scatter_Pair").transform.Find("Scatter").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Scatter_Pair").transform.Find("Scatter_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Scatter_Pair").transform.Find("Scatter").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Scatter_Pair").transform.Find("Scatter_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{d}/Scatter_Pair").transform.Find("Scatter").gameObject.SetActive(false);
                GameObject.Find($"Row_{d}/Scatter_Pair").transform.Find("Scatter_ani").gameObject.SetActive(true);
                yield return new WaitForSeconds(6);
                GameObject.Find($"Row_{a}/Scatter_Pair").transform.Find("Scatter").gameObject.SetActive(true);
                GameObject.Find($"Row_{a}/Scatter_Pair").transform.Find("Scatter_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Scatter_Pair").transform.Find("Scatter").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Scatter_Pair").transform.Find("Scatter_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Scatter_Pair").transform.Find("Scatter").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Scatter_Pair").transform.Find("Scatter_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{d}/Scatter_Pair").transform.Find("Scatter").gameObject.SetActive(true);
                GameObject.Find($"Row_{d}/Scatter_Pair").transform.Find("Scatter_ani").gameObject.SetActive(false);
                break;

            case (int)Symbols.wild:
                GameObject.Find($"Row_{a}/Wild_Pair").transform.Find("Wild").gameObject.SetActive(false);
                GameObject.Find($"Row_{a}/Wild_Pair").transform.Find("Wild_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Wild_Pair").transform.Find("Wild").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Wild_Pair").transform.Find("Wild_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Wild_Pair").transform.Find("Wild").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Wild_Pair").transform.Find("Wild_ani").gameObject.SetActive(true);
                GameObject.Find($"Row_{d}/Wild_Pair").transform.Find("Wild").gameObject.SetActive(false);
                GameObject.Find($"Row_{d}/Wild_Pair").transform.Find("Wild_ani").gameObject.SetActive(true);
                yield return new WaitForSeconds(6);
                GameObject.Find($"Row_{a}/Wild_Pair").transform.Find("Wild").gameObject.SetActive(true);
                GameObject.Find($"Row_{a}/Wild_Pair").transform.Find("Wild_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{b}/Wild_Pair").transform.Find("Wild").gameObject.SetActive(true);
                GameObject.Find($"Row_{b}/Wild_Pair").transform.Find("Wild_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{c}/Wild_Pair").transform.Find("Wild").gameObject.SetActive(true);
                GameObject.Find($"Row_{c}/Wild_Pair").transform.Find("Wild_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{d}/Wild_Pair").transform.Find("Wild").gameObject.SetActive(true);
                GameObject.Find($"Row_{d}/Wild_Pair").transform.Find("Wild_ani").gameObject.SetActive(false);
                break;
        }//switch
    }

    //이 함수가 호출될 확률은 거의 0에 수렴함
    public IEnumerator Five_Win_Animation(int symbol_value)
    {
        for (int i = 0; i < 5; i++)
        {
            switch (symbol_value)
            {
                case (int)Symbols.pig_green:
                    GameObject.Find($"Row_{i}/Green_Pig_Pair").transform.Find("green_pig").gameObject.SetActive(false);
                    GameObject.Find($"Row_{i}/Green_Pig_Pair").transform.Find("green_pig_ani").gameObject.SetActive(true);
                    break;
                case (int)Symbols.pig_blue:
                    GameObject.Find($"Row_{i}/Blue_Pig_Pair").transform.Find("blue_pig").gameObject.SetActive(false);
                    GameObject.Find($"Row_{i}/Blue_Pig_Pair").transform.Find("blue_pig_ani").gameObject.SetActive(true);
                    break;
                case (int)Symbols.pig_red:
                    GameObject.Find($"Row_{i}/Red_Pig_Pair").transform.Find("red_pig").gameObject.SetActive(false);
                    GameObject.Find($"Row_{i}/Red_Pig_Pair").transform.Find("red_pig_ani").gameObject.SetActive(true);
                    break;
                case (int)Symbols.A:
                    GameObject.Find($"Row_{i}/A_Pair").transform.Find("A").gameObject.SetActive(false);
                    GameObject.Find($"Row_{i}/A_Pair").transform.Find("A_ani").gameObject.SetActive(true);
                    break;
                case (int)Symbols.K:
                    GameObject.Find($"Row_{i}/K_Pair").transform.Find("K").gameObject.SetActive(false);
                    GameObject.Find($"Row_{i}/K_Pair").transform.Find("K_ani").gameObject.SetActive(true);
                    break;
                case (int)Symbols.Q:
                    GameObject.Find($"Row_{i}/Q_Pair").transform.Find("Q").gameObject.SetActive(false);
                    GameObject.Find($"Row_{i}/Q_Pair").transform.Find("Q_ani").gameObject.SetActive(true);
                    break;
                case (int)Symbols.J:
                    GameObject.Find($"Row_{i}/J_Pair").transform.Find("J").gameObject.SetActive(false);
                    GameObject.Find($"Row_{i}/J_Pair").transform.Find("J_ani").gameObject.SetActive(true);
                    break;
                case (int)Symbols.del:
                    GameObject.Find($"Row_{i}/Ccocal_Pair").transform.Find("Ccocal").gameObject.SetActive(false);
                    GameObject.Find($"Row_{i}/Ccocal_Pair").transform.Find("Ccocal_ani").gameObject.SetActive(true);
                    break;
                case (int)Symbols.box:
                    GameObject.Find($"Row_{i}/Box_Pair").transform.Find("Box").gameObject.SetActive(false);
                    GameObject.Find($"Row_{i}/Box_Pair").transform.Find("Box_ani").gameObject.SetActive(true);
                    break;
                case (int)Symbols.cart:
                    GameObject.Find($"Row_{i}/Cart_Pair").transform.Find("Cart").gameObject.SetActive(false);
                    GameObject.Find($"Row_{i}/Cart_Pair").transform.Find("Cart_ani").gameObject.SetActive(true);
                    break;
                case (int)Symbols.bonus:
                    GameObject.Find($"Row_{i}/Scatter_Pair").transform.Find("Scatter").gameObject.SetActive(false);
                    GameObject.Find($"Row_{i}/Scatter_Pair").transform.Find("Scatter_ani").gameObject.SetActive(true);
                    break;
                case (int)Symbols.wild:
                    GameObject.Find($"Row_{i}/Wild_Pair").transform.Find("Wild").gameObject.SetActive(false);
                    GameObject.Find($"Row_{i}/Wild_Pair").transform.Find("Wild_ani").gameObject.SetActive(true);
                    break;
            }//switch
        }

        yield return new WaitForSeconds(6);

        for (int i = 0; i < 5; i++)
        {
            switch (symbol_value)
            {
                case (int)Symbols.pig_green:
                    GameObject.Find($"Row_{i}/Green_Pig_Pair").transform.Find("green_pig").gameObject.SetActive(true);
                    GameObject.Find($"Row_{i}/Green_Pig_Pair").transform.Find("green_pig_ani").gameObject.SetActive(false);
                    break;
                case (int)Symbols.pig_blue:
                    GameObject.Find($"Row_{i}/Blue_Pig_Pair").transform.Find("blue_pig").gameObject.SetActive(true);
                    GameObject.Find($"Row_{i}/Blue_Pig_Pair").transform.Find("blue_pig_ani").gameObject.SetActive(false);
                    break;
                case (int)Symbols.pig_red:
                    GameObject.Find($"Row_{i}/Red_Pig_Pair").transform.Find("red_pig").gameObject.SetActive(true);
                    GameObject.Find($"Row_{i}/Red_Pig_Pair").transform.Find("red_pig_ani").gameObject.SetActive(false);
                    break;
                case (int)Symbols.A:
                    GameObject.Find($"Row_{i}/A_Pair").transform.Find("A").gameObject.SetActive(true);
                    GameObject.Find($"Row_{i}/A_Pair").transform.Find("A_ani").gameObject.SetActive(false);
                    break;
                case (int)Symbols.K:
                    GameObject.Find($"Row_{i}/K_Pair").transform.Find("K").gameObject.SetActive(true);
                    GameObject.Find($"Row_{i}/K_Pair").transform.Find("K_ani").gameObject.SetActive(false);
                    break;
                case (int)Symbols.Q:
                    GameObject.Find($"Row_{i}/Q_Pair").transform.Find("Q").gameObject.SetActive(true);
                    GameObject.Find($"Row_{i}/Q_Pair").transform.Find("Q_ani").gameObject.SetActive(false);
                    break;
                case (int)Symbols.J:
                    GameObject.Find($"Row_{i}/J_Pair").transform.Find("J").gameObject.SetActive(true);
                    GameObject.Find($"Row_{i}/J_Pair").transform.Find("J_ani").gameObject.SetActive(false);
                    break;
                case (int)Symbols.del:
                    GameObject.Find($"Row_{i}/Ccocal_Pair").transform.Find("Ccocal").gameObject.SetActive(true);
                    GameObject.Find($"Row_{i}/Ccocal_Pair").transform.Find("Ccocal_ani").gameObject.SetActive(false);
                    break;
                case (int)Symbols.box:
                    GameObject.Find($"Row_{i}/Box_Pair").transform.Find("Box").gameObject.SetActive(true);
                    GameObject.Find($"Row_{i}/Box_Pair").transform.Find("Box_ani").gameObject.SetActive(false);
                    break;
                case (int)Symbols.cart:
                    GameObject.Find($"Row_{i}/Cart_Pair").transform.Find("Cart").gameObject.SetActive(true);
                    GameObject.Find($"Row_{i}/Cart_Pair").transform.Find("Cart_ani").gameObject.SetActive(false);
                    break;
                case (int)Symbols.bonus:
                    GameObject.Find($"Row_{i}/Scatter_Pair").transform.Find("Scatter").gameObject.SetActive(true);
                    GameObject.Find($"Row_{i}/Scatter_Pair").transform.Find("Scatter_ani").gameObject.SetActive(false);
                    break;
                case (int)Symbols.wild:
                    GameObject.Find($"Row_{i}/Wild_Pair").transform.Find("Wild").gameObject.SetActive(true);
                    GameObject.Find($"Row_{i}/Wild_Pair").transform.Find("Wild_ani").gameObject.SetActive(false);
                    break;
            }//switch
        }
    }
    #endregion

    #region 애니메이션 강제 셧다운
    public void Animation_shutdown_trigger()
    {
        StartCoroutine("Animation_shutdown");
    }

    //코루틴으로 작성해야할듯 함
    public IEnumerator Animation_shutdown()
    {
        for (int i = 0; i < 5; i++)
        {
            //2,3,4열은 스캐터가 포함
            if (i == 1 || i == 2 || i == 3)
            {
                GameObject.Find($"Row_{i}/Green_Pig_Pair").transform.Find("green_pig").gameObject.SetActive(true);
                GameObject.Find($"Row_{i}/Green_Pig_Pair").transform.Find("green_pig_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{i}/Blue_Pig_Pair").transform.Find("blue_pig").gameObject.SetActive(true);
                GameObject.Find($"Row_{i}/Blue_Pig_Pair").transform.Find("blue_pig_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{i}/Red_Pig_Pair").transform.Find("red_pig").gameObject.SetActive(true);
                GameObject.Find($"Row_{i}/Red_Pig_Pair").transform.Find("red_pig_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{i}/A_Pair").transform.Find("A").gameObject.SetActive(true);
                GameObject.Find($"Row_{i}/A_Pair").transform.Find("A_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{i}/K_Pair").transform.Find("K").gameObject.SetActive(true);
                GameObject.Find($"Row_{i}/K_Pair").transform.Find("K_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{i}/Q_Pair").transform.Find("Q").gameObject.SetActive(true);
                GameObject.Find($"Row_{i}/Q_Pair").transform.Find("Q_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{i}/J_Pair").transform.Find("J").gameObject.SetActive(true);
                GameObject.Find($"Row_{i}/J_Pair").transform.Find("J_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{i}/Ccocal_Pair").transform.Find("Ccocal").gameObject.SetActive(true);
                GameObject.Find($"Row_{i}/Ccocal_Pair").transform.Find("Ccocal_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{i}/Box_Pair").transform.Find("Box").gameObject.SetActive(true);
                GameObject.Find($"Row_{i}/Box_Pair").transform.Find("Box_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{i}/Cart_Pair").transform.Find("Cart").gameObject.SetActive(true);
                GameObject.Find($"Row_{i}/Cart_Pair").transform.Find("Cart_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{i}/Wild_Pair").transform.Find("Wild").gameObject.SetActive(true);
                GameObject.Find($"Row_{i}/Wild_Pair").transform.Find("Wild_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{i}/Scatter_Pair").transform.Find("Scatter").gameObject.SetActive(true);
                GameObject.Find($"Row_{i}/Scatter_Pair").transform.Find("Scatter_ani").gameObject.SetActive(false);
            }
            else
            {
                GameObject.Find($"Row_{i}/Green_Pig_Pair").transform.Find("green_pig").gameObject.SetActive(true);
                GameObject.Find($"Row_{i}/Green_Pig_Pair").transform.Find("green_pig_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{i}/Blue_Pig_Pair").transform.Find("blue_pig").gameObject.SetActive(true);
                GameObject.Find($"Row_{i}/Blue_Pig_Pair").transform.Find("blue_pig_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{i}/Red_Pig_Pair").transform.Find("red_pig").gameObject.SetActive(true);
                GameObject.Find($"Row_{i}/Red_Pig_Pair").transform.Find("red_pig_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{i}/A_Pair").transform.Find("A").gameObject.SetActive(true);
                GameObject.Find($"Row_{i}/A_Pair").transform.Find("A_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{i}/K_Pair").transform.Find("K").gameObject.SetActive(true);
                GameObject.Find($"Row_{i}/K_Pair").transform.Find("K_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{i}/Q_Pair").transform.Find("Q").gameObject.SetActive(true);
                GameObject.Find($"Row_{i}/Q_Pair").transform.Find("Q_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{i}/J_Pair").transform.Find("J").gameObject.SetActive(true);
                GameObject.Find($"Row_{i}/J_Pair").transform.Find("J_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{i}/Ccocal_Pair").transform.Find("Ccocal").gameObject.SetActive(true);
                GameObject.Find($"Row_{i}/Ccocal_Pair").transform.Find("Ccocal_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{i}/Box_Pair").transform.Find("Box").gameObject.SetActive(true);
                GameObject.Find($"Row_{i}/Box_Pair").transform.Find("Box_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{i}/Cart_Pair").transform.Find("Cart").gameObject.SetActive(true);
                GameObject.Find($"Row_{i}/Cart_Pair").transform.Find("Cart_ani").gameObject.SetActive(false);
                GameObject.Find($"Row_{i}/Wild_Pair").transform.Find("Wild").gameObject.SetActive(true);
                GameObject.Find($"Row_{i}/Wild_Pair").transform.Find("Wild_ani").gameObject.SetActive(false);
            }
        }
        return null;
    }
    #endregion

    public void Animation_trigger(int[] m, int[] A)
    {
        int max_cnt = -1;
        int max_cnt_symbol = -1;
        for (int i = 0; i < 18; i++)
        {
            if (max_cnt < A[i])
            {
                max_cnt = A[i];
                max_cnt_symbol = i;
            }
        }

        if (max_cnt == 3)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < 5; i++)
            {
                if (m[i] == max_cnt_symbol)//3번 호출될것임
                {
                    //각 인덱스가 몇번째 릴인지를 의미함
                    list.Add(i);
                }
            }
            StartCoroutine(Three_Win_Animation(max_cnt_symbol, list[0], list[1], list[2]));
        }

        else if (max_cnt == 4)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < 5; i++)
            {
                if (m[i] == max_cnt_symbol)//4번 호출될것임
                {
                    list.Add(i);
                }
            }
            StartCoroutine(Four_Win_Animation(max_cnt_symbol, list[0], list[1], list[2], list[3]));
        }

        else if (max_cnt == 5)
        {
            StartCoroutine(Five_Win_Animation(max_cnt_symbol));
        }
    }//Animation_Trigger

    public ulong Payline_1()
    {
        //■■■■■
        //□□□□□
        //□□□□□
        int[] m = new int[5];
        m[0] = map[0, 0];
        m[1] = map[0, 1];
        m[2] = map[0, 2];
        m[3] = map[0, 3];
        m[4] = map[0, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }

        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_2()
    {
        //□□□□□
        //■■■■■
        //□□□□□
        int[] m = new int[5];
        m[0] = map[1, 0];
        m[1] = map[1, 1];
        m[2] = map[1, 2];
        m[3] = map[1, 3];
        m[4] = map[1, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_3()
    {
        //□□□□□
        //□□□□□
        //■■■■■
        int[] m = new int[5];
        m[0] = map[2, 0];
        m[1] = map[2, 1];
        m[2] = map[2, 2];
        m[3] = map[2, 3];
        m[4] = map[2, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_4()
    {
        //■□□□■
        //□■□■□
        //□□■□□

        int[] m = new int[5];
        m[0] = map[0, 0];
        m[1] = map[1, 1];
        m[2] = map[2, 2];
        m[3] = map[1, 3];
        m[4] = map[0, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_5()
    {
        //□□■□□
        //□■□■□
        //■□□□■

        int[] m = new int[5];
        m[0] = map[2, 0];
        m[1] = map[1, 1];
        m[2] = map[0, 2];
        m[3] = map[1, 3];
        m[4] = map[2, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_6()
    {
        //□■■■□
        //■□□□■
        //□□□□□

        int[] m = new int[5];
        m[0] = map[1, 0];
        m[1] = map[0, 1];
        m[2] = map[0, 2];
        m[3] = map[0, 3];
        m[4] = map[1, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_7()
    {
        //□□□□□
        //■□□□■
        //□■■■□
        int[] m = new int[5];
        m[0] = map[1, 0];
        m[1] = map[2, 1];
        m[2] = map[2, 2];
        m[3] = map[2, 3];
        m[4] = map[1, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_8()
    {
        //■■□□□
        //□□■□□
        //□□□■■
        int[] m = new int[5];
        m[0] = map[0, 0];
        m[1] = map[0, 1];
        m[2] = map[1, 2];
        m[3] = map[2, 3];
        m[4] = map[2, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_9()
    {
        //□□□■■
        //□□■□□
        //■■□□□
        int[] m = new int[5];
        m[0] = map[2, 0];
        m[1] = map[2, 1];
        m[2] = map[1, 2];
        m[3] = map[0, 3];
        m[4] = map[0, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_10()
    {
        //□□□■□
        //■□■□■
        //□■□□□
        int[] m = new int[5];
        m[0] = map[1, 0];
        m[1] = map[2, 1];
        m[2] = map[1, 2];
        m[3] = map[0, 3];
        m[4] = map[1, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_11()
    {
        //□■□□□
        //■□■□■
        //□□□■□
        int[] m = new int[5];
        m[0] = map[1, 0];
        m[1] = map[0, 1];
        m[2] = map[1, 2];
        m[3] = map[2, 3];
        m[4] = map[1, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_12()
    {
        //■□□□■
        //□■■■□
        //□□□□□
        int[] m = new int[5];
        m[0] = map[0, 0];
        m[1] = map[1, 1];
        m[2] = map[1, 2];
        m[3] = map[1, 3];
        m[4] = map[0, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_13()
    {
        //□□□□□
        //□■■■□
        //■□□□■
        int[] m = new int[5];
        m[0] = map[2, 0];
        m[1] = map[1, 1];
        m[2] = map[1, 2];
        m[3] = map[1, 3];
        m[4] = map[2, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_14()
    {
        //■□■□■
        //□■□■□
        //□□□□□
        int[] m = new int[5];
        m[0] = map[0, 0];
        m[1] = map[1, 1];
        m[2] = map[0, 2];
        m[3] = map[1, 3];
        m[4] = map[0, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_15()
    {
        //□□□□□
        //□■□■□
        //■□■□■
        int[] m = new int[5];
        m[0] = map[2, 0];
        m[1] = map[1, 1];
        m[2] = map[2, 2];
        m[3] = map[1, 3];
        m[4] = map[2, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_16()
    {
        //□□■□□
        //■■□■■
        //□□□□□
        int[] m = new int[5];
        m[0] = map[1, 0];
        m[1] = map[1, 1];
        m[2] = map[0, 2];
        m[3] = map[1, 3];
        m[4] = map[1, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_17()
    {
        //□□□□□
        //■■□■■
        //□□■□□
        int[] m = new int[5];
        m[0] = map[1, 0];
        m[1] = map[1, 1];
        m[2] = map[2, 2];
        m[3] = map[1, 3];
        m[4] = map[1, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_18()
    {
        //■■□■■
        //□□□□□
        //□□■□□
        int[] m = new int[5];
        m[0] = map[0, 0];
        m[1] = map[0, 1];
        m[2] = map[2, 2];
        m[3] = map[0, 3];
        m[4] = map[0, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_19()
    {
        //□□■□□
        //□□□□□
        //■■□■■
        int[] m = new int[5];
        m[0] = map[2, 0];
        m[1] = map[2, 1];
        m[2] = map[0, 2];
        m[3] = map[2, 3];
        m[4] = map[2, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_20()
    {
        //■□□□■
        //□□□□□
        //□■■■□
        int[] m = new int[5];
        m[0] = map[0, 0];
        m[1] = map[2, 1];
        m[2] = map[2, 2];
        m[3] = map[2, 3];
        m[4] = map[0, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_21()
    {
        //□■■■□
        //□□□□□
        //■□□□■
        int[] m = new int[5];
        m[0] = map[2, 0];
        m[1] = map[0, 1];
        m[2] = map[0, 2];
        m[3] = map[0, 3];
        m[4] = map[2, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_22()
    {
        //□□■□□
        //■□□□■
        //□■□■□
        int[] m = new int[5];
        m[0] = map[1, 0];
        m[1] = map[2, 1];
        m[2] = map[0, 2];
        m[3] = map[2, 3];
        m[4] = map[1, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_23()
    {
        //□■□■□
        //■□□□■
        //□□■□□
        int[] m = new int[5];
        m[0] = map[1, 0];
        m[1] = map[0, 1];
        m[2] = map[2, 2];
        m[3] = map[0, 3];
        m[4] = map[1, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_24()
    {
        //■□■□■
        //□□□□□
        //□■□■□
        int[] m = new int[5];
        m[0] = map[0, 0];
        m[1] = map[2, 1];
        m[2] = map[0, 2];
        m[3] = map[2, 3];
        m[4] = map[0, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_25()
    {
        //□■□■□
        //□□□□□
        //■□■□■
        int[] m = new int[5];
        m[0] = map[2, 0];
        m[1] = map[0, 1];
        m[2] = map[2, 2];
        m[3] = map[0, 3];
        m[4] = map[2, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_26()
    {
        //□■□□■
        //□□■□□
        //■□□■□
        int[] m = new int[5];
        m[0] = map[2, 0];
        m[1] = map[0, 1];
        m[2] = map[1, 2];
        m[3] = map[2, 3];
        m[4] = map[0, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_27()
    {
        //■□□■□
        //□□■□□
        //□■□□■
        int[] m = new int[5];
        m[0] = map[0, 0];
        m[1] = map[2, 1];
        m[2] = map[1, 2];
        m[3] = map[0, 3];
        m[4] = map[2, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_28()
    {
        //■□□□■
        //□□■□□
        //□■□■□
        int[] m = new int[5];
        m[0] = map[0, 0];
        m[1] = map[2, 1];
        m[2] = map[1, 2];
        m[3] = map[2, 3];
        m[4] = map[0, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_29()
    {
        //□■□■□
        //□□■□□
        //■□□□■
        int[] m = new int[5];
        m[0] = map[2, 0];
        m[1] = map[0, 1];
        m[2] = map[1, 2];
        m[3] = map[0, 3];
        m[4] = map[2, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_30()
    {
        //□□■■□
        //□■□□■
        //■□□□□
        int[] m = new int[5];
        m[0] = map[2, 0];
        m[1] = map[1, 1];
        m[2] = map[0, 2];
        m[3] = map[0, 3];
        m[4] = map[2, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_31()
    {
        //■□□□□
        //□■□□■
        //□□■■□
        int[] m = new int[5];
        m[0] = map[0, 0];
        m[1] = map[1, 1];
        m[2] = map[2, 2];
        m[3] = map[2, 3];
        m[4] = map[1, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_32()
    {
        //■■□□□
        //□□□□□
        //□□■■■
        int[] m = new int[5];
        m[0] = map[0, 0];
        m[1] = map[0, 1];
        m[2] = map[2, 2];
        m[3] = map[2, 3];
        m[4] = map[2, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_33()
    {
        //□□■■■
        //□□□□□
        //■■□□□
        int[] m = new int[5];
        m[0] = map[2, 0];
        m[1] = map[2, 1];
        m[2] = map[0, 2];
        m[3] = map[0, 3];
        m[4] = map[0, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_34()
    {
        //□■□□□
        //■□□■□
        //□□■□■
        int[] m = new int[5];
        m[0] = map[1, 0];
        m[1] = map[0, 1];
        m[2] = map[2, 2];
        m[3] = map[1, 3];
        m[4] = map[2, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_35()
    {
        //□□■□■
        //■□□■□
        //□■□□□
        int[] m = new int[5];
        m[0] = map[1, 0];
        m[1] = map[2, 1];
        m[2] = map[0, 2];
        m[3] = map[1, 3];
        m[4] = map[0, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_36()
    {
        //■□■□□
        //□■□■□
        //□□□□■
        int[] m = new int[5];
        m[0] = map[0, 0];
        m[1] = map[1, 1];
        m[2] = map[0, 2];
        m[3] = map[1, 3];
        m[4] = map[2, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_37()
    {
        //□□□□■
        //□■□■□
        //■□■□□
        int[] m = new int[5];
        m[0] = map[2, 0];
        m[1] = map[1, 1];
        m[2] = map[2, 2];
        m[3] = map[1, 3];
        m[4] = map[0, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_38()
    {
        //□□□■■
        //■□□□□
        //□■■□□
        int[] m = new int[5];
        m[0] = map[1, 0];
        m[1] = map[2, 1];
        m[2] = map[2, 2];
        m[3] = map[0, 3];
        m[4] = map[0, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_39()
    {
        //■■□□□
        //□□■■□
        //□□□□■
        int[] m = new int[5];
        m[0] = map[0, 0];
        m[1] = map[0, 1];
        m[2] = map[1, 2];
        m[3] = map[1, 3];
        m[4] = map[2, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_40()
    {
        //□□□□■
        //□□■■□
        //■■□□□
        int[] m = new int[5];
        m[0] = map[2, 0];
        m[1] = map[2, 1];
        m[2] = map[1, 2];
        m[3] = map[1, 3];
        m[4] = map[0, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_41()
    {
        //□■■■■
        //□□□□□
        //■□□□□
        int[] m = new int[5];
        m[0] = map[2, 0];
        m[1] = map[0, 1];
        m[2] = map[0, 2];
        m[3] = map[0, 3];
        m[4] = map[0, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_42()
    {
        //■□□□□
        //□□□□□
        //□■■■■
        int[] m = new int[5];
        m[0] = map[0, 0];
        m[1] = map[2, 1];
        m[2] = map[2, 2];
        m[3] = map[2, 3];
        m[4] = map[2, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_43()
    {
        //□□□□■
        //□□□□□
        //■■■■□
        int[] m = new int[5];
        m[0] = map[2, 0];
        m[1] = map[2, 1];
        m[2] = map[2, 2];
        m[3] = map[2, 3];
        m[4] = map[0, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_44()
    {
        //■■■■□
        //□□□□□
        //□□□□■
        int[] m = new int[5];
        m[0] = map[0, 0];
        m[1] = map[0, 1];
        m[2] = map[0, 2];
        m[3] = map[0, 3];
        m[4] = map[2, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_45()
    {
        //□■□■□
        //■□■□■
        //□□□□□
        int[] m = new int[5];
        m[0] = map[1, 0];
        m[1] = map[0, 1];
        m[2] = map[1, 2];
        m[3] = map[0, 3];
        m[4] = map[1, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_46()
    {
        //□□□□□
        //■□■□■
        //□■□■□
        int[] m = new int[5];
        m[0] = map[1, 0];
        m[1] = map[2, 1];
        m[2] = map[1, 2];
        m[3] = map[2, 3];
        m[4] = map[1, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_47()
    {
        //■□□□□
        //□■□□□
        //□□■■■
        int[] m = new int[5];
        m[0] = map[0, 0];
        m[1] = map[1, 1];
        m[2] = map[2, 2];
        m[3] = map[2, 3];
        m[4] = map[2, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_48()
    {
        //□□■■■
        //□■□□□
        //■□□□□
        int[] m = new int[5];
        m[0] = map[2, 0];
        m[1] = map[1, 1];
        m[2] = map[0, 2];
        m[3] = map[0, 3];
        m[4] = map[0, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_49()
    {
        //■□□□□
        //□■■■■
        //□□□□□
        int[] m = new int[5];
        m[0] = map[0, 0];
        m[1] = map[1, 1];
        m[2] = map[1, 2];
        m[3] = map[1, 3];
        m[4] = map[1, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public ulong Payline_50()
    {
        //□□□□□
        //□■■■■
        //■□□□□
        int[] m = new int[5];
        m[0] = map[2, 0];
        m[1] = map[1, 1];
        m[2] = map[1, 2];
        m[3] = map[1, 3];
        m[4] = map[1, 4];

        int[] A = new int[18];
        System.Array.Clear(A, 0, 18);

        for (int i = 0; i < 5; i++)
        {
            A[m[i]]++;
        }
        Animation_trigger(m, A);
        return Calculate_Score(A);
    }

    public int Jackpot_Prize()
    {
        int cnt = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (map[i, j] == (int)Symbols.coin1) cnt += 1;
                if (map[i, j] == (int)Symbols.coin2) cnt += 2;
                if (map[i, j] == (int)Symbols.coin3) cnt += 3;
            }
        }

        if (cnt == 5)
        {
            return 5;
        }
        else if (cnt == 6)
        {
            return 6;
        }
        else if (cnt == 7)
        {
            return 7;
        }
        else if (cnt == 8)
        {
            return 8;
        }
        else if (cnt >= 9)
        {
            return 9;
        }
        return -1;
    }
}

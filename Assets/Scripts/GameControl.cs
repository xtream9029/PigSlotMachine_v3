using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameControl : Symbol
{
    #region 각종 텍스트들과 변수들
    [SerializeField]//private 변수를 인스펙터에서 접근 가능하게 해주는 기능
    private Text prizeText;//점수표시
    public Text goldText;//점수표시
    public Text bettingText;
    public Text errorText;
    public Text bounsText;
    public Text DiaRedCount;
    public Text DiaBlueCount;
    public Text DiaGreenCount;
    public Text jackPot_5;
    public Text jackPot_6;
    public Text jackPot_7;
    public Text jackPot_8;
    public Text jackPot_9;
    public Text FreeSpinText;

    private int coin1_count = 0;
    private int coin2_count = 0;
    private int coin3_count = 0;

    private int dia_start = 0;

    public InputField InputMoney;

    [SerializeField]
    private Image FreeGameImage;

    public int diaBlueCount = 0;
    public int diaRedCount = 0;
    public int diaGreenCount = 0;

    public static ulong jackGold_5;
    public static ulong jackGold_6;
    public static ulong jackGold_7;
    public static ulong jackGold_8;
    public static ulong jackGold_9;
 

    public bool CanSpin = true;
    [SerializeField]
    protected Row[] rows;
    [SerializeField]
    private Transform handle;//스핀,스탑 버튼

    private ulong prizeValue = 0;//실제 점수값

    public static ulong goldValue = 1000000;//초기 금액
    public ulong bettingGold;
    private bool resultsChecked = false;
    #endregion

    #region Update
    void Update()
    {
        FreeGameImage.enabled = false;
        //강제 초기화-->따로 함수로 빼서 할것
        jackPot_5.text = Convert.ToString(GetThousandCommaText(bettingGold * 10));
        jackPot_6.text = Convert.ToString(GetThousandCommaText(bettingGold * 20));
        jackPot_7.text = Convert.ToString(GetThousandCommaText(bettingGold * 50));
        jackPot_8.text = Convert.ToString(GetThousandCommaText(bettingGold * 100));
        jackPot_9.text = Convert.ToString(GetThousandCommaText(bettingGold * 500));

        //슬롯의 릴이 돌고 점수가 나오는 부분을 컨트롤 하는 부분
        if (Input.GetKeyDown(KeyCode.Return))//엔터 입력
        {
            string tmp = InputMoney.text;//배팅금액 입력
            //배팅금액은 엔터를 쳤을때에만 결정이 되기 때문에
            bettingGold = Convert.ToUInt32(tmp);//string -> int 
            //가지고 있는 금액보다 더 많은 금액을 배팅했을때 배팅이 안되는 부분을 구현
            if (bettingGold > goldValue)
            {
                GameObject.Find("Betting").transform.Find("BettingText").gameObject.SetActive(false);
                GameObject.Find("Error").transform.Find("ErrorText").gameObject.SetActive(true);
                errorText.text = "Not enough money!";
                goldText.text = "Gold:" + GetThousandCommaText(goldValue);
                CanSpin = false;//더 이상 스핀을 돌릴수 없다는 플래그
            }
            else
            {
                GameObject.Find("Betting").transform.Find("BettingText").gameObject.SetActive(true);
                GameObject.Find("Error").transform.Find("ErrorText").gameObject.SetActive(false);
                bettingText.text = "Betting gold is" + " " + GetThousandCommaText(bettingGold) + "!";
                CanSpin = true;

                jackPot_5.text = Convert.ToString(GetThousandCommaText(bettingGold * 10));
                jackPot_6.text = Convert.ToString(GetThousandCommaText(bettingGold * 20));
                jackPot_7.text = Convert.ToString(GetThousandCommaText(bettingGold * 50));
                jackPot_8.text = Convert.ToString(GetThousandCommaText(bettingGold * 100));
                jackPot_9.text = Convert.ToString(GetThousandCommaText(bettingGold * 500));

                jackGold_5 = bettingGold * 10;
                jackGold_6 = bettingGold * 20;
                jackGold_7 = bettingGold * 50;
                jackGold_8 = bettingGold * 100;
                jackGold_9 = bettingGold * 500;
            }
        }

        //5개의 릴중에 하나라도 돌고 있으면 점수 안나옴
        else if (!rows[0].rowStopped || !rows[1].rowStopped || !rows[2].rowStopped || !rows[3].rowStopped || !rows[4].rowStopped)
        {
            prizeValue = 0;
            prizeText.enabled = false;//릴이 돌고 있는 동안에는 prizeText가 안보임
            goldText.enabled = true;
            goldText.text = "Gold:" + GetThousandCommaText(goldValue);
            GameObject.Find("PayLine").GetComponent<PayLine>().Animation_shutdown_trigger();
            resultsChecked = false;
        }

        //모든슬롯의 릴이 멈춰있고 결과 체크값이 거짓일때(아직 페이라인 계산을 하지 않았으므로 결과 체크값이 거짓)
        //맨 처음 시작 되는곳
        else if (rows[0].rowStopped && rows[1].rowStopped && rows[2].rowStopped && rows[3].rowStopped && rows[4].rowStopped && !resultsChecked)
        {
            //왜 잭팟이 스크립트상 초기화가 안되는지 모르겠음
            //단순 잭팟금액 디스플레이
            

            CheckResults();//점수 계산하는 부분으로 빠짐

            //스핀을 돌리고난 후 상금과 현재 가진 골드를 보여주는 부분
            if (goldValue == 0)
            {
                goldText.text = "Gold:" + 0;
            }
            else goldText.text = "Gold:" + GetThousandCommaText(goldValue);

            //============================================================================================

            if (prizeValue != 0)
            {
                GameObject.Find("GameControl").transform.Find("spin").gameObject.SetActive(false);
                GameObject.Find("GameControl").transform.Find("stop").gameObject.SetActive(false);
                GameObject.Find("GameControl").transform.Find("collect").gameObject.SetActive(true);
                //prizeValue = 0;
            }

            else if (prizeValue == 0)
            {
                GameObject.Find("GameControl").transform.Find("spin").gameObject.SetActive(true);
                GameObject.Find("GameControl").transform.Find("stop").gameObject.SetActive(false);
                GameObject.Find("GameControl").transform.Find("collect").gameObject.SetActive(false);
            }

            //다이아 몬드 카운팅하여 텍스팅 애니메이션은 따로처리
            DiaRedCount.text = Convert.ToString(diaRedCount);
            DiaBlueCount.text = Convert.ToString(diaBlueCount);
            DiaGreenCount.text = Convert.ToString(diaGreenCount);
        }
    }//Update(1초에 수십번 실행된다고 생각해야함)
    #endregion

    #region 돈을 세자리수씩 끊어서 표시
    public string GetThousandCommaText(ulong data)
    {
        return string.Format("{0:#,###}", data);
    }
    #endregion

    #region 회전 중지
    private IEnumerator Rotate_Stop()
    {
        //심볼을 결정짓지 않고 y좌표만 정해줌
        for (int i = 0; i < 5; i++)
        {
            if (i == 1 || i == 2 || i == 3)
            {
                //각 심볼들을 결정짓는 부분
                float Y = GameObject.Find($"Row_{i}").transform.localPosition.y;
                if (Y >= -2.2f && Y <= -0.5f)
                {
                    //코인 넣는 부분은 확률을 집어넣어줘야함
                    if (coin1_count == 50)
                    {
                        rows[i].transform.position = new Vector3(rows[i].transform.position.x, -0.5f, 0);
                        rows[i].row_stoppedSlot3 = (int)Symbols.coin1;
                        rows[i].row_stoppedSlot2 = (int)Symbols.coin2;
                        rows[i].row_stoppedSlot1 = (int)Symbols.coin3;
                        coin1_count = 0;
                    }
                    else if (coin1_count < 50)
                    {
                        float[] respawn = { -5.6f, -7.3f, -9f, -10.7f, -12.4f, -14.1f, -15.8f, -17.5f, -19.2f };
                        int ran = UnityEngine.Random.Range(0, 9);
                        GameObject.Find($"Row_{i}").transform.localPosition = new Vector3(GameObject.Find($"Row_{i}").transform.localPosition.x, respawn[ran], 0);

                        if (Y == -5.6f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.pig_red;
                            rows[i].row_stoppedSlot2 = (int)Symbols.pig_blue;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_green;
                        }

                        else if (Y == -7.3f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.A;
                            rows[i].row_stoppedSlot2 = (int)Symbols.pig_red;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_blue;
                        }

                        else if (Y == -9.0f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.K;
                            rows[i].row_stoppedSlot2 = (int)Symbols.A;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_red;
                        }

                        else if (Y == -10.7f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.Q;
                            rows[i].row_stoppedSlot2 = (int)Symbols.K;
                            rows[i].row_stoppedSlot1 = (int)Symbols.A;
                        }

                        else if (Y == -12.4f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.J;
                            rows[i].row_stoppedSlot2 = (int)Symbols.Q;
                            rows[i].row_stoppedSlot1 = (int)Symbols.K;
                        }

                        else if (Y == -14.1f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.del;
                            rows[i].row_stoppedSlot2 = (int)Symbols.J;
                            rows[i].row_stoppedSlot1 = (int)Symbols.Q;
                        }

                        else if (Y == -15.8f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.box;
                            rows[i].row_stoppedSlot2 = (int)Symbols.del;
                            rows[i].row_stoppedSlot1 = (int)Symbols.J;
                        }

                        else if (Y == -17.5f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.cart;
                            rows[i].row_stoppedSlot2 = (int)Symbols.box;
                            rows[i].row_stoppedSlot1 = (int)Symbols.del;
                        }

                        else if (Y == -19.2f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.bonus;
                            rows[i].row_stoppedSlot2 = (int)Symbols.cart;
                            rows[i].row_stoppedSlot1 = (int)Symbols.del;
                        }
                        coin1_count += 1;
                    }
                }

                else if (Y >= -3.9f && Y <= -2.2f)
                {
                    if (coin2_count == 30)
                    {
                        rows[i].transform.position = new Vector3(rows[i].transform.position.x, -2.2f, 0);
                        rows[i].row_stoppedSlot3 = (int)Symbols.pig_green;
                        rows[i].row_stoppedSlot2 = (int)Symbols.coin1;
                        rows[i].row_stoppedSlot1 = (int)Symbols.coin2;
                        coin2_count = 0;
                    }

                    else if (coin2_count < 30)
                    {
                        float[] respawn = { -5.6f, -7.3f, -9f, -10.7f, -12.4f, -14.1f, -15.8f, -17.5f, -19.2f };
                        int ran = UnityEngine.Random.Range(0, 9);
                        GameObject.Find($"Row_{i}").transform.localPosition = new Vector3(GameObject.Find($"Row_{i}").transform.localPosition.x, respawn[ran], 0);

                        if (Y == -5.6f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.pig_red;
                            rows[i].row_stoppedSlot2 = (int)Symbols.pig_blue;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_green;
                        }

                        else if (Y == -7.3f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.A;
                            rows[i].row_stoppedSlot2 = (int)Symbols.pig_red;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_blue;
                        }

                        else if (Y == -9.0f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.K;
                            rows[i].row_stoppedSlot2 = (int)Symbols.A;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_red;
                        }

                        else if (Y == -10.7f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.Q;
                            rows[i].row_stoppedSlot2 = (int)Symbols.K;
                            rows[i].row_stoppedSlot1 = (int)Symbols.A;
                        }

                        else if (Y == -12.4f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.J;
                            rows[i].row_stoppedSlot2 = (int)Symbols.Q;
                            rows[i].row_stoppedSlot1 = (int)Symbols.K;
                        }

                        else if (Y == -14.1f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.del;
                            rows[i].row_stoppedSlot2 = (int)Symbols.J;
                            rows[i].row_stoppedSlot1 = (int)Symbols.Q;
                        }

                        else if (Y == -15.8f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.box;
                            rows[i].row_stoppedSlot2 = (int)Symbols.del;
                            rows[i].row_stoppedSlot1 = (int)Symbols.J;
                        }

                        else if (Y == -17.5f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.cart;
                            rows[i].row_stoppedSlot2 = (int)Symbols.box;
                            rows[i].row_stoppedSlot1 = (int)Symbols.del;
                        }

                        else if (Y == -19.2f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.bonus;
                            rows[i].row_stoppedSlot2 = (int)Symbols.cart;
                            rows[i].row_stoppedSlot1 = (int)Symbols.del;
                        }
                        coin2_count += 1;
                    }
                }

                else if (Y >= -5.6f && Y <= -3.9f)
                {
                    if (coin3_count == 10)
                    {
                        rows[i].transform.position = new Vector3(rows[i].transform.position.x, -3.9f, 0);
                        rows[i].row_stoppedSlot3 = (int)Symbols.pig_blue;
                        rows[i].row_stoppedSlot2 = (int)Symbols.pig_green;
                        rows[i].row_stoppedSlot1 = (int)Symbols.coin1;
                        coin3_count += 1;
                    }
                    else if (coin3_count < 10)
                    {
                        float[] respawn = { -5.6f, -7.3f, -9f, -10.7f, -12.4f, -14.1f, -15.8f, -17.5f, -19.2f };
                        int ran = UnityEngine.Random.Range(0, 9);
                        GameObject.Find($"Row_{i}").transform.localPosition = new Vector3(GameObject.Find($"Row_{i}").transform.localPosition.x, respawn[ran], 0);

                        if (Y == -5.6f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.pig_red;
                            rows[i].row_stoppedSlot2 = (int)Symbols.pig_blue;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_green;
                        }

                        else if (Y == -7.3f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.A;
                            rows[i].row_stoppedSlot2 = (int)Symbols.pig_red;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_blue;
                        }

                        else if (Y == -9.0f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.K;
                            rows[i].row_stoppedSlot2 = (int)Symbols.A;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_red;
                        }

                        else if (Y == -10.7f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.Q;
                            rows[i].row_stoppedSlot2 = (int)Symbols.K;
                            rows[i].row_stoppedSlot1 = (int)Symbols.A;
                        }

                        else if (Y == -12.4f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.J;
                            rows[i].row_stoppedSlot2 = (int)Symbols.Q;
                            rows[i].row_stoppedSlot1 = (int)Symbols.K;
                        }

                        else if (Y == -14.1f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.del;
                            rows[i].row_stoppedSlot2 = (int)Symbols.J;
                            rows[i].row_stoppedSlot1 = (int)Symbols.Q;
                        }

                        else if (Y == -15.8f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.box;
                            rows[i].row_stoppedSlot2 = (int)Symbols.del;
                            rows[i].row_stoppedSlot1 = (int)Symbols.J;
                        }

                        else if (Y == -17.5f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.cart;
                            rows[i].row_stoppedSlot2 = (int)Symbols.box;
                            rows[i].row_stoppedSlot1 = (int)Symbols.del;
                        }

                        else if (Y == -19.2f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.bonus;
                            rows[i].row_stoppedSlot2 = (int)Symbols.cart;
                            rows[i].row_stoppedSlot1 = (int)Symbols.del;
                        }
                        coin3_count += 1;
                    }
                }

                else if (Y >= -7.3f && Y <= -5.6f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -5.6f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.pig_red;
                    rows[i].row_stoppedSlot2 = (int)Symbols.pig_blue;
                    rows[i].row_stoppedSlot1 = (int)Symbols.pig_green;
                }

                else if (Y >= -9.0f && Y <= -7.3f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -7.3f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.A;
                    rows[i].row_stoppedSlot2 = (int)Symbols.pig_red;
                    rows[i].row_stoppedSlot1 = (int)Symbols.pig_blue;
                }

                else if (Y >= -10.7f && Y <= -9.0f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -9.0f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.K;
                    rows[i].row_stoppedSlot2 = (int)Symbols.A;
                    rows[i].row_stoppedSlot1 = (int)Symbols.pig_red;
                }
                else if (Y >= -12.4f && Y <= -10.7f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -10.7f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.Q;
                    rows[i].row_stoppedSlot2 = (int)Symbols.K;
                    rows[i].row_stoppedSlot1 = (int)Symbols.A;
                }
                else if (Y >= -14.1f && Y <= -12.4f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -12.4f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.J;
                    rows[i].row_stoppedSlot2 = (int)Symbols.Q;
                    rows[i].row_stoppedSlot1 = (int)Symbols.K;
                }

                else if (Y >= -15.8f && Y <= -14.1f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -14.1f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.del;
                    rows[i].row_stoppedSlot2 = (int)Symbols.J;
                    rows[i].row_stoppedSlot1 = (int)Symbols.Q;
                }

                else if (Y >= -17.5f && Y <= -15.8f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -15.8f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.box;
                    rows[i].row_stoppedSlot2 = (int)Symbols.del;
                    rows[i].row_stoppedSlot1 = (int)Symbols.J;
                }

                else if (Y >= -19.2f && Y <= -17.5f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -17.5f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.cart;
                    rows[i].row_stoppedSlot2 = (int)Symbols.box;
                    rows[i].row_stoppedSlot1 = (int)Symbols.del;
                }

                else if (Y >= -20.9f && Y <= -19.2f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -19.2f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.bonus;
                    rows[i].row_stoppedSlot2 = (int)Symbols.cart;
                    rows[i].row_stoppedSlot1 = (int)Symbols.box;
                }

                else if (Y <= -20.9f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -20.9f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.wild;
                    rows[i].row_stoppedSlot2 = (int)Symbols.bonus;
                    rows[i].row_stoppedSlot1 = (int)Symbols.cart;
                }

                rows[i].rowStopped = true;
            }

            else if(i==0 || i == 4)
            {
                float Y = GameObject.Find($"Row_{i}").transform.localPosition.y;
                if (Y >= -2.2f && Y <= -0.5f)
                {
                    //코인 넣는 부분은 확률을 집어넣어줘야함
                    if (coin1_count == 50)
                    {
                        rows[i].transform.position = new Vector3(rows[i].transform.position.x, -0.5f, 0);
                        rows[i].row_stoppedSlot3 = (int)Symbols.coin1;
                        rows[i].row_stoppedSlot2 = (int)Symbols.coin2;
                        rows[i].row_stoppedSlot1 = (int)Symbols.coin3;
                        coin1_count = 0;
                    }
                    else if (coin1_count < 50)
                    {
                        float[] respawn = { -5.6f, -7.3f, -9f, -10.7f, -12.4f, -14.1f, -15.8f, -17.5f, -19.2f };
                        int ran = UnityEngine.Random.Range(0, 9);
                        GameObject.Find($"Row_{i}").transform.localPosition = new Vector3(GameObject.Find($"Row_{i}").transform.localPosition.x, respawn[ran], 0);

                        if (Y == -5.6f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.pig_red;
                            rows[i].row_stoppedSlot2 = (int)Symbols.pig_blue;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_green;
                        }

                        else if (Y == -7.3f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.A;
                            rows[i].row_stoppedSlot2 = (int)Symbols.pig_red;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_blue;
                        }

                        else if (Y == -9.0f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.K;
                            rows[i].row_stoppedSlot2 = (int)Symbols.A;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_red;
                        }

                        else if (Y == -10.7f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.Q;
                            rows[i].row_stoppedSlot2 = (int)Symbols.K;
                            rows[i].row_stoppedSlot1 = (int)Symbols.A;
                        }

                        else if (Y == -12.4f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.J;
                            rows[i].row_stoppedSlot2 = (int)Symbols.Q;
                            rows[i].row_stoppedSlot1 = (int)Symbols.K;
                        }

                        else if (Y == -14.1f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.del;
                            rows[i].row_stoppedSlot2 = (int)Symbols.J;
                            rows[i].row_stoppedSlot1 = (int)Symbols.Q;
                        }

                        else if (Y == -15.8f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.box;
                            rows[i].row_stoppedSlot2 = (int)Symbols.del;
                            rows[i].row_stoppedSlot1 = (int)Symbols.J;
                        }

                        else if (Y == -17.5f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.cart;
                            rows[i].row_stoppedSlot2 = (int)Symbols.box;
                            rows[i].row_stoppedSlot1 = (int)Symbols.del;
                        }

                        else if (Y == -19.2f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.wild;
                            rows[i].row_stoppedSlot2 = (int)Symbols.cart;
                            rows[i].row_stoppedSlot1 = (int)Symbols.box;
                        }
                        coin1_count += 1;
                    }
                }

                else if (Y >= -3.9f && Y <= -2.2f)
                {
                    if (coin2_count == 10)
                    {
                        rows[i].transform.position = new Vector3(rows[i].transform.position.x, -2.2f, 0);
                        rows[i].row_stoppedSlot3 = (int)Symbols.pig_green;
                        rows[i].row_stoppedSlot2 = (int)Symbols.coin1;
                        rows[i].row_stoppedSlot1 = (int)Symbols.coin2;
                        coin2_count = 0;
                    }

                    else if (coin2_count < 10)
                    {
                        float[] respawn = { -5.6f, -7.3f, -9f, -10.7f, -12.4f, -14.1f, -15.8f, -17.5f, -19.2f };
                        int ran = UnityEngine.Random.Range(0, 9);
                        GameObject.Find($"Row_{i}").transform.localPosition = new Vector3(GameObject.Find($"Row_{i}").transform.localPosition.x, respawn[ran], 0);

                        if (Y == -5.6f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.pig_red;
                            rows[i].row_stoppedSlot2 = (int)Symbols.pig_blue;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_green;
                        }

                        else if (Y == -7.3f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.A;
                            rows[i].row_stoppedSlot2 = (int)Symbols.pig_red;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_blue;
                        }

                        else if (Y == -9.0f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.K;
                            rows[i].row_stoppedSlot2 = (int)Symbols.A;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_red;
                        }

                        else if (Y == -10.7f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.Q;
                            rows[i].row_stoppedSlot2 = (int)Symbols.K;
                            rows[i].row_stoppedSlot1 = (int)Symbols.A;
                        }

                        else if (Y == -12.4f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.J;
                            rows[i].row_stoppedSlot2 = (int)Symbols.Q;
                            rows[i].row_stoppedSlot1 = (int)Symbols.K;
                        }

                        else if (Y == -14.1f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.del;
                            rows[i].row_stoppedSlot2 = (int)Symbols.J;
                            rows[i].row_stoppedSlot1 = (int)Symbols.Q;
                        }

                        else if (Y == -15.8f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.box;
                            rows[i].row_stoppedSlot2 = (int)Symbols.del;
                            rows[i].row_stoppedSlot1 = (int)Symbols.J;
                        }

                        else if (Y == -17.5f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.cart;
                            rows[i].row_stoppedSlot2 = (int)Symbols.box;
                            rows[i].row_stoppedSlot1 = (int)Symbols.del;
                        }

                        else if (Y == -19.2f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.wild;
                            rows[i].row_stoppedSlot2 = (int)Symbols.cart;
                            rows[i].row_stoppedSlot1 = (int)Symbols.box;
                        }
                        coin2_count += 1;
                    }
                }

                else if (Y >= -5.6f && Y <= -3.9f)
                {
                    if (coin3_count == 5)
                    {
                        rows[i].transform.position = new Vector3(rows[i].transform.position.x, -3.9f, 0);
                        rows[i].row_stoppedSlot3 = (int)Symbols.pig_blue;
                        rows[i].row_stoppedSlot2 = (int)Symbols.pig_green;
                        rows[i].row_stoppedSlot1 = (int)Symbols.coin1;
                        coin3_count += 1;
                    }
                    else if (coin3_count < 5)
                    {
                        float[] respawn = { -5.6f, -7.3f, -9f, -10.7f, -12.4f, -14.1f, -15.8f, -17.5f, -19.2f };
                        int ran = UnityEngine.Random.Range(0, 9);
                        GameObject.Find($"Row_{i}").transform.localPosition = new Vector3(GameObject.Find($"Row_{i}").transform.localPosition.x, respawn[ran], 0);

                        if (Y == -5.6f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.pig_red;
                            rows[i].row_stoppedSlot2 = (int)Symbols.pig_blue;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_green;
                        }

                        else if (Y == -7.3f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.A;
                            rows[i].row_stoppedSlot2 = (int)Symbols.pig_red;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_blue;
                        }

                        else if (Y == -9.0f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.K;
                            rows[i].row_stoppedSlot2 = (int)Symbols.A;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_red;
                        }

                        else if (Y == -10.7f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.Q;
                            rows[i].row_stoppedSlot2 = (int)Symbols.K;
                            rows[i].row_stoppedSlot1 = (int)Symbols.A;
                        }

                        else if (Y == -12.4f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.J;
                            rows[i].row_stoppedSlot2 = (int)Symbols.Q;
                            rows[i].row_stoppedSlot1 = (int)Symbols.K;
                        }

                        else if (Y == -14.1f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.del;
                            rows[i].row_stoppedSlot2 = (int)Symbols.J;
                            rows[i].row_stoppedSlot1 = (int)Symbols.Q;
                        }

                        else if (Y == -15.8f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.box;
                            rows[i].row_stoppedSlot2 = (int)Symbols.del;
                            rows[i].row_stoppedSlot1 = (int)Symbols.J;
                        }

                        else if (Y == -17.5f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.cart;
                            rows[i].row_stoppedSlot2 = (int)Symbols.box;
                            rows[i].row_stoppedSlot1 = (int)Symbols.del;
                        }

                        else if (Y == -19.2f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.wild;
                            rows[i].row_stoppedSlot2 = (int)Symbols.cart;
                            rows[i].row_stoppedSlot1 = (int)Symbols.box;
                        }
                        coin3_count += 1;
                    }
                }

                else if (Y >= -7.3f && Y <= -5.6f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -5.6f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.pig_red;
                    rows[i].row_stoppedSlot2 = (int)Symbols.pig_blue;
                    rows[i].row_stoppedSlot1 = (int)Symbols.pig_green;
                }

                else if (Y >= -9.0f && Y <= -7.3f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -7.3f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.A;
                    rows[i].row_stoppedSlot2 = (int)Symbols.pig_red;
                    rows[i].row_stoppedSlot1 = (int)Symbols.pig_blue;
                }

                else if (Y >= -10.7f && Y <= -9.0f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -9.0f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.K;
                    rows[i].row_stoppedSlot2 = (int)Symbols.A;
                    rows[i].row_stoppedSlot1 = (int)Symbols.pig_red;
                }
                else if (Y >= -12.4f && Y <= -10.7f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -10.7f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.Q;
                    rows[i].row_stoppedSlot2 = (int)Symbols.K;
                    rows[i].row_stoppedSlot1 = (int)Symbols.A;
                }
                else if (Y >= -14.1f && Y <= -12.4f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -12.4f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.J;
                    rows[i].row_stoppedSlot2 = (int)Symbols.Q;
                    rows[i].row_stoppedSlot1 = (int)Symbols.K;
                }

                else if (Y >= -15.8f && Y <= -14.1f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -14.1f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.del;
                    rows[i].row_stoppedSlot2 = (int)Symbols.J;
                    rows[i].row_stoppedSlot1 = (int)Symbols.Q;
                }

                else if (Y >= -17.5f && Y <= -15.8f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -15.8f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.box;
                    rows[i].row_stoppedSlot2 = (int)Symbols.del;
                    rows[i].row_stoppedSlot1 = (int)Symbols.J;
                }

                else if (Y >= -19.2f && Y <= -17.5f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -17.5f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.cart;
                    rows[i].row_stoppedSlot2 = (int)Symbols.box;
                    rows[i].row_stoppedSlot1 = (int)Symbols.del;
                }

                else if (Y <= -19.2f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -19.2f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.wild;
                    rows[i].row_stoppedSlot2 = (int)Symbols.cart;
                    rows[i].row_stoppedSlot1 = (int)Symbols.box;
                }
                rows[i].rowStopped = true;
            }
        }
        yield return null;
    }//Rotate_Stop
    #endregion

    #region 회전
    private IEnumerator Rotate()
    {
        //5개의릴을 순차적으로 돌리기 위해서
        for (int i = 0; i < 5; i++)
        {
            rows[i].rowStopped = false;//i+1 번째 릴이 돌고 있음을 의미함
            if (i == 1 || i == 2 || i == 3)
            {
                float Y = GameObject.Find($"Row_{i}").transform.localPosition.y;

                //j는 단순히 카운트횟수만 채우는 변수 
                for (int j = 0; j < 50; j++)
                {
                    if (GameObject.Find($"Row_{i}").transform.localPosition.y <= -20.9f)
                    {
                        GameObject.Find($"Row_{i}").transform.localPosition = new Vector3(GameObject.Find($"Row_{i}").transform.localPosition.x, -5.6f, 0);
                    }
                    GameObject.Find($"Row_{i}").transform.localPosition = new Vector3(GameObject.Find($"Row_{i}").transform.localPosition.x, GameObject.Find($"Row_{i}").transform.localPosition.y - 0.4f, 0);

                    //OnMouseDown에서 stopFlag값이 바뀜
                    if (rows[i].stopFlag == true)
                    {
                        rows[i].stopFlag = false;
                        break;//회전만 빠져나옴
                    }
                    yield return null;
                }//회전
               
                //각 심볼들을 결정짓는 부분
                Y = GameObject.Find($"Row_{i}").transform.localPosition.y;
                if (Y >= -2.2f && Y <= -0.5f)
                {
                    //코인 넣는 부분은 확률을 집어넣어줘야함
                    if (coin1_count == 50)
                    {
                        rows[i].transform.position = new Vector3(rows[i].transform.position.x, -0.5f, 0);
                        rows[i].row_stoppedSlot3 = (int)Symbols.coin1;
                        rows[i].row_stoppedSlot2 = (int)Symbols.coin2;
                        rows[i].row_stoppedSlot1 = (int)Symbols.coin3;
                        coin1_count = 0;
                    }
                    else if (coin1_count < 50)
                    {
                        float[] respawn = { -5.6f, -7.3f, -9f, -10.7f, -12.4f, -14.1f, -15.8f, -17.5f, -19.2f };
                        int ran = UnityEngine.Random.Range(0, 9);
                        GameObject.Find($"Row_{i}").transform.localPosition = new Vector3(GameObject.Find($"Row_{i}").transform.localPosition.x, respawn[ran], 0);

                        if (Y == -5.6f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.pig_red;
                            rows[i].row_stoppedSlot2 = (int)Symbols.pig_blue;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_green;
                        }

                        else if (Y == -7.3f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.A;
                            rows[i].row_stoppedSlot2 = (int)Symbols.pig_red;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_blue;
                        }

                        else if (Y == -9.0f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.K;
                            rows[i].row_stoppedSlot2 = (int)Symbols.A;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_red;
                        }

                        else if (Y == -10.7f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.Q;
                            rows[i].row_stoppedSlot2 = (int)Symbols.K;
                            rows[i].row_stoppedSlot1 = (int)Symbols.A;
                        }

                        else if (Y == -12.4f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.J;
                            rows[i].row_stoppedSlot2 = (int)Symbols.Q;
                            rows[i].row_stoppedSlot1 = (int)Symbols.K;
                        }

                        else if (Y == -14.1f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.del;
                            rows[i].row_stoppedSlot2 = (int)Symbols.J;
                            rows[i].row_stoppedSlot1 = (int)Symbols.Q;
                        }

                        else if (Y == -15.8f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.box;
                            rows[i].row_stoppedSlot2 = (int)Symbols.del;
                            rows[i].row_stoppedSlot1 = (int)Symbols.J;
                        }

                        else if (Y == -17.5f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.cart;
                            rows[i].row_stoppedSlot2 = (int)Symbols.box;
                            rows[i].row_stoppedSlot1 = (int)Symbols.del;
                        }

                        else if (Y == -19.2f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.bonus;
                            rows[i].row_stoppedSlot2 = (int)Symbols.cart;
                            rows[i].row_stoppedSlot1 = (int)Symbols.del;
                        }
                        coin1_count += 1;
                    }
                }

                else if (Y >= -3.9f && Y <= -2.2f)
                {
                    if (coin2_count == 30)
                    {
                        rows[i].transform.position = new Vector3(rows[i].transform.position.x, -2.2f, 0);
                        rows[i].row_stoppedSlot3 = (int)Symbols.pig_green;
                        rows[i].row_stoppedSlot2 = (int)Symbols.coin1;
                        rows[i].row_stoppedSlot1 = (int)Symbols.coin2;
                        coin2_count = 0;
                    }

                    else if (coin2_count < 30)
                    {
                        float[] respawn = { -5.6f, -7.3f, -9f, -10.7f, -12.4f, -14.1f, -15.8f, -17.5f, -19.2f };
                        int ran = UnityEngine.Random.Range(0, 9);
                        GameObject.Find($"Row_{i}").transform.localPosition = new Vector3(GameObject.Find($"Row_{i}").transform.localPosition.x, respawn[ran], 0);

                        if (Y == -5.6f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.pig_red;
                            rows[i].row_stoppedSlot2 = (int)Symbols.pig_blue;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_green;
                        }

                        else if (Y == -7.3f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.A;
                            rows[i].row_stoppedSlot2 = (int)Symbols.pig_red;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_blue;
                        }

                        else if (Y == -9.0f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.K;
                            rows[i].row_stoppedSlot2 = (int)Symbols.A;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_red;
                        }

                        else if (Y == -10.7f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.Q;
                            rows[i].row_stoppedSlot2 = (int)Symbols.K;
                            rows[i].row_stoppedSlot1 = (int)Symbols.A;
                        }

                        else if (Y == -12.4f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.J;
                            rows[i].row_stoppedSlot2 = (int)Symbols.Q;
                            rows[i].row_stoppedSlot1 = (int)Symbols.K;
                        }

                        else if (Y == -14.1f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.del;
                            rows[i].row_stoppedSlot2 = (int)Symbols.J;
                            rows[i].row_stoppedSlot1 = (int)Symbols.Q;
                        }

                        else if (Y == -15.8f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.box;
                            rows[i].row_stoppedSlot2 = (int)Symbols.del;
                            rows[i].row_stoppedSlot1 = (int)Symbols.J;
                        }

                        else if (Y == -17.5f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.cart;
                            rows[i].row_stoppedSlot2 = (int)Symbols.box;
                            rows[i].row_stoppedSlot1 = (int)Symbols.del;
                        }

                        else if (Y == -19.2f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.bonus;
                            rows[i].row_stoppedSlot2 = (int)Symbols.cart;
                            rows[i].row_stoppedSlot1 = (int)Symbols.del;
                        }
                        coin2_count += 1;
                    }
                }

                else if (Y >= -5.6f && Y <= -3.9f)
                {
                    if (coin3_count == 10)
                    {
                        rows[i].transform.position = new Vector3(rows[i].transform.position.x, -3.9f, 0);
                        rows[i].row_stoppedSlot3 = (int)Symbols.pig_blue;
                        rows[i].row_stoppedSlot2 = (int)Symbols.pig_green;
                        rows[i].row_stoppedSlot1 = (int)Symbols.coin1;
                        coin3_count += 1;
                    }
                    else if (coin3_count < 10)
                    {
                        float[] respawn = { -5.6f, -7.3f, -9f, -10.7f, -12.4f, -14.1f, -15.8f, -17.5f, -19.2f };
                        int ran = UnityEngine.Random.Range(0, 9);
                        GameObject.Find($"Row_{i}").transform.localPosition = new Vector3(GameObject.Find($"Row_{i}").transform.localPosition.x, respawn[ran], 0);

                        if (Y == -5.6f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.pig_red;
                            rows[i].row_stoppedSlot2 = (int)Symbols.pig_blue;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_green;
                        }

                        else if (Y == -7.3f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.A;
                            rows[i].row_stoppedSlot2 = (int)Symbols.pig_red;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_blue;
                        }

                        else if (Y == -9.0f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.K;
                            rows[i].row_stoppedSlot2 = (int)Symbols.A;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_red;
                        }

                        else if (Y == -10.7f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.Q;
                            rows[i].row_stoppedSlot2 = (int)Symbols.K;
                            rows[i].row_stoppedSlot1 = (int)Symbols.A;
                        }

                        else if (Y == -12.4f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.J;
                            rows[i].row_stoppedSlot2 = (int)Symbols.Q;
                            rows[i].row_stoppedSlot1 = (int)Symbols.K;
                        }

                        else if (Y == -14.1f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.del;
                            rows[i].row_stoppedSlot2 = (int)Symbols.J;
                            rows[i].row_stoppedSlot1 = (int)Symbols.Q;
                        }

                        else if (Y == -15.8f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.box;
                            rows[i].row_stoppedSlot2 = (int)Symbols.del;
                            rows[i].row_stoppedSlot1 = (int)Symbols.J;
                        }

                        else if (Y == -17.5f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.cart;
                            rows[i].row_stoppedSlot2 = (int)Symbols.box;
                            rows[i].row_stoppedSlot1 = (int)Symbols.del;
                        }

                        else if (Y == -19.2f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.bonus;
                            rows[i].row_stoppedSlot2 = (int)Symbols.cart;
                            rows[i].row_stoppedSlot1 = (int)Symbols.del;
                        }
                        coin3_count += 1;
                    }
                }

                else if (Y >= -7.3f && Y <= -5.6f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -5.6f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.pig_red;
                    rows[i].row_stoppedSlot2 = (int)Symbols.pig_blue;
                    rows[i].row_stoppedSlot1 = (int)Symbols.pig_green;
                }

                else if (Y >= -9.0f && Y <= -7.3f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -7.3f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.A;
                    rows[i].row_stoppedSlot2 = (int)Symbols.pig_red;
                    rows[i].row_stoppedSlot1 = (int)Symbols.pig_blue;
                }

                else if (Y >= -10.7f && Y <= -9.0f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -9.0f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.K;
                    rows[i].row_stoppedSlot2 = (int)Symbols.A;
                    rows[i].row_stoppedSlot1 = (int)Symbols.pig_red;
                }
                else if (Y >= -12.4f && Y <= -10.7f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -10.7f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.Q;
                    rows[i].row_stoppedSlot2 = (int)Symbols.K;
                    rows[i].row_stoppedSlot1 = (int)Symbols.A;
                }
                else if (Y >= -14.1f && Y <= -12.4f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -12.4f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.J;
                    rows[i].row_stoppedSlot2 = (int)Symbols.Q;
                    rows[i].row_stoppedSlot1 = (int)Symbols.K;
                }

                else if (Y >= -15.8f && Y <= -14.1f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -14.1f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.del;
                    rows[i].row_stoppedSlot2 = (int)Symbols.J;
                    rows[i].row_stoppedSlot1 = (int)Symbols.Q;
                }

                else if (Y >= -17.5f && Y <= -15.8f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -15.8f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.box;
                    rows[i].row_stoppedSlot2 = (int)Symbols.del;
                    rows[i].row_stoppedSlot1 = (int)Symbols.J;
                }

                else if (Y >= -19.2f && Y <= -17.5f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -17.5f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.cart;
                    rows[i].row_stoppedSlot2 = (int)Symbols.box;
                    rows[i].row_stoppedSlot1 = (int)Symbols.del;
                }

                else if (Y >= -20.9f && Y <= -19.2f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -19.2f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.bonus;
                    rows[i].row_stoppedSlot2 = (int)Symbols.cart;
                    rows[i].row_stoppedSlot1 = (int)Symbols.box;
                }

                else if (Y <= -20.9f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -20.9f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.wild;
                    rows[i].row_stoppedSlot2 = (int)Symbols.bonus;
                    rows[i].row_stoppedSlot1 = (int)Symbols.cart;
                }

                rows[i].rowStopped = true;
            }//if (2번째 3번째 4번째 릴 처리)

            //첫번째와 5번째 릴을 돌리는 부분
            else if (i == 0 || i == 4)
            {
                float Y = GameObject.Find($"Row_{i}").transform.localPosition.y;

                //회전중               
                for (int j = 0; j < 50; j++)
                {
                    if (GameObject.Find($"Row_{i}").transform.localPosition.y <= -19.2f)
                    {
                        GameObject.Find($"Row_{i}").transform.localPosition = new Vector3(GameObject.Find($"Row_{i}").transform.localPosition.x, -5.6f, 0);
                    }
                    GameObject.Find($"Row_{i}").transform.localPosition = new Vector3(GameObject.Find($"Row_{i}").transform.localPosition.x, GameObject.Find($"Row_{i}").transform.localPosition.y - 0.4f, 0);

                    if (rows[i].stopFlag == true)
                    {
                        rows[i].stopFlag = false;
                        break;
                    }
                    yield return null;
                }

                Y = GameObject.Find($"Row_{i}").transform.localPosition.y;
                if (Y >= -2.2f && Y <= -0.5f)
                {
                    //코인 넣는 부분은 확률을 집어넣어줘야함
                    if (coin1_count == 50)
                    {
                        rows[i].transform.position = new Vector3(rows[i].transform.position.x, -0.5f, 0);
                        rows[i].row_stoppedSlot3 = (int)Symbols.coin1;
                        rows[i].row_stoppedSlot2 = (int)Symbols.coin2;
                        rows[i].row_stoppedSlot1 = (int)Symbols.coin3;
                        coin1_count = 0;
                    }
                    else if (coin1_count < 50)
                    {
                        float[] respawn = { -5.6f, -7.3f, -9f, -10.7f, -12.4f, -14.1f, -15.8f, -17.5f, -19.2f };
                        int ran = UnityEngine.Random.Range(0, 9);
                        GameObject.Find($"Row_{i}").transform.localPosition = new Vector3(GameObject.Find($"Row_{i}").transform.localPosition.x, respawn[ran], 0);

                        if (Y == -5.6f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.pig_red;
                            rows[i].row_stoppedSlot2 = (int)Symbols.pig_blue;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_green;
                        }

                        else if (Y == -7.3f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.A;
                            rows[i].row_stoppedSlot2 = (int)Symbols.pig_red;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_blue;
                        }

                        else if (Y == -9.0f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.K;
                            rows[i].row_stoppedSlot2 = (int)Symbols.A;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_red;
                        }

                        else if (Y == -10.7f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.Q;
                            rows[i].row_stoppedSlot2 = (int)Symbols.K;
                            rows[i].row_stoppedSlot1 = (int)Symbols.A;
                        }

                        else if (Y == -12.4f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.J;
                            rows[i].row_stoppedSlot2 = (int)Symbols.Q;
                            rows[i].row_stoppedSlot1 = (int)Symbols.K;
                        }

                        else if (Y == -14.1f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.del;
                            rows[i].row_stoppedSlot2 = (int)Symbols.J;
                            rows[i].row_stoppedSlot1 = (int)Symbols.Q;
                        }

                        else if (Y == -15.8f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.box;
                            rows[i].row_stoppedSlot2 = (int)Symbols.del;
                            rows[i].row_stoppedSlot1 = (int)Symbols.J;
                        }

                        else if (Y == -17.5f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.cart;
                            rows[i].row_stoppedSlot2 = (int)Symbols.box;
                            rows[i].row_stoppedSlot1 = (int)Symbols.del;
                        }

                        else if (Y == -19.2f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.wild;
                            rows[i].row_stoppedSlot2 = (int)Symbols.cart;
                            rows[i].row_stoppedSlot1 = (int)Symbols.box;
                        }
                        coin1_count += 1;
                    }
                }

                else if (Y >= -3.9f && Y <= -2.2f)
                {
                    if (coin2_count == 10)
                    {
                        rows[i].transform.position = new Vector3(rows[i].transform.position.x, -2.2f, 0);
                        rows[i].row_stoppedSlot3 = (int)Symbols.pig_green;
                        rows[i].row_stoppedSlot2 = (int)Symbols.coin1;
                        rows[i].row_stoppedSlot1 = (int)Symbols.coin2;
                        coin2_count = 0;
                    }

                    else if (coin2_count < 10)
                    {
                        float[] respawn = { -5.6f, -7.3f, -9f, -10.7f, -12.4f, -14.1f, -15.8f, -17.5f, -19.2f };
                        int ran = UnityEngine.Random.Range(0, 9);
                        GameObject.Find($"Row_{i}").transform.localPosition = new Vector3(GameObject.Find($"Row_{i}").transform.localPosition.x, respawn[ran], 0);

                        if (Y == -5.6f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.pig_red;
                            rows[i].row_stoppedSlot2 = (int)Symbols.pig_blue;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_green;
                        }

                        else if (Y == -7.3f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.A;
                            rows[i].row_stoppedSlot2 = (int)Symbols.pig_red;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_blue;
                        }

                        else if (Y == -9.0f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.K;
                            rows[i].row_stoppedSlot2 = (int)Symbols.A;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_red;
                        }

                        else if (Y == -10.7f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.Q;
                            rows[i].row_stoppedSlot2 = (int)Symbols.K;
                            rows[i].row_stoppedSlot1 = (int)Symbols.A;
                        }

                        else if (Y == -12.4f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.J;
                            rows[i].row_stoppedSlot2 = (int)Symbols.Q;
                            rows[i].row_stoppedSlot1 = (int)Symbols.K;
                        }

                        else if (Y == -14.1f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.del;
                            rows[i].row_stoppedSlot2 = (int)Symbols.J;
                            rows[i].row_stoppedSlot1 = (int)Symbols.Q;
                        }

                        else if (Y == -15.8f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.box;
                            rows[i].row_stoppedSlot2 = (int)Symbols.del;
                            rows[i].row_stoppedSlot1 = (int)Symbols.J;
                        }

                        else if (Y == -17.5f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.cart;
                            rows[i].row_stoppedSlot2 = (int)Symbols.box;
                            rows[i].row_stoppedSlot1 = (int)Symbols.del;
                        }

                        else if (Y == -19.2f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.wild;
                            rows[i].row_stoppedSlot2 = (int)Symbols.cart;
                            rows[i].row_stoppedSlot1 = (int)Symbols.box;
                        }
                        coin2_count += 1;
                    }
                }

                else if (Y >= -5.6f && Y <= -3.9f)
                {
                    if (coin3_count == 5)
                    {
                        rows[i].transform.position = new Vector3(rows[i].transform.position.x, -3.9f, 0);
                        rows[i].row_stoppedSlot3 = (int)Symbols.pig_blue;
                        rows[i].row_stoppedSlot2 = (int)Symbols.pig_green;
                        rows[i].row_stoppedSlot1 = (int)Symbols.coin1;
                        coin3_count += 1;
                    }
                    else if (coin3_count < 5)
                    {
                        float[] respawn = { -5.6f, -7.3f, -9f, -10.7f, -12.4f, -14.1f, -15.8f, -17.5f, -19.2f };
                        int ran = UnityEngine.Random.Range(0, 9);
                        GameObject.Find($"Row_{i}").transform.localPosition = new Vector3(GameObject.Find($"Row_{i}").transform.localPosition.x, respawn[ran], 0);

                        if (Y == -5.6f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.pig_red;
                            rows[i].row_stoppedSlot2 = (int)Symbols.pig_blue;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_green;
                        }

                        else if (Y == -7.3f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.A;
                            rows[i].row_stoppedSlot2 = (int)Symbols.pig_red;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_blue;
                        }

                        else if (Y == -9.0f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.K;
                            rows[i].row_stoppedSlot2 = (int)Symbols.A;
                            rows[i].row_stoppedSlot1 = (int)Symbols.pig_red;
                        }

                        else if (Y == -10.7f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.Q;
                            rows[i].row_stoppedSlot2 = (int)Symbols.K;
                            rows[i].row_stoppedSlot1 = (int)Symbols.A;
                        }

                        else if (Y == -12.4f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.J;
                            rows[i].row_stoppedSlot2 = (int)Symbols.Q;
                            rows[i].row_stoppedSlot1 = (int)Symbols.K;
                        }

                        else if (Y == -14.1f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.del;
                            rows[i].row_stoppedSlot2 = (int)Symbols.J;
                            rows[i].row_stoppedSlot1 = (int)Symbols.Q;
                        }

                        else if (Y == -15.8f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.box;
                            rows[i].row_stoppedSlot2 = (int)Symbols.del;
                            rows[i].row_stoppedSlot1 = (int)Symbols.J;
                        }

                        else if (Y == -17.5f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.cart;
                            rows[i].row_stoppedSlot2 = (int)Symbols.box;
                            rows[i].row_stoppedSlot1 = (int)Symbols.del;
                        }

                        else if (Y == -19.2f)
                        {
                            rows[i].row_stoppedSlot3 = (int)Symbols.wild;
                            rows[i].row_stoppedSlot2 = (int)Symbols.cart;
                            rows[i].row_stoppedSlot1 = (int)Symbols.box;
                        }
                        coin3_count += 1;
                    }
                }

                else if (Y >= -7.3f && Y <= -5.6f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -5.6f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.pig_red;
                    rows[i].row_stoppedSlot2 = (int)Symbols.pig_blue;
                    rows[i].row_stoppedSlot1 = (int)Symbols.pig_green;
                }

                else if (Y >= -9.0f && Y <= -7.3f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -7.3f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.A;
                    rows[i].row_stoppedSlot2 = (int)Symbols.pig_red;
                    rows[i].row_stoppedSlot1 = (int)Symbols.pig_blue;
                }

                else if (Y >= -10.7f && Y <= -9.0f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -9.0f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.K;
                    rows[i].row_stoppedSlot2 = (int)Symbols.A;
                    rows[i].row_stoppedSlot1 = (int)Symbols.pig_red;
                }
                else if (Y >= -12.4f && Y <= -10.7f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -10.7f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.Q;
                    rows[i].row_stoppedSlot2 = (int)Symbols.K;
                    rows[i].row_stoppedSlot1 = (int)Symbols.A;
                }
                else if (Y >= -14.1f && Y <= -12.4f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -12.4f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.J;
                    rows[i].row_stoppedSlot2 = (int)Symbols.Q;
                    rows[i].row_stoppedSlot1 = (int)Symbols.K;
                }

                else if (Y >= -15.8f && Y <= -14.1f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -14.1f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.del;
                    rows[i].row_stoppedSlot2 = (int)Symbols.J;
                    rows[i].row_stoppedSlot1 = (int)Symbols.Q;
                }

                else if (Y >= -17.5f && Y <= -15.8f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -15.8f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.box;
                    rows[i].row_stoppedSlot2 = (int)Symbols.del;
                    rows[i].row_stoppedSlot1 = (int)Symbols.J;
                }

                else if (Y >= -19.2f && Y <= -17.5f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -17.5f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.cart;
                    rows[i].row_stoppedSlot2 = (int)Symbols.box;
                    rows[i].row_stoppedSlot1 = (int)Symbols.del;
                }

                else if (Y <= -19.2f)
                {
                    rows[i].transform.position = new Vector3(rows[i].transform.position.x, -19.2f, 0);
                    rows[i].row_stoppedSlot3 = (int)Symbols.wild;
                    rows[i].row_stoppedSlot2 = (int)Symbols.cart;
                    rows[i].row_stoppedSlot1 = (int)Symbols.box;
                }
                rows[i].rowStopped = true;
            }//첫번째 4번째 릴
          
        }//for
    }//Rotate
    #endregion

    #region 콜렉트
    private IEnumerator Collect()
    {
        yield return new WaitForSecondsRealtime(1);

        ulong initialMoney = goldValue;
        ulong diff = prizeValue;
        ulong targetMoney = initialMoney + diff;
        float t = 0;

        //상금을 조금이라도 탔을때 시작
        while (diff != 0)
        {
            //상금을 올려주는것은 4초를 넘기지 않음
            if (t >= 4 || goldValue >= targetMoney)
            {
                goldText.text = "Gold:" + GetThousandCommaText(targetMoney);
                goldValue = targetMoney;
                break;
            }

            float progress = t / 2;
            long diffPerTime = (long)Mathf.Round(diff * progress);
            goldValue = (ulong)initialMoney + (ulong)diffPerTime;
            //goldValue++; --> 이렇게하면 큰 수가 나올경우 시간이 너무 오래걸림
            prizeValue = 0;
            t += Time.smoothDeltaTime;
            goldText.text = "Gold:" + GetThousandCommaText(goldValue);

            GameObject.Find("GameControl").transform.Find("spin").gameObject.SetActive(true);
            GameObject.Find("GameControl").transform.Find("stop").gameObject.SetActive(false);
            GameObject.Find("GameControl").transform.Find("collect").gameObject.SetActive(false);
            yield return null;
        }//while
    }//Collect
    #endregion

    #region OnMouseDown
    private void OnMouseDown()
    {
        if (!CanSpin)
        {
            errorText.text = "Not enough money!";
            if (goldValue == 0)
            {
                goldText.text = "Gold:" + 0;
            }
            else goldText.text = "Gold:" + GetThousandCommaText(goldValue);
        }//더이상 스핀이 돌수 없을때

        //릴을 돌릴수 있을때
        else if (CanSpin)
        {
            if (rows[0].rowStopped && rows[1].rowStopped && rows[2].rowStopped && rows[3].rowStopped && rows[4].rowStopped)
            {
                if (prizeValue > 0)
                {
                    StartCoroutine("Collect");
                }

                else if (prizeValue == 0)
                {
                    if (goldValue - bettingGold >= 0)
                    {
                        if (goldValue == 0)
                        {
                            errorText.text = "Not enough money!";
                            goldText.text = "Gold:" + 0;
                            CanSpin = false;
                        }
                        else
                        {
                            goldValue -= bettingGold;
                            goldText.text = "Gold:" + GetThousandCommaText(goldValue);
                            if (CanSpin)
                            {
                                //PullHandle();//이 함수 호출이후 릴이 돌게 됨
                                for (int i = 0; i < 5; i++)
                                {
                                    rows[i].row_stoppedSlot3 = -1;
                                    rows[i].row_stoppedSlot2 = -1;
                                    rows[i].row_stoppedSlot1 = -1;
                                }

                                for(int i = 0; i < 5; i++)
                                {
                                    rows[i].stopFlag = false;
                                }
                                StartCoroutine("Rotate");
                            }
                            //릴이 돌고 있는 동안 stop버튼 표시
                            GameObject.Find("GameControl").transform.Find("spin").gameObject.SetActive(false);
                            GameObject.Find("GameControl").transform.Find("stop").gameObject.SetActive(true);
                            GameObject.Find("GameControl").transform.Find("collect").gameObject.SetActive(false);
                        }
                    }
                }
            }

            //릴이 돌고 있을때 강제 스톱 하는 부분
            else if (!rows[0].rowStopped || !rows[1].rowStopped || !rows[2].rowStopped || !rows[3].rowStopped || !rows[4].rowStopped)
            {
                //StopHandle();//여기서 stop함수가 delegate에 등록되고 stopFlag가 바뀜\
                for (int i = 0; i < 5; i++)
                {
                    //돌고 있는 릴 강제종료
                    if (rows[i].rowStopped == false)
                    {
                        rows[i].stopFlag = true;
                    }
                }
            }
        }
    }//OnMouseDown
    #endregion

    #region 슬롯머신의 결과를 맵핑하고 페이라인을 계산및 애니메이션을 처리하는 부분
    private void CheckResults()
    {
        GameObject.Find("PayLine").GetComponent<PayLine>().Mapping_symbol();
        GameObject.Find("PayLine").GetComponent<PayLine>().IsBonus();
        //prizeValue가 결정되는 부분
        #region 애니메이션셧다운 및 페이라인 계산
        GameObject.Find("PayLine").GetComponent<PayLine>().Animation_shutdown_trigger();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_1();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_2();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_3();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_4();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_5();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_6();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_7();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_8();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_9();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_10();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_11();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_12();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_13();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_14();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_15();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_16();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_17();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_18();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_19();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_20();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_21();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_22();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_23();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_24();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_25();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_26();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_27();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_28();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_29();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_30();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_31();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_32();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_33();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_34();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_35();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_36();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_37();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_38();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_39();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_40();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_41();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_42();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_43();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_44();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_45();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_46();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_47();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_48();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_49();
        prizeValue += GameObject.Find("PayLine").GetComponent<PayLine>().Payline_50();
        #endregion

        //이부분을 Payline으로 뺄것인지 아니면 GameControl클래스 안에서 처리할것인지 정해야함
        if (dia_start < 3)
        {
            dia_start += 1;
        }
        else if (dia_start == 3)
        {
            int k = UnityEngine.Random.Range(0, 3);
            if (k == 0)
            {//블루
                diaBlueCount += 1;
                //함수를 한번 거쳐서 해볼것
                StartCoroutine("Dia_blue_event");
            }
            else if (k == 1)
            {//그린
                diaGreenCount += 1;
                StartCoroutine("Dia_green_event");
            }
            else if (k == 2)
            {//레드
                diaRedCount += 1;
                StartCoroutine("Dia_red_event");
            }
            dia_start = 0;
        }

        #region 잭팟
        int jack = GameObject.Find("PayLine").GetComponent<PayLine>().Jackpot_Prize();
        if (jack == 5)
        {
            prizeValue += jackGold_5;
        }
        else if (jack == 6)
        {
            prizeValue += jackGold_6;
        }
        else if (jack == 7)
        {
            prizeValue += jackGold_7;
        }
        else if (jack == 8)
        {
            prizeValue += jackGold_8;
        }
        else if (jack == 7)
        {
            prizeValue += jackGold_9;
        }
        #endregion

        if (PayLine.Bonus_flag)
        {
            Bonus_TEST();
        }

        //최종 상금 디스플레이
        prizeText.enabled = true;
        if (prizeValue == 0)
        {
            prizeText.text = "Prize:" + 0;
        }
        else prizeText.text = "Prize:" + GetThousandCommaText(prizeValue);

        resultsChecked = true;//결과 체크값을 참으로 바꿔줌
    }//CheckResults
    #endregion

    #region 보너스
    public void Bonus_TEST()
    {
        StartCoroutine("Bonus_Event");
        StartCoroutine("Rotate");
        //goldValue -= bettingGold;
        goldValue += (prizeValue / 2);
        goldText.text = "Gold:" + GetThousandCommaText(goldValue);//이부분이 제대로 출력이 안됨
        PayLine.Bonus_flag = false;
    }//Bonus_TEST

    private IEnumerator Bonus_Event()
    {
        FreeSpinText.text = "Free Spin Game!";
        yield return new WaitForSeconds(1);
        FreeSpinText.text = "";
    }//단순 무료스핀 UI 띄우기
    #endregion

    



    #region 다이아 애니메이션
    private IEnumerator Dia_blue_event()
    {
        //현재 코루틴안에서 다이아를 띄울 심볼까지도 결정하고 있음
        int ran = UnityEngine.Random.Range(0, 3);
        int symbol_value = -1;
        if (ran == 0)
        {
            symbol_value = rows[4].row_stoppedSlot3;
        }
        else if (ran == 1)
        {
            symbol_value = rows[4].row_stoppedSlot2;
        }
        else if (ran == 2)
        {
            symbol_value = rows[4].row_stoppedSlot1;
        }

        switch (symbol_value)
        {
            case (int)Symbols.pig_green:
                GameObject.Find($"Row_{4}/Green_Pig_Pair").transform.Find("dia_blue").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/Green_Pig_Pair").transform.Find("dia_blue").gameObject.SetActive(false);
                break;

            case (int)Symbols.pig_blue:
                GameObject.Find($"Row_{4}/Blue_Pig_Pair").transform.Find("dia_blue ").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/Blue_Pig_Pair").transform.Find("dia_blue ").gameObject.SetActive(false);
                break;

            case (int)Symbols.pig_red:
                GameObject.Find($"Row_{4}/Red_Pig_Pair").transform.Find("dia_blue").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/Red_Pig_Pair").transform.Find("dia_blue").gameObject.SetActive(false);
                break;

            case (int)Symbols.A:
                GameObject.Find($"Row_{4}/A_Pair").transform.Find("dia_blue").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/A_Pair").transform.Find("dia_blue").gameObject.SetActive(false);
                break;

            case (int)Symbols.K:
                GameObject.Find($"Row_{4}/K_Pair").transform.Find("dia_blue").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/K_Pair").transform.Find("dia_blue").gameObject.SetActive(false);
                break;
            case (int)Symbols.Q:
                GameObject.Find($"Row_{4}/Q_Pair").transform.Find("dia_blue").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/Q_Pair").transform.Find("dia_blue").gameObject.SetActive(false);
                break;
            case (int)Symbols.J:
                GameObject.Find($"Row_{4}/J_Pair").transform.Find("dia_blue").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/J_Pair").transform.Find("dia_blue").gameObject.SetActive(false);
                break;
            case (int)Symbols.del:
                GameObject.Find($"Row_{4}/Ccocal_Pair").transform.Find("dia_blue").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/Ccocal_Pair").transform.Find("dia_blue").gameObject.SetActive(false);
                break;
            case (int)Symbols.box:
                GameObject.Find($"Row_{4}/Box_Pair").transform.Find("dia_blue").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/Box_Pair").transform.Find("dia_blue").gameObject.SetActive(false);
                break;
            case (int)Symbols.cart:
                GameObject.Find($"Row_{4}/Cart_Pair").transform.Find("dia_blue").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/Cart_Pair").transform.Find("dia_blue").gameObject.SetActive(false);
                break;
        }
    }

    private IEnumerator Dia_green_event()
    {
        int ran = UnityEngine.Random.Range(0, 3);
        int symbol_value = -1;
        if (ran == 0)
        {
            symbol_value = rows[4].row_stoppedSlot3;
        }
        else if (ran == 1)
        {
            symbol_value = rows[4].row_stoppedSlot2;
        }
        else if (ran == 2)
        {
            symbol_value = rows[4].row_stoppedSlot1;
        }

        switch (symbol_value)
        {
            case (int)Symbols.pig_green:
                GameObject.Find($"Row_{4}/Green_Pig_Pair").transform.Find("dia_green").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/Green_Pig_Pair").transform.Find("dia_green").gameObject.SetActive(false);
                break;

            case (int)Symbols.pig_blue:
                GameObject.Find($"Row_{4}/Blue_Pig_Pair").transform.Find("dia_green").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/Blue_Pig_Pair").transform.Find("dia_green").gameObject.SetActive(false);
                break;

            case (int)Symbols.pig_red:
                GameObject.Find($"Row_{4}/Red_Pig_Pair").transform.Find("dia_green").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/Red_Pig_Pair").transform.Find("dia_green").gameObject.SetActive(false);
                break;

            case (int)Symbols.A:
                GameObject.Find($"Row_{4}/A_Pair").transform.Find("dia_green").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/A_Pair").transform.Find("dia_green").gameObject.SetActive(false);
                break;

            case (int)Symbols.K:
                GameObject.Find($"Row_{4}/K_Pair").transform.Find("dia_green").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/K_Pair").transform.Find("dia_green").gameObject.SetActive(false);
                break;
            case (int)Symbols.Q:
                GameObject.Find($"Row_{4}/Q_Pair").transform.Find("dia_green").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/Q_Pair").transform.Find("dia_green").gameObject.SetActive(false);
                break;
            case (int)Symbols.J:
                GameObject.Find($"Row_{4}/J_Pair").transform.Find("dia_green").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/J_Pair").transform.Find("dia_green").gameObject.SetActive(false);
                break;
            case (int)Symbols.del:
                GameObject.Find($"Row_{4}/Ccocal_Pair").transform.Find("dia_green").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/Ccocal_Pair").transform.Find("dia_green").gameObject.SetActive(false);
                break;
            case (int)Symbols.box:
                GameObject.Find($"Row_{4}/Box_Pair").transform.Find("dia_green").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/Box_Pair").transform.Find("dia_green").gameObject.SetActive(false);
                break;
            case (int)Symbols.cart:
                GameObject.Find($"Row_{4}/Cart_Pair").transform.Find("dia_green").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/Cart_Pair").transform.Find("dia_green").gameObject.SetActive(false);
                break;
        }
    }

    private IEnumerator Dia_red_event()
    {
        int ran = UnityEngine.Random.Range(0, 3);
        int symbol_value = -1;
        if (ran == 0)
        {
            symbol_value = rows[4].row_stoppedSlot3;
        }
        else if (ran == 1)
        {
            symbol_value = rows[4].row_stoppedSlot2;
        }
        else if (ran == 2)
        {
            symbol_value = rows[4].row_stoppedSlot1;
        }

        switch (symbol_value)
        {
            case (int)Symbols.pig_green:
                GameObject.Find($"Row_{4}/Green_Pig_Pair").transform.Find("dia_red").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/Green_Pig_Pair").transform.Find("dia_red").gameObject.SetActive(false);
                break;

            case (int)Symbols.pig_blue:
                GameObject.Find($"Row_{4}/Blue_Pig_Pair").transform.Find("dia_red").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/Blue_Pig_Pair").transform.Find("dia_red").gameObject.SetActive(false);
                break;

            case (int)Symbols.pig_red:
                GameObject.Find($"Row_{4}/Red_Pig_Pair").transform.Find("dia_red").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/Red_Pig_Pair").transform.Find("dia_red").gameObject.SetActive(false);
                break;

            case (int)Symbols.A:
                GameObject.Find($"Row_{4}/A_Pair").transform.Find("dia_red").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/A_Pair").transform.Find("dia_red").gameObject.SetActive(false);
                break;

            case (int)Symbols.K:
                GameObject.Find($"Row_{4}/K_Pair").transform.Find("dia_red").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/K_Pair").transform.Find("dia_red").gameObject.SetActive(false);
                break;
            case (int)Symbols.Q:
                GameObject.Find($"Row_{4}/Q_Pair").transform.Find("dia_red").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/Q_Pair").transform.Find("dia_red").gameObject.SetActive(false);
                break;
            case (int)Symbols.J:
                GameObject.Find($"Row_{4}/J_Pair").transform.Find("dia_red").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/J_Pair").transform.Find("dia_red").gameObject.SetActive(false);
                break;
            case (int)Symbols.del:
                GameObject.Find($"Row_{4}/Ccocal_Pair").transform.Find("dia_red").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/Ccocal_Pair").transform.Find("dia_red").gameObject.SetActive(false);
                break;
            case (int)Symbols.box:
                GameObject.Find($"Row_{4}/Box_Pair").transform.Find("dia_red").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/Box_Pair").transform.Find("dia_red").gameObject.SetActive(false);
                break;
            case (int)Symbols.cart:
                GameObject.Find($"Row_{4}/Cart_Pair").transform.Find("dia_red").gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
                GameObject.Find($"Row_{4}/Cart_Pair").transform.Find("dia_red").gameObject.SetActive(false);
                break;
        }
    }
    #endregion
}

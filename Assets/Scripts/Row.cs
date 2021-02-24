using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Row : Symbol
{
    public bool rowStopped;
    public bool stopFlag = false;

    public int row_stoppedSlot1;//low
    public int row_stoppedSlot2;//mid
    public int row_stoppedSlot3;//high

    void Start()
    {
        //각 릴의 위치가 다 똑같이 시작할경우에 너무 단조로워 보일수 있으므로
        //코드가 너무 길어서 수정이 필요함
        float[] respawn = { -5.6f, -7.3f, -9f, -10.7f, -12.4f, -14.1f, -15.8f, -17.5f, -19.2f };
        int ran = Random.Range(0, 9);//0~15값을 가지는거 아닌가?
        transform.localPosition = new Vector3(transform.localPosition.x, respawn[ran], 0);
        rowStopped = true;
    }
}

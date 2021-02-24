using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Symbol : MonoBehaviour
{
    public enum Symbols
    {
        //잭팟 심볼
        coin1 = 0,
        coin2 = 1,
        coin3 = 2,

        //일반 심볼
        pig_green = 3,
        pig_blue = 4,
        pig_red = 5,
        A = 6,
        K = 7,
        Q = 8,
        J = 9,
        del = 10,
        box = 11,
        cart = 12,

        //스캐터 심볼
        bonus = 13,

        //와일드심볼
        wild = 14,
    }
}

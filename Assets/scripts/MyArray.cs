using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class MyArray
{
    public static String ToString(int[] arr)
    {
        string str = "";
        for(int i = 0; i < arr.Length; i++)
        {
            str += arr.GetValue(i) + " ";
        }
        return str;
    }
}

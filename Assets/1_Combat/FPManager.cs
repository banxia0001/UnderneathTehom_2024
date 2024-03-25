using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPManager : MonoBehaviour
{
    public static int FP;
    public static int FPInUse;


    public static void FPChange(int num)
    {
        FP += num;
        if (FP <= 0) FP = 0;
        if (FP > 9) FP = 9;
        if (num > 0)
        {
            FindObjectOfType<CombatPanelUI>(true).ManaIcon.SetTrigger("trigger");
        }
    }

}

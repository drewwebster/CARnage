using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalModifiers : MonoBehaviour {

    static List<ModController> mcList = new List<ModController>();

    public static void registerModController(ModController mc)
    {
        mcList.Add(mc);
    }

	public static float getWeaponReloadTime_Multiplier_GLOBAL()
    {
        float mult = 1f;
        foreach (ModController mc in mcList)
            if(mc)
                foreach (CARnageModifier mod in mc.getMods())
                    if (mod.modID == CARnageModifier.ModID.INERTIA)
                        mult += 5;

        return mult;
    }

    public static float getWeaponMagazine_Multiplier_GLOBAL(CARnageWeapon weapon)
    {
        float mult = 1f;
        foreach (ModController mc in mcList)
            if(mc)
                foreach (CARnageModifier mod in mc.getMods())
                    if (mod.modID == CARnageModifier.ModID.MUZZLE__LOADER)
                        mult = 1f/weapon.magazineSize;   // set to 1 (can be influenced by local mods tho)

        return mult;
    }

    //public static float sloMoCounter = 0f;
    //private void Update()
    //{
    //    if(sloMoCounter > 0f)
    //    {
    //        sloMoCounter -= Time.deltaTime;
    //        Time.timeScale = 0.2f;
    //        if(sloMoCounter <= 0)
    //        {
    //            Time.timeScale = 1f;
    //            sloMoCounter = 0;
    //        }
    //    }
    //}
}

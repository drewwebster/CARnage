using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestConsole : MonoBehaviour {

    public GameObject inputField;
    CARnageCar relCar;

    private void Start()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
            if (go.GetComponent<CARnageCar>() != null && go.GetComponent<CARnageCar>().controlledBy == CARnageAuxiliary.ControllerType.MouseKeyboard)
                relCar = go.GetComponent<CARnageCar>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            if (inputField.activeSelf)
                inputField.SetActive(false);
            else
            {
                inputField.SetActive(true);
                inputField.GetComponent<InputField>().Select();
                inputField.GetComponent<InputField>().ActivateInputField();
            }

        if(Input.GetKeyDown(KeyCode.Return) && inputField.activeSelf)
            if(inputField.GetComponent<InputField>().text.Equals(""))
            {
                inputField.GetComponent<InputField>().Select();
                inputField.GetComponent<InputField>().ActivateInputField();
            }
            else
            {
                sendTestCommand(inputField.GetComponent<InputField>().text);
                inputField.GetComponent<InputField>().text = "";
            }
                
    }

    public void sendTestCommand(string command)
    {
        command = command.ToUpper();
        string parameter = "";
        if(command.Contains(" "))
        {
            parameter = command.Split(" ".ToCharArray()[0])[1];
            command = command.Split(" ".ToCharArray()[0])[0];
        }

        switch(command)
        {
            case "DMG":
                float dmg = float.Parse(parameter);
                relCar.damageMe(dmg,relCar,DamageType.PROJECTILE);
                break;
            case "DESTROY":
                relCar.destroyMe();
                break;
            case "REPAIR":
                relCar.repair();
                break;
            case "BREAKSHIELD":
                if (relCar.currentShield <= 0)
                    return;
                relCar.damageMe(2000, relCar, DamageType.PROJECTILE);
                break;
            case "REPAIRSHIELD":
                relCar.regenerateShield(2000);
                break;
            case "FIRE":
                relCar.applyDebuff(CARnageCar.Debuff.Fire, relCar);
                break;
            case "ACID":
                relCar.applyDebuff(CARnageCar.Debuff.Acid, relCar);
                break;
            case "DRAIN":
                relCar.applyDebuff(CARnageCar.Debuff.Drain, relCar);
                break;
            case "ADDMOD":
                ModFactory.spawnMod((CARnageModifier.ModID)System.Enum.Parse(typeof(CARnageModifier.ModID), parameter), relCar);
                break;
            case "DELMOD":
                foreach (CARnageModifier mod in relCar.getModController().getMods())
                    if (mod.modID == (CARnageModifier.ModID)System.Enum.Parse(typeof(CARnageModifier.ModID), parameter))
                        mod.deleteMe();
                break;
            case "ADDWEAPON":
                CARnageWeapon weapon= WeaponFactory.spawnWeapon((CARnageWeapon.WeaponModel)System.Enum.Parse(typeof(CARnageWeapon.WeaponModel), parameter), relCar).GetComponent<CARnageWeapon>();
                weapon.onObtained();
                break;
            case "RNDWEAPON":
                WeaponFactory.spawnRndWeapon(relCar.transform.position);
                break;
        }
    }
}

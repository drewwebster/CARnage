using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestConsole : MonoBehaviour {

    public GameObject inputField;
    CARnageCar relCar;
    public List<string> cmdHistory;
    int cmdHistoryPointer;
    bool paused;

    private void Start()
    {
        cmdHistory = new List<string>();
        cmdHistoryPointer = 0;
        if (GameObject.Find("RCCCamera"))
            relCar = GameObject.Find("RCCCamera").GetComponent<RCC_Camera>().playerCar.GetComponent<CARnageCar>();
        else
            relCar = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<CARnageCar>();

        //foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
        //    if (go.GetComponent<CARnageCar>() != null && go.GetComponent<CARnageCar>().controlledBy == CARnageAuxiliary.ControllerType.MouseKeyboard)
        //        relCar = go.GetComponent<CARnageCar>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            if (inputField.activeSelf)
            {
                inputField.SetActive(false);
                CARnageAuxiliary.pauseEnd();
            }
            else
            {
                inputField.SetActive(true);
                inputField.GetComponent<InputField>().Select();
                inputField.GetComponent<InputField>().ActivateInputField();
                CARnageAuxiliary.pause();
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
        if (Input.GetKeyDown(KeyCode.UpArrow) && inputField.activeSelf && cmdHistoryPointer > 0)
        {
            cmdHistoryPointer--;
            inputField.GetComponent<InputField>().text = cmdHistory[cmdHistoryPointer];
            inputField.GetComponent<InputField>().Select();
            inputField.GetComponent<InputField>().ActivateInputField();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && inputField.activeSelf && cmdHistoryPointer < cmdHistory.Count)
        {
            cmdHistoryPointer++;
            inputField.GetComponent<InputField>().text = cmdHistory[cmdHistoryPointer];
            inputField.GetComponent<InputField>().Select();
            inputField.GetComponent<InputField>().ActivateInputField();
        }

    }

    public void sendTestCommand(string command)
    {
        cmdHistory.Add(command);
        cmdHistoryPointer = cmdHistory.Count;
        command = command.ToUpper();
        string parameter = "";
        if(command.Contains(" "))
        {
            parameter = command.Split(" ".ToCharArray()[0])[1];
            command = command.Split(" ".ToCharArray()[0])[0];
        }

        float mult = 1;
        switch (command)
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
            case "SHIELD":
                relCar.replenishShield(float.Parse(parameter));
                break;
            case "REPAIRSHIELD":
                relCar.regenerateShield(2000);
                break;
            case "FIRE":
                if (parameter.Length > 0)
                    mult = float.Parse(parameter);
                relCar.applyDebuff(CARnageCar.Debuff.Fire, relCar, mult);
                break;
            case "ACID":
                if (parameter.Length > 0)
                    mult = float.Parse(parameter);
                relCar.applyDebuff(CARnageCar.Debuff.Acid, relCar, mult);
                break;
            case "DRAIN":
                if (parameter.Length > 0)
                    mult = float.Parse(parameter);
                relCar.applyDebuff(CARnageCar.Debuff.Drain, relCar, mult);
                break;
            case "FREEZE":
            case "FRZ":
                if (parameter.Length > 0)
                    mult = float.Parse(parameter);
                relCar.applyDebuff(CARnageCar.Debuff.Freeze, relCar, mult);
                break;
            case "LOCK":
            case "LOCKED":
                if (parameter.Length > 0)
                    mult = float.Parse(parameter);
                relCar.applyDebuff(CARnageCar.Debuff.Locked, relCar, mult);
                break;
            case "ADDMOD":
            case "MOD":
                ModFactory.spawnMod((CARnageModifier.ModID)System.Enum.Parse(typeof(CARnageModifier.ModID), parameter), relCar);
                break;
            case "DELMOD":
                foreach (CARnageModifier mod in relCar.getModController().getMods())
                    if (mod.modID == (CARnageModifier.ModID)System.Enum.Parse(typeof(CARnageModifier.ModID), parameter))
                        mod.deleteMe();
                break;
            case "ADDWEAPON":
            case "WEAPON":
                CARnageWeapon weapon= WeaponFactory.spawnWeapon((CARnageWeapon.WeaponModel)System.Enum.Parse(typeof(CARnageWeapon.WeaponModel), parameter), relCar).GetComponent<CARnageWeapon>();
                weapon.onObtained();
                break;
            case "RNDWEAPON":
                WeaponFactory.spawnRndWeapon(relCar.transform.position);
                break;
            case "CAR":
                CarFactory.spawnCar(CarModel.TOOLTIME, relCar.transform.position + relCar.transform.forward * 10);
                break;
            case "UP":
            case "UPGRADE":
                foreach (CARnageWeapon w in relCar.getWeaponController().getAllWeapons())
                    w.addAllUpgrades();
                break;
            case "GEAR":
                if (parameter.Length > 0)
                    mult = float.Parse(parameter);
                Gear.spawnGears((int)mult,relCar,CARnageModifier.GearSource.OTHER);
                break;
        }
    }
}

using UnityEngine;
using BepInEx;
using BepInEx.Configuration;

namespace DSPMechaMod
{
  [BepInPlugin("top.backrunner.DSP.MechaMod", "DSP Mecha Mod", "1.0")]
  public class MechaMod: BaseUnityPlugin
  {
    bool enableInifiniteEnergy = false;
    ConfigEntry<KeyCode> addEnergyOneTimeHotKey, infiniteEnergyHotKey;
    void Start()
    {
      addEnergyOneTimeHotKey = Config.Bind("HotKey", "Add Energy At OneTime", KeyCode.F9, "一次性加满能源");
      infiniteEnergyHotKey = Config.Bind("HotKey", "Enable Inifinite Energy", KeyCode.F10, "无限能源");
    }
    void Update()
    {
      if (Input.GetKeyDown(addEnergyOneTimeHotKey.Value))
      {
        Mecha mecha = GameMain.data.mainPlayer.mecha;
        mecha.coreEnergy = mecha.coreEnergyCap;
        UIRealtimeTip.Popup("能源已补满");
      }
      if (Input.GetKeyDown(infiniteEnergyHotKey.Value))
      {
        enableInifiniteEnergy = !enableInifiniteEnergy;
        if (enableInifiniteEnergy)
        {
          UIRealtimeTip.Popup("无限能源已启用");
        } else
        {
          UIRealtimeTip.Popup("无限能源已关闭");
        }
      }
      if (enableInifiniteEnergy)
      {
        Mecha mecha = GameMain.data.mainPlayer.mecha;
        mecha.coreEnergy = mecha.coreEnergyCap;
      }
    }
  }
}

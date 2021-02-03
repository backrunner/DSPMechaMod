using UnityEngine;
using BepInEx;
using BepInEx.Configuration;

namespace DSPMechaMod
{
  [BepInPlugin("top.backrunner.DSP.MechaMod", "DSP Mecha Mod", "1.1")]
  public class MechaMod: BaseUnityPlugin
  {
    bool enableInifiniteEnergy = false;
    ConfigEntry<KeyCode> addEnergyOneTimeHotKey, infiniteEnergyHotKey, addWarpHotKey, upReplicateSpeedHotKey, downReplicateSpeedHotKey;
    ConfigEntry<int> warpAmount;
    ConfigEntry<float> replicateSpeedAmount;
    void Start()
    {
      addEnergyOneTimeHotKey = Config.Bind("HotKey", "Add Energy At OneTime", KeyCode.F9, "一次性加满能源");
      infiniteEnergyHotKey = Config.Bind("HotKey", "Enable Inifinite Energy", KeyCode.F10, "无限能源");
      addWarpHotKey = Config.Bind("HotKey", "Add warp to mecha", KeyCode.F12, "给机甲添加空间翘曲器");
      warpAmount = Config.Bind("Options", "How much warp you want to add", 10, "添加空间翘曲器的数量");
      downReplicateSpeedHotKey = Config.Bind("HotKey", "Speed down replicate", KeyCode.F3, "减慢机甲建造");
      upReplicateSpeedHotKey = Config.Bind("HotKey", "Speed up replicate", KeyCode.F4, "加速机甲建造");
      replicateSpeedAmount = Config.Bind("Options", "Replicate speed amount per tweaking", 1f, "每次调整生产速度的量");
    }
    void Update()
    {
      if (Input.GetKeyDown(addEnergyOneTimeHotKey.Value))
      {
        Mecha mecha = GameMain.data.mainPlayer.mecha;
        mecha.coreEnergy = mecha.coreEnergyCap;
        UIRealtimeTip.Popup("能源已补满", false);
      }
      if (Input.GetKeyDown(infiniteEnergyHotKey.Value))
      {
        enableInifiniteEnergy = !enableInifiniteEnergy;
        if (enableInifiniteEnergy)
        {
          UIRealtimeTip.Popup("无限能源已启用", false);
        } else
        {
          UIRealtimeTip.Popup("无限能源已关闭", false);
        }
      }
      if (Input.GetKeyDown(addWarpHotKey.Value))
      {
        Mecha mecha = GameMain.data.mainPlayer.mecha;
        mecha.warpStorage.AddItem(1210, warpAmount.Value, false);
        UIRealtimeTip.Popup("已为机甲补充空间翘曲器" + warpAmount.Value.ToString() + "个", false);
      }
      if (enableInifiniteEnergy)
      {
        Mecha mecha = GameMain.data.mainPlayer.mecha;
        mecha.coreEnergy = mecha.coreEnergyCap;
      }
      if (Input.GetKeyDown(upReplicateSpeedHotKey.Value))
      {
        GameMain.data.mainPlayer.mecha.replicateSpeed += replicateSpeedAmount.Value;
        float currentReplicateSpeed = GameMain.data.mainPlayer.mecha.replicateSpeed;
        UIRealtimeTip.Popup("机甲建造速度已调整为" + currentReplicateSpeed.ToString());
      }
      if (Input.GetKeyDown(downReplicateSpeedHotKey.Value))
      {
        GameMain.data.mainPlayer.mecha.replicateSpeed -= replicateSpeedAmount.Value;
        float currentReplicateSpeed = GameMain.data.mainPlayer.mecha.replicateSpeed;
        UIRealtimeTip.Popup("机甲建造速度已调整为" + currentReplicateSpeed.ToString());
      }
    }
  }
}

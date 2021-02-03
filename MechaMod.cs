using UnityEngine;
using BepInEx;
using BepInEx.Configuration;
using System;

namespace DSPMechaMod
{
  [BepInPlugin("top.backrunner.DSP.MechaMod", "DSP Mecha Mod", "1.2")]
  public class MechaMod: BaseUnityPlugin
  {
    bool enableInifiniteEnergy = false;
    bool inited = false;
    float initialConfigWarpSpeed, initialConfigSailSpeed;
    ConfigEntry<KeyCode> addEnergyOneTimeHotKey, infiniteEnergyHotKey, addWarpHotKey, upReplicateSpeedHotKey, downReplicateSpeedHotKey;
    ConfigEntry<int> warpAmount;
    ConfigEntry<bool> modifySailSpeed, modifyWarpSpeed;
    ConfigEntry<float> replicateSpeedAmount, maxSailSpeed, maxWarpSpeed;
    ConfigEntry<float> originalSailSpeedConfig, originalWarpSpeedConfig;
    void Start()
    {
      addEnergyOneTimeHotKey = Config.Bind("HotKey", "Add Energy At OneTime", KeyCode.F9, "一次性加满能源");
      infiniteEnergyHotKey = Config.Bind("HotKey", "Enable Inifinite Energy", KeyCode.F10, "无限能源");
      addWarpHotKey = Config.Bind("HotKey", "Add warp to mecha", KeyCode.F12, "给机甲添加空间翘曲器");
      warpAmount = Config.Bind("Options", "How much warp you want to add", 10, "添加空间翘曲器的数量");
      downReplicateSpeedHotKey = Config.Bind("HotKey", "Speed down replicate", KeyCode.F3, "减慢机甲建造");
      upReplicateSpeedHotKey = Config.Bind("HotKey", "Speed up replicate", KeyCode.F4, "加速机甲建造");
      replicateSpeedAmount = Config.Bind("Options", "Replicate speed amount per tweaking", 1f, "每次调整生产速度的量");
      modifySailSpeed = Config.Bind("Switches", "Enable modify max sail speed", false, "是否调整最大航行速度");
      modifyWarpSpeed = Config.Bind("Switches", "Enable modify max warp speed", false, "是否调整最大曲速航行速度");
    }
    void Update()
    {
      try
      {
        Mecha mecha = GameMain.data?.mainPlayer?.mecha;
        if (mecha == null)
        {
          return;
        }
        if (!inited)
        {
          float originalSailSpeed = mecha.maxSailSpeed;
          float originalWarpSpeed = mecha.maxWarpSpeed;
          maxSailSpeed = Config.Bind("Options", "Max sail speed", (float)originalSailSpeed, "最大航行速度");
          maxWarpSpeed = Config.Bind("Options", "Max warp speed", (float)originalWarpSpeed, "最大曲速航行速度");
          originalSailSpeedConfig = Config.Bind("Private", "Original sail speed", originalSailSpeed);
          originalWarpSpeedConfig = Config.Bind("Private", "Original warp speed", originalWarpSpeed);
          initialConfigSailSpeed = maxSailSpeed.Value;
          initialConfigWarpSpeed = maxWarpSpeed.Value;
          Config.Save();
          if (modifySailSpeed.Value)
          {
            mecha.maxSailSpeed = maxSailSpeed.Value;
          }
          else
          {
            mecha.maxSailSpeed = originalSailSpeedConfig.Value;
          }
          if (modifyWarpSpeed.Value)
          {
            mecha.maxWarpSpeed = maxWarpSpeed.Value;
          }
          else
          {
            mecha.maxWarpSpeed = originalWarpSpeedConfig.Value;
          }
          inited = true;
        }
        if (modifySailSpeed.Value)
        {
          if (maxSailSpeed.Value != initialConfigSailSpeed)
          {
            initialConfigSailSpeed = maxSailSpeed.Value;
            mecha.maxSailSpeed = maxSailSpeed.Value;
          }
          if (mecha.maxSailSpeed != maxSailSpeed.Value)
          {
            originalSailSpeedConfig.Value += mecha.maxSailSpeed - maxSailSpeed.Value;
            Config.Save();
          }
        } else
        {
          mecha.maxSailSpeed = originalSailSpeedConfig.Value;
        }
        if (modifyWarpSpeed.Value)
        {
          if (maxWarpSpeed.Value != initialConfigWarpSpeed)
          {
            initialConfigWarpSpeed = maxWarpSpeed.Value;
            mecha.maxWarpSpeed = maxWarpSpeed.Value;
          }
          if (mecha.maxWarpSpeed != maxWarpSpeed.Value)
          {
            originalWarpSpeedConfig.Value += mecha.maxWarpSpeed - maxWarpSpeed.Value;
            Config.Save();
          }
        } else
        {
          mecha.maxWarpSpeed = originalWarpSpeedConfig.Value;
        }
        // input event
        if (Input.GetKeyDown(addEnergyOneTimeHotKey.Value))
        {
          mecha.coreEnergy = mecha.coreEnergyCap;
          UIRealtimeTip.Popup("能源已补满", false);
        }
        if (Input.GetKeyDown(infiniteEnergyHotKey.Value))
        {
          enableInifiniteEnergy = !enableInifiniteEnergy;
          if (enableInifiniteEnergy)
          {
            UIRealtimeTip.Popup("无限能源已启用", false);
          }
          else
          {
            UIRealtimeTip.Popup("无限能源已关闭", false);
          }
        }
        if (Input.GetKeyDown(addWarpHotKey.Value))
        {
          mecha.warpStorage.AddItem(1210, warpAmount.Value, false);
          UIRealtimeTip.Popup("已为机甲补充空间翘曲器" + warpAmount.Value.ToString() + "个", false);
        }
        if (enableInifiniteEnergy)
        {
          mecha.coreEnergy = mecha.coreEnergyCap;
        }
        if (Input.GetKeyDown(upReplicateSpeedHotKey.Value))
        {
          mecha.replicateSpeed += replicateSpeedAmount.Value;
          float currentReplicateSpeed = GameMain.data.mainPlayer.mecha.replicateSpeed;
          UIRealtimeTip.Popup("机甲建造速度已调整为" + currentReplicateSpeed.ToString());
        }
        if (Input.GetKeyDown(downReplicateSpeedHotKey.Value))
        {
          mecha.replicateSpeed -= replicateSpeedAmount.Value;
          float currentReplicateSpeed = GameMain.data.mainPlayer.mecha.replicateSpeed;
          UIRealtimeTip.Popup("机甲建造速度已调整为" + currentReplicateSpeed.ToString());
        }
      } catch (NullReferenceException e) { 
        // do noting
      }
    }
  }
}

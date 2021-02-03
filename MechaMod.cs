using UnityEngine;
using BepInEx;
using BepInEx.Configuration;
using System;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;

namespace DSPMechaMod
{
  [BepInPlugin("top.backrunner.DSP.MechaMod", "DSP Mecha Mod", "1.5")]
  public class MechaMod: BaseUnityPlugin
  {
    public static ConfigEntry<bool> autoSortConfig;
    bool enableInifiniteEnergy = false;
    bool inited = false;
    float initialConfigWarpSpeed, initialConfigSailSpeed;
    ConfigEntry<KeyCode> addEnergyOneTimeHotKey, infiniteEnergyHotKey, addWarpHotKey, upReplicateSpeedHotKey, downReplicateSpeedHotKey;
    ConfigEntry<int> warpAmount;
    ConfigEntry<bool> modifySailSpeed, modifyWarpSpeed, enableCheating;
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
      enableCheating = Config.Bind("Switches", "Enable cheating", false, "是否启用作弊选项（能源、翘曲器、建造速度）");
      modifySailSpeed = Config.Bind("Switches", "Enable modify max sail speed", false, "是否调整最大航行速度");
      modifyWarpSpeed = Config.Bind("Switches", "Enable modify max warp speed", false, "是否调整最大曲速航行速度");
      autoSortConfig = Config.Bind("Switches", "Enable auto sort when open up inventory", true, "是否在打开物品面板时自动排序");
      Harmony.CreateAndPatchAll(typeof(MechaMod));
      Harmony.CreateAndPatchAll(typeof(AutoSortPatch));
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
          if (mecha.maxSailSpeed != maxSailSpeed.Value) {
            mecha.maxSailSpeed = maxSailSpeed.Value;
          }
          if (maxSailSpeed.Value != initialConfigSailSpeed)
          {
            initialConfigSailSpeed = maxSailSpeed.Value;
            mecha.maxSailSpeed = maxSailSpeed.Value;
          }
          if (mecha.maxSailSpeed != maxSailSpeed.Value && mecha.maxSailSpeed != originalSailSpeedConfig.Value)
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
          if (mecha.maxWarpSpeed != maxWarpSpeed.Value)
          {
            mecha.maxWarpSpeed = maxWarpSpeed.Value;
          }
          if (maxWarpSpeed.Value != initialConfigWarpSpeed)
          {
            initialConfigWarpSpeed = maxWarpSpeed.Value;
            mecha.maxWarpSpeed = maxWarpSpeed.Value;
          }
          if (mecha.maxWarpSpeed != maxWarpSpeed.Value && mecha.maxWarpSpeed != originalWarpSpeedConfig.Value)
          {
            originalWarpSpeedConfig.Value += mecha.maxWarpSpeed - maxWarpSpeed.Value;
            Config.Save();
          }
        } else
        {
          mecha.maxWarpSpeed = originalWarpSpeedConfig.Value;
        }
        // input event
        if (enableCheating.Value)
        {
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
            if (mecha.replicateSpeed < 1)
            {
              mecha.replicateSpeed = 1;
            }
            float currentReplicateSpeed = GameMain.data.mainPlayer.mecha.replicateSpeed;
            UIRealtimeTip.Popup("机甲建造速度已调整为" + currentReplicateSpeed.ToString());
          }
        }
      } catch (NullReferenceException)
      { 
        // do noting
      }
    }

    public static double getMechaMaxSailSpeed()
    {
      return GameMain.data.mainPlayer.mecha.maxSailSpeed;
    }

    [HarmonyTranspiler, HarmonyPatch(typeof(UISailPanel), "_OnUpdate")]
    public static IEnumerable<CodeInstruction> SailPanelPath(IEnumerable<CodeInstruction> instructions)
    {
      MethodInfo getSpeedMethod = AccessTools.Method(typeof(MechaMod), nameof(getMechaMaxSailSpeed));
      Mecha mecha = GameMain.data?.mainPlayer?.mecha;
      List<CodeInstruction> codes = instructions.ToList();
      foreach (CodeInstruction code in codes)
      {
        if (code.opcode == OpCodes.Ldc_R8 && (double)code.operand == 2000)
        {
          yield return new CodeInstruction(OpCodes.Call, getSpeedMethod);
        } else
        {
          yield return code;
        }
      }
    }
  }

  [HarmonyPatch(typeof(UIGame), "OpenPlayerInventory")]
  public class AutoSortPatch
  {
    public static void Prefix()
    {
      if (MechaMod.autoSortConfig.Value)
      {
        GameMain.data?.mainPlayer?.package?.Sort();
      }
    }
  }
}

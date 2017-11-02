using System;
using System.Collections.Generic;
using System.Linq;
using Modding;
using GlobalEnums;
using UnityEngine;
using HutongGames;
using FsmUtils;

namespace NGG
{
    public class NGG : Mod
    {

        private static string version = "0.2.1";

        public override string GetVersion()
        {
            return version;
        }

        public override void Initialize()
        {
            ModHooks.ModLog("Initializing Nightmare God Grimm");

            GameManager.instance.gameObject.AddComponent<BossFinder>();

            ModHooks.ModLog("Initialized Nightmare God Grimm");
        }

    }
}

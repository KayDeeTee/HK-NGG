using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Modding;
using FsmUtils;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
namespace NGG
{
    class BossFinder : MonoBehaviour
    {
        public GameObject grimm;
        public static GameObject grimm2;
        public bool done;

        public bool s1, s2, s3, s4 = false;

        PlayMakerFSM fsm, gfsm;

        tk2dSpriteAnimator grimm_anim;

        PlayMakerFSM hme;
        PlayMakerFSM hme2;

        public static System.Random random;

        public GameObject[] spikes;
        public PlayMakerFSM[] spikeFsms;

        public void Start()
        {
            random = new System.Random();
            spikes = new GameObject[15];
            spikeFsms = new PlayMakerFSM[15];
            ModHooks.Instance.BeforeSceneLoadHook += reset;
            done = false;
        }

        public string reset(string scenes)
        {
            if (scenes == "Grimm_Nightmare"){
                done = false;
                PlayerData.instance.AddMPCharge(99 * 2);
                PlayerData.instance.MPCharge = 99;
                PlayerData.instance.MPReserve = 99;
                s1 = false;
                s2 = false;
                s3 = false;
            }                
            else
                done = false;
            return scenes;
        }

        public void Update(){
            if (!done)
            {
                if (grimm == null )
                {
                    grimm = GameObject.FindGameObjectWithTag("Boss");
                }
                else
                {
                    //ModHooks.ModLog("[NGG]" + go.transform.parent.gameObject.name);
                    ModHooks.ModLog(grimm.name);

                    if( grimm2 == null )
                        grimm2 = Instantiate(grimm);

                    grimm_anim = grimm.GetComponent<tk2dSpriteAnimator>();

                    ModHooks.ModLog("[NGG] " + grimm_anim.ClipFps);

                    gfsm = FSMUtility.LocateFSM(grimm2, "Control");
                    fsm = FSMUtility.LocateFSM(grimm, "Control");

                    hme = FSMUtility.LocateFSM(grimm, "health_manager_enemy");
                    hme2 = FSMUtility.LocateFSM(grimm2, "health_manager_enemy");

                    hme.FsmVariables.GetFsmInt("HP").Value = 3200;

                    fsm.SetState("Set Balloon HP");


                    gfsm.FsmVariables.GetFsmBool("Done Balloon 1").Value = true;
                    gfsm.FsmVariables.GetFsmBool("Done Balloon 2").Value = true;
                    gfsm.FsmVariables.GetFsmBool("Done Balloon 3").Value = true;

                    FsmUtil.ChangeTransition(fsm, "Move Choice", "PILLARS", "AD Pos");
                    FsmUtil.ChangeTransition(fsm, "Move Choice", "SPIKES", "Slash Pos");

                    FsmUtil.ChangeTransition(fsm, "Firebat 3", "FINISHED", "FB Behind");
                    FsmUtil.ChangeTransition(fsm, "G Dash Recover", "FINISHED", "Slash End");

                    //FsmUtil.ChangeTransition(gfsm, "Firebat 3", "FINISHED", "FB Behind");
                    //FsmUtil.ChangeTransition(gfsm, "G Dash Recover", "FINISHED", "Slash End");

                    FsmUtil.ChangeTransition(gfsm, "Move Choice", "FIREBATS", "Spike Attack");
                    FsmUtil.ChangeTransition(gfsm, "Move Choice", "SLASH", "Spike Attack");
                    FsmUtil.ChangeTransition(gfsm, "Move Choice", "AIR DASH", "Spike Attack");
                    FsmUtil.ChangeTransition(gfsm, "Move Choice", "SPIKES", "Spike Attack");
                    FsmUtil.ChangeTransition(gfsm, "Move Choice", "PILLARS", "Spike Attack");

                    GameObject[] objects = UnityEngine.GameObject.FindObjectsOfType<GameObject>();
                    int i = 0;
                    foreach (GameObject go in objects)
                    {
                        if (go.name.Contains("Nightmare Spike"))
                        {
                            spikes[i] = go;
                            spikeFsms[i] = FSMUtility.LocateFSM(go, "Control");
                            i++;
                            //Modding.ModHooks.ModLog(go.transform.position.ToString());
                            //Modding.ModHooks.ModLog("[NGG] Test");
                            //PlayMakerFSM spike_fsm = FSMUtility.LocateFSM(go, "Control");
                            //FsmUtil.ChangeTransition(spike_fsm, "DOWN", "FINISHED", "Dormant");
                        }
                    }

                    //SetBoolValue sbv = new SetBoolValue();
                    //FsmBool falseValue = new FsmBool().Value = false;
                    //sbv.boolValue = falseValue;
                    //sbv.boolVariable = fsm.FsmVariables.GetFsmBool("First Move");
                    //sbv.everyFrame = false;

                    //SpawnObjectFromGlobalPoolOverTime firetrail = (SpawnObjectFromGlobalPoolOverTime)FsmUtil.GetAction(fsm, "Ad Fire", 7);
                    //FsmUtil.AddAction(fsm, "Slash Pos", firetrail);

                    //FsmUtil.RemoveAction(fsm, "Move Choice", 0);
                    //FsmUtil.RemoveAction(fsm, "Move Choice", 0);

                    done = true;
                }
            }
            else
            {
                for( int i = 0; i < 15; i++ ){
                    if ( spikeFsms[i].ActiveStateName == "Dormant" )
                    {
                        spikes[i].transform.position = new Vector3((float)(66 + (2.5 * i) + (random.NextDouble() * 2.8)), 4.5f, (fsm.ActiveStateName == "AD Antic" || fsm.ActiveStateName == "AD Fire" || fsm.ActiveStateName == "AD Edge" || fsm.ActiveStateName == "GD Antic" || fsm.ActiveStateName == "G Dash Recover" || fsm.ActiveStateName == "G Dash") ? -1.0f : 1.0f);
                        //spikes[i].transform.localScale = new Vector3((float)(65 + (3 * i) + (random.NextDouble() * 3.0)), 4.5f, 0.0f);
                        PlayMakerFSM sFsm = FSMUtility.LocateFSM(spikes[i], "damages_hero");
                        //sFsm.FsmVariables.GetFsmInt("damageDealt").Value = 1;
                    }
                }
                if (fsm.FsmVariables.GetFsmBool("Done Balloon 1").Value && s1 == false )
                {
                    Modding.ModHooks.ModLog("[NGG] Rage1");

                    for (int i = 0; i < 15; i++)
                    {
                        PlayMakerFSM sFsm = FSMUtility.LocateFSM(spikes[i], "damages_hero");
                        sFsm.FsmVariables.GetFsmInt("damageDealt").Value = 2;
                    }
                    grimm_anim.GetClipByName("Tele In").fps = 24;
                    grimm_anim.GetClipByName("Tele Out").fps = 24;
                    grimm_anim.GetClipByName("Uppercut End").fps = 24;
                    grimm_anim.GetClipByName("Slash Recover").fps = 24;
                    grimm_anim.GetClipByName("Spike Up").fps = 6;
                    grimm_anim.GetClipByName("Evade End").fps = 24;
                    s1 = true;
                }
                if (fsm.FsmVariables.GetFsmBool("Done Balloon 2").Value && s2 == false)
                {
                    Modding.ModHooks.ModLog("[NGG] Rage2");

                    for (int i = 0; i < 15; i++)
                    {
                        PlayMakerFSM sFsm = FSMUtility.LocateFSM(spikes[i], "damages_hero");
                        sFsm.FsmVariables.GetFsmInt("damageDealt").Value = 4;
                    }
                    grimm_anim.GetClipByName("Tele In").fps = 36;
                    grimm_anim.GetClipByName("Tele Out").fps = 36;
                    grimm_anim.GetClipByName("Uppercut End").fps = 36;
                    grimm_anim.GetClipByName("Slash Recover").fps = 36;
                    grimm_anim.GetClipByName("Spike Up").fps = 4;
                    grimm_anim.GetClipByName("Evade End").fps = 36;
                    s2 = true;
                }
                if (fsm.FsmVariables.GetFsmBool("Done Balloon 3").Value && s3 == false)
                {
                    Modding.ModHooks.ModLog("[NGG] Rage3");

                    for (int i = 0; i < 15; i++)
                    {
                        PlayMakerFSM sFsm = FSMUtility.LocateFSM(spikes[i], "damages_hero");
                        sFsm.FsmVariables.GetFsmInt("damageDealt").Value = 2;
                    }
                    grimm_anim.GetClipByName("Tele In").fps = 48;
                    grimm_anim.GetClipByName("Tele Out").fps = 48;
                    grimm_anim.GetClipByName("Uppercut End").fps = 48;
                    grimm_anim.GetClipByName("Slash Recover").fps = 48;
                    grimm_anim.GetClipByName("Spike Up").fps = 2;
                    grimm_anim.GetClipByName("Evade End").fps = 38;

                    hme.FsmVariables.GetFsmInt("HP").Value = 3200;
                    hme2.FsmVariables.GetFsmInt("HP").Value = 3200;

                    FsmUtil.ChangeTransition(fsm, "Firebat 3", "FINISHED", "Firebat 4");
                    FsmUtil.ChangeTransition(fsm, "G Dash Recover", "FINISHED", "Tele Out");

                    FsmUtil.ChangeTransition(gfsm, "Move Choice", "FIREBATS", "FB Hero Pos");
                    FsmUtil.ChangeTransition(gfsm, "Move Choice", "SLASH", "Slash Pos");
                    FsmUtil.ChangeTransition(gfsm, "Move Choice", "AIR DASH", "AD Pos");
                    FsmUtil.ChangeTransition(gfsm, "Move Choice", "SPIKES", "Spike Attack");
                    FsmUtil.ChangeTransition(gfsm, "Move Choice", "PILLARS", "Pillar Pos");


                    s3 = true;
                }
                if (s3 == true && s4 == false)
                {
                    if ((hme.FsmVariables.GetFsmInt("HP").Value + hme2.FsmVariables.GetFsmInt("HP").Value <= 5600)){
                        hme.SetState("Pause");
                        hme2.SetState("Pause");
                        s4 = true;
                    }
                    
                }
            }        
        }
    }
}

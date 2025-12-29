using System;
using IngameDebugConsole;
using MeteorMadness.Contracts;
using MeteorMadness.Managers;
using MeteorMadness.Managers.Cosmetics;
using MeteorMadness.Managers.Save;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.MyCommands
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public static class GameModeCommands
    {
        [ConsoleMethod("gm_help", "List of GameMode commands")]
        public static void GameModeHelp()
        {
            Debug.Log(
                "=== GAME MODE COMMANDS ===\n" +
                "gm_pause <true/false>\n" +
                "gm_canpause <true/false>"
            );
        }
        
        /*[ConsoleMethod("gm_init", "Inicializa valores de GameMode")]
        public static void GM_Init()
        {
            GameModeEventCaller.InitializeValues();
        }*/

        [ConsoleMethod("gm_pause", "Set pause (true/false)")]
        public static void GM_Pause(bool value)
        {
            GameModeEventCaller.SetPause(value);
        }

        [ConsoleMethod("gm_canpause", "Enables or disables pause")]
        public static void GM_CanPause(bool value)
        {
            GameModeEventCaller.SetEnablePause(value);
        }
    }
    public static class EarthCommands
    {
        [ConsoleMethod("earth_help", "List of Earth commands")]
        public static void EarthHelp()
        {
            Debug.Log(
                "=== EARTH COMMANDS ===\n" +
                "earth_restart\n" +
                "earth_restart_done\n" +
                "earth_shake\n" +
                "earth_heal\n" +
                "earth_damage_on\n" +
                "earth_damage_off\n" +
                "earth_kill\n" +
                "earth_preslice"
            );
        }
        
        [ConsoleMethod("earth_restart", "Restarts Earth")]
        public static void EarthRestart()
        {
            EarthEventCaller.Restart();
        }

        [ConsoleMethod("earth_restart_done", "Finishes Earth's restart")]
        public static void EarthRestartDone()
        {
            Debug.Log("Restart Command");
            EarthEventCaller.RestartFinished();
        }

        [ConsoleMethod("earth_shake", "Starts Earth's shake")]
        public static void EarthShake()
        {
            EarthEventCaller.ShakeStart();
        }

        [ConsoleMethod("earth_heal", "Heals Earth")]
        public static void EarthHeal()
        {
            EarthEventCaller.Heal();
        }

        [ConsoleMethod("earth_damage_on", "Enables Earth's damage")]
        public static void EarthDamageOn()
        {
            EarthEventCaller.EnableDamage();
        }

        [ConsoleMethod("earth_damage_off", "Disables Earth's damage")]
        public static void EarthDamageOff()
        {
            EarthEventCaller.DisableDamage();
        }

        [ConsoleMethod("earth_kill", "Kills Earth")]
        public static void EarthDeath()
        {
            for (int i = 0; i < 10; i++)
            {
                ProjectileCommand.Collision(ProjectileType.Meteor);
            }
        }

        [ConsoleMethod("earth_preslice", "Pre Slices Earth")]
        public static void EarthPreSlice()
        {
            EarthEventCaller.PreSlice();
        }
    }
    public static class ShieldCommands
    {
        [ConsoleMethod("shield_cmd_help", "List of Shield commands")]
        public static void ShieldCommandHelp()
        {
            Debug.Log(
                "=== SHIELD COMMANDS ===\n" +
                "shield_enable\n" +
                "shield_disable\n" +
                "shield_enable_type <ShieldType>\n" +
                "shield_disable_type <ShieldType>"
            );
        }
        
        [ConsoleMethod("shield_help", "List of Shield Types")]
        public static void ShieldHelp()
        {
            Debug.Log(
                "=== Shield Types ===\n" +
                "Super\n" +
                "Gold\n" +
                "Automatic\n" +
                "Slow\n");
        }

        [ConsoleMethod("shield_enable", "Enables Shield")]
        public static void ShieldEnable()
        {
            ShieldEventCaller.Enable();
        }

        [ConsoleMethod("shield_disable", "Disables Shield")]
        public static void ShieldDisable()
        {
            ShieldEventCaller.Disable();
        }

        [ConsoleMethod("shield_enable_type", "Enables a shield specific type")]
        public static void ShieldEnableType(ShieldType type)
        {
            ShieldEventCaller.RequestEnableShieldType(type);
        }

        [ConsoleMethod("shield_disable_type", "Disables a shield specific type")]
        public static void ShieldDisableType(ShieldType type)
        {
            ShieldEventCaller.RequestDisableShieldType(type);
        }
    }
    public static class CameraCommands
    {
        [ConsoleMethod("cam_help", "List of Camera commands")]
        public static void CameraHelp()
        {
            Debug.Log(
                "=== CAMERA COMMANDS ===\n" +
                "cam_zoomin <time>\n" +
                "cam_zoomout <time>\n" +
                "cam_center <time>\n" +
                "cam_right <time>\n" +
                "cam_left <time>\n" +
                "cam_up <time>\n" +
                "cam_down <time>"
            );
        }
        
        [ConsoleMethod("cam_zoomin", "Zoom in")]
        public static void CamZoomIn(float time = 0.5f)
        {
            CameraEventCaller.ZoomIn(time);
        }

        [ConsoleMethod("cam_zoomout", "Zoom out")]
        public static void CamZoomOut(float time = 0.5f)
        {
            CameraEventCaller.ZoomOut(time);
        }

        [ConsoleMethod("cam_center", "Look center")]
        public static void CamCenter(float time = 0.5f)
        {
            CameraEventCaller.LookCenter(time);
        }

        [ConsoleMethod("cam_right", "Look right")]
        public static void CamRight(float time = 0.5f)
        {
            CameraEventCaller.LookRight(time);
        }

        [ConsoleMethod("cam_left", "Look left")]
        public static void CamLeft(float time = 0.5f)
        {
            CameraEventCaller.LookLeft(time);
        }

        [ConsoleMethod("cam_up", "Look up")]
        public static void CamUp(float time = 0.5f)
        {
            CameraEventCaller.LookUp(time);
        }

        [ConsoleMethod("cam_down", "Look down")]
        public static void CamDown(float time = 0.5f)
        {
            CameraEventCaller.LookDown(time);
        }
    }
    public static class InputCommands
    {
        [ConsoleMethod("input_help", "List of Input commands")]
        public static void InputHelp()
        {
            Debug.Log(
                "=== INPUT COMMANDS ===\n" +
                "input_enable <true/false>\n" +
                "ui_enable <true/false>"
            );
        }
        
        [ConsoleMethod("input_enable", "Sets enable inputs")]
        public static void InputEnable(bool value)
        {
            InputsEventCaller.SetEnable(value);
        }

        [ConsoleMethod("ui_enable", "Sets enable UI")]
        public static void UIEnable(bool value)
        {
            InputsEventCaller.SetUIEnable(value);
        }
    }
    public static class AbilitiesCommands
    {
        [ConsoleMethod("ability_cmd_help", "List of Ability commands")]
        public static void AbilityCommandHelp()
        {
            Debug.Log(
                "=== ABILITY COMMANDS ===\n" +
                "ability_canuse <true/false>\n" +
                "ability_enable\n" +
                "ability_disable\n" +
                "ability_timer\n" +
                "ability_next <AbilityType>\n" +
                "ability_send <AbilityType>"
            );
        }
        
        [ConsoleMethod("ability_help", "List of Abilities Types")]
        public static void AbilityHelp()
        {
            Debug.Log(
                "=== Abilities Types ===\n" +
                "SuperShield\n" +
                "Health\n" +
                "SlowMotion\n" +
                "Automatic\n" +
                "DoublePoints\n");
        }
        
        [ConsoleMethod("ability_canuse", "Set if can use abilities")]
        public static void AbilityCanUse(bool value)
        {
            AbilitiesEventCaller.SetCanUse(value);
        }

        [ConsoleMethod("ability_enable", "Enables ability system")]
        public static void AbilityEnable()
        {
            AbilitiesEventCaller.Enable();
        }

        [ConsoleMethod("ability_disable", "Disables ability system")]
        public static void AbilityDisable()
        {
            AbilitiesEventCaller.Disable();
        }

        [ConsoleMethod("ability_timer", "Runs ability timer")]
        public static void AbilityTimer()
        {
            AbilitiesEventCaller.RunTimer();
        }

        [ConsoleMethod("ability_next", "Sets the next ability")]
        public static void AbilityNext(AbilityType type)
        {
            AbilitiesEventCaller.SetNextSpawn(type);
        }

        [ConsoleMethod("ability_send", "Spawns an Ability Sphere")]
        public static void SendAbility(AbilityType type)
        {
            AbilitiesEventCaller.SetNextSpawn(type);
            ProjectileEventCaller.GrantSpawn(ProjectileType.AbilitySphere);
        }

    }
    public static class ProjectileCommand
    {
        [ConsoleMethod("proj_help", "List of Projectile commands")]
        public static void ProjectileHelp()
        {
            Debug.Log(
                "=== PROJECTILE COMMANDS ===\n" +
                "proj_collision <ProjectileType>\n" +
                "proj_deflected <ProjectileType> <byte>\n" +
                "proj_spawn_on\n" +
                "proj_spawn_off\n" +
                "proj_clear\n" +
                "proj_level <int>"
            );
        }
        
        [ConsoleMethod("proj_collision", "Executes a projectile collision")]
        public static void Collision(ProjectileType projectileType)
        {
            ProjectileEventCaller.Collision(new CollisionData
            {
                Position = Vector3.zero,
                Direction = Vector2.zero,
                Rotation = Quaternion.identity,
                Type = projectileType
            });
        }
        
        [ConsoleMethod("proj_deflected", "Executes a projectile deflection")]
        public static void Deflected(ProjectileType projectileType, byte projectileValue)
        {
            ProjectileEventCaller.Deflected(new DeflectData
            {
                Position = Vector3.zero,
                Direction = Vector2.zero,
                Rotation = Quaternion.identity,
                Type = projectileType,
                Value = projectileValue
            });
        }
        
        [ConsoleMethod("proj_spawn_on", "Enables projectile spawning")]
        public static void EnableProjectileSpawn()
        {
            ProjectileEventCaller.EnableSpawn();
        }

        [ConsoleMethod("proj_spawn_off", "Disables projectile spawning")]
        public static void DisableProjectileSpawn()
        {
            ProjectileEventCaller.DisableSpawn();
        }
        
        [ConsoleMethod("proj_clear", "Clears projectile spawn queue")]
        public static void ClearProjectileQueue()
        {
            ProjectileEventCaller.ClearQueue();
        }
        
        [ConsoleMethod("proj_level", "Updates projectile system level")]
        public static void UpdateProjectileLevel(int level)
        {
            ProjectileEventCaller.UpdateLevel(level);
        }
    }
    public static class MeteorCommand
    {
        [ConsoleMethod("meteor_help", "List of Meteor commands")]
        public static void MeteorHelp()
        {
            Debug.Log(
                "=== METEOR COMMANDS ===\n" +
                "meteor_single\n" +
                "meteor_ring"
            );
        }
        
                
        [ConsoleMethod("meteor_single", "Spawns a meteor")]
        public static void GrantProjectile()
        {
            MeteorEventCaller.GrantSpawnSingle();
        }
        
        [ConsoleMethod("meteor_ring", "Spawns a meteor ring")]
        public static void SpawnRing()
        {
            MeteorEventCaller.SpawnRing();
        }
    }
    public static class GameScreenCommand
    {
        [ConsoleMethod("screen_cmd_help", "List of Screen commands")]
        public static void ScreenCommandHelp()
        {
            Debug.Log(
                "=== SCREEN COMMANDS ===\n" +
                "screen_open <ScreenType>\n" +
                "screen_last"
            );
        }
        
        [ConsoleMethod("screen_help", "List of Game Screens")]
        public static void ScreenHelp()
        {
            Debug.Log(
                "=== Game Screens ===\n" +
                "MainMenu\n" +
                "GameMode\n" +
                "Tutorial\n" +
                "Cosmetic\n" +
                "OptionsMenu\n" +
                "Defeat\n" +
                "Pause\n" +
                "Stats\n"
                );
        }

        [ConsoleMethod("screen_open", "Open Selected Screen")]
        public static void OpenScreen(ScreenType screenType)
        {
            GameScreenEventCaller.EnableScreen(screenType, EventRequestType.Requested);
        }
        
        [ConsoleMethod("screen_last", "Open Last Screen")]
        public static void LoadLastScreen()
        {
            GameScreenEventCaller.LoadLastScreen();
        }
    }
    public static class SkinCommands
    {
        [ConsoleMethod("skin_cmd_help", "List of Shield commands")]
        public static void SkinCommandHelp()
        {
            Debug.Log(
                "=== Skin COMMANDS ===\n" +
                "skin_select <SkinType>\n" +
                "skin_help\n"
            );
        }
        
        [ConsoleMethod("skin_help", "List of Skin Types")]
        public static void ShieldHelp()
        {
            Debug.Log(
                "=== Skin Types ===\n" +
                "Default\n" +
                "Pizza\n"
                );
        }
        
        [ConsoleMethod("skin_select", "List of Skin Types")]
        public static void SelectSkin(SkinType type)
        {
            SkinManager.Instance.ForceSkin(type);
        }
    }
    public static class SettingsCommands
    {
        [ConsoleMethod("settings_cmd_help", "List of Settings commands")]
        public static void SettingsCommandHelp()
        {
            Debug.Log(
                "=== Settings COMMANDS ===\n" +
                "settings_language <SystemLanguage>\n" +
                "settings_volume <float>\n" +
                "settings_vibration <bool>\n" +
                "settings_help" 
            );
        }
        
        [ConsoleMethod("settings_help", "List of Languages")]
        public static void SettingsHelp()
        {
            Debug.Log(
                "=== Languages ===\n" +
                "English\n" +
                "Spanish\n" +
                "French\n" +
                "Portuguese\n" +
                "Italian\n" +
                "German"
            );
        }
        
        [ConsoleMethod("settings_language", "Selects language")]
        public static void SelectLanguage(SystemLanguage index)
        {
            SettingsManager.Instance.SetLanguageIndex((int)index);
        }
        
        [ConsoleMethod("settings_volume", "Sets volume")]
        public static void SetVolume(float volume)
        {
            SettingsManager.Instance.SetMasterVolume(volume);
        }

        [ConsoleMethod("settings_vibration", "Set enables vibration")]
        public static void SetVibration(bool enable)
        {
            SettingsManager.Instance.SetVibration(enable);
        }
    }
    public static class SaveCommands
    {
        [ConsoleMethod("save_cmd_help", "List of Settings commands")]
        public static void SettingsCommandHelp()
        {
            Debug.Log(
                "=== Save COMMANDS ===\n" +
                "save_erase <SystemLanguage>\n"
            );
        }
        
        [ConsoleMethod("save_erase", "Clear Save Data")]
        public static void EraseSave()
        {
            DataManager.Instance.ClearSaveData();
        }
    }
    public static class TimeScaleCommands
    {
        [ConsoleMethod("time_cmd_help", "List of Settings commands")]
        public static void SettingsCommandHelp()
        {
            Debug.Log(
                "=== Time COMMANDS ===\n" +
                "time_scale <float>\n" +
                "time_pause \n" +
                "time_resume\n"
            );
        }
        
        [ConsoleMethod("time_scale", "Set Time Scale")]
        public static void SetTimeScale(float timeScale)
        {
            timeScale = Mathf.Clamp01(timeScale);
            CustomTime.GlobalTimeScale = timeScale;
        }
        
        [ConsoleMethod("time_pause", "Pauses Global Time")]
        public static void SetPause()
        {
            CustomTime.GlobalTimeScale = 0f;
        }
        
        [ConsoleMethod("time_resume", "Resumes Global Time")]
        public static void SetResume()
        {
            CustomTime.GlobalTimeScale = 1f;
        }
    }

    // ========================= //
    public static class CommandGlobal
    {
        [ConsoleMethod("help", "List of all command groups")]
        public static void GlobalHelp()
        {
            Debug.Log(
                "=== COMMAND GROUPS ===\n" +
                "gm_cmd_help\n" +
                "earth_cmd_help\n" +
                "shield_cmd_help\n" +
                "shield_cmd_help\n" +
                "cam_cmd_help\n" +
                "input_cmd_help\n" +
                "ability_cmd_cmd_help\n" +
                "ability_cmd_help\n" +
                "proj_cmd_help\n" +
                "meteor_cmd_help\n" +
                "screen_cmd_help\n" +
                "screen_cmd_help\n" +
                "skin_cmd_help\n" +
                "settings_cmd_help\n" +
                "save_cmd_help\n" +
                "time_cmd_help\n"
            );
        }
    }
    
#endif
}
using C3;
using Unfoundry;
using UnityEngine;

namespace WaterPlacer
{
    public class WaterPlacerCHM : CustomHandheldMode
    {
        private LiquidTemplate _waterTemplate = null;
        private byte _waterIndex = 0;

        private string GetHelpText()
        {
            return $"Press {GameRoot.getHotkeyStringFromAction("Action")} to place water.\nPress {GameRoot.getHotkeyStringFromAction("Alternate Action")} to flood fill an area.";
        }

        public override void Enter()
        {
            if (_waterTemplate == null)
            {
                AssetManager.tryGetAsset("lt__base_water", out _waterTemplate);
                _waterIndex = (byte)GameRoot.LiquidIdxLookupTable.getKeyByValue(_waterTemplate.id);
            }
        }

        public override void Exit()
        {
            GameRoot.setInfoText("");
        }

        public override void UpdateBehavoir()
        {
            var helpText = GetHelpText();
            GameRoot.setInfoText(helpText);
            TabletHelper.SetTabletTextAnalyzer("Water Placer");
            TabletHelper.SetTabletTextQuickActions(helpText);
            TabletHelper.SetTabletTextLastCopiedConfig("");

            if (_waterTemplate == null) return;

            if (GameRoot.World.Systems.Get<RaycastHelperSystem>().raycastFromCameraToTerrain(out var worldPos, out var worldCellPos))
            {
                GameRoot.pushPerFrameHighlighterBox((Vector3)worldCellPos + new Vector3(0.5f, 0.5f, 0.5f), Vector3.one, 1);

                if (GlobalStateManager.getRewiredPlayer0().GetButtonUp("Action"))
                {
                    GameRoot.addLockstepEvent(new SetLiquidCellEvent(worldCellPos.x, worldCellPos.y, worldCellPos.z, _waterIndex, byte.MaxValue));
                    if (Plugin.configPlaySounds.Get())
                    {
                        var audioClipArray = ResourceDB.resourceLinker.audioClip_liquidExitSound;
                        AudioManager.playUISoundEffect(audioClipArray[Random.Range(0, audioClipArray.Length)]);
                    }
                }
                else if (GlobalStateManager.getRewiredPlayer0().GetButtonUp("Alternate Action"))
                {
                    GameRoot.addLockstepEvent(new LiquidFloodFillEvent(worldCellPos.x, worldCellPos.y, worldCellPos.z, _waterIndex, byte.MaxValue));
                    if (Plugin.configPlaySounds.Get())
                    {
                        var audioClipArray = ResourceDB.resourceLinker.audioClip_liquidExitSound;
                        AudioManager.playUISoundEffect(audioClipArray[Random.Range(0, audioClipArray.Length)]);
                    }
                }
            }
        }

        public override bool OnRotateY() => false;

        public override void ShowMenu()
        {
        }
    }
}

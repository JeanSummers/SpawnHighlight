using System;
using System.Collections.Generic;
using System.Threading;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace SpawnHighlight
{
    public class SpawnHighlightModSystem : ModSystem
    {
        private ICoreClientAPI api;
        private Config config;
        private Thread thread;
        private Commands commands;
        private IBlockAccessor blockAccessor;
        private bool enabled = false;

        public override bool ShouldLoad(EnumAppSide forSide) => forSide == EnumAppSide.Client;

        public override void StartClientSide(ICoreClientAPI api)
        {
            this.api = api;
            config = new Config(api, Mod);
            commands = new Commands(api, config, ToggleRun);

            RegisterHotkey();
        }

        private void RegisterHotkey()
        {
            api.Input.RegisterHotKey(config.HotkeyCode, config.HotkeyDescriptionString, GlKeys.L, type: HotkeyType.HelpAndOverlays, ctrlPressed: true);
            api.Input.SetHotKeyHandler(config.HotkeyCode, OnHotkey);
        }

        private bool OnHotkey(KeyCombination _)
        {
            ToggleRun();
            return true;
        }

        private void ToggleRun()
        {
            // Prevent starting new thread before old one has ended
            if (!enabled && thread?.IsAlive == true) return;

            api.ShowChatMessage(config.EnabledString(enabled));

            enabled = !enabled;
            if (!enabled) return;

            blockAccessor = api.World.GetLockFreeBlockAccessor();

            thread = new Thread(RunThread)
            {
                IsBackground = true,
                Name = config.ThreadName
            };
            thread.Start();
        }

        private void RunThread()
        {
            while (enabled)
            {
                try
                {
                    RenderHighlights();
                }
                catch (Exception ex)
                {
                    api.Logger.Error(ex);
                }

                Thread.Sleep(250);
            }

            ClearHighlights();
        }

        private void RenderHighlights()
        {
            var position = api.World.Player.Entity.Pos.AsBlockPos;

            List<BlockPos> positions = new();
            List<int> colors = new();

            var r = config.Radius;
            var px = position.X;
            var py = position.Y;
            var pz = position.Z;

            blockAccessor.WalkBlocks(
                new(px - r, py - r, pz - r),
                new(px + r, py + r, pz + r),
                (block, x, y, z) =>
                {
                    if (block.Id == 0) return;
                    if (!block.SideSolid[BlockFacing.UP.Index]) return;

                    var blockPosition = new BlockPos(x, y, z);

                    var upperBlock = blockAccessor.GetBlock(blockPosition.UpCopy());
                    if (upperBlock.Id != 0 && !upperBlock.Code.Path.StartsWith("tallgrass-")) return;

                    var lightLevel = api.World.BlockAccessor.GetLightLevel(blockPosition, EnumLightLevelType.OnlyBlockLight);

                    positions.Add(blockPosition);
                    colors.Add(lightLevel > 6 ? config.SafeColor : config.SpawnableColor);
                });

            ShowHighlights(positions, colors);
        }

        private void ShowHighlights(List<BlockPos> positions, List<int> colors)
        {
            api.Event.EnqueueMainThreadTask(() => api.World.HighlightBlocks(api.World.Player, config.HighlighSlot, positions, colors), config.TaskCode);
        }
        private void ClearHighlights()
        {
            api.Event.EnqueueMainThreadTask(() => api.World.HighlightBlocks(api.World.Player, config.HighlighSlot, new List<BlockPos>()), config.TaskCode);
        }
    }
}

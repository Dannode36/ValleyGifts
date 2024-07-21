using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SMUI.Elements;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace ValleyGifts
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        private List<Image> villagerIcons = new();

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Display.RenderedWorld += OnRenderedWorld;

            var iconTex = helper.ModContent.Load<Texture2D>("assets/icons.png");

            int icons = iconTex.Height / 32;

            for (int i = 0; i < icons; i++)
            {
                villagerIcons.Add(new()
                {
                    Texture = helper.ModContent.Load<Texture2D>("assets/icons.png"),
                    TextureArea = new(0, i * 32, 32, 32),
                    LocalPosition = new(i * 64, 0),
                    Scale = 2f
                });
            }
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
        }

        private void OnRenderedWorld(object? sender, RenderedWorldEventArgs e)
        {
            foreach (var icon in villagerIcons)
            {
                icon.Draw(e.SpriteBatch);
            }
        }
    }
}

using System;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SMUI.Elements;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace ValleyGifts
{
    //Potentially helpful functions:
    // Utility.getTodaysBirthdayNPC()
    // Utility.ForEachVillager()

    public class Icon
    {
        public Image Image { get; set; }
        public string Name { get; set; }
        public int GiftsToday { get; set; }
        public int GiftsThisWeek { get; set; }

        public Icon(Image image, string name)
        {
            Image = image;
            Name = name;
        }
    }

    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        Dictionary<int, string> IconMap = new();
        private List<Icon> villagerIcons = new();
        Image GiftIcon;
        Image IridiumStarIcon;
        Texture2D background;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Display.RenderedWorld += OnRenderedWorld;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;

            IconMap = helper.Data.ReadJsonFile<Dictionary<int, string>>("assets/icons.json") ?? new();

            var iconTex = helper.ModContent.Load<Texture2D>("assets/icons.png");

            int icons = iconTex.Height / 32;

            for (int i = 0; i < icons; i++)
            {
                villagerIcons.Add(new(new()
                {
                    Texture = helper.ModContent.Load<Texture2D>("assets/icons.png"),
                    TextureArea = new(0, i * 32, 32, 32),
                    LocalPosition = new((i % 8) * 64, i / 8 * 64),
                    Scale = 2f
                }, IconMap[i]));
            }

            GiftIcon = new()
            {
                Texture = helper.ModContent.Load<Texture2D>("assets/gift.png"),
                TextureArea = new(0, 0, 10, 10),
                Scale = 3f
            };

            IridiumStarIcon = new()
            {
                TextureArea = new(346, 392, 8, 8),
                Scale = 2f
            };

            background = helper.ModContent.Load<Texture2D>("assets/white.png");
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            IridiumStarIcon.Texture = Game1.mouseCursors;
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            int npcsUpdated = 0;
            Utility.ForEachVillager((npc) =>
            {
                int i = villagerIcons.FindIndex(x => x.Name == npc.Name);

                if(i > -1)
                {
                    if (Game1.player.friendshipData.TryGetValue(npc.Name, out var friendship))
                    {
                        npcsUpdated++;
                        villagerIcons[i].GiftsToday = friendship.GiftsToday;
                        villagerIcons[i].GiftsThisWeek = friendship.GiftsThisWeek;
                    }
                    else
                    {
                        Monitor.LogOnce("Could not find friendship data for " + npc.Name, LogLevel.Error);
                    }
                }

                return npcsUpdated < IconMap.Count;
            });
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
            IClickableMenu.drawTextureBox(e.SpriteBatch, background, new(0, 0, 10, 10), 0, 0, 512, 512, Color.Transparent, 1f, drawShadow: false);

            foreach (var icon in villagerIcons)
            {
                icon.Image.Draw(e.SpriteBatch);

                GiftIcon.LocalPosition = icon.Image.LocalPosition + new Vector2(icon.Image.Width - GiftIcon.Width, icon.Image.Height - GiftIcon.Height);
                GiftIcon.Draw(e.SpriteBatch);
                
            }
        }
    }
}

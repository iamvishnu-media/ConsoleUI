﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUI {
    public abstract class Scene {
        protected Dictionary<string,Sprite> SpriteHash;
        protected bool Rerender;

        protected internal Scene Subscene = null;
        internal Scene Superscene = null;

        internal bool Hidden = false;
        internal string Buffer = "";

        public Scene() {
            SpriteHash = new Dictionary<string, Sprite>();
            Rerender = true;
        }

        /// <summary>
        /// Main loop of the scene. Allows conditionally calling the update function depending on which scene is on top of the stack.
        /// </summary>
        public void Main() {
            if (Subscene != null && !Subscene.Hidden) Subscene.Main();
            else {
                if (Rerender) Render();
                Update();
            }
        }

        /// <summary>
        /// Updates the state of the scene and processes input.
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Updates the graphics of the scene. If any subscenes are present, they are rendered instead.
        /// </summary>
        public string Render(bool print = true) {
            if (print) Console.Clear();
            StringBuilder buffer = new StringBuilder();
            if (Subscene != null) buffer.Append(Subscene.Render());
            else foreach (string spritename in SpriteHash.Keys) if (SpriteHash[spritename] != null) buffer.AppendLine(SpriteHash[spritename].Render());
            if (print) Console.Write(buffer.ToString());
            Rerender = false;
            return buffer.ToString();
        }

        /// <summary>
        /// Adds a subscene to the top of the scene stack.
        /// </summary>
        /// <param name="s">The scene to render on top.</param>
        public void AddSubscene(Scene s) { AddSubscene(s, false); }
        public void AddSubscene(Scene s,bool immediate) {
            if (!immediate && Subscene != null) Subscene.AddSubscene(s);
            else {
                Subscene = s;
                s.Superscene = this;
            }
            Console.Clear();
        }

        /// <summary>
        /// Ends the subscene that is on top of the scene stack
        /// </summary>
        public void EndSubscene() { EndSubscene(false); }
        public void EndSubscene(bool immediate) {
            if (!immediate && Subscene != null) Subscene.EndSubscene();
            else {
                EndScene();
                Rerender = true;
            }
            Console.Clear();
        }

        /// <summary>
        /// Ends this scene and any subscenes. If this is the base scene, the entire scene stack is unloaded.
        /// </summary>
        public void EndScene() {
            if (Superscene != null) {
                Superscene.Subscene = null;
                Superscene.Rerender = true;
                Console.Clear();
            }
        }

        /// <summary>
        /// Moves the scene at the current level to a new scene, ending this scene.
        /// </summary>
        /// <param name="s">The scene to switch to.</param>
        public void NextScene(Scene s) {
            if (Superscene != null) {
                Superscene.Subscene = s;
                Console.Clear();
            }
        }

        public void Hide() {
            Hidden = true;
            if (Superscene != null) Superscene.Rerender = true;
        }

        /// <summary>
        /// Gets the scene on the top of the stack (the one currently being rendered, and the subscene of all subscenes).
        /// </summary>
        /// <returns>The scene at the top of the stack, even if it's this scene.</returns>
        public Scene GetTopScene() {
            if (Subscene == null) return this;
            else return Subscene.GetTopScene();
        }
    }
}

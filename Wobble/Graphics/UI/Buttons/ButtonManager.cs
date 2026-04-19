using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emik;

namespace Wobble.Graphics.UI.Buttons
{
    public static class ButtonManager
    {
        /// <summary>
        ///     The list of buttons that are currently drawn.
        /// </summary>
        public static Concurrent.List<Button> Buttons { get; } = new Concurrent.List<Button>();

        /// <summary>
        ///     A stack of drawables that are currently serving as the input root.
        ///     If there is anything in this stack, only descendants of the top-most
        ///     drawable will be considered for input.
        /// </summary>
        public static Stack<Drawable> InputStack { get; } = new Stack<Drawable>();

        /// <summary>
        ///     Pushes a new input root onto the stack.
        /// </summary>
        /// <param name="root"></param>
        public static void PushInputRoot(Drawable root)
        {
            if (root == null || root.IsDisposed)
                return;

            lock (InputStack)
                InputStack.Push(root);
        }

        /// <summary>
        ///     Pops the current input root from the stack.
        /// </summary>
        public static void PopInputRoot()
        {
            lock (InputStack)
            {
                if (InputStack.Count > 0)
                    InputStack.Pop();
            }
        }

        /// <summary>
        ///     Returns the current active input root, if any.
        ///     Also cleans up any disposed roots.
        /// </summary>
        /// <returns></returns>
        public static Drawable GetActiveRoot()
        {
            lock (InputStack)
            {
                while (InputStack.Count > 0)
                {
                    var root = InputStack.Peek();
                    if (root == null || root.IsDisposed || !root.Visible)
                    {
                        InputStack.Pop();
                        continue;
                    }

                    return root;
                }
            }

            return null;
        }


        /// <summary>
        ///     Adds a button to the manager.
        /// </summary>
        /// <param name="btn"></param>
        public static void Add(Button btn)
        {
            Buttons.Add(btn);
        }

        /// <summary>
        ///     Removes a button from the manager.
        /// </summary>
        /// <param name="btn"></param>
        public static void Remove(Button btn)
        {
            Buttons.Remove(btn);
        }

        public static void ResetDrawOrder()
        {
            lock (Buttons)
            {
                foreach (var b in Buttons)
                {
                    if (b == null)
                        continue;

                    b.DrawOrder = 0;
                }
            }
        }
    }
}

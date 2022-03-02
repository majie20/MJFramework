#if UNITY_EDITOR

using UnityEditor;
using System.Collections;

namespace APlus
{
    /// <summary>
    /// Gives the possibility to use coroutines in editorscripts.
    /// from https://github.com/FelixEngl/EditorCoroutines/blob/master/Editor/EditorCoroutine.cs
    /// </summary>
    public class EditorCoroutine
    {

        //The given coroutine
        //
        readonly IEnumerator routine;

        //The subroutine of the given coroutine
        private IEnumerator internalRoutine;

        //Constructor
        EditorCoroutine(IEnumerator routine)
        {
            this.routine = routine;
        }


        #region static functions
        /// <summary>
        /// Starts a new EditorCoroutine.
        /// </summary>
        /// <param name="routine">Coroutine</param>
        /// <returns>new EditorCoroutine</returns>
        public static EditorCoroutine StartCoroutine(IEnumerator routine)
        {
            EditorCoroutine coroutine = new EditorCoroutine(routine);
            coroutine.Start();
            return coroutine;
        }

        /// <summary>
        /// Clears the EditorApplication.update delegate by setting it null
        /// </summary>
        public static void ClearEditorUpdate()
        {
            EditorApplication.update = null;
        }

        #endregion



        //Delegate to EditorUpdate
        void Start()
        {
            EditorApplication.update += Update;
        }

        //Undelegate
        public void Stop()
        {
            if (EditorApplication.update != null)
                EditorApplication.update -= Update;

        }

        //Updatefunction
        void Update()
        {

            //if the internal routine is null
            if (internalRoutine == null)
            {
                //if given routine doesn't continue
                if (!routine.MoveNext())
                {
                    Stop();
                }
            }

            if (internalRoutine != null)
            {
                if (!internalRoutine.MoveNext())
                {
                    internalRoutine = null;
                }
                if (internalRoutine.Current != null && (bool)internalRoutine.Current)
                {
                    internalRoutine = null;
                }
            }
        }

        //IEnumerator for a EditorYieldInstruction, false if EditorYieldInstruction is false, else true and leaving
        private IEnumerator isTrue(EditorYieldInstruction editorYieldInstruction)
        {
            while (!editorYieldInstruction.IsDone)
            {
                yield return false;
            }
            yield return true;
        }
    }

    /// <summary>
    /// Abstract Class for a EditorYieldInstruction.
    /// Be careful with the abstract function: <see cref="InternalLogic"/>
    /// </summary>
    public abstract class EditorYieldInstruction
    {
        //EditorYieldInstruction done?
        private bool isDone = false;

        //internal logik routine of the EditorYieldInstruction
        readonly IEnumerator routine;

        /// <summary>
        /// Updates the EditorYieldInstruction and returns it's state. True if done.
        /// </summary>
        internal bool IsDone
        {
            get { Update(); return isDone; }
        }


        //basic constructor
        protected internal EditorYieldInstruction()
        {
            routine = InternalLogic();
        }

        //internal updatefunction, called with readonly
        protected internal void Update()
        {
            if (routine != null)
            {
                if (routine.MoveNext())
                {
                    if (routine.Current != null)
                        isDone = (bool)routine.Current;
                }

            }
        }

        /// <summary>
        /// Internal logic routine of the EditorYieldInstruction.
        /// yield return false when not finished
        /// yield return true when finished.
        /// </summary>
        /// <returns>IEnumerator with true for done and false for not done</returns>
        protected internal abstract IEnumerator InternalLogic();
    }
}
#endif
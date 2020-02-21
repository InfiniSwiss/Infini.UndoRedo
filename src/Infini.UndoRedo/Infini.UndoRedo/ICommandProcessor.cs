﻿namespace Infini.UndoRedo
{
    using Infini.UndoRedo.Commands;

    /// <summary>
    /// The command processor's interface.
    /// </summary>
    public interface ICommandProcessor
    {
        /// <summary>
        /// Adds and executes a command in the current processor.
        /// </summary>
        /// <param name="command">The command to be added and executed.</param>
        void AddAndExecute(IUndoRedoCommand command);

        /// <summary>
        /// Reverts the last command executed.
        /// </summary>
        void Undo();

        /// <summary>
        /// Executes the last command reverted.
        /// </summary>
        void Redo();
    }
}
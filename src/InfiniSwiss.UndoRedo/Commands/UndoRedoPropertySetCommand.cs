﻿using System;
using System.Linq.Expressions;
using System.Reflection;

namespace InfiniSwiss.UndoRedo.Commands
{
    /// <summary>
    /// Represents a <seealso cref="IUndoRedoCommand"/> which is used to set properties on a specific element of type <see cref="TState" />.
    /// </summary>
    /// <typeparam name="TState">The type of the state object.</typeparam>
    public class UndoRedoPropertySetCommand<TState> : IUndoRedoCommand
    {
        private readonly TState state;
        private readonly PropertyInfo propertyInfo;
        private readonly object? newValue;
        private readonly object? oldValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="UndoRedoPropertySetCommand{TState}"/> class.
        /// </summary>
        /// <param name="state">The state object.</param>
        /// <param name="propertySelector">The property which needs to be set.</param>
        /// <param name="newValue">The value which needs to be set.</param>
        public UndoRedoPropertySetCommand(TState state, Expression<Func<TState, object>> propertySelector, object? newValue)
        {
            var propertyBody = propertySelector.Body;

            // Sometimes, a convert can occur in the expression tree.
            if (propertyBody is UnaryExpression unaryExpression)
            {
                propertyBody = unaryExpression.Operand;
            }

            if (!(propertyBody is MemberExpression memberExpression && memberExpression.Member is PropertyInfo propertyInfo))
            {
                throw new ArgumentException("The property selector must point to a property.");
            }

            if (state == null)
            {
                throw new NullReferenceException("The state cannot be null");
            }

            this.state = state;
            this.propertyInfo = propertyInfo;
            this.newValue = newValue;
            this.oldValue = propertyInfo.GetValue(state);

            if (Equals(this.oldValue, newValue))
            {
                this.IsRedundant = true;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UndoRedoPropertySetCommand{TState}"/> class.
        /// </summary>
        /// <param name="state">The state object.</param>
        /// <param name="propertyName">The property name of the propertiy to be set.</param>
        /// <param name="newValue">The value which needs to be set.</param>
        public UndoRedoPropertySetCommand(TState state, string propertyName, object? newValue)
        {
            if (state == null)
            {
                throw new NullReferenceException("The state cannot be null");
            }

            this.state = state;
            this.propertyInfo = this.state.GetType().GetProperty(propertyName);
            this.newValue = newValue;
            this.oldValue = this.propertyInfo.GetValue(state);

            if (Equals(this.oldValue, newValue))
            {
                this.IsRedundant = true;
            }
        }

        /// <inheritdoc/>
        public bool IsRedundant { get; } = false;

        /// <inheritdoc/>
        public void Execute()
        {
            this.propertyInfo.SetValue(this.state, this.newValue);
        }

        /// <inheritdoc/>
        public void Revert()
        {
            this.propertyInfo.SetValue(this.state, this.oldValue);
        }
    }
}

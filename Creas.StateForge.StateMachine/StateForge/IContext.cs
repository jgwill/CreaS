#region Copyright
//------------------------------------------------------------------------------
// <copyright file="IContext.cs" company="StateForge">
//      Copyright (c) 2010-2012 StateForge.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
#endregion

namespace StateForge.StateMachine
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Runtime.Serialization;

  /// <summary>
  /// This is the interface for context classes
  /// </summary>
  public abstract class ContextBase
  {
    /// <summary>
    /// The list of sub contexts in case of parallel state machine.
    /// </summary>
    private List<ContextBase> contextList;

    /// <summary>
    /// The eventual Observer.
    /// </summary>
    private IObserver observer;

    /// <summary>
    /// Event to indicate the end of the state machine.
    /// </summary>
    public event EventHandler<EventArgs> EndHandler;

    /// <summary>
    /// Gets or sets the eventual current parallel context.
    /// </summary>
    public ContextParallel ContextParallel { get; set; }

    /// <summary>
    /// Gets or sets the context name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the name of the current transition.
    /// </summary>
    public string TransitionName { get; set; }

    /// <summary>
    /// Gets or sets the eventual observer.
    /// </summary>
    public IObserver Observer
    {
      get
      {
        return this.observer;
      }

      set
      {
        this.observer = value;
        if (this.contextList != null)
        {
          foreach (ContextBase context in this.contextList)
          {
            context.Observer = value;
          }
        }
      }
    }

    /// <summary>
    /// Enter the initial state, that calls the OnEntry functions from the top to the initial state.
    /// </summary>
    public virtual void EnterInitialState()
    {
    }

    /// <summary>
    ///Set the current state from its name
    /// </summary>
    public virtual void SetState(string stateName)
    {

    }

    /// <summary>
    /// Invoked when the context ends.
    /// </summary>
    public virtual void OnEnd()
    {
      if (this.EndHandler != null)
      {
        this.EndHandler(this, EventArgs.Empty);
      }
    }

    /// <summary>
    /// Register the end handler
    /// </summary>
    /// <param name="endHandler"></param>
    public void RegisterEndHandler(EventHandler<EventArgs> endHandler)
    {
      this.EndHandler += endHandler;
    }

    /// <summary>
    /// Add a child context.
    /// </summary>
    /// <param name="childContext"></param>
    public void AddChild(ContextBase childContext)
    {
      if (this.contextList == null)
      {
        this.contextList = new List<ContextBase>();
      }

      this.contextList.Add(childContext);
    }

    /// <summary>
    /// Serialize the context.
    /// </summary>
    /// <param name="streamWriter"></param>
    public abstract void Serialize(StreamWriter streamWriter);

    /// <summary>
    /// Serialize a single state.
    /// </summary>
    /// <param name="streamWriter"></param>
    protected virtual void SerializeState(StreamWriter streamWriter)
    {

    }

    /// <summary>
    /// Serialize a parallel context
    /// </summary>
    /// <param name="streamWriter"></param>
    protected void SerializeParallel(StreamWriter streamWriter)
    {
      if (this.contextList != null)
      {
        foreach (ContextBase context in this.contextList)
        {
          context.Serialize(streamWriter);
        }
      }
    }

    /// <summary>
    /// DeSerialize the context.
    /// </summary>
    /// <param name="streamReader"></param>
    public abstract void DeSerialize(StreamReader streamReader);

    /// <summary>
    /// DeSerialize the current state.
    /// </summary>
    /// <param name="streamReader"></param>
    public void DeSerializeState(StreamReader streamReader)
    {
      string stateName = streamReader.ReadLine();

      SetDeserializedState(stateName);
    }

    private void SetDeserializedState(string stateName)
    {
      SetState(stateName);
      Observer.OnEntry(this.Name, stateName);
      fireDeserialized(stateName);
    }

    #region Event StateDeserialized 

    /// <summary>
    /// Event to indicate StateDeserialized from disk
    /// </summary><remarks><see cref="StateDeserializedEventArgs" /></remarks>
    public event EventHandler<StateDeserializedEventArgs> StateDeserialized;
    /// <summary>
    /// Called to signal to subscribers that StateDeserialized from disk
    /// </summary>
    /// <param name="e"></param>
    /// <param name="sender"></param>
    internal virtual void OnStateDeserialized(object sender, StateDeserializedEventArgs e)
    {
      if (StateDeserialized != null)
      {
        StateDeserialized(this, e);
      }
    }
    #region event args StateDeserializedEventArgs   
    /// <summary>
    /// EventArgs for <see cref="StateDeserialized" /> StateDeserialized from disk
    /// </summary>
    public class StateDeserializedEventArgs : EventArgs
    {
      /// <summary>string LastStateName Property</summary>
      public string LastStateName { get; set; }
      /// <summary>Constructor  StateDeserializedEventArgs</summary>
      public StateDeserializedEventArgs(string _LastStateName)
      {
        this.LastStateName = _LastStateName;
      }
    }
    #endregion
    //sample to call and create event
    private void fireDeserialized(string laststate)
    {
      string _LastStateName = laststate; //Param type
      StateDeserializedEventArgs e = new StateDeserializedEventArgs(_LastStateName);
      OnStateDeserialized(this, e);
    }
    #endregion





    /// <summary>
    /// DeSerialize a parallel context
    /// </summary>
    /// <param name="streamReader"></param>
    public void DeSerializeParallel(StreamReader streamReader)
    {
      if (this.contextList != null)
      {
        foreach (ContextBase context in this.contextList)
        {
          context.DeSerialize(streamReader);
        }
      }
    }
  }
}

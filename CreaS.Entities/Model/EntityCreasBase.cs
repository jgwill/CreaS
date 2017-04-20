using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Creas.Entities;
using StateForge.StateMachine;

namespace CreaS.Entities
{

  /// <summary>
  /// Base class for entities manage thru State Context.
  /// </summary>
  /// <typeparam name="TContext"></typeparam>
  public abstract class EntityCreaBase<TContext> : EntityBase
    where TContext : ContextBase
    //where TState : State<TContext, TState>
    //where TContext : Context<TState, TContext>
  {

    protected TContext context;

    /// <summary>
    /// State Context managing the State Chart of the Entity.
    /// </summary>
    public TContext Context
    {
      get { return context; }

    }


    /// <summary>
    /// State Context observer.
    /// </summary>
    public IObserver Observer
    {
      get
      {
        return this.context.Observer;
      }
      set
      {
        this.context.Observer = value;
      }
    }


    #region Save / Load Entity State

    private string getAssemblyName()
    {
      return typeof(TContext).Assembly.GetName().Name;
    }

    private string fn = "statecontext.txt";
    public void Save()
    {
      using (StreamWriter writer =
              new StreamWriter(fn))
      {
        //  this.context.EnterHistoryState()          ;
        this.context.Serialize(writer);

      }

    }

    public void Load()
    {
      //using (StreamReader reader = new StreamReader(fn))
      //{
      //  this.context.DeSerialize(reader);

      //}

      using (StreamReader reader = new StreamReader(fn))
      {
        this.context.DeSerialize(reader);

        // this.context.EnterHistoryState();
        //    this.context.SetState("History");
        // this.context.DeSerializeState(reader);
        // this.context.EnterHistoryState();
      }

    }
    #endregion

    #region Event StateRestored 

    /// <summary>
    /// Event to indicate State restored from dsk
    /// </summary><remarks><see cref="StateRestoredEventArgs" /></remarks>
    public event EventHandler<StateRestoredEventArgs> StateRestored;
    /// <summary>
    /// Called to signal to subscribers that State restored from dsk
    /// </summary>
    /// <param name="e"></param>
    /// <param name="sender"></param>
    internal virtual void OnStateRestored(object sender, StateRestoredEventArgs e)
    {
      if (StateRestored != null)
      {
        StateRestored(this, e);
      }
    }
    #region event args StateRestoredEventArgs   
    /// <summary>
    /// EventArgs for <see cref="StateRestored" /> State restored from dsk
    /// </summary>
    public class StateRestoredEventArgs : EventArgs
    {
      /// <summary>string LastState Property</summary>
      public string LastState { get; set; }
      /// <summary>Constructor  StateRestoredEventArgs</summary>
      public StateRestoredEventArgs(string _LastState)
      {
        this.LastState = _LastState;
      }
    }
    #endregion
    //sample to call and create event
    internal void SetRestoredState(string _LastState)
    {
      //= null ; //Param type
      StateRestoredEventArgs e = new StateRestoredEventArgs(_LastState);

      //DONE in base class 
      /*
       * Observer.OnEntry(this.context.Name,_LastState);
       * this.context.SetState(_LastState);
       *
       * this.context.SetInitialState();
       * */

      OnStateRestored(this, e);
    }
    #endregion


  }
}
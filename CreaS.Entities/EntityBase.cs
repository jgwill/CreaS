using System;

namespace Creas.Entities
{
  public abstract class EntityBase
  {
    protected Guid idug = Guid.Empty;

    public virtual Guid IduGuid
    {
      get
      {
        if (idug == Guid.Empty) idug = Guid.NewGuid();
        return idug;
      }
      set { idug = value; }
    }


  }
}
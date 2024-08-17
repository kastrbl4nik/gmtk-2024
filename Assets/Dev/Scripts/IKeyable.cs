using UnityEngine;

public interface IKeyable
{
    public bool IsHoldingKey { get; set;  }
    public GameObject Key { get; set; }
}
